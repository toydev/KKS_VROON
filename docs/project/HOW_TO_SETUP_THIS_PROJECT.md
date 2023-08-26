# How to set up this project
## Prerequisites
- Koikatsu Sunshine
  - Must be a standard installation directory (C:\illusion\KoikatsuSunshine).
- [Visual Studio Community](https://visualstudio.microsoft.com/vs/community/)
- [Unity Hub](https://unity.com/download)

----

## Setup
1. Open SteamVRUnity as a project from Unity Hub.

Download the Unity Editor as needed.

The Unity Editor downloads the SteamVR plugin sources. The SteamVRLib_Xxx Visual Studio project links to the sources.

2. Open KKS_VROON.sln in Visual Studio.

The build will pass if the necessary sources have been downloaded in the Unity Editor.

3. Run Scripts\setup_debug_env.bat.

Copy the resources required to run KKS_VROON to Koikatsu Sunshine.

Set the link of KKS_VROON's Release directory to Koikatsu Sunshine. So post-build copy is no longer needed.

Run clean_debug_env.bat when you no longer need it.

