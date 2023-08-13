# PROJECT
## Preparing IllusionLibs
Configure nuget.config according to "How to use IllusionLibs in your project" below.

https://github.com/IllusionMods/IllusionLibs

## Preparing SteamVR
### Create a SteamVRUnity project into ths solution
- Install Unity Editor(2021.3.27f1) ... I forgot why I chose this version.
- Create a SteamVRUnity Unity project with VR template.
- Import the SteamVR Plugin(https://assetstore.unity.com/packages/tools/integration/steamvr-plugin-32647?locale=ja-JP).
- Select "Window" ⇒ "SteamVR Input" in UnityEditor, open the "SteamVR Input" window, press the "Save and generate" button, and generate the SteamVR_Input source.
- Set a .gitignore(https://github.com/github/gitignore/blob/main/Unity.gitignore).

### Build in Visual Studio
Create some Class Library (.NET Framework) projects as follows.
Add the source with Add As Link of Existing Item.
- SteamVRLib_UnityEngine.SpatialTracking
  - source: SteamVRUnity/Library/PackageCache/com.unity.xr.legacyinputhelpers@2.1.10/Runtime/TrackedPoseDriver/*.cs
  - conditional compliation symbols: UNITY_2019_3_OR_NEWER,ENABLE_VR
  - references:
    - IllusionLibs.KoikatsuSunshine.UnityEngine.CoreModules
    - IllusionLibs.KoikatsuSunshine.UnityEngine.XRModule
- SteamVRLib_UnityEngine.XR.Management
  - source: SteamVRUnityProject/Library/PackageCache/com.unity.xr.management@4.3.3/Runtime/*.cs
  - conditional compliation symbols: -
  - references:
    - IllusionLibs.KoikatsuSunshine.UnityEngine.CoreModules
    - UnityEngine.SubsystemsModule.dll (from C:\illusion\KoikatsuSunshine\KoikatsuSunshine_Data\Managed)
