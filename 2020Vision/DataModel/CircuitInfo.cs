using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Vision2020
{
    public class CircuitInfo
    {
        public int CircuitID { get; private set; } = -1;
        public bool IsComplete { get; set; } = false;

        public RectangleF _bounds = new RectangleF(0, 0, 200, 200);
        public RectangleF Bounds { get; set; }
        public int NumberOfCars { get; set; }
        public PacketLapData LapData { get; set; }
        public PacketMotionData MotionInfo { get; set; }
        public PacketSessionData CircuitData { get; set; }

        private static object Locker = new object();

        public CircuitLayoutData CircuitLayoutData { get; set; }

        // Active player lap information
        private int lastSpeed = 0;

        private float Breaking = -1;
        
        public CircuitInfo(PacketSessionData data)
        {
            if (CircuitID != data.trackId)
            {
                Save();
                CircuitID = data.trackId;

                if (CircuitID != -1)
                {
                    if (File.Exists(LapDatabase.GetCircuitFileName(CircuitID)))
                    {
                        CircuitLayoutData = JsonSerializer.Deserialize<CircuitLayoutData>(File.ReadAllText(LapDatabase.GetCircuitFileName(CircuitID)));
                    }
                    else
                    {
                        CircuitLayoutData = new CircuitLayoutData();
                    }
                }
            }
            CircuitData = data;
        }

        // Afstand tussen twee punten (hemelsbreed)
        public float Distance(float X1, float X2, float Y1, float Y2, float Z1, float Z2)
        {
            return ((X1 - X2) * (X1 - X2) + (Y1 - Y2) * (Y1 - Y2) + (Z1 - Z2) * (Z1 - Z2));
        }

        // Sla packetlapdata op, wordt gebruikt om lapinformatie te tonen (rechterkant)
        public void Update(PacketLapData data)
        {
            LapData = data;
        }

        ///Verwerk een packetMotionData bericht
        // public void Update(PacketHeader context, PacketMotionData data, int playerIndex)
        public void Update(PacketMotionData data)
        {
            MotionInfo = data;
        }

        /// <summary>
        /// Toon het rechtergedeelte van de informatie die bij een coureur staat
        /// </summary>
        /// <param name="g"></param>
        /// <param name="index"></param>
        /// <param name="lapRect"></param>
        internal void UpdateLapInfo(Graphics g, int playerIndex, RectangleF lapRect)
        {
            //uint8 m_driverStatus;              // Status of driver - 0 = in garage, 1 = flying lap
            // 2 = in lap, 3 = out lap, 4 = on track
            //uint8 m_resultStatus;              
            // Result status - 0 = invalid, 1 = inactive, 2 = active
            // 3 = finished, 4 = disqualified, 5 = not classified
            // 6 = retired

            if (LapData.lapData == null)
            {
                return;
            }
            var lapInfo = LapData.lapData[playerIndex];

            if (lapInfo.driverStatus > 0)
            {
                var br = new SolidBrush(Color.Yellow);
                var progressRect = lapRect;
                if (CircuitData.trackLength > 0 && lapInfo.lapDistance > 0)
                {
                    float pctCovered = lapInfo.lapDistance / (float)CircuitData.trackLength;

                    progressRect.Width = progressRect.Width * pctCovered;
                    g.FillRectangle(br, progressRect);
                }
                String s = TimeSpan.FromMilliseconds(lapInfo.currentLapTimeInMS).ToString("mm\\:ss\\.fff");
                var f = new Font("Courier New", 8);
                br = new SolidBrush(Color.Blue);
                g.DrawString(s, f, br, lapRect);
            }
            //data.lapData[i].lapDistance;
            //data.lapData[i].lastLapTime;
            //data.lapData[i].currentLapTime;
            //data.lapData[i].bestLapSector1TimeInMS;
            //data.lapData[i].bestLapSector2TimeInMS;
            //data.lapData[i].bestLapSector3TimeInMS;
            //data.lapData[i].sector1TimeInMS;
            //data.lapData[i].sector2TimeInMS;
        }

        // Add Completed valid lap to circuit information
        internal void AddLapInfo(LapRecording lapInfo)
        {
            var me = lapInfo.lapMotion.GetEnumerator();
            var m = me.Current;
            var te = lapInfo.lapTelemetry.GetEnumerator();  // 2..3
            var t = te.Current;
            foreach (var l in lapInfo.lapData)  // 1..2..3
            {
                // l is next frame. If next contextFrame
                if ( (l.context.frameIdentifier < m.context.frameIdentifier) ||
                      (l.context.frameIdentifier < t.context.frameIdentifier))
                {
                    continue;
                }
                while (me.Current.context.frameIdentifier < l.context.frameIdentifier && me.MoveNext()) ;
                m = me.Current;
                while (te.Current.context.frameIdentifier < l.context.frameIdentifier && te.MoveNext()) ;
                t = te.Current;
                if (m.context.frameIdentifier == t.context.frameIdentifier &&
                    m.context.frameIdentifier == l.context.frameIdentifier)
                {
                    AnalyzeApex(l.lapData, t.carTelemetry, m.carMotion);
                    CircuitLayoutData.AddData(l.lapData, m.carMotion);
                }
            }
        }

        LapData? lastLapData = new LapData();
        CarMotionData? lastCarMotionData = null;
        CarTelemetryData? lastCarTelemetryData = null;
        uint lastLapFrame = uint.MaxValue;
        uint lastMotionFrame = uint.MaxValue;
        uint lastTelemetryFrame = uint.MaxValue;
        object locker = new object();

        public void CheckMyApex(uint frame, LapData data)
        {
            lock (locker)
            {
                lastLapData = data;
                lastLapFrame = frame;
                CheckMyApex();
            }
        }

        public void CheckMyApex(uint frame, CarMotionData data)
        {
            lock (locker)
            {
                lastCarMotionData = data;
                lastMotionFrame = frame;
                CheckMyApex();
            }
        }

        public void CheckMyApex(uint frame, CarTelemetryData data)
        {
            lock (locker)
            {
                lastCarTelemetryData = data;
                lastTelemetryFrame = frame;
                CheckMyApex();
            }
        }


        private ApexInfo myNextApex = null;
        private ApexInfo myLastApex = null;

        /// <summary>
        ///     Check if apex narration is needed and speak!
        ///     Already in 'locked' state. Prevent deadlocks!
        /// </summary>
        public void CheckMyApex()
        {
            if (CircuitLayoutData?.apexInfo == null)
                return;
            // Check if data is consistent
            if (lastMotionFrame == lastLapFrame && lastMotionFrame == lastTelemetryFrame &&
                lastCarMotionData != null && lastLapData != null && lastCarTelemetryData != null)
            {
                // Are we alre
                if (myNextApex == null)
                {
                    // Next apex (first apex in upcoming 200 m)
                    myNextApex = CircuitLayoutData.apexInfo.Where(a =>
                                        a.numMeasurements > 3
                                        && a.fastAverage > lastLapData.Value.lapDistance &&
                                        a.fastAverage - lastLapData.Value.lapDistance < 200)
                                        .FirstOrDefault();
                    if (myNextApex != null)
                    {
                        myNextApex.LaptimeBeforeApex = 0;
                        myNextApex.Breaking = 0;
                    }
                }
                if (myNextApex != null)
                {
                    // Store lap time at 70m before apex
                    if (myNextApex.fastAverage - lastLapData.Value.lapDistance < 70 && myNextApex.LaptimeBeforeApex == 0)
                    {
                        myNextApex.LaptimeBeforeApex = lastLapData.Value.currentLapTimeInMS;
                    }
                    if (myNextApex.Breaking == 0 && lastCarTelemetryData.Value.brake>0)
                    {
                        // Hitting the brakes!
                        myNextApex.Breaking = lastLapData.Value.lapDistance;
                        if (Config.UserConfig.AnnounceDinstance2Apex)
                        {
                            SpeachSynthesizer.QueueText((myNextApex.fastAverage - lastLapData.Value.lapDistance).ToString());
                        }
                    }
                    if (lastLapData.Value.lapDistance > myNextApex.fastAverage)
                    {
                        // just after apex
                        myLastApex = myNextApex;
                        if (Config.UserConfig.AnnounceApexSpeed)
                        {
                            SpeachSynthesizer.QueueText(lastCarTelemetryData.Value.speed.ToString());
                        }
                        myNextApex = null;
                    }
                }
                if (myLastApex != null && lastLapData.Value.lapDistance > myLastApex.fastAverage + 70)
                {
                    // X m after apex
                    if (myLastApex.LaptimeBeforeApex > 0)
                    {
                        string s = (lastLapData.Value.currentLapTimeInMS - myLastApex.LaptimeBeforeApex).ToString();
                        // Call out ms from 150m before apex to now
                        if (Config.UserConfig.AnnounceCornerTime)
                        {
                            SpeachSynthesizer.QueueNumber(s);
                        }
                        myLastApex = null;
                    }
                }
            }
        }

        internal void Save()
        {
            if (CircuitLayoutData != null && CircuitID != -1)
            {
                File.WriteAllText(LapDatabase.GetCircuitFileName(CircuitID), JsonSerializer.Serialize(CircuitLayoutData));
            }
        }

        private float lastCandidate = -1;
        private int BreakingSpeed = 300;
        private int ApexSpeed = 300;

        internal void AnalyzeApex(LapData lapData, CarTelemetryData carTelemetryData, CarMotionData motionData)
        {
            var nextSpeed = carTelemetryData.speed;
            if ((lastSpeed < 50 || lastSpeed > 230) && (nextSpeed < 50 || nextSpeed > 230))
            {
                // ignore
                Breaking = -1;
                return;
            }
            
           
            lock (Locker)
            {
                var m = motionData;
                var t = carTelemetryData;
                var l = lapData;
                var distance = (int)Math.Round(l.lapDistance, 0);

                if (Breaking == -1 && t.brake > 0 && l.lapDistance > 0)
                {
                    Breaking = l.lapDistance;
                    BreakingSpeed = t.speed;
                    lastCandidate = -1;
                }
                else if (Breaking > 0 && t.speed > 0 && t.brake == 0 && t.throttle > 0.2f)
                {
                    if (lastCandidate == -1)
                    {
                        lastCandidate = l.lapDistance;
                        ApexSpeed = t.speed;
                    }
                }
                
                if (Breaking!=-1 && BreakingSpeed - ApexSpeed > 50 && lastCandidate > 0 && (t.speed - ApexSpeed)> 10)
                {
                    AddApex((int)Math.Round(lastCandidate, 0));
                    Breaking = -1;
                }
            }
            lastSpeed = nextSpeed;
        }

        private void AddApex(int distance)
        {
            if (CircuitLayoutData.apexInfo == null)
            {
                return;
            }
            GetNearestApex(distance).AddMeasurement(distance);
        }

        private ApexInfo GetNextApex(int distance)
        {
            return CircuitLayoutData.apexInfo.Where(a => distance - a.fastAverage < 200 && a.fastAverage > distance).FirstOrDefault();
        }

        private ApexInfo GetNearestApex(int distance)
        {
            var nearest = CircuitLayoutData.apexInfo.Where(a => Math.Abs(distance - a.fastAverage) < 50).FirstOrDefault();
            if (nearest == null)
            {
                nearest = new ApexInfo();
                nearest.AddMeasurement(distance);
                CircuitLayoutData.apexInfo.Add(nearest);
            }
            return nearest;
        }

        private int CheckApex(int distance)
        {
            var nextApex = GetNextApex(distance);
            if (nextApex == null)
            {
                return distance;
            }
            return nextApex.fastAverage - distance;
        }
    }

    public class ApexInfo
    {
        public List<int> measurements { get; set; } = new List<int>();

        public int fastAverage { get; set; } = 0;
        [JsonIgnore]
        public int average => numMeasurements == 0 ? 0 : measurements.Take(numMeasurements).Sum() / numMeasurements;
        public int numMeasurements { get; set; }


        [JsonIgnore]
        // laptime X m before apex
        public uint LaptimeBeforeApex { get; set; }
        public float Breaking { get; internal set; }

        public void AddMeasurement(int distance)
        {
            if (measurements.Contains(distance))
            {
                // Ignore duplucates
                return;
            }
            if (numMeasurements < Constants.MaxApexMeasurements)
            {
                measurements.Add(distance);
                numMeasurements++;
                fastAverage = average;
            }
            else
            {
                var min = measurements.Min();
                var max = measurements.Max();
                if (distance < min || distance > max)
                {
                    return;
                }
                var avg = average;
                if (distance == avg)
                {
                    return;
                }
                if (distance > avg)
                {
                    for (int i = 0; i < numMeasurements; i++)
                    {
                        if (measurements[i] == max)
                        {
                            measurements[i] = distance;
                        }
                    }
                }
                if (distance < avg)
                {
                    for (int i = 0; i < numMeasurements; i++)
                    {
                        if (measurements[i] == min)
                        {
                            measurements[i] = distance;
                        }
                    }
                }
                fastAverage = average;
            }
        }
    }

    public class CircuitLocation
    {
        public bool InPit { get; set; }
        public float lapDistance { get; set; }
        public int surfaceType { get; set; }
        public float worldPositionX { get; set; }           // World space X position
        public float worldPositionY { get; set; }           // World space Y position
        public float worldPositionZ { get; set; }           // World space Z position
        public UInt16 worldForwardDirX { get; set; }         // World space forward X direction (normalised)
        public UInt16 worldForwardDirY { get; set; }         // World space forward Y direction (normalised)
        public UInt16 worldForwardDirZ { get; set; }         // World space forward Z direction (normalised)
        //public UInt16 worldRightDirX { get; set; }           // World space right X direction (normalised)
        //public UInt16 worldRightDirY { get; set; }           // World space right Y direction (normalised)
        //public UInt16 worldRightDirZ { get; set; }           // World space right Z direction (normalised)
    }

    public class CircuitLayoutData
    {
        public List<CircuitLocation> pointVectors { get; set; }
        public List<ApexInfo> apexInfo { get; set; }
        public float minx { get; set; } = 0;
        public float miny { get; set; } = 0;
        public float minz { get; set; } = 0;
        public float maxx { get; set; } = 0;
        public float maxy { get; set; } = 0;
        public float maxz { get; set; } = 0;
        public CircuitLayoutData()
        {
            pointVectors = new List<CircuitLocation>();
            apexInfo = new List<ApexInfo>();
        }

        public void AddData(LapData lapData, CarMotionData motionData)
        {
            if (lapData.currentLapInvalid == 1 || lapData.lapDistance < 0)
            {
                return;
            }

            // minder dan 5 meter sinds vorig punt
            var samePoints = pointVectors.Where(pv => pv.InPit == (lapData.pitStatus != 0) && 
                                                      lapData.lapDistance - pv.lapDistance < 5 &&  
                                                      lapData.lapDistance - pv.lapDistance > 0);
            if (samePoints.Any())
            {
                return;
                //var nextPoint = samePoints.First();
                //if (samePoints.Count() > 1)
                //{
                //    pointVectors.RemoveAll(pv => lapData.lapDistance != nextPoint.lapDistance &&  
                //                                    pv.InPit == (lapData.pitStatus != 0) &&
                //                                    pv.lapDistance - lapData.lapDistance > 5 &&
                //                                    pv.lapDistance - lapData.lapDistance < 10);
                //}
                //else
                //{
                //    nextPoint.lapDistance = (lapData.lapDistance + nextPoint.lapDistance) / 2;
                //    nextPoint.worldForwardDirX = (ushort)((motionData.worldForwardDirX / 2 + nextPoint.worldForwardDirX) / 2);
                //    nextPoint.worldForwardDirY = (ushort)((motionData.worldForwardDirY / 2 + nextPoint.worldForwardDirY) / 2);
                //    nextPoint.worldForwardDirZ = (ushort)((motionData.worldForwardDirZ / 2 + nextPoint.worldForwardDirZ) / 2);
                //    nextPoint.worldPositionX = (motionData.worldPositionX + nextPoint.worldPositionX) / 2f;
                //    nextPoint.worldPositionY = (motionData.worldPositionY + nextPoint.worldPositionY) / 2f;
                //    nextPoint.worldPositionZ = (motionData.worldPositionZ + nextPoint.worldPositionZ) / 2f;
                //}
            }

            //avg = samePoints.All()
            //    if (pointVectors.Any(pv =>  &&
            //    ((pv.worldPositionX - motionData.worldPositionX) * (pv.worldPositionX - motionData.worldPositionX) +
            //    (pv.worldPositionY - motionData.worldPositionY) * (pv.worldPositionY - motionData.worldPositionY) +
            //    (pv.worldPositionZ - motionData.worldPositionZ) * (pv.worldPositionZ - motionData.worldPositionZ)) < 10))

            else
            {
                pointVectors.Add(new CircuitLocation()
                {
                    InPit = (lapData.pitStatus != 0),
                    worldPositionX = motionData.worldPositionX,
                    worldPositionY = motionData.worldPositionY,
                    worldPositionZ = motionData.worldPositionZ,
                    lapDistance = lapData.lapDistance,
                    surfaceType = 0,
                    worldForwardDirX = motionData.worldForwardDirX,
                    worldForwardDirY = motionData.worldForwardDirY,
                    worldForwardDirZ = motionData.worldForwardDirZ,
                    //worldRightDirX = motionData.worldRightDirX,
                    //worldRightDirY = motionData.worldRightDirY,
                    //worldRightDirZ = motionData.worldRightDirZ
                });
                if (motionData.worldPositionX < minx) minx = motionData.worldPositionX;
                if (motionData.worldPositionX > maxx) maxx = motionData.worldPositionX;
                if (motionData.worldPositionY < miny) miny = motionData.worldPositionY;
                if (motionData.worldPositionY > maxy) maxy = motionData.worldPositionY;
                if (motionData.worldPositionZ < minz) minz = motionData.worldPositionZ;
                if (motionData.worldPositionZ > maxz) maxz = motionData.worldPositionZ;
                pointVectors = pointVectors.OrderBy(pv => pv.lapDistance).ToList();

            }
        }
    }
}

