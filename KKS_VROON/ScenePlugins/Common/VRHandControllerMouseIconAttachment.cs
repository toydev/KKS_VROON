using UnityEngine;

using KKS_VROON.Patches.HandPatches;
using KKS_VROON.VRUtils;

namespace KKS_VROON.ScenePlugins.Common
{
    public class VRHandControllerMouseIconAttachment : MonoBehaviour
    {
        #region Cursor
        void OnEnable()
        {
            CursorPatch.onChangeCursor += OnChangeCursor;
        }

        void OnDisable()
        {
            var handController = GetComponent<VRHandController>();
            if (handController) handController.SetHandIcon(null);
            CursorPatch.onChangeCursor -= OnChangeCursor;
        }

        void OnChangeCursor(Texture2D texture)
        {
            var handController = GetComponent<VRHandController>();
            if (handController) handController.SetHandIcon(texture);
        }
        #endregion
    }
}
