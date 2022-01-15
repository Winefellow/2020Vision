# 2020Vision
C# F1 2020 Telemetry Data Plotter and analyzer

Instructions:
- Run compiled version of 2020 Vision
- Start F1 2020 and set telemetry IP address to IP Address of the machine running 2020 vision (port 20777)
- Play F1 2020 (Click player to see live speed)
- Session is auto recorded. Laps that are valid and completed are extracted automatically into directory for tracks

Features:
- Record and Replay session
- Analyze recorded session (Extra laps)
- Lap database
- Compare multiple laps

Status:
- Hobby project, Version 0.1
- Limited UI
- Zero config 
- Basic, but limited functionality

Bugs:
- Only 1 lap per player can be selected. Crashes otherwise
- Laps of different tracks may be compared. Pretty useless.

Techniques:
- Struct packet reader in C#
- Paint current state based on time slots (50fps)
 
Plans:
- f1 2021 support
- Audio feedback (Apex speed, distance and angle)
- Select only section of lap (in stead of whole lap)
- Pause, rewind, slowmo replay of lap
- Better lap filtering

