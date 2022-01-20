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
using System.Drawing;

namespace Vision2020
{
    /// <summary>
    /// Helper class for speach synthesizing. Thread safe
    /// </summary>
    internal static class SpeachSynthesizer
    {
        private static object locker = new object();
        private static List<String> speachQueue = null;
        private static SpeechSynthesizer synth = null;

        public static bool DoNotSpeak = false;
        /// <summary>
        /// Speak text. If the synthesizer is active, the text will be queued. 
        /// </summary>
        /// <param name="textToSpeak"></param>
        /// <param name="force">If true, current text is not queue and spoken immediately</param>
        public static void QueueText(String textToSpeak, bool force = false)
        {
            if (DoNotSpeak)
            {
                return;
            }
            if (synth == null)
            {
                Initialize();
            }
            if (force)
            {
                lock (locker)
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
            lock (locker)
            {
                speachQueue = new List<String>();
                synth = new SpeechSynthesizer();
                //var voiceList = synth.GetInstalledVoices();
                //synth.SelectVoice(voiceList[0].ToString());
                synth.SelectVoiceByHints(VoiceGender.Neutral, VoiceAge.Teen, 0, CultureInfo.GetCultureInfo("nl-nl"));
                synth.Rate = Config.UserConfig.AnnouceSpeed;
                synth.StateChanged += Synth_StateChanged;
            }
        }

        public static void UpdateSpeed()
        {
            synth.Rate = Config.UserConfig.AnnouceSpeed;
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

        internal static void QueueNumber(string s)
        {
            string numbers = "";
            foreach (char number in s)
            {
                numbers = numbers + number + " ";
            }
            QueueText(numbers);
        }
    }
}
