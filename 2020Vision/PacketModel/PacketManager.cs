﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Vision2020
{
    class PacketManager
    {
    }
    public class PacketInfo
    {
        public int sequence;
        public int packetLength;
        public PacketHeader header;
        public byte[] data;
        public object details;
    }

    public static class PacketHelper
    {
        public static unsafe byte[] Serialize<T>(T value) where T : unmanaged
        {
            byte[] buffer = new byte[sizeof(T)];

            fixed (byte* bufferPtr = buffer)
            {
                Buffer.MemoryCopy(&value, bufferPtr, sizeof(T), sizeof(T));
            }

            return buffer;
        }

        public static unsafe T Deserialize<T>(byte[] buffer) where T : unmanaged
        {
            T result = new T();

            fixed (byte* bufferPtr = buffer)
            {
                Buffer.MemoryCopy(bufferPtr, &result, sizeof(T), sizeof(T));
            }

            return result;
        }

        private static byte[] data = new byte[50000];
        public static object GetObjectFromBytes(byte[] buffer, Type objType)
        {
            object obj = null;
            if ((buffer != null) && (buffer.Length > 0))
            {
                IntPtr ptrObj = IntPtr.Zero;
                try
                {
                    int objSize = Marshal.SizeOf(objType);
                    if (objSize > 0)
                    {
                        if (buffer.Length < objSize)
                            throw new Exception(String.Format("Buffer smaller than needed for creation of object of type {0}", objType));
                        ptrObj = Marshal.AllocHGlobal(objSize);
                        if (ptrObj != IntPtr.Zero)
                        {
                            Marshal.Copy(buffer, 0, ptrObj, objSize);
                            obj = Marshal.PtrToStructure(ptrObj, objType);
                        }
                        else
                            throw new Exception(String.Format("Couldn't allocate memory to create object of type {0}", objType));
                    }
                }
                finally
                {
                    if (ptrObj != IntPtr.Zero)
                        Marshal.FreeHGlobal(ptrObj);
                }
            }
            return obj;
        }

        internal static String GetString(byte[] name, int v)
        {
            if (name == null) return ("");
            int i = 0;
            while ((name[i] != 0) && (i<v)) i++;
            
            var str = System.Text.Encoding.UTF8.GetString(name, 0, i);
            return str;
        }

        public static T SafeRead<T>(Stream dataStream, int size)
        {
            bool duurtTeLang = false;

            if ((dataStream.Length - dataStream.Position) < size)
            {
                // Not enough data available
                return default(T);
            }
            else
            {
                int read = dataStream.Read(data, 0, size);
                while (read != size || duurtTeLang)
                {
                    int extraBytes = dataStream.Read(data, read, size - read);
                    read = read + extraBytes;
                    duurtTeLang = (extraBytes == 0);
                }
                if (read != size) { throw new Exception("Incomplete read"); }
                return (T)PacketHelper.GetObjectFromBytes(data, typeof(T));
            }
        }

        public static byte[] ToByteArray2(object o)
        {
            byte[] arr = null;
            IntPtr ptr = IntPtr.Zero;
            try
            {
                int size = Marshal.SizeOf(o);
                arr = new byte[size];
                ptr = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(o, ptr, true);
                Marshal.Copy(ptr, arr, 0, size);
            }
            catch // (Exception e)
            {
                throw;
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }

            return arr;
        }

        internal static void SafeWrite<T>(Stream outFile, T data, int checkLength)
        {
            byte[] bytes = ToByteArray2(data);
            outFile.Write(bytes, 0, bytes.Length);
            if (bytes.Length != checkLength)
            {
                throw new Exception($"Unexpected length: {bytes.Length} in stead of {checkLength}");
            }
        }

        public static T fromBytes<T>(byte[] arr, int size)
        {
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.Copy(arr, 0, ptr, size);

            var x = (T)Marshal.PtrToStructure(ptr, typeof(T));
            Marshal.FreeHGlobal(ptr);

            return x;
        }

        public static EventType GetEventType(Stream dataStream)
        {
            //	“SSTA”	Session Started	Sent when the session starts
            //	“SEND”	Session Ended	Sent when the session ends
            //	“FTLP”	Fastest Lap	When a driver achieves the fastest lap
            //	“RTMT”	Retirement	When a driver retires
            //	“DRSE”	DRS enabled	Race control have enabled DRS
            //	“DRSD”	DRS disabled	Race control have disabled DRS
            //	“TMPT”	Team mate in pits	Your team mate has entered the pits
            //	“CHQF”	Chequered flag	The chequered flag has been waved
            //	“RCWN”	Race Winner	The race winner is announced
            //	“PENA”	Penalty Issued	A penalty has been issued – details in event
            //	“SPTP”	Speed Trap Triggered	Speed trap has been triggered by fastest speed
            //	“STLG”	Start lights	Start lights – number shown
            //	“LGOT”	Lights out	Lights out
            //	“DTSV”	Drive through served	Drive through penalty served
            //	“SGSV”	Stop go served	Stop go penalty served
            //	“FLBK”	Flashback	Flashback activated
            //	“BUTN”	Button status	Button status changed

            byte[] data = new byte[4];
            int read = dataStream.Read(data, 0, 4);

            if ( data[0] == 'S') 
            {
                if (data[1] == 'S')  //   Session Started “SSTA”	Sent when the session starts
                {
                    return EventType.SessionStarted;
                }
                if (data[1] == 'E')  //   Session Ended   “SEND”	Sent when the session 
                {
                    return EventType.SessionEnded;
                }
                if (data[1] == 'P')  //   “SPTP”	Speed Trap Triggered
                {
                    return EventType.SpeedTrapTriggered;
                }
                if (data[1] == 'G')  //	“SGSV”	Stop go served	Stop go penalty served
                {
                    return EventType.StopGoPenaltyServed;
                }
                //	“STLG”	Start lights	Start lights – number shown
                return EventType.StartLights;
            }
            else if (data[0] == 'F') // Fastest Lap “FTLP”	When a driver achieves the fastest lap
            {
                if (data[1] == 'L')  //	“FLBK”	Flashback	Flashback activated
                {
                    return EventType.Flashback;
                }
                return EventType.FastestLap;
                
            }
            else if (data[0] == 'R') // Retirement  “RTMT”	When a driver retires
            {
                if (data[1] == 'T')
                {
                    return EventType.Retirement;
                }
                // Race Winner “RCWN”	The race winner is announced
                return EventType.RaceWinner;
            }
            else if (data[0] == 'D') // DRS enabled “DRSE”	Race control have enabled DRS
                                // DRS disabled “DRSD”	Race control have disabled DRS
            {
                if (data[3] == 'E')
                {
                    return EventType.DRSEnabled;
                }
                if (data[3] == 'D')
                {
                    return EventType.DRSDisabled;
                }
                //	“DTSV”	Drive through served	Drive through penalty served
                return EventType.DriveThroughPenaltyServed;
            }
            else if (data[0] == 'T') // Team mate in pits   “TMPT”	Your team mate has entered the pits
            {
                return EventType.TeammateInPits;
            }
            else if (data[0] == 'C') // Chequered flag  “CHQF”	The chequered flag has been waved
            {
                return EventType.CheckeredFlag;
            }
            else if (data[0] == 'P') // Penalty Issued  “PENA”	A penalty has been issued – details in event
            {
                return EventType.PenaltyIssued;
            }
            else if (data[0] == 'L') //	“LGOT”	Lights out	Lights out
            {
                return EventType.LightsOut;
            }
            else if (data[0] == 'B')  //	“BUTN”	Button status	Button status changed
            {
                return EventType.Buttons;
            }
            return EventType.Unknown;
        }

        public static string Country(int countryNumer)
        {
            return Constants.CountryList.First(c => c.id == countryNumer).name;
        }

        public static string CountryShort(int countryNumer)
        {
            if (countryNumer != 255)
                return Constants.CountryList.FirstOrDefault(c => c.id == countryNumer)?.shortName ?? "??";
            return ("??");
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            if (unixTimeStamp > 0)
            {
                dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            }
            return dtDateTime;
        }
    }
}
