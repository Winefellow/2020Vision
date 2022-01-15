using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vision2020
{
    public enum ReaderMode { rmIdle, rmReplay, rmRecord, rmAnalyze };
    public class PacketReader
    {
        public bool KeepGoing = true;
        
        protected bool logPackets = false;
        protected int packetCount = 0;
        protected TimeSpan diff = TimeSpan.MinValue;
        protected PacketInfo lastPacket = null;

        private Stream dataStream;
        private AsyncUI callBack;

        private ReaderMode _readerMode = ReaderMode.rmIdle;
        public ReaderMode ReaderMode { get { return _readerMode; }  }
        public PacketReader(AsyncUI owner)
        {
            callBack = owner;
        }

        List<Byte[]> packetList = new List<byte[]>();

        public void Read(ReaderMode readerMode, CancellationToken token)
        {
            _readerMode = readerMode;
            
            int totalBytes = 0;

            if (ReaderMode != ReaderMode.rmRecord)
            {
                var pktFileName = callBack.SelectedFile;

                Stream fIn = new FileStream(pktFileName, FileMode.Open, FileAccess.Read);
                dataStream = fIn;
                
                while(fIn.Position < fIn.Length && !token.IsCancellationRequested)
                {
                    packetCount++;
                    HandlePacket(fIn, token);
                }
            }
            else
            {
                using (var socket = new UDPSocket())
                {
                    socket.Client(20777);
                    callBack.LogLine("Waiting for packets (port 20777)...");

                    while (!token.IsCancellationRequested)
                    {
                        byte[] data = socket.NextMessage();
                        if (data != null)
                        {
                            packetCount++;
                            totalBytes = totalBytes + data.Length;

                            MemoryStream s = new MemoryStream(data);
                            HandlePacket(s, token);

                            packetList.Add(data);
                        }
                        else
                        {
                            if (!token.IsCancellationRequested)
                            {
                                //callBack.Log($"Restart");
                            }
                        }
                    }
                }
            }
            callBack.LogLine("");
            SpeachSynthesizer.QueueText("Done");
            if (ReaderMode == ReaderMode.rmRecord)
            {
                SaveData();
                callBack.LogLine("Cancelled");
            }
            if (ReaderMode == ReaderMode.rmAnalyze)
            {
                callBack.LogLine("Data loaded");
            }
            if (ReaderMode == ReaderMode.rmReplay)
            {
                callBack.LogLine("Replay finished");
            }
            _readerMode = ReaderMode.rmIdle;
        }

        public void Splt()
        {

        }

        public void SaveData()
        {
            if (packetList.Count > 0)
            {
                String dateExt = DateTime.Now.ToString("ddMMyyyy");
                int seq = 1;
                while (File.Exists($"f1_2020_{dateExt}_{callBack.SessionID}_{seq}.data"))
                {
                    callBack.LogLine("File exists " + $"f1_2020_{dateExt}_{callBack.SessionID}_{seq}.data");
                    seq++;
                }
                callBack.LogLine("Saving data to " + $"f1_2020_{dateExt}_{callBack.SessionID}_{seq}.data");

                int bytesWritten = 0;
                using (var stream = new FileStream($"f1_2020_{dateExt}_{callBack.SessionID}_{seq}.data", FileMode.Append))
                {
                    foreach (var data in packetList)
                    {
                        stream.Write(data, 0, data.Length);
                        bytesWritten = bytesWritten + data.Length;
                    }
                    stream.Close();
                    callBack.LogLine($"{bytesWritten} bytes written to file");
                }
                // Erase data;
                packetList = new List<byte[]>();
            }
        }

        void HandlePacket(Stream fIn, CancellationToken token)
        {
            // read Header
            var packet = new PacketInfo()
            {
                sequence = packetCount
            };
            
            packet.header = PacketHelper.SafeRead<PacketHeader>(fIn, PacketSize.PacketHeaderSize);
            if (fIn.Position == fIn.Length)
            {
                // No more data
                return;
            }
            // callBack.Log($"{packet.header.frameIdentifier}-");

            while (packet.header.frameIdentifier == 0 && packet.header.packetFormat != 2021)
            {
                // empty packet?
                packet.header = PacketHelper.SafeRead<PacketHeader>(fIn, PacketSize.PacketHeaderSize);
            }

            //if (packet.header.packetFormat != 2020 && packet.header.packetFormat != 2021)
            //{
            //    callBack.LogLine($"FATAL: Invalid packet format: last Packet {packet.header.packetFormat}");
            //    //throw new Exception($"Invalid packet format: last Packet {packet.header.packetFormat}");
            //}

            if (ReaderMode == ReaderMode.rmReplay)
            {
                // Simulate time
                if (diff == TimeSpan.MinValue)
                {
                    diff = (DateTime.Now - PacketHelper.UnixTimeStampToDateTime(packet.header.sessionTime));
                }
                else
                {
                    bool inPause = callBack.IsPausing();
                    DateTime startPause = DateTime.Now;
                    while (!token.IsCancellationRequested && 
                                (callBack.IsPausing() || 
                                (PacketHelper.UnixTimeStampToDateTime(packet.header.sessionTime) + diff) > DateTime.Now))
                    {
                        Thread.Sleep(1);
                    }
                    if (inPause)
                    {
                        diff = diff + (DateTime.Now - startPause);
                    }
                }
            }

            switch (packet.header.packetId)
            {
                case 0: // Motion
                    {
                        if (logPackets) callBack.Log(".");                 // Motion info
                        packet.details = PacketHelper.SafeRead<PacketMotionData>(fIn, PacketSize.PacketMotionDataSize);
                        callBack.UpdateMotion(packet.header, (PacketMotionData)packet.details);
                        break;
                    }
                case 1: // Session
                    {
                        if (logPackets) callBack.Log($"1S"); // Data about the session – track, time left
                        packet.details = PacketHelper.SafeRead<PacketSessionData>(fIn, PacketSize.PacketSessionDataSize);

                        callBack.UpdateSession(packet.header, (PacketSessionData)packet.details);
                        break;
                    }
                case 2: // Lap
                    {
                        if (logPackets) callBack.Log($"2L");  // Data about all the lap times of cars in the session
                        packet.details = PacketHelper.SafeRead<PacketLapData>(fIn, PacketSize.PacketLapDataSize);
                        callBack.UpdateLapdata(packet.header, (PacketLapData)packet.details);
                        break;
                    }
                case 3: // Event
                    {
                        if (logPackets) callBack.Log($"3E");
                        var ped = new PacketEventData()
                        {
                            eventStringCode = PacketHelper.GetEventType(fIn),
                            eventDetails = null
                        };
                        packet.details = ped;
                        // Depending on type of event the contents may differ. In C++ this is a union. Therefore the largest packetsize is the size.
                        byte[] data = new byte[PacketSize.FlashbackSize];
                        if (fIn.Read(data, 0, PacketSize.FlashbackSize) != PacketSize.FlashbackSize)
                        {
                            throw new Exception("Size error");
                        }
                        // Read all the remaining bytes into a memory stream to be able to proces them later
                        MemoryStream ms = new MemoryStream(data);

                        switch (ped.eventStringCode)
                        {
                            case EventType.Unknown:
                                break;
                            case EventType.SessionStarted:

                                callBack.StartSession(packet.header);
                                break;
                            case EventType.SessionEnded:
                                
                                callBack.EndSession(packet.header);
                                SaveData();
                                break;
                            case EventType.FastestLap:
                                ped.eventDetails = PacketHelper.SafeRead<FastestLap>(ms, PacketSize.FastestLapSize);
                                break;
                            case EventType.Retirement:
                                ped.eventDetails = PacketHelper.SafeRead<Retirement>(ms, PacketSize.RetirementSize);
                                break;
                            case EventType.DRSEnabled:
                                break;
                            case EventType.DRSDisabled:
                                break;
                            case EventType.TeammateInPits:
                                ped.eventDetails = PacketHelper.SafeRead<TeamMateInPits>(ms, PacketSize.TeamMateInPitsSize);
                                break;
                            case EventType.CheckeredFlag:
                                break;
                            case EventType.RaceWinner:
                                ped.eventDetails = PacketHelper.SafeRead<RaceWinner>(ms, PacketSize.RaceWinnerSize);
                                break;
                            case EventType.PenaltyIssued:
                                ped.eventDetails = PacketHelper.SafeRead<Penalty>(ms, PacketSize.PenaltySize);
                                break;
                            case EventType.SpeedTrapTriggered:
                                ped.eventDetails = PacketHelper.SafeRead<SpeedTrap>(ms, PacketSize.SpeedTrapSize);
                                break;
                            default:
                                break;
                        }
                        if (logPackets) Console.Write("Evt "); // Various notable events that happen during a session
                        break;
                    }
                case 4: // Participants
                    {
                        if (logPackets) callBack.Log($"4P");   /// List of participants in the session, mostly relevant for multiplayer
                        packet.details = PacketHelper.SafeRead<PacketParticipantsData>(fIn, PacketSize.PacketParticipantsDataSize);
                        Task t = new Task(() =>
                        {
                            callBack.UpdatePaticipants((PacketParticipantsData)packet.details);
                        });
                        t.Start();

                        break;
                    }
                case 5: // Car Setup
                    {
                        if (logPackets) callBack.Log("5C");
                        packet.details = PacketHelper.SafeRead<PacketCarSetupData>(fIn, PacketSize.PacketCarSetupDataSize);
                        Console.Write("Setup "); /// Packet detailing car setups for cars in the race
                        callBack.UpdateSetup(packet.header, (PacketCarSetupData)packet.details);
                        break;
                    }
                case 6: // Car telemetry
                    {
                        if (logPackets) callBack.Log("6T"); /// Telemetry data for all carsbreak;
                        packet.details = PacketHelper.SafeRead<PacketCarTelemetryData>(fIn, PacketSize.PacketCarTelemetryDataSize);
                        callBack.UpdateTelemetry(packet.header, (PacketCarTelemetryData)packet.details);
                        break;
                    }
                case 7:  // Car Status
                    {
                        if (logPackets) callBack.Log("7ST");
                        packet.details = PacketHelper.SafeRead<PacketCarStatusData>(fIn, PacketSize.PacketCarStatusDataSize);
                        break;
                    }

                case 8: // Final classification
                    {
                        if (logPackets) callBack.Log("8F");
                        packet.details = PacketHelper.SafeRead<PacketFinalClassificationData>(fIn, PacketSize.PacketFinalClassificationDataSize);
                        break;
                    }
                case 9: // Lobby
                    {
                        if (logPackets) callBack.Log("9L");
                        packet.details = PacketHelper.SafeRead<PacketLobbyInfoData>(fIn, PacketSize.PacketLobbyInfoDataSize);
                        break;
                    }
                case 10: // Car Damage
                    {
                        if (logPackets) callBack.Log("10L");
                        packet.details = PacketHelper.SafeRead<PacketCarDamageData>(fIn, PacketSize.PacketCarDamageDataSize);
                        break;
                    }
                case 11: // Session history
                    {
                        if (logPackets) callBack.Log("11L");
                        packet.details = PacketHelper.SafeRead<PacketSessionHistoryData>(fIn, PacketSize.PacketSessionHistoryDataSize);
                        break;
                    }
                default:
                    {
                        Console.Write($"??? Unknown packet type: {packet.header.packetId}");
                        callBack.LogLine($"??? Unknown packet type: {packet.header.packetId}");
                        break;
                    }
            }
            // notificate UI of packet
            callBack.AddPacket(packet);

            lastPacket = packet;
        }
    }
}

