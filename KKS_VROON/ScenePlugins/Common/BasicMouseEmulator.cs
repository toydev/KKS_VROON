using UnityEngine;

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

        private float ScaleAxis(float value)
        {
            return Mathf.Sign(value) * Mathf.Pow(Mathf.Abs(value), 2);
        }

        private float getDeltaTime()
        {
            return 0.0f < Time.deltaTime ? Time.deltaTime : 1.0f / 60.0f;
        }

        public virtual float? GetAxis(string axisName)
        {
            if (!HandController) return null;

            if (axisName == "Mouse X") return ScaleAxis(HandController.State.JoystickAxis.x) * PluginConfig.JoystickViewSpeed * getDeltaTime();
            else if (axisName == "Mouse Y") return ScaleAxis(HandController.State.JoystickAxis.y) * PluginConfig.JoystickViewSpeed * getDeltaTime();
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
            if (0.01f < Mathf.Abs(HandController.State.JoystickAxis.y)) MouseKeyboardUtils.MouseWheel((int)(ScaleAxis(HandController.State.JoystickAxis.y) * PluginConfig.MouseWheelScalingFactor.Value * getDeltaTime()));
        }
    }
}
