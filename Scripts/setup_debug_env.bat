@ECHO OFF
SETLOCAL

CALL variables.bat

CALL clean_debug_env.bat called

REM Plugin
MKLINK /D "%KKS_ROOT_DIR%\BepInEx\plugins\KKS_VROON" "%KKS_VROON_SOLUTION_DIR%\KKS_VROON\bin\%KKS_SOLUTION_CONFIGURATION%"

REM ========== KoikatsuSunshine_Data
REM DLL files
COPY "%KKS_VROON_SOLUTION_DIR%\SteamVRUnity\Library\PackageCache\com.valvesoftware.unity.openvr@3ee6c452bc34\Runtime\x64\openvr_api.dll" "%KKS_ROOT_DIR%\KoikatsuSunshine_Data\Plugins\x86_64"
COPY "%KKS_VROON_SOLUTION_DIR%\SteamVRUnity\Library\PackageCache\com.valvesoftware.unity.openvr@3ee6c452bc34\Runtime\x64\XRSDKOpenVR.dll" "%KKS_ROOT_DIR%\KoikatsuSunshine_Data\Plugins\x86_64"

REM SteamVR Input action files
MKDIR "%KKS_ROOT_DIR%\KoikatsuSunshine_Data\StreamingAssets\SteamVR"
COPY "%KKS_VROON_SOLUTION_DIR%\SteamVRUnity\Assets\StreamingAssets\SteamVR\*.json" "%KKS_ROOT_DIR%\KoikatsuSunshine_Data\StreamingAssets\SteamVR"

REM OpenVRSettings.asset
COPY "%KKS_VROON_SOLUTION_DIR%\SteamVRUnity\Assets\XR\Settings\OpenVRSettings.asset" "%KKS_ROOT_DIR%\KoikatsuSunshine_Data\StreamingAssets\SteamVR"

REM UnitySubsystemsManifest.json
MKDIR "%KKS_ROOT_DIR%\KoikatsuSunshine_Data\UnitySubsystems\XRSDKOpenVR"
COPY "%KKS_VROON_SOLUTION_DIR%\SteamVRUnity\Library\PackageCache\com.valvesoftware.unity.openvr@3ee6c452bc34\Runtime\UnitySubsystemsManifest.json" "%KKS_ROOT_DIR%\KoikatsuSunshine_Data\UnitySubsystems\XRSDKOpenVR"

REM ========== CharaStudio_Data
REM DLL files
COPY "%KKS_VROON_SOLUTION_DIR%\SteamVRUnity\Library\PackageCache\com.valvesoftware.unity.openvr@3ee6c452bc34\Runtime\x64\openvr_api.dll" "%KKS_ROOT_DIR%\CharaStudio_Data\Plugins\x86_64"
COPY "%KKS_VROON_SOLUTION_DIR%\SteamVRUnity\Library\PackageCache\com.valvesoftware.unity.openvr@3ee6c452bc34\Runtime\x64\XRSDKOpenVR.dll" "%KKS_ROOT_DIR%\CharaStudio_Data\Plugins\x86_64"

REM SteamVR Input action files
MKDIR "%KKS_ROOT_DIR%\CharaStudio_Data\StreamingAssets\SteamVR"
COPY "%KKS_VROON_SOLUTION_DIR%\SteamVRUnity\Assets\StreamingAssets\SteamVR\*.json" "%KKS_ROOT_DIR%\CharaStudio_Data\StreamingAssets\SteamVR"

REM OpenVRSettings.asset
COPY "%KKS_VROON_SOLUTION_DIR%\SteamVRUnity\Assets\XR\Settings\OpenVRSettings.asset" "%KKS_ROOT_DIR%\CharaStudio_Data\StreamingAssets\SteamVR"

REM UnitySubsystemsManifest.json
MKDIR "%KKS_ROOT_DIR%\CharaStudio_Data\UnitySubsystems\XRSDKOpenVR"
COPY "%KKS_VROON_SOLUTION_DIR%\SteamVRUnity\Library\PackageCache\com.valvesoftware.unity.openvr@3ee6c452bc34\Runtime\UnitySubsystemsManifest.json" "%KKS_ROOT_DIR%\CharaStudio_Data\UnitySubsystems\XRSDKOpenVR"

PAUSE
