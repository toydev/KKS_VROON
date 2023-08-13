using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace KKS_VROON.WindowNativeUtils
{
    public class WindowUtils
    {
        public static void InitializeGameWindowHandle()
        {
            GameWindowHandle = NativeMethods.GetForegroundWindow();
        }

        public static Rect GetGameWindowRect()
        {
            var rect = default(RECT);
            NativeMethods.GetClientRect(GameWindowHandle, ref rect);
            var point = default(POINT);
            NativeMethods.ClientToScreen(GameWindowHandle, ref point);
            rect.Left = point.X;
            rect.Top = point.Y;
            rect.Right += point.X;
            rect.Bottom += point.Y;
            return new Rect(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
        }

        private static IntPtr GameWindowHandle { get; set; }

        #region NativeMethods
        public class NativeMethods
        {
            [DllImport("user32.dll")]
            public static extern IntPtr GetForegroundWindow();

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool GetClientRect(IntPtr hWnd, ref RECT lpRect);

            [DllImport("USER32.dll")]
            public static extern bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);
        }

        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            public RECT(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }
        }
        #endregion
    }
}
