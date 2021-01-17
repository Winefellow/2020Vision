using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vision2020
{
    public partial class TelemetryForm : Form, AsyncUI
    {
        // statistics
        public int[] packetCount = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public int[] UpdateCount = new int[] { 0, 0 };
        public TimeSpan[] UpdateTime = new TimeSpan[] { new TimeSpan(0), new TimeSpan(0) };
        public PlayerInfo selectedPlayer = null;

        public string SelectedFile { get { return selectedFileName; } }

        // Session data
        public SessionInfo sessionInfo = null;
        public DateTime sessionTime = DateTime.MinValue;

        // Window
        public bool IAmClosing = false;
        public bool IAmPausing = false;

        public Rectangle closeButton;
        public Rectangle settingsButton;
        public Rectangle pauseButton;

        float xFactor = (float)Math.Sin((2f * Math.PI) * 30 / 360f);
        float yFactor = (float)Math.Cos((2f * Math.PI) * 30 / 360f);

        private PacketReader pr = null;
        private Task ActiveTask = null;
        CancellationTokenSource cancelSource = null;
        private string selectedFileName = "";

        public TelemetryForm()
        {
            InitializeComponent();
        }

        public void LogLine(string s)
        {
            Log(s + Environment.NewLine);
        }

        public void Log(string s)
        {
            tbLog.InvokeIfRequired(() =>
            {
                if (!IAmClosing)
                {
                    tbLog.AppendText(s);
                }
            });
        }

        public void UpdatePaticipants(PacketParticipantsData participantsData)
        {
            if (sessionInfo != null)
            {
                lock (sessionInfo)
                {
                    try
                    {
                        sessionInfo.Update(participantsData);
                    }
                    catch(Exception e)
                    {
                        LogLine(e.Message);
                    }
                }
            }
        }

        private String TS(float time)
        {
            return PacketHelper.UnixTimeStampToDateTime(time).ToString("mm.ss.fff");
        }

        private void DriverListBox_Paint(object sender, PaintEventArgs e)
        {
            bool inSession = sessionInfo != null;
            DrawingPlayers = true;

            // Draw with double-buffering.
            UpdateCount[0]++;
            DateTime start = DateTime.Now;
            Bitmap bitmap = new Bitmap(driverListBox.Width, driverListBox.Height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                int nameWidth = driverListBox.Width / 2;
                int statusWidth = driverListBox.Width / 4;
                int extWidth = driverListBox.Width - nameWidth - statusWidth;
                int centerX = driverListBox.Width / 2;
                int centerY = driverListBox.Height / 2;
                Brush br = new SolidBrush(Color.DarkBlue);
                Font f = new Font("Courier New", 14);

                int top = 0;
                int left = 0;
                int marginY = 1;
                int marginX = 1;

                //Buttons
                var size = g.MeasureString("X", f);
                closeButton = new Rectangle(driverListBox.Width - (int)size.Width + left, top, (int)size.Width + marginX, (int)size.Height + marginY);
                g.DrawString("X", f, br, closeButton);

                size = g.MeasureString("*", f);
                settingsButton = new Rectangle(closeButton.Left - (int)size.Width - left, top, (int)size.Width + marginX, (int)size.Height + marginY);
                g.DrawString("*", f, br, settingsButton);

                size = g.MeasureString("=", f);
                pauseButton = new Rectangle(settingsButton.Left - (int)size.Width - left, top, (int)size.Width + marginX, (int)size.Height + marginY);
                g.DrawString("=", f, br, pauseButton);

                if (inSession)
                {
                    // SessionInfo
                    string s = sessionTime.ToString("mm.ss") + " " + Constants.SessionTypeList.First(t => t.id == sessionInfo.circuit.circuitData.sessionType)?.name +
                         " " +Constants.TrackList.First(t => t.id == sessionInfo.circuit.circuitData.trackId)?.name;
                    //foreach (var count in packetCount)
                    //{
                    //    if (s != "")
                    //    {
                    //        s = s + " ";
                    //    }
                    //    s = s + count.ToString();
                    //}
                    //s = s + " " + 
                    f = new Font("Courier New", 8);
                    var statusrect = new Rectangle(0, 0, settingsButton.Left, (int)size.Height + marginY);
                    g.DrawString(s, f, br, statusrect);
                    top = top + (int)size.Height + 2 * marginY;

                    f = new Font("Courier New", 9);
                    
                    if (sessionInfo.playerInfo == null || (sessionInfo.playerInfo.Count() == 0))
                    {
                        //Center text
                        size = g.MeasureString($"{sessionInfo.SessionID}", f);
                        var pf = new PointF(centerX - size.Width / 2, centerY - size.Height);
                        g.DrawString($"{sessionInfo.SessionID}", f, br, pf);
                    }
                    else
                    {
                        int playerIndex = 0;
                        foreach (var player in sessionInfo.playerInfo)
                        {
                            int logoWidth = 20;

                            if (player != null)
                            {

                                size = g.MeasureString(player.Name, f);

                                player.SetBox(new RectangleF(left + 1, top + marginY, nameWidth + logoWidth, size.Height));
                                // Logo (=Autonummer)
                                player.DrawNumber(g, new RectangleF(left + 1, top + marginY, logoWidth, size.Height));

                                // Name
                                RectangleF nameRect = new RectangleF(left + marginX + logoWidth, top + marginY, nameWidth - marginX, size.Height);
                                g.DrawString(player.Name, f, br, nameRect);

                                // Ronde informatie
                                RectangleF lapRect = new RectangleF(left + marginX + logoWidth + nameWidth + marginX, top + marginY,
                                                        driverListBox.Width - (left + marginX + logoWidth + nameWidth + marginX), size.Height);
                                if (selectedPlayer?.Id == player.Id)
                                {
                                    g.DrawRectangle(new Pen(new SolidBrush(Color.Green)), player.selectBox);
                                }

                                if (sessionInfo.circuit != null)
                                {
                                    sessionInfo.circuit.UpdateLapInfo(g, playerIndex, lapRect);
                                }
                                // Scheidings lijn
                                top = top + (int)nameRect.Height + 2 * marginY;
                                g.DrawLine(new Pen(Color.DimGray), 0, nameRect.Y + (int)size.Height,
                                                                        driverListBox.Width, nameRect.Y + (int)size.Height);
                                //LogLine($"{player.Name} Rect: ({nameRect.X},{nameRect.Y}-({nameRect.Width},{nameRect.Height})");
                            }
                            playerIndex++;
                        }
                    }
                    Rectangle funbox = new Rectangle(left + marginX, top + marginY * 4,
                                                        driverListBox.Width - marginX, driverListBox.Height - top - marginY * 4);
                    if (selectedLaps != null)
                    {
                        funbox.Height = funbox.Height / selectedLaps.Count;
                        foreach (var lap in selectedLaps)
                        {
                            g.FillRectangle(new SolidBrush(Color.White), funbox);
                            
                            Font largeFont = new Font("Comic Sans MS", 72);
                            var lapData = lap.lapInfo.Details.lap.lapData[lap.lapDataIndex].lapData;
                            var telemetry = lap.lapInfo.Details.lap.lapTelemetry[lap.telemetryIndex].carTelemetry;
                            
                            string speed = telemetry.speed.ToString();
                            size = g.MeasureString(speed, largeFont);
                            RectangleF speedRect = new RectangleF(funbox.Left, funbox.Top, size.Width, size.Height);
                            g.DrawString(speed, largeFont, br, speedRect);
                            var offset = size.Height + 3;
                            string distance = lapData.lapDistance.ToString();
                            size = g.MeasureString(distance, f);
                            RectangleF distanceRectRect = new RectangleF(funbox.Left, funbox.Top + offset, size.Width, size.Height);
                            g.DrawString(distance, f, br, speedRect);
                            funbox.Location = new Point(funbox.Left, funbox.Top + funbox.Height);

                            //String gear = cd.gear;
                            //String rpm= cd.engineRPM;
                            //String rev = cd.revLightsPercent;
                            //String steer = cd.steer;
                            //String throttle = cd.throttle;
                            //String surface = cd.surfaceType;
                            //String leftInner = cd.tyresInnerTemperature[0];
                            //String rightInner = cd.tyresOuterTemperature[1];
                            //String leftOuter = cd.tyresInnerTemperature[0];
                            //String rightOuter = cd.tyresOuterTemperature[1];
                            //LogLine($"{player.Name} Rect: ({nameRect.X},{nameRect.Y}-({nameRect.Width},{nameRect.Height})");
                        }

                    }
                    else if (selectedPlayer != null)
                    {
                        if (selectedPlayer.GetActualTelemetry() != null)
                        {
                            Font largeFont = new Font("Comic Sans MS", 72);
                            CarTelemetryData cd = (CarTelemetryData)selectedPlayer.GetActualTelemetry();
                            String speed = cd.speed.ToString();
                            int Rood = 255;
                            int Groen = 255;
                            int Blauw = 255;
                            if (cd.speed < 120)
                            {
                                Rood = 255 - cd.speed * 2;
                                Blauw = 255 - cd.speed * 2;
                            }
                            else if (cd.speed < 180)
                            {
                                Rood = 255 - (cd.speed - 120) * 2;
                                Groen = 255 - (cd.speed - 120)  * 2;
                            }
                            else
                            {
                                Groen = 255 - (cd.speed - 120);
                                Blauw = 255 - (cd.speed - 120);
                            }
                            g.FillRectangle(new SolidBrush(Color.FromArgb(Rood,Groen,Blauw)), funbox);
                            //String gear = cd.gear;
                            //String rpm= cd.engineRPM;
                            //String rev = cd.revLightsPercent;
                            //String steer = cd.steer;
                            //String throttle = cd.throttle;
                            //String surface = cd.surfaceType;
                            //String leftInner = cd.tyresInnerTemperature[0];
                            //String rightInner = cd.tyresOuterTemperature[1];
                            //String leftOuter = cd.tyresInnerTemperature[0];
                            //String rightOuter = cd.tyresOuterTemperature[1];
                            //LogLine($"{player.Name} Rect: ({nameRect.X},{nameRect.Y}-({nameRect.Width},{nameRect.Height})");
                            size = g.MeasureString(speed, largeFont);

                            RectangleF speedRect = new RectangleF(funbox.Left, funbox.Top, size.Width, size.Height);
                            g.DrawString(speed, largeFont, br, speedRect);
                        }
                        else
                        {
                            g.FillRectangle(new SolidBrush(Color.Blue), funbox);
                        }
                    }
                }
                e.Graphics.DrawImage(bitmap, new Point(0, 0));
            }
            UpdateTime[0] = UpdateTime[0] + (DateTime.Now - start);
            DrawingPlayers = false;

        }

        private void DriverListBox_Click(object sender, EventArgs e)
        {
            Point x = MousePosition;  // Mouseposition is dynamic, also during debugging :-((
                                      // Stored mouse position just for useful debugging
            Point c = driverListBox.PointToClient(x);
            if (closeButton.Contains(c))
            {
                IAmClosing = true;
                Close();
            }
            else if (settingsButton.Contains(c))
            {
                LogLine("Settings");
                IAmPausing = true;
                WindowState = FormWindowState.Minimized;
            }

            else if (pauseButton.Contains(c))
            {
                IAmPausing = !IAmPausing;
            }

            if (sessionInfo != null && sessionInfo.playerInfo != null)
            {
                foreach(var player in sessionInfo.playerInfo)
                {
                    if (player != null && player.selectBox.Contains(c))
                    {
                        selectedPlayer = player;
                    }
                }
            }
        }

        public void AddPacket(PacketInfo packet)
        {
            packetCount[packet.header.packetId]++;
            sessionTime = PacketHelper.UnixTimeStampToDateTime(packet.header.sessionTime);
        }

        float minx = 0, miny = 0, minz = 0, maxx = 1, maxy = 1, maxz = 0;

        bool DrawingCircuit = false;
        bool DrawingPlayers = false;
        bool DrawingTelemetry = false;

        private float N(float X)
        {
            if (X>0)
            {
                return 4;
            }
            if (X<0)
            {
                return -4;
            }
            return 0;
        }

        private void circuitBox_Paint(object sender, PaintEventArgs e)
        {
            int paints = 0;
            if (DrawingCircuit || (sessionInfo == null))
            {
                // LogLine("Abort!");
                return;
            }
            DrawingCircuit = true;
            DateTime start = DateTime.Now;
            PacketMotionData dat;
            lock (sessionInfo)
            {
                if (sessionInfo == null ||
                        sessionInfo.playerInfo == null ||
                        sessionInfo.circuit == null ||
                        sessionInfo.circuit.motionInfo.carMotionData == null)
                {
                    // LogLine($"Abort 2! { (sessionInfo == null ? "s" : "") } { (sessionInfo.playerInfo == null ? "p" : "") } { (sessionInfo?.circuit == null ? "c" : "") } { (sessionInfo?.circuit?.motionInfo.carMotionData == null ? "m" : "") }");
                    DrawingCircuit = false;
                    // No Session
                    return;
                }
                //clone
                dat = sessionInfo.circuit.motionInfo;
            }
            UpdateCount[1]++;
            Bitmap bitmap = new Bitmap(circuitBox.Width, circuitBox.Height);

            // Draw with double-buffering.
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                var f = new Font("Arial", 10);
                var br = new SolidBrush(Color.Black);

#if EXTRA_DEBUG                
                String sInfo = $"{UpdateCount[0]}:{(int)UpdateTime[0].Seconds}.{UpdateTime[0].Milliseconds:00} ";
                sInfo = sInfo + $"{UpdateCount[1]}:{(int)UpdateTime[1].Seconds}.{UpdateTime[1].Milliseconds:00} ";
                var pf = new PointF(circuitBox.Width / 2 - size.Width / 2, circuitBox.Height / 2 - size.Height);
                var size = g.DrawString(sInfo, f, br, pf);
#endif

                foreach (CarMotionData cmd in dat.carMotionData)
                {
                    // Validate if we are looking in the right window
                    if (cmd.worldPositionX < minx) minx = cmd.worldPositionX;
                    if (cmd.worldPositionX > maxx) maxx = cmd.worldPositionX;
                    if (cmd.worldPositionY < miny) miny = cmd.worldPositionY;
                    if (cmd.worldPositionY > maxy) maxy = cmd.worldPositionY;
                    if (cmd.worldPositionZ < minz) minz = cmd.worldPositionZ;
                    if (cmd.worldPositionZ > maxz) maxz = cmd.worldPositionZ;
                }
                if (maxz == 0 && minz == 0)
                {
                    foreach (MotionInContext cmd in sessionInfo.circuit.motionLog)
                    {
                        if (cmd.carMotion.worldPositionX < minx) minx = cmd.carMotion.worldPositionX;
                        if (cmd.carMotion.worldPositionX > maxx) maxx = cmd.carMotion.worldPositionX;
                        if (cmd.carMotion.worldPositionY < miny) miny = cmd.carMotion.worldPositionY;
                        if (cmd.carMotion.worldPositionY > maxy) maxy = cmd.carMotion.worldPositionY;
                        if (cmd.carMotion.worldPositionZ < minz) minz = cmd.carMotion.worldPositionZ;
                        if (cmd.carMotion.worldPositionZ > maxz) maxz = cmd.carMotion.worldPositionZ;
                    }
                }

                PointF bottomW = Translate(maxx, maxy, maxz);
                PointF topW = Translate(minx, miny, minz);
                PointF sizeW = new PointF(bottomW.X - topW.X, bottomW.Y - topW.Y); //The Size of the world

                float scalexS = (float)(circuitBox.Width - 30) / (sizeW.X);  // W.X * scaleX
                float scaleyS = (float)(circuitBox.Height - 30) / (sizeW.Y);

                PointF offsetS = new PointF(topW.X * scalexS, topW.Y * scaleyS);
                int index = 0;
                lock (sessionInfo.circuit.motionLog)
                {
                    foreach (MotionInContext cmd in sessionInfo.circuit.motionLog)
                    {
                        // Calculate world coordinates
                        var m = cmd.carMotion;
                        PointF lineStartW = Translate(cmd.carMotion.worldPositionX, cmd.carMotion.worldPositionY, cmd.carMotion.worldPositionZ);
                        PointF lineEndW = Translate(cmd.carMotion.worldPositionX - cmd.carMotion.worldVelocityX / 5, 
                                                    cmd.carMotion.worldPositionY - cmd.carMotion.worldVelocityY / 5, 
                                                    cmd.carMotion.worldPositionZ - cmd.carMotion.worldVelocityZ / 5);
                        // PointF lineEndW = Translate(cmd.worldPositionX + N(cmd.worldVelocityX), cmd.worldPositionY + N(cmd.worldVelocityY), cmd.worldPositionZ + N(cmd.worldVelocityZ));
                        // Convert to Screen coordinates
                        PointF lineStartS = new PointF(lineStartW.X * scalexS - offsetS.X, lineStartW.Y * scaleyS - offsetS.Y);
                        PointF lineEndS = new PointF(lineEndW.X * scalexS - offsetS.X, lineEndW.Y * scaleyS - offsetS.Y);
                        g.DrawLine(new Pen(new SolidBrush(Color.GreenYellow), 2), lineStartS, lineEndS);
                    }
                }
                if (selectedLaps == null)
                {
                    foreach (CarMotionData cmd in dat.carMotionData)
                    {
                        PointF positionW = Translate(cmd.worldPositionX, cmd.worldPositionY, cmd.worldPositionZ);
                        PointF positionS = new PointF(positionW.X * scalexS - offsetS.X, positionW.Y * scaleyS - offsetS.Y);
                        RectangleF boundRect = new RectangleF(positionS.X, positionS.Y, 20, 18);
                        if (index < sessionInfo.playerInfo.Count())
                        {
                            paints++;
                            sessionInfo.GetPlayerByIndex(index)?.DrawNumber(g, boundRect);
                        }
                        index++;
                    }
                }
                else
                {
                    foreach (var lap in selectedLaps)
                    {
                        var motion = lap.lapInfo.Details.lap.lapMotion[lap.motionIndex];
                        var cmd = motion.carMotion;
                        PointF positionW = Translate(cmd.worldPositionX, cmd.worldPositionY, cmd.worldPositionZ);
                        PointF positionS = new PointF(positionW.X * scalexS - offsetS.X, positionW.Y * scaleyS - offsetS.Y);
                        RectangleF boundRect = new RectangleF(positionS.X, positionS.Y, 20, 18);
                        if (index < sessionInfo.playerInfo.Count())
                        {
                            paints++;
                            sessionInfo.GetPlayerByIndex(index)?.DrawNumber(g, boundRect);
                        }
                        index++;
                    }
                }

            }
            e.Graphics.DrawImage(bitmap, new Point(0, 0));
            // bitmap.Save("images\\"+UpdateCount[1].ToString()+".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
            UpdateTime[1] = UpdateTime[1] + (DateTime.Now - start);
            DrawingCircuit = false;
        }

        public void EndSession(PacketHeader data)
        {
            if (sessionInfo != null)
            {
                LogLine($"Ending session: {data.sessionUID}");
            }
            while (DrawingCircuit || DrawingPlayers || DrawingTelemetry)
            {
                Thread.Sleep(1);
            }
        }

        private void ScreenUpdateTimer_Tick(object sender, EventArgs e)
        {
            // Screen updates happen indepedent from data input. The timer may be adjusted to refresh rate of data and screen, to enable more fluent screen painting, but for now this wil do.
            driverListBox.Invalidate();
            circuitBox.Invalidate();
            if (lapLineBox.Visible)
            {
                lapLineBox.Invalidate();
            }
        }

        public void StartSession(PacketHeader data)
        {
            if ((sessionInfo != null) && (sessionInfo.SessionID != data.sessionUID.ToString()))
            {
                if (pr != null)
                {
                    if (pr.ReaderMode == ReaderMode.rmRecord)
                    {
                        // New Session during recording.
                        // Save data of current Session and continue
                        pr.SaveData();
                    }
                    selectedPlayer = null;
                    sessionInfo = null;
                }
                else
                {
                    LogLine($"Ignoring sessionstart: {data.sessionUID}");
                }
            }

            if (sessionInfo == null)
            {
                sessionInfo = new SessionInfo(data);
            }
        }

        public void UpdateSession(PacketHeader header, PacketSessionData data)
        {
            if (sessionInfo == null)
            {
                sessionInfo = new SessionInfo(header);
                lock (sessionInfo)
                {
                    sessionInfo.Update(data);
                }

                LogLine($"New session`detected: {header.sessionUID}");
            }
            else
            {
                lock (sessionInfo)
                {
                    sessionInfo.Update(data);
                }
            }
        }

        private void ReplayButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (!Reset())
            {
                return;
            }

            using (var dlg = new OpenFileDialog() { Filter = "Data File|*.data|All files|*.*" })
            {
                if (dlg.ShowDialog() == DialogResult.Cancel)
                {
                    return;
                }
                selectedFileName = dlg.FileName;
            }

            pr = new PacketReader(this);
            cancelSource = new CancellationTokenSource();
            ActiveTask = new Task(() =>
            {
                pr.Read(ReaderMode.rmReplay, cancelSource.Token);
            }
            , cancelSource.Token);
            ActiveTask.ContinueWith((x) => { LogLaps(x); });
            ActiveTask.Start();
            ScreenUpdateTimer.Enabled = true;
        }


        private void StartOrStopRecording()
        {
            if (!Reset())
            {
                return;
            }

            pr = new PacketReader(this);
            cancelSource = new CancellationTokenSource();

            ActiveTask = new Task(() =>
            {
                pr.Read(ReaderMode.rmRecord, cancelSource.Token);
            }, cancelSource.Token);
            ActiveTask.ContinueWith((x) => { LogLaps(x); });
            ActiveTask.Start();
            ScreenUpdateTimer.Enabled = true;
        }

        private void RecordButton_Click(object sender, EventArgs e)
        {
            StartOrStopRecording();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Zie MouseClick
        }

        public void UpdateMotion(PacketHeader context, PacketMotionData data)
        {
            if (sessionInfo == null || data.carMotionData == null)
            {
                LogLine("Error: Unexpected motion update. Not in session");
            }
            else
            {
                lock (sessionInfo)
                {
                    sessionInfo.Update(context, data);
                }
            }
        }

        public bool IsPausing()
        {
            return IAmPausing;
        }

        int lastY = -1;
        int lastX = -1;
        private void circuitBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (lastY == -1)
                {
                    lastY = e.Y;
                    lastX = e.X;
                }
                else
                {
                    UpdateAngles(lastY - e.Y, lastX - e.X);
                }
            }

        }

        private void UpdateAngles(int dX, int dY)
        {
            xFactor = (float)Math.Sin((2f * Math.PI) * (30 + dX) / 360f);
            yFactor = (float)Math.Cos((2f * Math.PI) * (30 + dY) / 360f);
        }

        private void circuitBox_MouseUp(object sender, MouseEventArgs e)
        {
            lastY = -1;
        }

        public void UpdateLapdata(PacketHeader context, PacketLapData lapData)
        {
            // Context is ignored. Lapdata is not stored.
            if (sessionInfo == null)
            {
                return;
            }
            lock (sessionInfo)
            {
                sessionInfo.Update(context, lapData, this);
            }
        }

        public void UpdateTelemetry(PacketHeader context, PacketCarTelemetryData telemetryData)
        {
            if (sessionInfo == null)
            {
                return;
            }
            lock (sessionInfo)
            {
                sessionInfo.Update(context, telemetryData);
            }
        }


        private void TelemetryForm_Shown(object sender, EventArgs e)
        {
            var LapLoader = new Task(() =>
            {
                LapDatabase.LoadDictionary(this);
            });
            //LapLoader.ContinueWith((x) => { LoadingDone(x); });
            LapLoader.Start();

            StartOrStopRecording();
        }

        private void TelemetryForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!Reset())
            {
                e.Cancel = true;
            }
        }

        List<LapAnalyzer> selectedLaps = null;

        private void btnAddCompareLap_Click(object sender, EventArgs e)
        {
            if (Reset())
            {
                LapSelect ls = new LapSelect();
                if (ls.ShowDialog() == DialogResult.OK && ls.GetSelectedLaps().Count>0)
                {
                    selectedLaps = new List<LapAnalyzer>();
                    lapLineBox.Visible = true;
                    foreach (var lap in ls.GetSelectedLaps())
                    {
                        lap.LoadDetails();
                        selectedLaps.Add(new LapAnalyzer()
                        {
                            lapInfo = lap,
                            motionIndex = 0,
                            offset = DateTime.Now - PacketHelper.UnixTimeStampToDateTime(lap.Details.lap.lapMotion.First().context.sessionTime),
                            telemetryIndex = 0
                        }) ;
                        var lapInfo = lap.Details.lap.lapData[0].lapData;
                        // LogLine($"0 Status={lapInfo.driverStatus} Result={lapInfo.resultStatus} Sector={lapInfo.sector} inv={lapInfo.currentLapInvalid} lap={lapInfo.currentLapNum}");
                        //foreach (var lapTimings in lap.Details.lap.lapData)
                        //{
                        //    if ((lapTimings.lapData.driverStatus != lapInfo.driverStatus) ||
                        //       (lapTimings.lapData.resultStatus != lapInfo.resultStatus) ||
                        //       (lapTimings.lapData.sector != lapInfo.sector) ||
                        //       (lapTimings.lapData.currentLapNum != lapInfo.currentLapNum) ||
                        //       (lapTimings.lapData.currentLapInvalid != lapInfo.currentLapInvalid))
                        //    {
                        //        lapInfo = lapTimings.lapData;
                        //        LogLine($"{lapTimings.context.frameIdentifier} Status={lapInfo.driverStatus} Result={lapInfo.resultStatus} Sector={lapInfo.sector} inv={lapInfo.currentLapInvalid} lap={lapInfo.currentLapNum}");
                        //    }
                        //}
                        LogLine($"Loaded {lap.FileName}");
                    }
                    lock(sessionInfo = new SessionInfo(selectedLaps));
                    DrawTelemetryBitmap();
                    replayTimer.Enabled = true;
                }
            }
        }

        private void replayTimer_Tick(object sender, EventArgs e)
        {
            if (selectedLaps == null)
            {
                return;
            }

            foreach(var lap in selectedLaps)
            {
                // Progress time in recorded lap (if needed)
                //          Notice:
                //                  while( < .Count-1 ) // Last lapMotion will stay 'active'
                //                  lap.offset          // set at loading time. 
                //                                         This makes the sessionTime in the recorded lap relative      
                while (lap.motionIndex < (lap.lapInfo.Details.lap.lapMotion.Count-1) &&
                      PacketHelper.UnixTimeStampToDateTime(lap.lapInfo.Details.lap.lapMotion[lap.motionIndex].context.sessionTime) 
                      <(DateTime.Now - lap.offset))
                {
                    lap.motionIndex++;
                }
                while (lap.telemetryIndex < (lap.lapInfo.Details.lap.lapTelemetry.Count-1) &&
                      PacketHelper.UnixTimeStampToDateTime(lap.lapInfo.Details.lap.lapTelemetry[lap.telemetryIndex].context.sessionTime)
                      < (DateTime.Now - lap.offset))
                {
                    lap.telemetryIndex++;
                }
                while (lap.lapDataIndex < (lap.lapInfo.Details.lap.lapData.Count - 1) &&
                      PacketHelper.UnixTimeStampToDateTime(lap.lapInfo.Details.lap.lapTelemetry[lap.lapDataIndex].context.sessionTime)
                      < (DateTime.Now - lap.offset))
                {
                    lap.lapDataIndex++;
                }
            }
      //      sessionInfo.Update(re)
        }

        private void lapLineBox_Paint(object sender, PaintEventArgs e)
        {
            if (DrawingTelemetry || (sessionInfo == null))
            {
                // LogLine("Abort!");
                return;
            }
            DrawingTelemetry = true;
            e.Graphics.DrawImage(telemetryBitmap, new Point(0, 0));

            var trackPixels = lapLineBox.Width - 12;
            var length = sessionInfo.circuit.circuitData.trackLength;
            float meterPerPixel = (float)length / (float)trackPixels;
            var top = 0;

            var SpeedPen = new Pen(new SolidBrush(Color.Yellow), 2);
            var ThrottlePen = new Pen(new SolidBrush(Color.LightGreen), 1);
            var BrakePen = new Pen(new SolidBrush(Color.LightPink), 1);
            var SteerPen = new Pen(new SolidBrush(Color.White), 2);
            var PositionPen = new Pen(new SolidBrush(Color.Gray), 1);

            foreach (var lap in selectedLaps)
            {
                var myBox = new Rectangle(6, top, lapLineBox.Width - 12, lapLineBox.Height / selectedLaps.Count);

                var data = lap.lapInfo.Details.lap.lapData[lap.lapDataIndex].lapData;
                var telemetry = lap.lapInfo.Details.lap.lapTelemetry[lap.telemetryIndex].carTelemetry;
                PointF positionBottom = new PointF(myBox.Left + data.lapDistance / meterPerPixel, myBox.Bottom);
                PointF positionTop = new PointF(myBox.Left + data.lapDistance / meterPerPixel, myBox.Top);

                float boxHeight = (float)myBox.Height;
                PointF Throttle = new PointF(positionBottom.X, positionBottom.Y - boxHeight * telemetry.throttle); // Scale 0.0 - 1.0
                PointF Brake = new PointF(positionBottom.X, positionBottom.Y - (boxHeight * (1 - telemetry.brake))); // Scale 0.0 - 1.0
                PointF Steering = new PointF(positionBottom.X, positionBottom.Y - (boxHeight / 2 * (1 - telemetry.steer))); // Scale -1.0 - 1.0
                PointF Speed = new PointF(positionBottom.X, positionBottom.Y - (boxHeight * ((float)telemetry.speed / 350f))); // Scale: 0 .. 350 (assumed max speed)
                                                                                                                               // Throttle is drawn from the bottom up
                e.Graphics.DrawLine(PositionPen, positionBottom, positionTop);

                e.Graphics.DrawLine(ThrottlePen, positionBottom, Throttle);
                // Brake is drawn from top down
                e.Graphics.DrawLine(BrakePen, positionTop, Brake);
                // Drawing current speed as a Point
                e.Graphics.DrawLine(SpeedPen, Speed, new PointF(Speed.X+1,Speed.Y));
                // Drawing current Steering as a Point
                e.Graphics.DrawLine(SteerPen, Steering, new PointF(Steering.X + 1, Steering.Y));

                top = top + myBox.Height; 

            }
            DrawingTelemetry = false;
        }

        Bitmap telemetryBitmap = null;
        private void DrawTelemetryBitmap()
        {
            telemetryBitmap = new Bitmap(lapLineBox.Width, lapLineBox.Height);

            // Draw with double-buffering.
            using (Graphics g = Graphics.FromImage(telemetryBitmap))
            {
                var trackPixels = lapLineBox.Width - 12;
                var length = sessionInfo.circuit.circuitData.trackLength;
                float meterPerPixel = (float)length / (float)trackPixels;
                
                // 6 pixels left, 6 pixels right
                var top = 0;
                var SpeedPen = new Pen(new SolidBrush(Color.White),2);
                var ThrottlePen = new Pen(new SolidBrush(Color.Green), 1);
                var BrakePen = new Pen(new SolidBrush(Color.Red), 1);
                var SteerPen = new Pen(new SolidBrush(Color.HotPink), 2);
                foreach (var lap in selectedLaps)
                {
                    var myBox = new Rectangle(6, top, lapLineBox.Width-12, lapLineBox.Height / selectedLaps.Count);
                    int motionIndex = 0; 
                    int telemetryIndex = 0; 
                    int lapDataIndex = 0;
                    var lapTelemetry = lap.lapInfo.Details.lap.lapTelemetry;
                    var lapMotion = lap.lapInfo.Details.lap.lapMotion;
                    var lapData = lap.lapInfo.Details.lap.lapData;
                    while (motionIndex < lapMotion.Count)
                    {
                        var motion = lapMotion[motionIndex].carMotion;
                        var context = lapMotion[motionIndex].context;
                        while ((Math.Abs(lapTelemetry[telemetryIndex].context.sessionTime - context.sessionTime)>0.01) &&
                                (lapTelemetry[telemetryIndex].context.sessionTime < context.sessionTime) &&
                                (lapTelemetry.Count-1 > telemetryIndex))
                        {
                            telemetryIndex++;
                        }
                        while ((Math.Abs(lapData[lapDataIndex].context.sessionTime - context.sessionTime) > 0.01) &&
                                (lapData[lapDataIndex].context.sessionTime < context.sessionTime) &&
                                (lapData.Count - 1 > lapDataIndex))
                        {
                            lapDataIndex++;
                        }
                        var telemetry = lapTelemetry[telemetryIndex].carTelemetry;
                        var data = lapData[lapDataIndex].lapData;

                        PointF positionBottom = new PointF(myBox.Left + data.lapDistance / meterPerPixel, myBox.Bottom);
                        PointF positionTop = new PointF(myBox.Left + data.lapDistance / meterPerPixel, myBox.Top);

                        float boxHeight = (float) myBox.Height;

                        //-1 = boxHeight     (
                        // 0 = boxHeight/2   1 * boxHeight/2 - Steer * boxHeight/2
                        // 1 = 0

                        PointF Throttle = new PointF(positionBottom.X, positionBottom.Y - boxHeight * telemetry.throttle); // Scale 0.0 - 1.0
                        PointF Brake = new PointF(positionBottom.X, positionBottom.Y - (boxHeight * (1-telemetry.brake))); // Scale 0.0 - 1.0
                        PointF Steering = new PointF(positionBottom.X, positionBottom.Y - (boxHeight / 2 * (1 - telemetry.steer))); // Scale -1.0 - 1.0
                        PointF Speed = new PointF(positionBottom.X, positionBottom.Y - (boxHeight * ((float) telemetry.speed / 350f))); // Scale: 0 .. 350 (assumed max speed)
                        // Throttle is drawn from the bottom up
                        g.DrawLine(ThrottlePen, positionBottom, Throttle);
                        // Brake is drawn from top down
                        g.DrawLine(BrakePen, positionTop, Brake);
                        // Drawing current speed as a Point
                        g.DrawLine(SpeedPen, Speed, new PointF(Speed.X-1,Speed.Y));
                        // Drawing current Steering as a Point
                        g.DrawLine(SteerPen, Steering, new PointF(Steering.X-1, Steering.Y));
                        motionIndex++;
                    }
                    //if (index < sessionInfo.playerInfo.Count())
                    //{
                    //    paints++;
                    //    sessionInfo.GetPlayerByIndex(index)?.DrawNumber(g, boundRect);
                    //}
                    //index++;
                    top = top + myBox.Height;
                }
            }
        }

        private void lapLineBox_SizeChanged(object sender, EventArgs e)
        {
            if (lapLineBox.Visible && replayTimer.Enabled)
            {
                DrawTelemetryBitmap();
            }
        }

        private bool Reset()
        {
            // Stop replay
            replayTimer.Enabled = false;
            if (ActiveTask != null)
            {
                if (sessionInfo == null || MessageBox.Show("Wilt u de huidige taak stoppen?", "Reset", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    cancelSource.Cancel();
                }
                else
                {
                    return false;
                }
            }
            while (ActiveTask != null)
            {
                // Give Active task some time to finish
                Thread.Sleep(100);
            }

            if (sessionInfo != null)
            {
                lock (sessionInfo)
                {
                    selectedPlayer = null;
                    sessionInfo = null;
                }
            }

            pr = null;
            sessionTime = DateTime.MinValue;
            lapLineBox.Visible = false;

            return true;
        }

        private void Analyze_Click(object sender, EventArgs e)
        {
            if (!Reset())
            {
                return;
            }
            using (var dlg = new OpenFileDialog() { Filter = "Data File|*.data|All files|*.*" })
            {
                if (dlg.ShowDialog() == DialogResult.Cancel)
                {
                    return;
                }
                selectedFileName = dlg.FileName;
            }

            pr = new PacketReader(this);
            cancelSource = new CancellationTokenSource();
            ActiveTask = new Task(() =>
            {
                pr.Read(ReaderMode.rmAnalyze, cancelSource.Token);
            }
            , cancelSource.Token);
            ActiveTask.ContinueWith((x) => { LogLaps(x); });
            ActiveTask.Start();

            ScreenUpdateTimer.Enabled = true;
        }

        public void LogLaps(Task t)
        {
            if (sessionInfo != null)
            {
                if (sessionInfo.playerInfo != null)
                {
                    foreach (var player in sessionInfo.playerInfo)
                    {
                        if (player != null)
                        {
                            LogLine($"{player.CarNumber} {player.Name}");
                            foreach (var lap in player.lapStore)
                            {
                                if (lap.Value.started)
                                {
                                    var ti = lap.Value.lapTimings;

                                    // if { ti.bestLapNum}={ TS(ti.bestLapTime)}
                                    LogLine($" {lap.Key} {(lap.Value.valid ? "OK" : "--")} {(lap.Value.complete ? "OK" : "--")}" +
                                        $" current {ti.currentLapNum}={TS(ti.currentLapTime)}"+
                                        $" {ti.driverStatus} {ti.currentLapInvalid} {ti.resultStatus} {ti.lapDistance} {ti.totalDistance}"+
                                        $" ({lap.Value.lapTelemetry.Count}) ");
                                }
                                else
                                {
                                    // LogLine($" {lap.Key}  Incufficiant data {lap.Value.lapTimings.currentLapNum}:{lap.Value.lapTimings.currentLapTime} ");
                                }
                            }
                        }
                    }
                }
                lock (sessionInfo)
                {
                    ActiveTask = null;
                }
            }
            else
            {
                ActiveTask = null;
            }
        }

        public string SessionID
        {
            get
            {
                if (sessionInfo == null) { return "no_session"; }
                return sessionInfo.SessionID;
            }
        }

        public PointF Translate(float X, float Y, float Z)
        {
            return (new PointF(X + xFactor * Y, Z + yFactor * Y));
        }
    }
}