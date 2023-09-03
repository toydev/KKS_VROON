# Basic implementation concepts
## 1. Use game camera perspective as is.
Sets the game camera as the parent of the VR camera. And fix the VR camera's local transform considering the front.

- [VRCamera.cs](/KKS_VROON/VRUtils/VRCamera.cs)
```C#:VRCamera.cs
VR.origin.rotation = parentCamera.transform.rotation * Quaternion.Inverse(BaseHeadLocalRotation);
VR.origin.position = parentCamera.transform.position - VR.origin.rotation * BaseHeadLocalPosition;
VR.origin.SetParent(parentCamera.transform);
```

BaseHeadLocalPosition and BaseHeadLocalRotation are set once at startup and then reset at the player's request.

Once the VR camera's local transform is fixed, the VR camera transform can be left to the game.

----

## 2. Convert hand controller operation to basic mouse operation
Convert hand controller button operations to mouse operations using mouse_event.

- [BasicMouseEmulator.cs](/KKS_VROON/ScenePlugins/Common/BasicMouseEmulator.cs)
  - SendMouseEvent

In addition, HarmonyLib interrupts the operation of the hand controllers.

Additionally, replace the following operations in HarmonyLib.

- [InputPatch.cs](/KKS_VROON/Patches/InputPatches/InputPatch.cs)
  - UnityEngine.Input.GetAxis
- [ScreenPointToRayPatch.cs](/KKS_VROON/Patches/HandPatches/ScreenPointToRayPatch.cs)
  - UnityEngine.Camera.ScreenPointToRay

Simply interrupting ScreenPointToRay will have a large impact, and UI operations will not work.

Therefore, the interrupt valid / invalid section is controlled by Prefix / Finalizer of a specific function.

- [SendMouseEventsPatch.cs](/KKS_VROON/Patches/HandPatches/SendMouseEventsPatch.cs)
```C#:SendMouseEventsPatch.cs
[HarmonyPatch(typeof(SendMouseEvents))]
public class SendMouseEventsPatch
{
    [HarmonyPatch(nameof(SendMouseEvents.DoSendMouseEvents))]
    [HarmonyPrefix]
    public static bool Prefix()
    {
        ScreenPointToRayPatch.Enabled = false;
        return true;
    }

    [HarmonyPatch(nameof(SendMouseEvents.DoSendMouseEvents))]
    [HarmonyFinalizer]
    public static Exception Finalizer(Exception __exception)
    {
        ScreenPointToRayPatch.Enabled = true;
        return __exception;
    }
}
```

However, this method is not perfect. It cannot be used if the valid / invalid section cannot be specified.
