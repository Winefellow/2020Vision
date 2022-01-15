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
        public UInt16 packetFormat;             // 2020
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
    };

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
    };

    
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
        public byte airTemperature;                  // Air temp. in degrees celsius
    };

    
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
    }
    #endregion

    #region Type 2: Lap Data
    
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LapData
    {
        public float lastLapTime;              // Last lap time in seconds
        public float currentLapTime;           // Current time around the lap in seconds

        //UPDATED in Beta 3:
        public UInt16 sector1TimeInMS;         // Sector 1 time in milliseconds
        public UInt16 sector2TimeInMS;         // Sector 2 time in milliseconds
        public float bestLapTime;              // Best lap time of the session in seconds
        public byte bestLapNum;                // Lap number best time achieved on
        public UInt16 bestLapSector1TimeInMS;    // Sector 1 time of best lap in the session in milliseconds
        public UInt16 bestLapSector2TimeInMS;    // Sector 2 time of best lap in the session in milliseconds
        public UInt16 bestLapSector3TimeInMS;    // Sector 3 time of best lap in the session in milliseconds
        public UInt16 bestOverallSector1TimeInMS;// Best overall sector 1 time of the session in milliseconds
        public byte bestOverallSector1LapNum;    // Lap number best overall sector 1 time achieved on
        public UInt16 bestOverallSector2TimeInMS;// Best overall sector 2 time of the session in milliseconds
        public byte bestOverallSector2LapNum;    // Lap number best overall sector 2 time achieved on
        public UInt16 bestOverallSector3TimeInMS;// Best overall sector 3 time of the session in milliseconds
        public byte bestOverallSector3LapNum;    // Lap number best overall sector 3 time achieved on


        public float lapDistance;               // Distance vehicle is around current lap in metres – could
                                                // be negative if line hasn’t been crossed yet
        public float totalDistance;             // Total distance travelled in session in metres – could
                                                // be negative if line hasn’t been crossed yet
        public float safetyCarDelta;            // Delta in seconds for safety car
        public byte carPosition;                // Car race position
        public byte currentLapNum;              // Current lap number
        public byte pitStatus;                  // 0 = none, 1 = pitting, 2 = in pit area
        public byte sector;                     // 0 = sector1, 1 = sector2, 2 = sector3
        public byte currentLapInvalid;          // Current lap invalid - 0 = valid, 1 = invalid
        public byte penalties;                  // Accumulated time penalties in seconds to be added
        public byte gridPosition;               // Grid position the vehicle started the race in
        public byte driverStatus;               // Status of driver - 0 = in garage, 1 = flying lap
                                                // 2 = in lap, 3 = out lap, 4 = on track
        public byte resultStatus;               // Result status - 0 = invalid, 1 = inactive, 2 = active
                                                // 3 = finished, 4 = disqualified, 5 = not classified
                                                // 6 = retired
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
    }

    
    public enum EventType
    {
        Unknown, SessionStarted, SessionEnded, FastestLap, Retirement, DRSEnabled, DRSDisabled,
        TeammateInPits, CheckeredFlag, RaceWinner, PenaltyIssued, SpeedTrapTriggered
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
        public byte teamId;                 // Team id - see appendix
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
        public byte m_frontWing;                // Front wing aero
        public byte m_rearWing;                 // Rear wing aero
        public byte m_onThrottle;               // Differential adjustment on throttle (percentage)
        public byte byteoffThrottle;              // Differential adjustment off throttle (percentage)
        public float m_frontCamber;              // Front camber angle (suspension geometry)
        public float m_rearCamber;               // Rear camber angle (suspension geometry)
        public float m_frontToe;                 // Front toe angle (suspension geometry)
        public float m_rearToe;                  // Rear toe angle (suspension geometry)
        public byte bytefrontSuspension;          // Front suspension
        public byte byterearSuspension;           // Rear suspension
        public byte bytefrontAntiRollBar;         // Front anti-roll bar
        public byte byterearAntiRollBar;          // Front anti-roll bar
        public byte bytefrontSuspensionHeight;    // Front ride height
        public byte byterearSuspensionHeight;     // Rear ride height
        public byte bytebrakePressure;            // Brake pressure (percentage)
        public byte bytebrakeBias;                // Brake bias (percentage)
        public float m_rearLeftTyrePressure;     // Rear left tyre pressure (PSI)
        public float m_rearRightTyrePressure;    // Rear right tyre pressure (PSI)
        public float m_frontLeftTyrePressure;    // Front left tyre pressure (PSI)
        public float m_frontRightTyrePressure;   // Front right tyre pressure (PSI)
        public byte byteballast;                  // Ballast
        public float m_fuelLoad;                 // Fuel load
    };


    
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PacketCarSetupData
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 22)]
        public CarSetupData[] m_carSetups;
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

        public UInt32 buttonStatus;   // Bit flags specifying which buttons are being pressed
                                      // currently - see appendices

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

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] tyresWear;             // Tyre wear percentage
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
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] tyresDamage;           // Tyre damage (percentage)
        public byte frontLeftWingDamage;     // Front left wing damage (percentage)
        public byte frontRightWingDamage;    // Front right wing damage (percentage)
        public byte rearWingDamage;          // Rear wing damage (percentage)

        // Added Beta 3:
        public byte drsFault;                // Indicator for DRS fault, 0 = OK, 1 = fault

        public byte engineDamage;            // Engine damage (percentage)
        public byte gearBoxDamage;           // Gear box damage (percentage)
        public byte vehicleFiaFlags;         // -1 = invalid/unknown, 0 = none, 1 = green
                                             // 2 = blue, 3 = yellow, 4 = red
        public float ersStoreEnergy;         // ERS energy store in Joules
        public byte ersDeployMode;           // ERS deployment mode, 0 = none, 1 = medium
                                             // 2 = overtake, 3 = hotlap
        public float ersHarvestedThisLapMGUK;  // ERS energy harvested this lap by MGU-K
        public float ersHarvestedThisLapMGUH;  // ERS energy harvested this lap by MGU-H
        public float ersDeployedThisLap;       // ERS energy deployed this lap
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
        public float bestLapTime;          // Best lap time of the session in seconds
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
        public char[] m_name;                // Name of participant in UTF-8 format – null terminated
                                             // Will be truncated with ... (U+2026) if too long
        public byte readyStatus;             // 0 = not ready, 1 = ready, 2 = spectating
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PacketLobbyInfoData
    {
        // Packet specific data
        public byte numPlayers;                    // Number of players in the lobby data
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 22)]
        public LobbyInfoData[] lobbyPlayers;
    };
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

}
