[日本語](README.ja.md)

# KKS_VROON - Simple VR plugin for Koikatsu Sunshine

**It is a VR mod based on the concept of lying down and playing with one hand.**

Do you know why this concept is important?

The concept was important to me. Howevery, modifying KKS_VR was too difficult for me.
So I made a simple VR mod from scratch. Of course, KKS_VR is a great mod and I used it as a great reference.

The following is the basic idea of implementation.

- Simply use the perspective of the game camera for the VR camera. And you can reset the front of the VR camera at any time. It is easy to do because the camera control is simple.
- Convert hand controller operation to mouse operation. All mouse operations can be performed with one hand.

Currently, only the main game is supported, but I think that the same concept can be applied to CharaStudio.
Maybe it needs to support IMGUI

----

# Controller Support

- Meta Quest 2

Welcome pull requests for any controller support.

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
## Mouse and Hand controller
The left and right hand controller operations are the same.

|Mouse operation|Hand controller operation|
|----|----|
|Left click|Press trigger|
|Right click|Press grip|
|Middle click|Press X / Press A|
|Select menu with pointer|Select menu with laser pointer|
|Select a body part with pointer|Select a body part with laser pointer|
|Lateral rotation when moving|Joystick|
|Vertical rotation when moving|Looking up with HMD|

## Hand controller only
|Hand controller Operation|Action|
|----|----|
|Press Y / Press B|Reset the front of the VR camera|

----

# For developers

- [How to set up this project](/docs/project/HOW_TO_SETUP_THIS_PROJECT.md)
- [Basic concepts and their implementations](/docs/project/BASIC_CONCEPTS_AND_IMPLEMENTATIONS.md)
- [How to create a SteamVR project from scratch](/docs/projects/HOW_TO_CREATE_STEAMVR_PROJECT.md)
