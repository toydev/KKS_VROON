# About the VR controller
SteamVR_Input maps between VR controller manipulation and programmatic action names.

Detailed definitions are in the file below.

- [actions.json](/SteamVRUnity/Assets/StreamingAssets/SteamVR/actions.json)

actions.json sets the names and types of actions used, and references to configuration files for each device.

Each device's configuraiton file defines mappings. The configuraiton file must be created for each device type.

UnityEditor has a editor for these files.

Open the SteamVRUnity project in UnityEditor and select "Window" ⇒ "SteamVR Input" from the menu. Next, Press the "Open binding UI" button. However, that editor is difficult, so I edited directly with a text editor.

If you add or change the prerequisite action, you need to regenerate the SteamVR_Iput related source with "Save and generate" button.

The SteamVRLib_Valve.VR project references those sources as link of existing item. Also adjust the SteamVRLib_Valve.VR project if the variation of the output source changes.
