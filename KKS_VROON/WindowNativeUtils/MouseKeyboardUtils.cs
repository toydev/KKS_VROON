using System.Runtime.InteropServices;

namespace KKS_VROON.WindowNativeUtils
{
    public class MouseKeyboardUtils
    {
        public static class NativeMethods
        {
            [DllImport("user32.dll")]
            public static extern bool SetCursorPos(int x, int y);
        }
    }
}
