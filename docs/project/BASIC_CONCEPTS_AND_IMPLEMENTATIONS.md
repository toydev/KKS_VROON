# Basic concepts and their implementations
## Basic concepts

1. Simply use the perspective of the game camera for the VR camera. And you can reset the front of the VR camera at any time. It is easy to do because the camera control is simple.
2. Convert hand controller operation to mouse operation. All mouse operations can be performed with one hand.

----

## Their implementations
### 1. Simply use the perspective of the game camera for the VR camera
Parent the game camera to the VR camera. Then, fix the local transform of the VR camera by adding the front.

- [ActiveScenePlugin.cs](/KKS_VROON/ScenePlugins/ActiveScene/ActiveScenePlugin.cs)
```C#:ActiveScenePlugin.cs
// Link the game main camera and the VR main camera.
MainCamera.VR.origin.rotation = gameMainCamera.transform.rotation * Quaternion.Inverse(VRCamera.BaseHeadLocalRotation);
MainCamera.VR.origin.position = gameMainCamera.transform.position - MainCamera.VR.origin.rotation * VRCamera.BaseHeadLocalPosition;
MainCamera.VR.origin.SetParent(gameMainCamera.transform);
```

VRCamera.BaseHeadLocalPosition and VRCamera.BaseHeadLocalRotation are set once at startup and then reset at the player's request.

Once the relative position is fixed, the VR camera transform can be left to the game.

### 2. Convert hand controller operation to mouse operation
HarmonyLib interrupts the following basic mouse operations with hand controller operations.

- [InputPatch.cs](/KKS_VROON/Patches/InputPatches/InputPatch.cs)
  - UnityEngine.Input.GetAxis
  - UnityEngine.Input.GetMouseButton
  - UnityEngine.Input.GetMouseButtonDown
  - UnityEngine.Input.GetMouseButtonUp
- [ScreenPointToRayPatch.cs](/KKS_VROON/Patches/HandPatches/ScreenPointToRayPatch.cs)
  - UnityEngine.Camera.ScreenPointToRay

Simply interrupting ScreenPointToRay will have a large impact, and UI operations will not work.

Therefore, the interrupt valid section is controlled by Prefix / Finalizer of a specific function.

- [HandCtrlPatch.cs](/KKS_VROON/Patches/HandPatches/HandCtrlPatch.cs)
```C#:HandCtrlPatch.cs
[HarmonyPatch(typeof(HandCtrl))]
public class HandCtrlPatch
{
    [HarmonyPatch(nameof(HandCtrl.LateProc))]
    [HarmonyPrefix]
    public static bool PrefixLateProc()
    {
        ScreenPointToRayPatch.Enabled = true;
        return true;
    }

    [HarmonyPatch(nameof(HandCtrl.LateProc))]
    [HarmonyFinalizer]
    public static Exception FinalizerLateProc(Exception __exception)
    {
        ScreenPointToRayPatch.Enabled = false;
        return __exception;
    }
}
```

However, this method is not perfect. It cannot be used if the valid section cannot be specified.
