using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vision2020
{
    public class SessionInfo
    {
        public CircuitInfo circuit = null;
        public string SessionID;
        public int playerCarIndex;
        public PlayerInfo[] playerInfo = new PlayerInfo[22];

        public bool sessionDetailsPresent { get; private set; }
        public PacketParticipantsData participantsData;
        public SessionInfo(PacketHeader packet)
        {
            SessionID = packet.sessionUID.ToString();
            playerCarIndex = packet.playerCarIndex;
        }

        public SessionInfo(List<LapAnalyzer> lapData)
        {
            SessionID = "0";
            playerCarIndex = 0;
            participantsData = new PacketParticipantsData()
            {
                numActiveCars = (byte)lapData.Count,
                participants = new ParticipantData[22]
            };
            Update(lapData[0].lapInfo.Details.circuitInfo);
            circuit.motionInfo = new PacketMotionData()
            {
                carMotionData = new CarMotionData[22]
            };
            int i = 0;
            foreach(var lap in lapData)
            {
                participantsData.participants[i] = lap.lapInfo.Details.playerInfo;
                circuit.motionLog.AddRange(lap.lapInfo.Details.lap.lapMotion);
                i++;
            }
            participantsData.numActiveCars = (byte) i;
            Update(participantsData);
            i = 0;
            foreach (var lap in lapData)
            {
                var player = GetPlayerByIndex(i);
                player.AddLap(lap.lapInfo.Details.lap);
                i++;
            }
        }

        public void Update(PacketParticipantsData data)
        {
            participantsData = data;
            if (circuit != null)
            {
                // LogLine("Aantal auto's: " + participantsData.numActiveCars);
                circuit.NumberOfCars = data.numActiveCars;
            }
            int participantIndex = 0;
            foreach (var participant in data.participants)
            {
                var str = PacketHelper.GetString(participant.name, 48);
                // Data might be empty if not enough cars available
                if (str != "" || participant.driverId != 0)
                {
                    var isAI = (participant.aiControlled == (byte)1 ? "AI" : "=>");
                    // LogLine($"{isAI} {(int)participant.raceNumber} {str} {PacketHelper.Country(participant.nationality)}, {participant.driverId}, Team:{participant.teamId})");
                    lock (playerInfo)
                    {
                        var player = GetPlayerByDriverID(participant.driverId);

                        if (player == null)
                        {
                            playerInfo[participantIndex] = new PlayerInfo(participant, participantIndex);
                        }
                        else
                        {
                            if (player.CarNumber != participant.raceNumber)
                            {
                                if (participant.raceNumber == 0)
                                {
                                    playerInfo[participantIndex] = new PlayerInfo(participant, participantIndex);
                                }
                                else
                                {
                                    throw new Exception($"Car switch detected {player.Name},{player.CarNumber} => {participant.name},{participant.raceNumber} ");
                                }
                            }
                            else
                            {
                                playerInfo[participantIndex]?.Update(participant);
                            }
                        }
                    }
                }
                participantIndex++;
            }
        }

        public PlayerInfo GetPlayerByDriverID(int driverId)
        {
            return playerInfo.FirstOrDefault(p => (p != null) && (p.driverId == driverId));
        }

        public PlayerInfo GetPlayerByIndex(int index)
        {
            return playerInfo[index];
        }

        int[] currentLapNum = new int[] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
        public void Update(PacketHeader context, PacketLapData data, AsyncUI callback)
        {
            if (circuit != null)
            {
                circuit.Update(data);

                for (int i = 0; i < participantsData.numActiveCars; i++)
                {
                    if (data.lapData[i].currentLapNum >= 0)
                    {
                        lock (playerInfo)
                        {
                            var playerData = GetPlayerByIndex(i);
                            currentLapNum[i] = data.lapData[i].currentLapNum;
                            if (playerData != null)
                            {
                                var lapInfo = playerData.AddLapData(context, data.lapData[i]);
                                if (lapInfo != null)
                                {
                                    callback.LogLine($"{playerData.CarNumber}:{playerData.Name} - lap {data.lapData[i].currentLapNum} : {lapInfo.lapTime.ToString()}");
                                    LapDatabase.Add(
                                        new CompletedLap()
                                        { 
                                            circuitInfo = this.circuit.circuitData,
                                            playerInfo = participantsData.participants[i],
                                            lap = lapInfo
                                        });
                                }
                            }
                        }
                    }
                    else
                        currentLapNum[i] = data.lapData[i].currentLapNum;
                }
            }
            else
            {
                for (int i = 0; i < currentLapNum.Length; i++) currentLapNum[i] = -1;
            }
        }

        public void Update(PacketSessionData data)
        {
            if (data.trackId >=0 && data.trackId<255 && circuit == null)
            {
                sessionDetailsPresent = true;
                circuit = new CircuitInfo(data);
            }
        }

        public void Update(PacketHeader context, PacketMotionData data)
        {
            if (circuit != null)
            {
                circuit.Update(context, data,playerCarIndex);
                for (int i = 0; i < participantsData.numActiveCars; i++)
                {
                    lock (playerInfo)
                    {
                        var playerData = GetPlayerByIndex(i);

                        if (currentLapNum[i] >= 0 && playerData != null)
                        {
                            playerData.AddMotionData(currentLapNum[i], context, data.carMotionData[i]);
                        }
                    }
                }
            }
        }

        public void Update(PacketHeader context, PacketCarTelemetryData data)
        {
            if (circuit != null)
            {
                for (int i = 0; i < participantsData.numActiveCars; i++)
                {
                    lock (playerInfo)
                    {
                        var playerData = GetPlayerByIndex(i);

                        if (currentLapNum[i] >= 0 && playerData != null)
                        {
                            playerData.AddTelemetryData(currentLapNum[i], context, data.carTelemetryData[i]);
                        }
                    }
                }
            }
        }

        public void Update(PacketHeader context, PacketCarSetupData data)
        {
            if (circuit != null)
            {
                for (int i = 0; i < participantsData.numActiveCars; i++)
                {
                    lock (playerInfo)
                    {
                        var playerData = GetPlayerByIndex(i);

                        if (currentLapNum[i] >= 0 && playerData != null)
                        {
                            playerData.AddSetupData(currentLapNum[i], context, data.m_carSetups[i]);
                        }
                    }
                }
            }
        }
        

    }
}
