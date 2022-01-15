using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Vision2020
{
    internal class ApexInfo
    {
        [JsonInclude]
        int[] measurements { get; set; } = new int[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        public int fastAverage { get; set; } = 0;
        public int average => numMeasurements == 0 ? 0 : measurements.Take(numMeasurements).Sum() / numMeasurements;
        [JsonInclude]
        public int numMeasurements { get; set; }
        public void AddMeasurement(int distance)
        {
            if (numMeasurements < 10)
            {
                measurements[numMeasurements] = distance;
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
                        if (measurements[i] == min)
                        {
                            measurements[i] = distance;
                        }
                    }
                }
                if (distance < avg)
                {
                    for (int i = 0; i < numMeasurements; i++)
                    {
                        if (measurements[i] == max)
                        {
                            measurements[i] = distance;
                        }
                    }
                }
                fastAverage = average;
            }
        }
    }
    /// <summary>
    /// Helper class for speach synthesizing. Thread safe
    /// </summary>
    internal static class SpeachSynthesizer
    {
        private static object locker = new object();
        private static List<String> speachQueue = null;
        private static SpeechSynthesizer synth = null;

        private static List<ApexInfo> apexInfo = null;
        private static int lastTrack = -1;
        /// <summary>
        /// Speak text. If the synthesizer is active, the text will be queued. 
        /// </summary>
        /// <param name="textToSpeak"></param>
        /// <param name="force">If true, current text is not queue and spoken immediately</param>
        public static void QueueText(String textToSpeak, bool force=false)
        {
            if (synth == null)
            {
                Initialize();
            }
            if (force)
            {
                lock(locker)
                {
                    speachQueue.Clear();
                }
                synth.SpeakAsyncCancelAll();
            }
            if (synth.State == SynthesizerState.Speaking)
            {
                lock (locker)
                {
                    speachQueue.Add(textToSpeak);
                }
            }
            else
            {
                synth.SpeakAsync(textToSpeak);
            }
        }

        public static void Initialize()
        {
            lock(locker)
            {
                speachQueue = new List<String>();
                synth = new SpeechSynthesizer();
                //var voiceList = synth.GetInstalledVoices();
                //synth.SelectVoice(voiceList[0].ToString());
                synth.SelectVoiceByHints(VoiceGender.Neutral, VoiceAge.Teen, 0, CultureInfo.GetCultureInfo("nl-nl"));
                synth.Rate = 1;
                synth.StateChanged += Synth_StateChanged;
            }
        }

        private static void Synth_StateChanged(object sender, StateChangedEventArgs e)
        {
            if (e.State == SynthesizerState.Ready)
            {
                string nextText = "";
                lock (locker)
                {
                    if (speachQueue.Count > 0)
                    {
                        nextText = speachQueue.First();
                        speachQueue.RemoveAt(0);
                    }
                }
                synth.SpeakAsync(nextText);
            }
        }

        internal static int lastSpeed = 0;
        
        internal static uint currentMotionFrame = uint.MaxValue;
        internal static uint currentTelemetryLogFrame = uint.MaxValue;
        internal static uint currentLapDataFrame = uint.MaxValue;

        internal static List<CarMotionData> motionLog = null;
        internal static List<CarTelemetryData> telemetryLog = null;
        internal static List<LapData> lapDataLog = null;

        internal static bool Breaking = false;

        internal static void SetTrack(PacketSessionData trackInfo)
        {
            if (trackInfo.trackId != lastTrack)
            {
                if (apexInfo != null && lastTrack != -1)
                {
                    File.WriteAllText(LapDatabase.GetApexFileName(lastTrack), JsonSerializer.Serialize(apexInfo));
                }
                lastTrack = trackInfo.trackId;
                if (File.Exists(LapDatabase.GetApexFileName(lastTrack)))
                {
                    apexInfo = JsonSerializer.Deserialize<List<ApexInfo>>(File.ReadAllText(LapDatabase.GetApexFileName(lastTrack)));
                }
                else
                {
                    apexInfo = new List<ApexInfo>();
                }
            }
        }

        internal static void AddMotionInfo(uint frameIdentifier, CarMotionData carMotionData)
        {
            if (lastSpeed < 50 || lastSpeed > 230)
            {
                // ignore
            }
            else
            {
                lock (locker)
                {
                    currentMotionFrame = frameIdentifier;
                    motionLog.Add(carMotionData);
                }
                Analyse();
            }
        }

        internal static void AddTelemetry(uint frameIdentifier, CarTelemetryData carTelemetryData)
        {
            var nextSpeed = carTelemetryData.speed;
            if ((lastSpeed < 50 || lastSpeed > 230) && (nextSpeed < 50 || nextSpeed > 230))
            {
                // ignore
            }
            
            if (lastSpeed < 50 || lastSpeed > 230)
            {
                lock (locker)
                {
                    currentMotionFrame = uint.MaxValue;
                    currentLapDataFrame = uint.MaxValue;
                    Breaking = false;
                    motionLog = new List<CarMotionData>();
                    telemetryLog = new List<CarTelemetryData>();
                    lapDataLog = new List<LapData>();

                }
            }
            lock (locker)
            {
                currentTelemetryLogFrame = frameIdentifier;
                telemetryLog.Add(carTelemetryData);
            }
            Analyse();
            lastSpeed = nextSpeed;
        }

        internal static void AddLapInfo(uint frameIdentifier, LapData lapData)
        {
            if (lastSpeed < 50 || lastSpeed > 230)
            {
                // ignore
            }
            else
            {
                lock (locker)
                {
                    currentLapDataFrame = frameIdentifier;
                    lapDataLog.Add(lapData);
                }
                Analyse();
            }
        }

        internal static void Analyse()
        {
            string s = null;
            lock (locker)
            {
                if (currentLapDataFrame == currentTelemetryLogFrame && currentTelemetryLogFrame == currentMotionFrame
                    && motionLog.Count>0 && telemetryLog.Count>0 && lapDataLog.Count>0)
                {
                    var m = motionLog.Last();
                    var t = telemetryLog.Last();
                    var l = lapDataLog.Last();
                    var distance = (int)Math.Round(l.lapDistance, 0);

                    if (!Breaking && t.brake > 0 && l.lapDistance>0)
                    {
                        Breaking = true;
                        s = CheckApex(distance).ToString();
                    }
                    else if (Breaking && t.speed>0 && t.brake == 0 && t.throttle > 0)
                    {
                        AddApex(distance);
                        Breaking = false;
                        s = t.speed.ToString();
                    }
                }
            }
            if (s != null)
            {
                QueueText(s);
            }
        }

        private static void AddApex(int distance)
        {
            if (apexInfo == null)
            {
                return;
            }
            GetNearestApex(distance).AddMeasurement(distance);
        }

        private static ApexInfo GetNextApex(int distance)
        {
            return apexInfo.Where(a => distance - a.fastAverage < 200 && a.fastAverage > distance).FirstOrDefault();
        }

        private static ApexInfo GetNearestApex(int distance)
        {
            var nearest = apexInfo.Where(a => Math.Abs(distance - a.fastAverage)<50).FirstOrDefault();
            if (nearest == null)
            {
                nearest = new ApexInfo();
                nearest.AddMeasurement(distance);
                apexInfo.Add(nearest);
            }
            return nearest;
        }

        private static int CheckApex(int distance)
        {
            var nextApex = GetNextApex(distance);
            if (nextApex==null)
            {
                return distance;
            }    
            return nextApex.fastAverage-distance;
        }
    }
}
