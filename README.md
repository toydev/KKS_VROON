[日本語](README.ja.md)

# KKS_VROON - Simple VR plugin for Koikatsu Sunshine

**Preparing for release**

"It is a VR mod based on the concept of lying down and playing with one hand."

Do you know why this concept is important?

The concept was important to me. Howevery, modifying KKS_VR was too difficult for me.
So I made a simple VR mod from scratch. Of course, KKS_VR is a great mod and I used it as a great reference.

The following is the basic idea of implementation.

- Simply use the perspective of the game camera for the VR camera. And you can reset the front of the VR camera at any time.
- Convert hand controller operation to mouse operation. All mouse operations can be performed with one hand.

Currently, only the main game is supported, but I think that the same concept can be applied to CharaStudio.
Maybe it needs to support IMGUI

----

# Controller Support

- Oculus Quest 2

Welcome pull requests.

----

# How to play

Install this mod into the game, connect the HMD and SteamVR, and start the game.

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
