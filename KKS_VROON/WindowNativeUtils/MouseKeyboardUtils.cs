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

        public static void MouseLeftDown()
        {
            NativeMethods.mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
        }

        public static void MouseLeftUp()
        {
            NativeMethods.mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }

        public static void MouseRightDown()
        {
            NativeMethods.mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0);
        }

        public static void MouseRightUp()
        {
            NativeMethods.mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
        }

        public static void MouseMiddleDown()
        {
            NativeMethods.mouse_event(MOUSEEVENTF_MIDDLEDOWN, 0, 0, 0, 0);
        }

        public static void MouseMiddleUp()
        {
            NativeMethods.mouse_event(MOUSEEVENTF_MIDDLEUP, 0, 0, 0, 0);
        }

        public static void MouseWheel(int delta)
        {
            NativeMethods.mouse_event(MOUSEEVENTF_WHEEL, 0, 0, (uint)delta, 0);
        }

        public static class NativeMethods
        {
            [DllImport("user32.dll")]
            public static extern bool SetCursorPos(int x, int y);

            [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
            public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);
        }

        private const uint MOUSEEVENTF_LEFTDOWN = 0x02;
        private const uint MOUSEEVENTF_LEFTUP = 0x04;
        private const uint MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const uint MOUSEEVENTF_RIGHTUP = 0x10;
        private const uint MOUSEEVENTF_MIDDLEDOWN = 0x20;
        private const uint MOUSEEVENTF_MIDDLEUP = 0x40;
        private const uint MOUSEEVENTF_WHEEL = 0x0800;
    }
}
