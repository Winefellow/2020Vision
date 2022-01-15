using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Vision2020
{
    public struct ColorScheme
    {
        public Color Background;
        public Color Text;
    }

    public static class ColorSchemes
    {
        public static List<ColorScheme> schemes = new List<ColorScheme>()
        {
            new ColorScheme() { Background=Color.Aqua, Text=Color.Black},
            new ColorScheme() { Background=Color.Aquamarine, Text=Color.Black},
            new ColorScheme() { Background=Color.Azure, Text=Color.Black},
            new ColorScheme() { Background=Color.Brown, Text=Color.White},
            new ColorScheme() { Background=Color.Black, Text=Color.White},
            new ColorScheme() { Background=Color.Blue, Text=Color.White},
            new ColorScheme() { Background=Color.BlueViolet, Text=Color.White},
            new ColorScheme() { Background=Color.BurlyWood, Text=Color.White},
            new ColorScheme() { Background=Color.CadetBlue, Text=Color.Yellow},
            new ColorScheme() { Background=Color.Chartreuse, Text=Color.Black },
            new ColorScheme() { Background=Color.CornflowerBlue, Text=Color.Black},
            new ColorScheme() { Background=Color.DarkBlue, Text=Color.White},
            new ColorScheme() { Background=Color.DarkCyan, Text=Color.White},
            new ColorScheme() { Background=Color.DarkMagenta, Text=Color.White},
            new ColorScheme() { Background=Color.DarkSalmon, Text=Color.White},
            new ColorScheme() { Background=Color.DarkSeaGreen, Text=Color.Black},
            new ColorScheme() { Background=Color.DarkSlateBlue, Text=Color.Yellow},
            new ColorScheme() { Background=Color.Gold, Text=Color.Black},
            new ColorScheme() { Background=Color.HotPink, Text=Color.Black},
            new ColorScheme() { Background=Color.Red, Text=Color.White},
            new ColorScheme() { Background=Color.Magenta, Text=Color.Black},
            new ColorScheme() { Background=Color.YellowGreen, Text=Color.White},
            new ColorScheme() { Background=Color.White, Text=Color.Red},
            new ColorScheme() { Background=Color.YellowGreen, Text=Color.Red}
        };
    }

    public class LapRecording
    {
        public LapData FirstTiming { get; set; }
        public LapData lapTimings { get; set;  } 
        public List<TelemetryInContext> lapTelemetry { get; set; }
        public List<MotionInContext> lapMotion { get; set; }
        public List<LapDataInContext> lapData { get; set; }
        public CarSetupData Setup { get; set; }  // Might change during session, but only the last setup is recorded
        public bool complete { get; set; }
        public bool valid { get; set; }
        public bool started { get; set; }
        public float lapTime => (float) lapTimeInMs / 1000f;
        public UInt32 lapTimeInMs { get; set; }
    }

    public class PlayerInfo
    {
        public int Id;
        public int driverId;
        public String Name;
        public int CarNumber;
        public Color BackColor;
        public Color TextColor;
        public int currentLapIndex = -1;
        public Dictionary<int, LapRecording> lapStore;
        public Rectangle selectBox;
        public CarMotionData lastMotion;
        public CarTelemetryData? lastTelemetry;

        public CarMotionData CarInfo;
        public List<PacketEventData> Events;
        public ParticipantData participantInfo;
        public CarStatusData CarStatus;
        public PlayerInfo(ParticipantData p, int index)
        {
            participantInfo = p;
            Id = index;
            driverId = p.driverId;
            lapStore = new Dictionary<int, LapRecording>();

            Name = PacketHelper.GetString(participantInfo.name, 48) + " (" + PacketHelper.CountryShort(participantInfo.nationality) + ")";
            CarNumber = participantInfo.raceNumber;
            if (index < ColorSchemes.schemes.Count)
            {
                BackColor = ColorSchemes.schemes[index].Background;
                TextColor = ColorSchemes.schemes[index].Text;
            }
            else
            {
                BackColor = ColorSchemes.schemes[0].Background;
                TextColor = ColorSchemes.schemes[0].Text;
            }
        }

        public void Update(ParticipantData p)
        {
            participantInfo = p;
            Name = PacketHelper.GetString(participantInfo.name, 48) + " (" + PacketHelper.CountryShort(participantInfo.nationality) + ")";
        }

        public void SetBox(RectangleF r)
        {
            selectBox = new Rectangle( (int)r.X, (int)r.Y, (int)r.Width, (int) r.Height);
        }
        internal void DrawNumber(Graphics g, RectangleF logoRect)
        {
            var foreGroundBrush = new SolidBrush(TextColor);
            var backGroundBrush = new SolidBrush(BackColor);
            g.FillRectangle(backGroundBrush, logoRect);
            Font f = new Font("Courier", 9);
            g.DrawString(CarNumber.ToString(), f, foreGroundBrush, logoRect);
        }

        internal LapRecording AddLapData(PacketHeader context, LapData lapData)
        {
            LapRecording completedLap = null;

            // uint8 m_resultStatus;              
            // Result status - 0 = invalid, 1 = inactive, 2 = active
            // 3 = finished, 4 = disqualified, 5 = not classified
            // 6 = retired
            // Status of driver - 0 = in garage, 1 = flying lap
            // 2 = in lap, 3 = out lap, 4 = on track)
            if (lapData.driverStatus == 0 || lapData.driverStatus == 3 || lapData.driverStatus == 2)
            {
                // In garage
                return null;
            }
            LapRecording activeLap = GetLap(lapData.currentLapNum);

            if (lapData.resultStatus == 3)
            {
                if (activeLap.lapTime > 0 && activeLap.lapMotion.Count > 100)
                {
                    activeLap.valid = true;
                    activeLap.complete = true;
                    completedLap = activeLap;
                }
                else 
                    return null;
            }
            if (currentLapIndex != lapData.currentLapNum)
            {
                // New lap? Starting Lap? Something else?
                if (currentLapIndex > lapData.currentLapNum)
                {
                    currentLapIndex = lapData.currentLapNum;
                }
                else if (currentLapIndex > 0)
                {
                    var previousLap = GetLap(currentLapIndex);

                    var lastLap = previousLap.lapTimings;
                    if (lastLap.currentLapInvalid == 0)
                    {
                        // Sanity check. Packages with ResultStatus 3 are missing, so calculate if it is a valid lap
                        //  by checking sinsible sector and lap times.
                        previousLap.complete =
                            previousLap.started && lastLap.sector1TimeInMS > 0 && lastLap.sector2TimeInMS > 0 &&
                                (lastLap.currentLapTimeInMS) > (lastLap.sector1TimeInMS + lastLap.sector2TimeInMS);
                        if (previousLap.complete)
                        {
                            previousLap.lapTimeInMs = lapData.lastLapTimeInMS;
                            completedLap = previousLap;
                        }
                    }
                    else
                    {
                        previousLap.valid = false;
                    }
                }
                currentLapIndex = lapData.currentLapNum;

            }
            if (!activeLap.started)
            {
                activeLap.FirstTiming = lapData;
                if (lapData.lapDistance < 10)
                {
                    activeLap.started = true;
                }
            }
            activeLap.lapTimings = lapData;
            activeLap.lapData.Add(new LapDataInContext() { context = context, lapData = lapData });
            // GetLap(lapData.currentLapNum).started |= (lapData.sector == 0);

            return completedLap;
        }

        internal LapRecording GetLap(int lapNumber)
        {
            if (!lapStore.ContainsKey(lapNumber))
            {
                lapStore.Add(lapNumber, new LapRecording()
                {
                    lapTelemetry = new List<TelemetryInContext>(),
                    lapMotion = new List<MotionInContext>(),
                    lapData = new List<LapDataInContext>(),
                    complete = false,
                    valid = true,
                    started = false
                }); 
            }
            return lapStore[lapNumber];
        }

        internal void AddMotionData(int lapNumber, PacketHeader context, CarMotionData carMotionData)
        {
            if (currentLapIndex == lapNumber)
            {
                GetLap(lapNumber).lapMotion.Add(new MotionInContext() { context = context, carMotion = carMotionData });
            }
        }

        internal void AddTelemetryData(int lapNumber, PacketHeader context, CarTelemetryData carTelemetryData)
        {
            if (currentLapIndex == lapNumber)
            {
                GetLap(lapNumber).lapTelemetry.Add(new TelemetryInContext() { context = context, carTelemetry = carTelemetryData });
                lastTelemetry = carTelemetryData;
            }
        }

        internal void AddSetupData(int lapNumber, PacketHeader context, CarSetupData carSetupData)
        {
            if (currentLapIndex == lapNumber)
            {
                LapRecording rec = GetLap(lapNumber);
                if (carSetupData.fuelLoad>0)
                    rec.Setup = carSetupData;
            }
        }

        public CarTelemetryData? GetActualTelemetry()
        {
            if (currentLapIndex != -1)
            {
                return lastTelemetry;
            }
            return null;
        }

        internal void AddLap(LapRecording li)
        {
            lapStore.Add(1, li);

            lastMotion = li.lapMotion.Last().carMotion;
            lastTelemetry = li.lapTelemetry.Last().carTelemetry;
        }

    }
}
