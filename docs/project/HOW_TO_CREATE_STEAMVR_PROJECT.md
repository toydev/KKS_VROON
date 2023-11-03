# How to create a SteamVR project from scratch
## Prerequisites
- Set the Koikatsu Sunshine installation directory to the KKS_VROON_GAME_HOME environment variable (Example: KKS_VROON_GAME_HOME=C:\illusion\KoikatsuSunshine).
- [Visual Studio Community](https://visualstudio.microsoft.com/vs/community/)
- [Unity Hub](https://unity.com/download)

## Preparing IllusionLibs
Configure nuget.config according to "How to use IllusionLibs in your project" below.

https://github.com/IllusionMods/IllusionLibs

## Preparing SteamVR
### Create a SteamVRUnity project into the solution
- Install Unity Editor(2021.3.27f1) ... I forgot why I chose this version.
- Create a SteamVRUnity Unity project with VR template.
- Import the SteamVR Plugin(https://assetstore.unity.com/packages/tools/integration/steamvr-plugin-32647?locale=ja-JP).
- Select "Window" ⇒ "SteamVR Input" in UnityEditor, open the "SteamVR Input" window, press the "Save and generate" button, and generate the SteamVR_Input source.
- Set a .gitignore(https://github.com/github/gitignore/blob/main/Unity.gitignore).
- Modify Assets\XR\Settings\OpenVRSettings.asset
  - StereoRenderingMode: 1 => 0

### Build in Visual Studio
Create some Class Library (.NET Framework) projects as follows.
Add the source with Add As Link of Existing Item.
- SteamVRLib_UnityEngine.SpatialTracking
  - source: SteamVRUnity\Library\PackageCache\com.unity.xr.legacyinputhelpers@2.1.10\Runtime\TrackedPoseDriver\*.cs
  - conditional compliation symbols: UNITY_2019_3_OR_NEWER,ENABLE_VR
  - references:
    - IllusionLibs.KoikatsuSunshine.UnityEngine.CoreModules
    - IllusionLibs.KoikatsuSunshine.UnityEngine.XRModule
- SteamVRLib_UnityEngine.XR.Management
  - source: SteamVRUnity\Library\PackageCache\com.unity.xr.management@4.3.3\Runtime\*.cs
  - conditional compliation symbols: -
  - references:
    - IllusionLibs.KoikatsuSunshine.UnityEngine.CoreModules
    - UnityEngine.SubsystemsModule.dll (from $(KKS_VROON_GAME_HOME)\KoikatsuSunshine_Data\Managed)
- SteamVRLib_Unity.XR.OpenVR
  - source: SteamVRUnity\Library\PackageCache\com.valvesoftware.unity.openvr@3ee6c452bc34\Runtime\*.cs
  - conditional compliation symbols: UNITY_5_3_OR_NEWER,UNITY_XR_MANAGEMENT
  - references:
    - SteamVRLib_UnityEngine.XR.Management
    - IllusionLibs.KoikatsuSunshine.UnityEngine.CoreModules
    - IllusionLibs.KoikatsuSunshine.UnityEngine.XRModule
    - UnityEngine.SubsystemsModule.dll (from $(KKS_VROON_GAME_HOME)\KoikatsuSunshine_Data\Managed)
- SteamVRLib_Valve.VR
  - source:
    - SteamVRUnity\Assets\SteamVR\Input\*.cs
    - SteamVRUnity\Assets\SteamVR\Input\BehaviourUnityEvents\*.cs
    - SteamVRUnity\Assets\SteamVR\Scripts\*.cs
    - SteamVRUnity\Assets\SteamVR_Input\*.cs
    - SteamVRUnity\Assets\SteamVR_Input\ActionSetClasses\*.cs
  - conditional compliation symbols: UNITY_5_3_OR_NEWER,UNITY_2017_2_OR_NEWER,OPENVR_XR_API
  - references:
    - SteamVRLib_Unity.XR.OpenVR
    - IllusionLibs.KoikatsuSunshine.UnityEngine.AudioModule
    - IllusionLibs.KoikatsuSunshine.UnityEngine.CoreModules
    - IllusionLibs.KoikatsuSunshine.UnityEngine.ImageConversionModule
    - IllusionLibs.KoikatsuSunshine.UnityEngine.IMGUIModule
    - IllusionLibs.KoikatsuSunshine.UnityEngine.InputLegacyModule
    - IllusionLibs.KoikatsuSunshine.UnityEngine.PhysicsModule
    - IllusionLibs.KoikatsuSunshine.UnityEngine.VRModule
    - Valve.Newtonsoft.Json.dll (from SteamVRUnity\Assets\SteamVR\Input\Plugins\JSON.NET)
