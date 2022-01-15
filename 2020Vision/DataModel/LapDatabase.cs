using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Vision2020
{
    [Serializable]
    public class LapInfo
    {
        public string CircuitName { get; set; }
        public string PlayerName { get; set; }
        public string TeamName { get; set; }
        public string SessionType { get; set; }
        public string CarNumber { get; set; }
        public float LapTime { get; set; }
        public string FileName { get; set; }
        public string Tyre { get; set; }
        [JsonIgnore]
        public CompletedLap Details { get; set; }
        public void LoadDetails()
        {
            try
            {
                Details = new CompletedLap(LapDatabase.GetFileName(this));
            }
            catch (Exception e)
            {
                Details = null;
                LastError = e.Message;
            }
        }
        [JsonIgnore]
        public string LastError { get; set; }

    }
    public class LapAnalyzer
    {
        public TimeSpan offset;
        public int motionIndex;
        public int telemetryIndex;
        public int lapDataIndex;
        public LapInfo lapInfo;
    }

    public class CompletedLap
    {
        public PacketSessionData circuitInfo { get; set; }
        public ParticipantData playerInfo { get; set; }
        public LapRecording lap { get; set; }

        public string FileName()
        {

            return Constants.GetName(Constants.StockType.stSessionType, circuitInfo.sessionType) + (playerInfo.aiControlled == 1 ? " AI " : " ") +
                playerInfo.raceNumber.ToString()+" "+ PacketHelper.GetString(playerInfo.name, 48) + (circuitInfo.formula == 0 ? " " : " F" + (circuitInfo.formula + 1).ToString() + " ") +
                lap.lapTime.ToString("0.000").Replace(",", ".");
        }

        public string Dirname()
        {
            return Constants.TrackList.First(t => t.id == circuitInfo.trackId).name;
        }

        public string Pathname()
        {
            return Dirname() + "\\" + FileName();
        }

        public CompletedLap(string filename)
        {
            Stream fData = new FileStream(filename, FileMode.Open, FileAccess.Read);
            byte[] data = new byte[fData.Length];
            long totalRead = 0;
            while(totalRead < fData.Length)
            {
                totalRead += fData.Read(data, (int) totalRead, (int)fData.Length);
            }

            MemoryStream fIn = new MemoryStream(data);


            int version = PacketHelper.SafeRead<int>(fIn, PacketSize.IntSize);

            if (version != 211)
            {
                // Invalid;
                return;
            }

            lap = new LapRecording()
            {
                lapTelemetry = new List<TelemetryInContext>(),
                lapMotion = new List<MotionInContext>(),
                lapData = new List<LapDataInContext>()
            };

            circuitInfo = PacketHelper.SafeRead<PacketSessionData>(fIn, PacketSize.PacketSessionDataSize);
            playerInfo = PacketHelper.SafeRead<ParticipantData>(fIn, PacketSize.ParticipantDataSize);
            lap.complete = PacketHelper.SafeRead<bool>(fIn, PacketSize.BoolSize);
            lap.FirstTiming = PacketHelper.SafeRead<LapData>(fIn, PacketSize.LapDataSize);
            if (version == 210)
            {
                var lapTimeAsFloat = PacketHelper.SafeRead<float>(fIn, PacketSize.FloatSize);
                lap.lapTimeInMs = (UInt32) Math.Round(lapTimeAsFloat * 1000f, 0);
            }
            else
            {
                lap.lapTimeInMs = PacketHelper.SafeRead<UInt32>(fIn, PacketSize.UInt32Size);
            }
            lap.started = PacketHelper.SafeRead<bool>(fIn, PacketSize.BoolSize);
            lap.valid = PacketHelper.SafeRead<bool>(fIn, PacketSize.BoolSize);
            lap.Setup = PacketHelper.SafeRead<CarSetupData>(fIn, PacketSize.CarSetupDataSize);
            var timingCount = PacketHelper.SafeRead<int>(fIn, PacketSize.IntSize);
            
            for (int i = 0; i<timingCount; i++)
            {
                lap.lapMotion.Add(PacketHelper.SafeRead<MotionInContext>(fIn, PacketSize.MotionInContextSize));
            }
            var telemetryCount = PacketHelper.SafeRead<int>(fIn, PacketSize.IntSize);
            for (int i = 0; i < telemetryCount; i++)
            {
                lap.lapTelemetry.Add(PacketHelper.SafeRead<TelemetryInContext>(fIn, PacketSize.TelemetryInContextSize));
            }
            var lapDataCount = PacketHelper.SafeRead<int>(fIn, PacketSize.IntSize);
            for (int i = 0; i < lapDataCount; i++)
            {
                lap.lapData.Add(PacketHelper.SafeRead<LapDataInContext>(fIn, PacketSize.LapDataInContextSize));
            }
            fIn.Close();
        }

        public CompletedLap()
        {
        }

        public void Save(string baseDir)
        {
            var fileName = BS(baseDir) + Pathname();
            Stream fOut = new FileStream(fileName, FileMode.Create, FileAccess.Write);

            PacketHelper.SafeWrite<int>(fOut,211, PacketSize.IntSize); // Version Number

            PacketHelper.SafeWrite<PacketSessionData>(fOut, circuitInfo, PacketSize.PacketSessionDataSize);
            PacketHelper.SafeWrite<ParticipantData>(fOut, playerInfo, PacketSize.ParticipantDataSize);

            PacketHelper.SafeWrite<bool>(fOut, lap.complete, PacketSize.BoolSize);
            PacketHelper.SafeWrite<LapData>(fOut, lap.FirstTiming, PacketSize.LapDataSize);
            PacketHelper.SafeWrite<UInt32>(fOut, lap.lapTimeInMs, PacketSize.UInt32Size);
            PacketHelper.SafeWrite<bool>(fOut, lap.started, PacketSize.BoolSize);
            PacketHelper.SafeWrite<bool>(fOut, lap.valid, PacketSize.BoolSize);
            PacketHelper.SafeWrite<CarSetupData>(fOut, lap.Setup, PacketSize.CarSetupDataSize);

            PacketHelper.SafeWrite<int>(fOut, lap.lapMotion.Count, PacketSize.IntSize);            
            foreach (var motiondata in lap.lapMotion)
            {
                PacketHelper.SafeWrite<MotionInContext>(fOut, motiondata, PacketSize.MotionInContextSize);
            }
            
            PacketHelper.SafeWrite<int>(fOut, lap.lapTelemetry.Count, PacketSize.IntSize);
            foreach(var lapTelemetry in lap.lapTelemetry)
            {
                PacketHelper.SafeWrite<TelemetryInContext>(fOut, lapTelemetry, PacketSize.TelemetryInContextSize);
            }
            
            PacketHelper.SafeWrite<int>(fOut, lap.lapData.Count, PacketSize.IntSize);
            foreach (var lapData in lap.lapData)
            {
                PacketHelper.SafeWrite<LapDataInContext>(fOut, lapData, PacketSize.LapDataInContextSize);
            }
            
            fOut.Close();

        }
        private static String BS(String s)
        {
            if (s.EndsWith("\\"))
            {
                return s;
            }
            return s + "\\";
        }
    }

    public static class LapDatabase
    {
        public static string baseDir = Directory.GetCurrentDirectory();
        public static String BS(String s)
        {
            if (s.EndsWith("\\"))
            {
                return s;
            }
            return s + "\\";
        }

        public static String GetFileName(LapInfo l)
        {
            return BS(baseDir) + BS(l.CircuitName) + l.FileName;
        }

        private static string dictFileName = "Vision2020.dict";

        public static List<LapInfo> Laps = new List<LapInfo>();
        public static void Add(CompletedLap completedLap)
        {
            if (Laps.FirstOrDefault(l => l.FileName == completedLap.Pathname()) == null)
            {
                // New Lap. Save to disk
                Directory.CreateDirectory(BS(baseDir) + completedLap.Dirname());
                if (!File.Exists(BS(baseDir) + completedLap.Pathname()))
                {
                    completedLap.Save(baseDir);
                    Laps.Add(
                        new LapInfo()
                        {
                            CarNumber = completedLap.playerInfo.raceNumber.ToString(),
                            CircuitName = Constants.TrackList.First(t => t.id == completedLap.circuitInfo.trackId)?.name,
                            FileName = completedLap.FileName(),
                            LapTime = completedLap.lap.lapTime,
                            PlayerName = PacketHelper.GetString(completedLap.playerInfo.name, 48),
                            SessionType = Constants.SessionTypeList.First(t => t.id == completedLap.circuitInfo.sessionType)?.name,
                            TeamName = Constants.TeamList.First(t => t.id == completedLap.playerInfo.teamId)?.name,
                            Tyre = "?",  // Packet type Car Status not handled yet 
                            Details = completedLap
                        });
                    SaveDictionary();
                }
            }
        }
        private static void UnitTest()
        {
            var testfileName = "unittest.data";
            Stream fOut = new FileStream(testfileName, FileMode.Create, FileAccess.Write);

            bool waar = true;
            int drie = 3;
            bool onwaar = false;
            float vloot = 17.5f;

            PacketHelper.SafeWrite<bool>(fOut, waar, PacketSize.BoolSize);
            PacketHelper.SafeWrite<bool>(fOut, onwaar, PacketSize.BoolSize);
            PacketHelper.SafeWrite<int>(fOut, drie, PacketSize.IntSize);
            PacketHelper.SafeWrite<float>(fOut, vloot, PacketSize.FloatSize);
            PacketHelper.SafeWrite<bool>(fOut, waar, PacketSize.BoolSize);
            PacketHelper.SafeWrite<bool>(fOut, onwaar, PacketSize.BoolSize);

            fOut.Close();

            Stream fIn = new FileStream(testfileName, FileMode.Open, FileAccess.Read);

            waar = PacketHelper.SafeRead<bool>(fIn, PacketSize.BoolSize);
            if (waar != true) { throw new Exception("fout"); };
            onwaar = PacketHelper.SafeRead<bool>(fIn, PacketSize.BoolSize);
            if (onwaar != false) { throw new Exception("fout"); };
            drie = PacketHelper.SafeRead<int>(fIn, PacketSize.IntSize);
            if (drie != 3) { throw new Exception("fout"); };
            vloot = PacketHelper.SafeRead<float>(fIn, PacketSize.FloatSize);
            if (vloot != 17.5f) { throw new Exception("fout"); };
            waar = PacketHelper.SafeRead<bool>(fIn, PacketSize.BoolSize);
            if (waar != true) { throw new Exception("fout"); };
            onwaar = PacketHelper.SafeRead<bool>(fIn, PacketSize.BoolSize);
            if (onwaar != false) { throw new Exception("fout"); };

            fIn.Close();
        }

        private static void Load(string fileName)
        {

            if (File.Exists(fileName))
            {
                try
                {
                    var newLap = new CompletedLap(fileName);
                    if (newLap.lap == null || newLap.lap.lapTime == 0)
                    {
                        // Ignored invalid lap                    
                    }
                    else
                    {
                        Laps.Add(
                            new LapInfo()
                            {
                                CarNumber = newLap.playerInfo.raceNumber.ToString(),
                                CircuitName = Constants.TrackList.First(t => t.id == newLap.circuitInfo.trackId).name,
                                FileName = newLap.FileName(),
                                LapTime = newLap.lap.lapTime,
                                PlayerName = PacketHelper.GetString(newLap.playerInfo.name, 48),
                                SessionType = Constants.SessionTypeList.First(t => t.id == newLap.circuitInfo.sessionType).name,
                                TeamName = Constants.TeamList.First(t => t.id == newLap.playerInfo.teamId)?.name,
                                Tyre = "?",  // Packet type Car Status not handled yet 
                                Details = newLap
                            });
                    }
                }
                catch // (Exception e)
                {
                    //lama
                    // MessageDialog.Show(e);
                }
            }
        }

        private static bool loading = false;
        private static AsyncUI mainProcess = null;

        private static void SaveDictionary()
        {
            if (loading) return;
            String jsonData = JsonSerializer.Serialize(Laps);
            File.WriteAllText(BS(baseDir) + dictFileName, jsonData);
        }

        public static void LoadDictionary(AsyncUI mainWindow)
        {
            String json = "[]";
            if (File.Exists(BS(baseDir) + dictFileName))
            {
                json = File.ReadAllText(BS(baseDir) + dictFileName);

            }
            Laps = JsonSerializer.Deserialize<List<LapInfo>>(json);

            int i = Laps.Count;
            while (i > 0)
            {
                i--;
                if (!File.Exists(BS(baseDir) + BS(Laps[i].CircuitName) + Laps[i].FileName))
                {
                    mainWindow.LogLine( "Not found:" + Laps[i].FileName);
                    Laps.RemoveAt(i);
                }
            }

            mainProcess = mainWindow;
            mainWindow.Log($"{Laps.Count} laps loaded");
        }

        internal static void RemoveLap(LapInfo lapInfo)
        {
            DeleteFileOrFolder(GetFileName(lapInfo));
            Laps.Remove(lapInfo);
        }
        private const int FO_DELETE = 0x0003;
        private const int FOF_ALLOWUNDO = 0x0040;           // Preserve undo information, if possible. 
        private const int FOF_NOCONFIRMATION = 0x0010;      // Show no confirmation dialog box to the user

        // Struct which contains information that the SHFileOperation function uses to perform file operations. 
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SHFILEOPSTRUCT
        {
            public IntPtr hwnd;
            [MarshalAs(UnmanagedType.U4)]
            public int wFunc;
            public string pFrom;
            public string pTo;
            public short fFlags;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fAnyOperationsAborted;
            public IntPtr hNameMappings;
            public string lpszProgressTitle;
        }

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        static extern int SHFileOperation(ref SHFILEOPSTRUCT FileOp);

        // Move to recycle bin
        public static void DeleteFileOrFolder(string path)
        {
            SHFILEOPSTRUCT fileop = new SHFILEOPSTRUCT();
            fileop.wFunc = FO_DELETE;
            fileop.pFrom = path + '\0' + '\0';
            fileop.fFlags = FOF_ALLOWUNDO | FOF_NOCONFIRMATION;
            SHFileOperation(ref fileop);
        }
    }


}
