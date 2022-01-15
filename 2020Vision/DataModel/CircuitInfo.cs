using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;

namespace Vision2020
{
    public class CircuitInfo
    {
        public int CircuitID { get; private set; }
        public bool isComplete { get { return true; } }

        public RectangleF _bounds = new RectangleF(0, 0, 200, 200);
        public RectangleF bounds { get; set; }
        public int NumberOfCars { get; set; }
        public PacketLapData lapData { get; set; }
        public PacketMotionData motionInfo { get; set; }
        public PacketSessionData circuitData { get; set; }
        public List<MotionInContext> motionLog { get; set; }

        public CircuitInfo(PacketSessionData data)
        {
            CircuitID = data.trackId;
            circuitData = data;
            motionLog = new List<MotionInContext>();
            if (File.Exists($"{CircuitID}.circuit"))
            {

                
                ///
            }
        }

        CarMotionData? lastPosition = null;
        public void Update(PacketHeader context, PacketMotionData data, int playerIndex)
        {
            motionInfo = data;
            lock (motionLog)
            {
                if (motionLog.Count < 1000)
                {
                    if (lastPosition == null || (
                            Distance(lastPosition.Value.worldPositionX, motionInfo.carMotionData[playerIndex].worldPositionX,
                            lastPosition.Value.worldPositionY, motionInfo.carMotionData[playerIndex].worldPositionY,
                            lastPosition.Value.worldPositionZ, motionInfo.carMotionData[playerIndex].worldPositionZ) > 40.0F))
                    {
                        lastPosition = motionInfo.carMotionData[playerIndex];
                        motionLog.Add(new MotionInContext() { context = context, carMotion = motionInfo.carMotionData[playerIndex] });
                    }
                    else
                    {
                        // lastPosition = 
                    }
                }
            }
        }

        public float Distance(float X1, float X2, float Y1, float Y2, float Z1, float Z2)
        {
            return ((X1 - X2) * (X1 - X2) + (Y1 - Y2) * (Y1 - Y2) + (Z1 - Z2) * (Z1 - Z2));
        }

        public void Update(PacketLapData data)
        {
            lapData = data;
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

            if (lapData.lapData == null)
            {
                return;
            }
            var lapInfo = lapData.lapData[playerIndex];

            if (lapInfo.driverStatus > 0)
            {
                var br = new SolidBrush(Color.Yellow);
                var progressRect = lapRect;
                if (circuitData.trackLength > 0 && lapInfo.lapDistance > 0)
                {

                    float pctCovered = lapInfo.lapDistance / (float) circuitData.trackLength;

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
    }
}
