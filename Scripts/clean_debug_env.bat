@ECHO OFF
SETLOCAL

CALL variables.bat

REM Plugin
IF EXIST "%KKS_VROON_GAME_HOME%\BepInEx\plugins\KKS_VROON" (
  ECHO DELETE %KKS_VROON_GAME_HOME%\BepInEx\plugins\KKS_VROON
  RMDIR /S /Q "%KKS_VROON_GAME_HOME%\BepInEx\plugins\KKS_VROON"
)

REM ========== KoikatsuSunshine_Data
REM DLL files
IF EXIST "%KKS_VROON_GAME_HOME%\KoikatsuSunshine_Data\Plugins\x86_64\openvr_api.dll" (
  ECHO DELETE %KKS_VROON_GAME_HOME%\KoikatsuSunshine_Data\Plugins\x86_64\openvr_api.dll
  DEL "%KKS_VROON_GAME_HOME%\KoikatsuSunshine_Data\Plugins\x86_64\openvr_api.dll"
)

IF EXIST "%KKS_VROON_GAME_HOME%\KoikatsuSunshine_Data\Plugins\x86_64\XRSDKOpenVR.dll" (
  ECHO DELETE %KKS_VROON_GAME_HOME%\KoikatsuSunshine_Data\Plugins\x86_64\XRSDKOpenVR.dll
  DEL "%KKS_VROON_GAME_HOME%\KoikatsuSunshine_Data\Plugins\x86_64\XRSDKOpenVR.dll"
)

REM SteamVR Input action files
IF EXIST "%KKS_VROON_GAME_HOME%\KoikatsuSunshine_Data\StreamingAssets\SteamVR\*.json" (
  ECHO DELETE %KKS_VROON_GAME_HOME%\KoikatsuSunshine_Data\StreamingAssets\SteamVR\*.json
  DEL "%KKS_VROON_GAME_HOME%\KoikatsuSunshine_Data\StreamingAssets\SteamVR\*.json"
)

REM OpenVRSettings.asset
IF EXIST "%KKS_VROON_GAME_HOME%\KoikatsuSunshine_Data\StreamingAssets\SteamVR\OpenVRSettings.asset" (
  ECHO DELETE %KKS_VROON_GAME_HOME%\KoikatsuSunshine_Data\StreamingAssets\SteamVR\OpenVRSettings.asset
  DEL "%KKS_VROON_GAME_HOME%\KoikatsuSunshine_Data\StreamingAssets\SteamVR\OpenVRSettings.asset"
)

REM UnitySubsystemsManifest.json
IF EXIST "%KKS_VROON_GAME_HOME%\KoikatsuSunshine_Data\UnitySubsystems\XRSDKOpenVR\UnitySubsystemsManifest.json" (
  ECHO DELETE %KKS_VROON_GAME_HOME%\KoikatsuSunshine_Data\UnitySubsystems\XRSDKOpenVR\UnitySubsystemsManifest.json
  DEL "%KKS_VROON_GAME_HOME%\KoikatsuSunshine_Data\UnitySubsystems\XRSDKOpenVR\UnitySubsystemsManifest.json"
)

REM ========== CharaStudio_Data
REM DLL files
IF EXIST "%KKS_VROON_GAME_HOME%\CharaStudio_Data\Plugins\x86_64\openvr_api.dll" (
  ECHO DELETE %KKS_VROON_GAME_HOME%\CharaStudio_Data\Plugins\x86_64\openvr_api.dll
  DEL "%KKS_VROON_GAME_HOME%\CharaStudio_Data\Plugins\x86_64\openvr_api.dll"
)

IF EXIST "%KKS_VROON_GAME_HOME%\CharaStudio_Data\Plugins\x86_64\XRSDKOpenVR.dll" (
  ECHO DELETE %KKS_VROON_GAME_HOME%\CharaStudio_Data\Plugins\x86_64\XRSDKOpenVR.dll
  DEL "%KKS_VROON_GAME_HOME%\CharaStudio_Data\Plugins\x86_64\XRSDKOpenVR.dll"
)

REM SteamVR Input action files
IF EXIST "%KKS_VROON_GAME_HOME%\CharaStudio_Data\StreamingAssets\SteamVR\*.json" (
  ECHO DELETE %KKS_VROON_GAME_HOME%\CharaStudio_Data\StreamingAssets\SteamVR\*.json
  DEL "%KKS_VROON_GAME_HOME%\CharaStudio_Data\StreamingAssets\SteamVR\*.json"
)

REM OpenVRSettings.asset
IF EXIST "%KKS_VROON_GAME_HOME%\CharaStudio_Data\StreamingAssets\SteamVR\OpenVRSettings.asset" (
  ECHO DELETE %KKS_VROON_GAME_HOME%\CharaStudio_Data\StreamingAssets\SteamVR\OpenVRSettings.asset
  DEL "%KKS_VROON_GAME_HOME%\CharaStudio_Data\StreamingAssets\SteamVR\OpenVRSettings.asset"
)

REM UnitySubsystemsManifest.json
IF EXIST "%KKS_VROON_GAME_HOME%\CharaStudio_Data\UnitySubsystems\XRSDKOpenVR\UnitySubsystemsManifest.json" (
  ECHO DELETE %KKS_VROON_GAME_HOME%\CharaStudio_Data\UnitySubsystems\XRSDKOpenVR\UnitySubsystemsManifest.json
  DEL "%KKS_VROON_GAME_HOME%\CharaStudio_Data\UnitySubsystems\XRSDKOpenVR\UnitySubsystemsManifest.json"
)

IF "%1" NEQ "called" PAUSE
