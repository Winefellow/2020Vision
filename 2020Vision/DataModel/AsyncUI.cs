using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vision2020
{
    public interface AsyncUI
    {
        void Log(String s);
        void LogLine(string s);
        void UpdatePaticipants(PacketParticipantsData participantsData);
        void UpdateSession(PacketHeader header, PacketSessionData sessionData);
        void UpdateLapdata(PacketHeader context, PacketLapData lapData);
        void UpdateSetup(PacketHeader context, PacketCarSetupData setupData);
        void UpdateTelemetry(PacketHeader context, PacketCarTelemetryData telemetryData);
        void AddPacket(PacketInfo Packet);
        void StartSession(PacketHeader data);
        void EndSession(PacketHeader data);
        void UpdateMotion(PacketHeader context, PacketMotionData data);
        bool IsPausing();
        string SelectedFile { get; }
        string SessionID { get; }
    }

    public static class InvokeExtension
    {
        public static void InvokeIfRequired(this Control c, MethodInvoker action)
        {
            if (c.InvokeRequired)
            {
                var args = new object[0];
                c.BeginInvoke(action, args);
            }
            else
            {
                action();
            }
        }
    }

}
