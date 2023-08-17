@ECHO OFF
SETLOCAL

CALL variables.bat

IF EXIST "%KKS_VROON_PACKAGE_DIR%" (
  RMDIR /s /q "%KKS_VROON_PACKAGE_DIR%"
)

REM Plugin
MKDIR "%KKS_VROON_PACKAGE_DIR%\BepInEx\plugins\KKS_VROON"
COPY "%KKS_VROON_SOLUTION_DIR%\KKS_VROON\bin\%KKS_SOLUTION_CONFIGURATION%\*.dll" "%KKS_VROON_PACKAGE_DIR%\BepInEx\plugins\KKS_VROON"

REM DLL files
MKDIR "%KKS_VROON_PACKAGE_DIR%\KoikatsuSunshine_Data\Plugins\x86_64"
COPY "%KKS_VROON_SOLUTION_DIR%\SteamVRUnity\Library\PackageCache\com.valvesoftware.unity.openvr@3ee6c452bc34\Runtime\x64\openvr_api.dll" "%KKS_VROON_PACKAGE_DIR%\KoikatsuSunshine_Data\Plugins\x86_64"
COPY "%KKS_VROON_SOLUTION_DIR%\SteamVRUnity\Library\PackageCache\com.valvesoftware.unity.openvr@3ee6c452bc34\Runtime\x64\XRSDKOpenVR.dll" "%KKS_VROON_PACKAGE_DIR%\KoikatsuSunshine_Data\Plugins\x86_64"

REM SteamVR Input action files
MKDIR "%KKS_VROON_PACKAGE_DIR%\KoikatsuSunshine_Data\StreamingAssets\SteamVR"
COPY "%KKS_VROON_SOLUTION_DIR%\SteamVRUnity\Assets\StreamingAssets\SteamVR\*.json" "%KKS_VROON_PACKAGE_DIR%\KoikatsuSunshine_Data\StreamingAssets\SteamVR"

REM OpenVRSettings.asset
COPY "%KKS_VROON_SOLUTION_DIR%\SteamVRUnity\Assets\XR\Settings\OpenVRSettings.asset" "%KKS_VROON_PACKAGE_DIR%\KoikatsuSunshine_Data\StreamingAssets\SteamVR"

REM UnitySubsystemsManifest.json
MKDIR "%KKS_VROON_PACKAGE_DIR%\KoikatsuSunshine_Data\UnitySubsystems\XRSDKOpenVR"
COPY "%KKS_VROON_SOLUTION_DIR%\SteamVRUnity\Library\PackageCache\com.valvesoftware.unity.openvr@3ee6c452bc34\Runtime\UnitySubsystemsManifest.json" "%KKS_VROON_PACKAGE_DIR%\KoikatsuSunshine_Data\UnitySubsystems\XRSDKOpenVR"

powershell Compress-Archive -Path "%KKS_VROON_PACKAGE_DIR%\*" -DestinationPath "%KKS_VROON_PACKAGE_DIR%\KKS_VROON.zip"

PAUSE
