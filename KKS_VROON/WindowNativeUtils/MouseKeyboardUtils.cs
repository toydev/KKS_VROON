using System.Runtime.InteropServices;
using UnityEngine;

namespace KKS_VROON.WindowNativeUtils
{
    public class MouseKeyboardUtils
    {
        public static void SetCursorPos(Vector2 pos)
        {
            NativeMethods.SetCursorPos((int)pos.x, (int)pos.y);
        }

        public static class NativeMethods
        {
            [DllImport("user32.dll")]
            public static extern bool SetCursorPos(int x, int y);
        }
    }
}
