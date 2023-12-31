﻿using System.Linq;

using KKS_VROON.Patches.HandPatches;
using KKS_VROON.ScenePlugins.Common;
using KKS_VROON.VRUtils;

namespace KKS_VROON.ScenePlugins.ActionScene
{
    public class ActionSceneMouseEmulator : BasicMouseEmulator
    {
        public ActionSceneMouseEmulator(VRHandController handController) : base(handController)
        {
        }

        public override float? GetAxis(string axisName)
        {
            if (!HandController) return null;

            // Translate hand movements to mouse movements while processing HandCtrlPatch.
            if (HandCtrlPatch.Processing)
            {
                var deltaThreshold = 0.1f;  /* 10cm */
                if (axisName == "Mouse X")
                {
                    var value = HandController.State.PositionDelta.x;
                    if (value <= -deltaThreshold) return -1.0f;
                    if (deltaThreshold <= value) return 1.0f;
                    return value / deltaThreshold;
                }
                else if (axisName == "Mouse Y")
                {
                    var value = HandController.State.PositionDelta.y;
                    if (value <= -deltaThreshold) return -1.0f;
                    if (deltaThreshold <= value) return 1.0f;
                    return value / deltaThreshold;
                }
            }
            // Stop vertical movement of vision when walking.
            else if (Manager.Scene.NowSceneNames?.First() == SceneNames.ACTION && axisName == "Mouse Y")
            {
                return null;
            }

            return base.GetAxis(axisName);
        }
    }
}
