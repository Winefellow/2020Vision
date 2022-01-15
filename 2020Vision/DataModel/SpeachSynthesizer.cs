using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;

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
                    speachQueue.Append(textToSpeak);
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
    }
}
