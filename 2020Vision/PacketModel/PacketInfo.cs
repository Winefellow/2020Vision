using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Vision2020
{
    // (u)int16 -> UInt16
    // int8 -> byte
    // int8 -> byte
    // (u)int64 -> UInt64
    // float -> float
    #region Header and Size
    class SizeInfo<T> where T : new()
    {
        private T dataDinges = new T();
        private int dataDingesSize = -1;
        public int Size
        {
            get
            {
                if (dataDingesSize == -1)
                {
                    dataDingesSize = Marshal.SizeOf(dataDinges);
                }
                return dataDingesSize;
            }
        }
    }
    public class PacketSize
    {
        private static PacketSize _instance = new PacketSize();
        private int packetHeaderSize = new SizeInfo<PacketHeader>().Size;
        private int speedTrapSize = new SizeInfo<SpeedTrap>().Size;
        private int packetMotionDataSize = new SizeInfo<PacketMotionData>().Size;
        private int fastestLapSize = new SizeInfo<FastestLap>().Size;
        private int retirementSize = new SizeInfo<Retirement>().Size;
        private int teamMateInPitsSize = new SizeInfo<TeamMateInPits>().Size;
        private int raceWinnerSize = new SizeInfo<RaceWinner>().Size;
        private int penaltySize = new SizeInfo<Penalty>().Size;
        private int packetSessionDataSize = new SizeInfo<PacketSessionData>().Size;
        private int packetParticipantsDataSize = new SizeInfo<PacketParticipantsData>().Size;
        private int packetCarSetupDataSize = new SizeInfo<PacketCarSetupData>().Size;
        private int packetCarTelemetryDataSize = new SizeInfo<PacketCarTelemetryData>().Size;
        private int packetLapDataSize = new SizeInfo<PacketLapData>().Size;
        private int packetCarStatusDataSize = new SizeInfo<PacketCarStatusData>().Size;
        private int packetFinalClassificationDataSize = new SizeInfo<PacketFinalClassificationData>().Size;
        private int packetLobbyInfoDataSize = new SizeInfo<PacketLobbyInfoData>().Size;
        private int participantDataSize = new SizeInfo<ParticipantData>().Size;
        private int packetCarDamageDataSize = new SizeInfo<PacketCarDamageData>().Size;
        private int packetSessionHistoryDataSize = new SizeInfo<PacketSessionHistoryData>().Size;

        private int boolSize = new SizeInfo<bool>().Size;
        private int floatSize = new SizeInfo<float>().Size;
        private int intSize = new SizeInfo<int>().Size;
        private int lapDataSize = new SizeInfo<LapData>().Size;
        private int carMotionSize = new SizeInfo<CarMotionData>().Size;
        private int carTelemetrySize = new SizeInfo<CarTelemetryData>().Size;
        private int telemetryInContextSize = new SizeInfo<TelemetryInContext>().Size;
        private int motionInContextSize = new SizeInfo<MotionInContext>().Size;
        private int lapDataInContextSize = new SizeInfo<LapDataInContext>().Size;
        private int carSetupDataSize = new SizeInfo<CarSetupData>().Size;

        // For (a tiny bit of) speed wins, calculate packet size only once at startup
        public static int PacketHeaderSize { get { return _instance.packetHeaderSize; } }
        public static int SpeedTrapSize { get { return _instance.speedTrapSize; } }
        public static int PacketMotionDataSize { get { return _instance.packetMotionDataSize; } }
        public static int FastestLapSize { get { return _instance.fastestLapSize; } }
        public static int RetirementSize { get { return _instance.retirementSize; } }
        public static int TeamMateInPitsSize { get { return _instance.teamMateInPitsSize; } }
        public static int RaceWinnerSize { get { return _instance.raceWinnerSize; } }
        public static int PenaltySize { get { return _instance.penaltySize; } }
        public static int PacketSessionDataSize { get { return _instance.packetSessionDataSize; } }
        public static int PacketParticipantsDataSize { get { return _instance.packetParticipantsDataSize; } }
        public static int PacketCarSetupDataSize { get { return _instance.packetCarSetupDataSize; } }
        public static int PacketCarTelemetryDataSize { get { return _instance.packetCarTelemetryDataSize; } }
        public static int PacketLapDataSize { get { return _instance.packetLapDataSize; } }
        public static int PacketCarStatusDataSize { get { return _instance.packetCarStatusDataSize; } }
        public static int PacketFinalClassificationDataSize { get { return _instance.packetFinalClassificationDataSize; } }
        public static int PacketLobbyInfoDataSize { get { return _instance.packetLobbyInfoDataSize; } }
        public static int ParticipantDataSize { get { return _instance.participantDataSize; } }
        public static int PacketCarDamageDataSize { get { return _instance.packetCarDamageDataSize; } }
        public static int PacketSessionHistoryDataSize { get { return _instance.packetSessionHistoryDataSize; } }
        public static int BoolSize { get { return _instance.boolSize; } }
        public static int FloatSize { get { return _instance.floatSize; } }
        public static int IntSize { get { return _instance.intSize; } }
        public static int LapDataSize { get { return _instance.lapDataSize; } }
        public static int CarMotionSize { get { return _instance.carMotionSize; } }
        public static int CarTelemetrySize { get { return _instance.carTelemetrySize; } }
        public static int TelemetryInContextSize { get { return _instance.telemetryInContextSize; } }
        public static int MotionInContextSize { get { return _instance.motionInContextSize; } }
        public static int LapDataInContextSize { get { return _instance.lapDataInContextSize; } }
        public static int CarSetupDataSize { get { return _instance.carSetupDataSize; } }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PacketHeader
    {
        public UInt16 packetFormat;             // 2020 / 2021
        public byte gameMajorVersion;           // Game major version - "X.00"
        public byte gameMinorVersion;           // Game minor version - "1.XX"
        public byte packetVersion;              // Version of this packet type, all start from 1
        public byte packetId;                   // Identifier for the packet type, see below
        public UInt64 sessionUID;               // Unique identifier for the session
        public float sessionTime;               // Session timestamp
        public UInt32 frameIdentifier;          // Identifier for the frame the data was retrieved on
        public byte playerCarIndex;             // Index of player's car in the array

        // ADDED IN BETA 2: 
        public byte secondaryPlayerCarIndex;    // Index of secondary player's car in the array (splitscreen)
    }
    #endregion

    #region Type 0: Motion 

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CarMotionData
    {
        public float worldPositionX;           // World space X position
        public float worldPositionY;           // World space Y position
        public float worldPositionZ;           // World space Z position
        public float worldVelocityX;           // Velocity in world space X
        public float worldVelocityY;           // Velocity in world space Y
        public float worldVelocityZ;           // Velocity in world space Z
        public UInt16 worldForwardDirX;         // World space forward X direction (normalised)
        public UInt16 worldForwardDirY;         // World space forward Y direction (normalised)
        public UInt16 worldForwardDirZ;         // World space forward Z direction (normalised)
        public UInt16 worldRightDirX;           // World space right X direction (normalised)
        public UInt16 worldRightDirY;           // World space right Y direction (normalised)
        public UInt16 worldRightDirZ;           // World space right Z direction (normalised)
        public float gForceLateral;            // Lateral G-Force component
        public float gForceLongitudinal;       // Longitudinal G-Force component
        public float gForceVertical;           // Vertical G-Force component
        public float yaw;                      // Yaw angle in radians
        public float pitch;                    // Pitch angle in radians
        public float roll;                     // Roll angle in radians
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PacketMotionData
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 22)]
        public CarMotionData[] carMotionData;      // Data for all cars on track 22
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] suspensionPosition;      // Note: All wheel arrays have the following order:
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] suspensionVelocity;      // RL, RR, FL, FR
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] suspensionAcceleration;  // RL, RR, FL, FR
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] wheelSpeed;              // Speed of each wheel
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] wheelSlip;               // Slip ratio for each wheel

        public float localVelocityX;             // Velocity in local space
        public float localVelocityY;             // Velocity in local space
        public float localVelocityZ;             // Velocity in local space
        public float angularVelocityX;           // Angular velocity x-component
        public float angularVelocityY;           // Angular velocity y-component
        public float angularVelocityZ;           // Angular velocity z-component
        public float angularAccelerationX;       // Angular velocity x-component
        public float angularAccelerationY;       // Angular velocity y-component
        public float angularAccelerationZ;       // Angular velocity z-component
        public float frontWheelsAngle;           // Current front wheels angle in radians        
    }
    #endregion

    #region Type 1: Session

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MarshalZone
    {
        float zoneStart;   // Fraction (0..1) of way through the lap the marshal zone starts
        byte zoneFlag;    // -1 = invalid/unknown, 0 = none, 1 = green, 2 = blue, 3 = yellow, 4 = red
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct WeatherForecastSample
    {
        public byte sessionType;                     // 0 = unknown, 1 = P1, 2 = P2, 3 = P3, 4 = Short P, 5 = Q1
                                                     // 6 = Q2, 7 = Q3, 8 = Short Q, 9 = OSQ, 10 = R, 11 = R2
                                                     // 12 = Time Trial
        public byte timeOffset;                      // Time in minutes the forecast is for
        public byte weather;                         // Weather - 0 = clear, 1 = light cloud, 2 = overcast
                                                     // 3 = light rain, 4 = heavy rain, 5 = storm
        public byte trackTemperature;                // Track temp. in degrees celsius
        public byte trackTemperatureChange;        // Track temp. change – 0 = up, 1 = down, 2 = no change
        public byte airTemperature;                // Air temp. in degrees celsius
        public byte airTemperatureChange;          // Air temp. change – 0 = up, 1 = down, 2 = no change
        public byte rainPercentage;                // Rain percentage (0-100)
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PacketSessionData
    {
        public byte weather;                   // Weather - 0 = clear, 1 = light cloud, 2 = overcast
                                               // 3 = light rain, 4 = heavy rain, 5 = storm
        public byte trackTemperature;          // Track temp. in degrees celsius
        public byte airTemperature;            // Air temp. in degrees celsius
        public byte totalLaps;                 // Total number of laps in this race
        public UInt16 trackLength;               // Track length in metres
        public byte sessionType;               // 0 = unknown, 1 = P1, 2 = P2, 3 = P3, 4 = Short P
                                               // 5 = Q1, 6 = Q2, 7 = Q3, 8 = Short Q, 9 = OSQ
                                               // 10 = R, 11 = R2, 12 = Time Trial
        public byte trackId;                   // -1 for unknown, 0-21 for tracks, see appendix
        public byte formula;                   // Formula, 0 = F1 Modern, 1 = F1 Classic, 2 = F2,
                                               // 3 = F1 Generic
        public UInt16 sessionTimeLeft;           // Time left in session in seconds
        public UInt16 sessionDuration;           // Session duration in seconds
        public byte pitSpeedLimit;             // Pit speed limit in kilometres per hour
        public byte gamePaused;                // Whether the game is paused
        public byte isSpectating;              // Whether the player is spectating
        public byte spectatorCarIndex;         // Index of the car being spectated
        public byte sliProNativeSupport;     // SLI Pro support, 0 = inactive, 1 = active
        public byte numMarshalZones;           // Number of marshal zones to follow
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 21, ArraySubType = UnmanagedType.Struct)]
        public MarshalZone[] marshalZones;          // List of marshal zones – max 21
        public byte safetyCarStatus;           // 0 = no safety car, 1 = full safety car
                                               // 2 = virtual safety car
        public byte networkGame;               // 0 = offline, 1 = online
        public byte numWeatherForecastSamples; // Number of weather samples to follow
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20, ArraySubType = UnmanagedType.Struct)]
        public WeatherForecastSample[] weatherForecastSamples;   // Array of weather forecast samples
        // added 2021
        public byte forecastAccuracy;          // 0 = Perfect, 1 = Approximate
        public byte aiDifficulty;              // AI Difficulty rating – 0-110
        public UInt32 seasonLinkIdentifier;      // Identifier for season - persists across saves
        public UInt32 weekendLinkIdentifier;     // Identifier for weekend - persists across saves
        public UInt32 sessionLinkIdentifier;     // Identifier for session - persists across saves
        public byte pitStopWindowIdealLap;     // Ideal lap to pit on for current strategy (player)
        public byte pitStopWindowLatestLap;    // Latest lap to pit on for current strategy (player)
        public byte pitStopRejoinPosition;     // Predicted position to rejoin at (player)
        public byte steeringAssist;            // 0 = off, 1 = on
        public byte brakingAssist;             // 0 = off, 1 = low, 2 = medium, 3 = high
        public byte gearboxAssist;             // 1 = manual, 2 = manual & suggested gear, 3 = auto
        public byte pitAssist;                 // 0 = off, 1 = on
        public byte pitReleaseAssist;          // 0 = off, 1 = on
        public byte ERSAssist;                 // 0 = off, 1 = on
        public byte DRSAssist;                 // 0 = off, 1 = on
        public byte dynamicRacingLine;         // 0 = off, 1 = corners only, 2 = full
        public byte dynamicRacingLineType;     // 0 = 2D, 1 = 3D

    }
    #endregion

    #region Type 2: Lap Data

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LapData
    {
        public UInt32 lastLapTimeInMS;            // Last lap time in milliseconds
        public UInt32 currentLapTimeInMS;     // Current time around the lap in milliseconds
        public UInt16 sector1TimeInMS;           // Sector 1 time in milliseconds
        public UInt16 sector2TimeInMS;           // Sector 2 time in milliseconds
        public float lapDistance;         // Distance vehicle is around current lap in metres – could
                                          // be negative if line hasn’t been crossed yet
        public float totalDistance;       // Total distance travelled in session in metres – could
                                          // be negative if line hasn’t been crossed yet
        public float safetyCarDelta;            // Delta in seconds for safety car
        public byte carPosition;             // Car race position
        public byte currentLapNum;       // Current lap number
        public byte pitStatus;               // 0 = none, 1 = pitting, 2 = in pit area
        public byte numPitStops;                 // Number of pit stops taken in this race
        public byte sector;                  // 0 = sector1, 1 = sector2, 2 = sector3
        public byte currentLapInvalid;       // Current lap invalid - 0 = valid, 1 = invalid
        public byte penalties;               // Accumulated time penalties in seconds to be added
        public byte warnings;                  // Accumulated number of warnings issued
        public byte numUnservedDriveThroughPens;  // Num drive through pens left to serve
        public byte numUnservedStopGoPens;        // Num stop go pens left to serve
        public byte gridPosition;            // Grid position the vehicle started the race in
        public byte driverStatus;            // Status of driver - 0 = in garage, 1 = flying lap
                                             // 2 = in lap, 3 = out lap, 4 = on track
        public byte resultStatus;              // Result status - 0 = invalid, 1 = inactive, 2 = active
                                               // 3 = finished, 4 = didnotfinish, 5 = disqualified
                                               // 6 = not classified, 7 = retired
        public byte pitLaneTimerActive;          // Pit lane timing, 0 = inactive, 1 = active
        public UInt16 pitLaneTimeInLaneInMS;      // If active, the current time spent in the pit lane in ms
        public UInt16 pitStopTimerInMS;           // Time of the actual pit stop in ms
        public byte pitStopShouldServePen;   	 // Whether the car should serve a penalty at this stop

    };


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PacketLapData
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 22)]
        public LapData[] lapData;        // Lap data for all cars on track
    };

    #endregion

    #region Type 3: EVent

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FastestLap
    {
        public byte vehicleIdx; // Vehicle index of car achieving fastest lap
        public float lapTime;    // Lap time is in seconds
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Retirement
    {
        public byte vehicleIdx; // Vehicle index of car retiring
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TeamMateInPits
    {
        public byte vehicleIdx; // Vehicle index of team mate
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RaceWinner
    {
        public byte vehicleIdx; // Vehicle index of the race winner
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Penalty
    {

        public byte penaltyType;          // Penalty type – see Appendices
        public byte infringementType;     // Infringement type – see Appendices
        public byte vehicleIdx;           // Vehicle index of the car the penalty is applied to
        public byte otherVehicleIdx;      // Vehicle index of the other car involved
        public byte time;                 // Time gained, or time spent doing action in seconds
        public byte lapNum;               // Lap the penalty occurred on
        public byte placesGained;         // Number of places gained by this
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SpeedTrap
    {
        public byte vehicleIdx; // Vehicle index of the vehicle triggering speed trap
        public float speed;      // Top speed achieved in kilometres per hour
        public byte overallFastestInSession;   // Overall fastest speed in session = 1, otherwise 0
        public byte driverFastestInSession;    // Fastest speed for driver in session = 1, otherwise 0
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct StartLights
    {
        public byte vehicleIdx; // Vehicle index of the race winner
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DriveThroughPenaltyServed
    {
        public byte vehicleIdx; // Vehicle index of the race winner
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct StopGoPenaltyServed
    {
        public byte vehicleIdx; // Vehicle index of the race winner
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Flashback
    {
        public UInt32 flashbackFrameIdentifier;  // Frame identifier flashed back to
        public float flashbackSessionTime;       // Session time flashed back to
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Buttons
    {
        public UInt32 buttonStatus; // Vehicle index of the race winner
    }


    public enum EventType
    {
        Unknown, SessionStarted, SessionEnded,
        FastestLap, Retirement, DRSEnabled, DRSDisabled,
        TeammateInPits, CheckeredFlag, RaceWinner,
        PenaltyIssued, SpeedTrapTriggered, StartLights,
        DriveThroughPenaltyServed, StopGoPenaltyServed,
        Flashback, Buttons
    }


    public struct PacketEventData
    {
        public EventType eventStringCode; // Event string code, see below
        public object eventDetails;       // Event details - should be interpreted differently
                                          // for each type
    }
    #endregion

    #region Type 4: Participants

    public struct ParticipantData
    {
        public byte aiControlled;           // Whether the vehicle is AI (1) or Human (0) controlled
        public byte driverId;               // Driver id - see appendix
        public byte networkId;               // Network id – unique identifier for network players
        public byte teamId;                 // Team id - see appendix
        public byte myTeam;                 // My team flag – 1 = My Team, 0 = otherwise
        public byte raceNumber;             // Race number of the car
        public byte nationality;            // Nationality of the driver
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 48)]
        public byte[] name;               // Name of participant in UTF-8 format – null terminated
                                          // Will be truncated with … (U+2026) if too long
        public byte yourTelemetry;          // The player's UDP setting, 0 = restricted, 1 = public
    }

    public struct PacketParticipantsData
    {
        public byte numActiveCars;  // Number of active cars in the data – should match number of
                                    // cars on HUD
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 22)]
        public ParticipantData[] participants;
    }
    #endregion

    #region Type 5: Car Setups

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CarSetupData
    {
        public byte frontWing;                // Front wing aero
        public byte rearWing;                 // Rear wing aero
        public byte onThrottle;               // Differential adjustment on throttle (percentage)
        public byte byteoffThrottle;              // Differential adjustment off throttle (percentage)
        public float frontCamber;              // Front camber angle (suspension geometry)
        public float rearCamber;               // Rear camber angle (suspension geometry)
        public float frontToe;                 // Front toe angle (suspension geometry)
        public float rearToe;                  // Rear toe angle (suspension geometry)
        public byte bytefrontSuspension;          // Front suspension
        public byte byterearSuspension;           // Rear suspension
        public byte bytefrontAntiRollBar;         // Front anti-roll bar
        public byte byterearAntiRollBar;          // Front anti-roll bar
        public byte bytefrontSuspensionHeight;    // Front ride height
        public byte byterearSuspensionHeight;     // Rear ride height
        public byte bytebrakePressure;            // Brake pressure (percentage)
        public byte bytebrakeBias;                // Brake bias (percentage)
        public float rearLeftTyrePressure;     // Rear left tyre pressure (PSI)
        public float rearRightTyrePressure;    // Rear right tyre pressure (PSI)
        public float frontLeftTyrePressure;    // Front left tyre pressure (PSI)
        public float frontRightTyrePressure;   // Front right tyre pressure (PSI)
        public byte byteballast;                  // Ballast
        public float fuelLoad;                 // Fuel load
    };



    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PacketCarSetupData
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 22)]
        public CarSetupData[] carSetups;
    };
    #endregion

    #region Type 6: Car Telemetry

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CarTelemetryData
    {
        public UInt16 speed;                      // Speed of car in kilometres per hour
        public float throttle;                    // Amount of throttle applied (0.0 to 1.0)
        public float steer;                       // Steering (-1.0 (full lock left) to 1.0 (full lock right))
        public float brake;                       // Amount of brake applied (0.0 to 1.0)
        public byte clutch;                       // Amount of clutch applied (0 to 100)
        public byte gear;                         // Gear selected (1-8, N=0, R=-1)
        public UInt16 engineRPM;                  // Engine RPM
        public byte drs;                          // 0 = off, 1 = on
        public byte revLightsPercent;             // Rev lights indicator (percentage)
        public UInt16 m_revLightsBitValue;        // Rev lights (bit 0 = leftmost LED, bit 14 = rightmost LED)
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public UInt16[] brakesTemperature;          // Brakes temperature (celsius)
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] tyresSurfaceTemperature;    // Tyres surface temperature (celsius)
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] tyresInnerTemperature;      // Tyres inner temperature (celsius)
        public UInt16 engineTemperature;          // Engine temperature (celsius)
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] tyresPressure;             // Tyres pressure (PSI)
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] surfaceType;                // Driving surface, see appendices
    };


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PacketCarTelemetryData
    {

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 22)]
        public CarTelemetryData[] carTelemetryData;

        // Added in Beta 3:
        public byte mfdPanelIndex;   // Index of MFD panel open - 255 = MFD closed
                                     // Single player, race – 0 = Car setup, 1 = Pits
                                     // 2 = Damage, 3 =  Engine, 4 = Temperatures
                                     // May vary depending on game mode
        public byte mfdPanelIndexSecondaryPlayer;   // See above
        public byte suggestedGear;  // Suggested gear for the player (1-8)
                                    // 0 if no gear suggested
    };
    #endregion

    #region Type 7: Car Status

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CarStatusData
    {
        public byte tractionControl;          // 0 (off) - 2 (high)
        public byte antiLockBrakes;           // 0 (off) - 1 (on)
        public byte fuelMix;                  // Fuel mix - 0 = lean, 1 = standard, 2 = rich, 3 = max
        public byte frontBrakeBias;           // Front brake bias (percentage)
        public byte pitLimiterStatus;         // Pit limiter status - 0 = off, 1 = on
        public float fuelInTank;              // Current fuel mass
        public float fuelCapacity;            // Fuel capacity
        public float fuelRemainingLaps;       // Fuel remaining in terms of laps (value on MFD)
        public UInt16 maxRPM;                 // Cars max RPM, point of rev limiter
        public UInt16 idleRPM;                // Cars idle RPM
        public byte maxGears;                 // Maximum number of gears
        public byte drsAllowed;               // 0 = not allowed, 1 = allowed, -1 = unknown


        // Added in Beta3:
        public UInt16 drsActivationDistance;  // 0 = DRS not available, non-zero - DRS will be available
                                              // in [X] metres

        public byte actualTyreCompound;      // F1 Modern - 16 = C5, 17 = C4, 18 = C3, 19 = C2, 20 = C1
                                             // 7 = inter, 8 = wet
                                             // F1 Classic - 9 = dry, 10 = wet
                                             // F2 – 11 = super soft, 12 = soft, 13 = medium, 14 = hard
                                             // 15 = wet
        public byte visualTyreCompound;      // F1 visual (can be different from actual compound)
                                             // 16 = soft, 17 = medium, 18 = hard, 7 = inter, 8 = wet
                                             // F1 Classic – same as above
                                             // F2 – same as above
        public byte tyresAgeLaps;            // Age in laps of the current set of tyres

        public byte vehicleFiaFlags;         // -1 = invalid/unknown, 0 = none, 1 = green
                                             // 2 = blue, 3 = yellow, 4 = red
        public float ersStoreEnergy;         // ERS energy store in Joules
        public byte ersDeployMode;           // ERS deployment mode, 0 = none, 1 = medium
                                             // 2 = overtake, 3 = hotlap
        public float ersHarvestedThisLapMGUK;  // ERS energy harvested this lap by MGU-K
        public float ersHarvestedThisLapMGUH;  // ERS energy harvested this lap by MGU-H
        public float ersDeployedThisLap;       // ERS energy deployed this lap
        // added 2021
        public byte networkPaused;            // Whether the car is paused in a network game

    };


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PacketCarStatusData
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 22)]
        public CarStatusData[] carStatusData;
    };
    #endregion

    #region Type 8: Final Classification

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FinalClassificationData
    {
        public byte position;              // Finishing position
        public byte numLaps;               // Number of laps completed
        public byte gridPosition;          // Grid position of the car
        public byte points;                // Number of points scored
        public byte numPitStops;           // Number of pit stops made
        public byte resultStatus;          // Result status - 0 = invalid, 1 = inactive, 2 = active
                                           // 3 = finished, 4 = disqualified, 5 = not classified
                                           // 6 = retired
        public UInt32 bestLapTime;         // Best lap time of the session in seconds
        public double totalRaceTime;       // Total race time in seconds without penalties
        public byte penaltiesTime;         // Total penalties accumulated in seconds
        public byte numPenalties;          // Number of penalties applied to this driver
        public byte numTyreStints;         // Number of tyres stints up to maximum
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] tyreStintsActual;    // Actual tyres used by this driver
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] tyreStintsVisual;    // Visual tyres used by this driver
    };


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PacketFinalClassificationData
    {
        public byte numCars;                 // Number of cars in the final classification
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 22)]
        public FinalClassificationData[] classificationData;
    };
    #endregion

    #region Type 8: Lobby Info

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LobbyInfoData
    {
        public byte aiControlled;            // Whether the vehicle is AI (1) or Human (0) controlled
        public byte teamId;                  // Team id - see appendix (255 if no team currently selected)
        public byte nationality;             // Nationality of the driver
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 48)]
        public char[] name;                // Name of participant in UTF-8 format – null terminated
                                           // Will be truncated with ... (U+2026) if too long
        public byte carNumber;               // Car number of the player
        public byte readyStatus;             // 0 = not ready, 1 = ready, 2 = spectating
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PacketLobbyInfoData
    {
        // Packet specific data
        public byte numPlayers;                    // Number of players in the lobby data
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 22)]
        public LobbyInfoData[] lobbyPlayers;
    }
    #endregion

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MotionInContext
    {
        public CarMotionData carMotion;
        public PacketHeader context;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TelemetryInContext
    {
        public CarTelemetryData carTelemetry;
        public PacketHeader context;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LapDataInContext
    {
        public LapData lapData;
        public PacketHeader context;
    }

    #region Type 10: Car Damage
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct PacketCarDamageData
    {
        public PacketHeader header;               // Header

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 22, ArraySubType = UnmanagedType.Struct)]
        public CarDamageData[] carDamageData;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CarDamageData
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] tyresWear;                     // Tyre wear (percentage)
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] tyresDamage;                   // Tyre damage (percentage)
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] brakesDamage;                  // Brakes damage (percentage)
        public byte frontLeftWingDamage;              // Front left wing damage (percentage)
        public byte frontRightWingDamage;             // Front right wing damage (percentage)
        public byte rearWingDamage;                   // Rear wing damage (percentage)
        public byte floorDamage;                      // Floor damage (percentage)
        public byte diffuserDamage;                   // Diffuser damage (percentage)
        public byte sidepodDamage;                    // Sidepod damage (percentage)
        public byte drsFault;                         // Indicator for DRS fault, 0 = OK, 1 = fault
        public byte gearBoxDamage;                    // Gear box damage (percentage)
        public byte engineDamage;                     // Engine damage (percentage)
        public byte engineMGUHWear;                   // Engine wear MGU-H (percentage)
        public byte engineESWear;                     // Engine wear ES (percentage)
        public byte engineCEWear;                     // Engine wear CE (percentage)
        public byte engineICEWear;                    // Engine wear ICE (percentage)
        public byte engineMGUKWear;                   // Engine wear MGU-K (percentage)
        public byte engineTCWear;                     // Engine wear TC (percentage)
                                                      // Name of participant in UTF-8 format – null terminated
    }
    #endregion

    #region Type 11: Session History

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LapHistoryData
    {
        public UInt32 lapTimeInMS;           // Lap time in milliseconds
        public UInt16 sector1TimeInMS;       // Sector 1 time in milliseconds
        public UInt16 sector2TimeInMS;       // Sector 2 time in milliseconds
        public UInt16 sector3TimeInMS;       // Sector 3 time in milliseconds
        public byte lapValidBitFlags;      // 0x01 bit set-lap valid,      0x02 bit set-sector 1 valid
                                           // 0x04 bit set-sector 2 valid, 0x08 bit set-sector 3 valid
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct TyreStintHistoryData
    {
        public byte endLap;                // Lap the tyre usage ends on (255 of current tyre)
        public byte tyreActualCompound;    // Actual tyres used by this driver
        public byte tyreVisualCompound;    // Visual tyres used by this driver
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct PacketSessionHistoryData
    {
        PacketHeader header;                   // Header

        public byte carIdx;                   // Index of the car this lap data relates to
        public byte numLaps;                  // Num laps in the data (including current partial lap)
        public byte numTyreStints;            // Number of tyre stints in the data

        public byte bestLapTimeLapNum;        // Lap the best lap time was achieved on
        public byte bestSector1LapNum;        // Lap the best Sector 1 time was achieved on
        public byte bestSector2LapNum;        // Lap the best Sector 2 time was achieved on
        public byte bestSector3LapNum;        // Lap the best Sector 3 time was achieved on

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100, ArraySubType = UnmanagedType.Struct)]
        public LapHistoryData[] lapHistoryData;   // 100 laps of data max
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = UnmanagedType.Struct)]
        public TyreStintHistoryData[] tyreStintsHistoryData;
    }
    #endregion
}
