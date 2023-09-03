using KKS_VROON.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using UnityEngine;

namespace KKS_VROON.WindowNativeUtils
{
    public class WindowUtils
    {
        public static bool InitializeGameWindowHandle()
        {
            var currentProcessId = (uint)Process.GetCurrentProcess().Id;
            NativeMethods.EnumWindows((hWnd, _) =>
            {
                NativeMethods.GetWindowThreadProcessId(hWnd, out var processId);
                if (processId == currentProcessId && GetWindowClassName(hWnd) == "UnityWndClass")
                {
                    GameWindowHandle = hWnd;
                    return false;
                }

                return true;
            }, IntPtr.Zero);

            return GameWindowHandle != IntPtr.Zero;
        }

        public static bool IsSteamVRRunning()
        {
            try
            {
                return Process.GetProcesses().Any(i => i.ProcessName == "vrcompositor");
            }
            catch (Exception e)
            {
                PluginLog.Error(e.Message);
                return false;
            }
        }

        public static void MakeWindowTopMost()
        {
            PluginLog.Debug("GameWindow set to topmost.");
            NativeMethods.SetWindowPos(GameWindowHandle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
        }

        public static Rect GetGameClientRect() => GetClientRect(GameWindowHandle);

        public static Rect GetClientRect(IntPtr hWnd)
        {
            var rect = default(RECT);
            NativeMethods.GetClientRect(hWnd, ref rect);
            var point = default(POINT);
            NativeMethods.ClientToScreen(hWnd, ref point);
            rect.Left = point.X;
            rect.Top = point.Y;
            rect.Right += point.X;
            rect.Bottom += point.Y;
            return new Rect(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
        }

        public static Rect GetWindowRect(IntPtr hWnd)
        {
            var rect = default(RECT);
            NativeMethods.GetWindowRect(hWnd, ref rect);
            return new Rect(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
        }

        public static string GetWindowClassName(IntPtr hWnd)
        {
            var result = new StringBuilder(256);
            NativeMethods.GetClassName(hWnd, result, result.Capacity);
            return result.ToString();

        }

        public static List<IntPtr> GetGameChildWindows()
        {
            var currentProcessId = (uint)Process.GetCurrentProcess().Id;
            var result = new List<IntPtr>();
            NativeMethods.EnumWindows((hWnd, _) =>
            {
                NativeMethods.GetWindowThreadProcessId(hWnd, out var processId);
                if (processId == currentProcessId) result.Add(hWnd);
                return true;
            }, IntPtr.Zero);
            return result;
        }

        private static IntPtr GameWindowHandle { get; set; } = IntPtr.Zero;

        #region NativeMethods
        public class NativeMethods
        {
            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool GetClientRect(IntPtr hWnd, ref RECT lpRect);

            [DllImport("USER32.dll")]
            public static extern bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

            public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

            [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
            public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

            [DllImport("user32.dll")]
            public static extern bool IsWindowVisible(IntPtr hWnd);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
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

        public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        public const uint SWP_NOSIZE = 0x0001;
        public const uint SWP_NOMOVE = 0x0002;
        #endregion
    }
}
