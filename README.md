[日本語](README.ja.md)

# KKS_VROON - Simple VR plugin for Koikatsu Sunshine
**It is a VR MOD compatible with both the main game and CharaStudio that you can play with one hand while lying down.**

KKS has a veriety of VR options. The main features of this MOD are as follows.

- You can reset the front at any time. Sitting / standing / lying down, you can play in any position you like.
- All mouse operations can be performed with just one hand controller. You can keep your other hand free.

----

# HMD Support
- Meta Quest 2

Pull requests for additional support are welcome.

----

# Prerequisites
- Koikatsu Sunshine
- Latest version of BepInEx 5.x and KKSAPI/ModdingAPI
- SteamVR
- Meta Quest 2

- Other VR mods must not be included

----

# How to play
Install [KKS_VROON](https://github.com/toydev/KKS_VROON/releases) into the game, connect the HMD and SteamVR, and start the game.

Enabled by detecting the SteamVR process on startup.

----

# Operations
## Mouse and VR controller
The left and right VR controller operations are the same.

|Mouse operation|VR controller operation|
|----|----|
|Left click|Press trigger|
|Right click|Press grip|
|Middle click|Press X / Press A|
|Select menu with pointer|Select menu with laser pointer|
|Select a body part with pointer|Select a body part with laser pointer|
|Lateral rotation when moving|Joystick|
|Vertical rotation when moving|Looking up with HMD|

## VR controller only
|VR controller Operation|Action|
|----|----|
|Press Y / Press B|Reset the front of the VR camera|

----

# Configuration Options
The plugin provides the following configuration options:

|Setting Name|Type|Default Value|Description|
|----|----|----|----|
|GameWindowTopMost|Boolean|`true`|Set the game window to always be on top.|
|EnableMirror|Boolean|`false`|Enable or disable mirror in VR.|
|MainGameAxisScalingFactor|Float|`30.0f`|Scaling factor for hand controller axis in MainGame.|
|CharaStudioAxisScalingFactor|Float|`50.0f`|Scaling factor for hand controller axis in CharaStudio.|

You can change these settings via the BepInEx configuration manager.

----

# For developers
## Issues / Pull requests

Please write in English or Japanese.

- Bug report
- Added controller support
- Other
  - Provide knowledge about this project if there is a feature you wolud like to implement.

I don't usually use CharaStudio, so please write details about CharaStudio.

## Document
- [How to set up this project](/docs/project/HOW_TO_SETUP_THIS_PROJECT.md)
- [Basic implementation concepts](/docs/project/BASIC_IMPLEMENTATION_CONCEPTS.md)
- [About the VR controller](/docs/project/ABOUT_VR_CONTROLLER.md)
- [How to create a SteamVR project from scratch](/docs/project/HOW_TO_CREATE_STEAMVR_PROJECT.md)
