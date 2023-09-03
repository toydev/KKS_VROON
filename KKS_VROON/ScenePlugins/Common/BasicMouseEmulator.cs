using KKS_VROON.Patches.InputPatches;
using KKS_VROON.VRUtils;
using KKS_VROON.WindowNativeUtils;

namespace KKS_VROON.ScenePlugins.Common
{
    public class BasicMouseEmulator : IMouseEmulator
    {
        public BasicMouseEmulator(VRHandController handController)
        {
            HandController = handController;
        }

        public VRHandController HandController { get; set; }

        public virtual float? GetAxis(string axisName)
        {
            if (!HandController) return null;

            if (axisName == "Mouse X") return HandController.State.JoystickAxis.x;
            else if (axisName == "Mouse Y") return HandController.State.JoystickAxis.y;
            return null;
        }

        public virtual void SendMouseEvent()
        {
            if (HandController.State.IsTriggerUp) MouseKeyboardUtils.MouseLeftUp();
            if (HandController.State.IsGripUp) MouseKeyboardUtils.MouseRightUp();
            if (HandController.State.IsButtonXUp || HandController.State.IsButtonAUp) MouseKeyboardUtils.MouseMiddleUp();
            if (HandController.State.IsTriggerDown) MouseKeyboardUtils.MouseLeftDown();
            if (HandController.State.IsGripDown) MouseKeyboardUtils.MouseRightDown();
            if (HandController.State.IsButtonXDown || HandController.State.IsButtonADown) MouseKeyboardUtils.MouseMiddleDown();
        }
    }
}
