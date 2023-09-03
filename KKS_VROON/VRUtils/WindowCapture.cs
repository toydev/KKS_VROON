using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

using UnityEngine;

using KKS_VROON.WindowNativeUtils;

namespace KKS_VROON.VRUtils
{
    public class WindowCapture : MonoBehaviour
    {
        public Func<IntPtr, bool> TargetWindowHandle { get; set; } = (hWnd) => false;
        public Func<IntPtr, Rect> TargetWindowRect { get; set; } = (hWnd) => WindowUtils.GetWindowRect(hWnd);

        private Thread CaptureThread { get; set; }
        private bool IsRunning { get; set; } = true;

        private IntPtr CurrentWindowHandle { get; set; } = IntPtr.Zero;
        private Rect Rect { get; set; }
        private Bitmap Bitmap { get; set; }
        private System.Drawing.Graphics Graphics { get; set; }
        private Bitmap FlippedBitmap { get; set; }
        private System.Drawing.Graphics FlippedGraphics { get; set; }

        private bool Show { get; set; }
        private byte[] TexturePixelData { get; set; }
        private Size TextureSize { get; set; }
        private Texture2D Texture2D { get; set; }

        void Update()
        {
            Monitor.Enter(this);
            if (TexturePixelData != null)
            {
                if (!Texture2D || Texture2D.width != TextureSize.Width || Texture2D.height != TextureSize.Height)
                {
                    if (Texture2D) Destroy(Texture2D);
                    Texture2D = new Texture2D(TextureSize.Width, TextureSize.Height, TextureFormat.BGRA32, false);
                }
                Texture2D.LoadRawTextureData(TexturePixelData);
                Texture2D.Apply();
                TexturePixelData = null;
            }
            Monitor.Exit(this);
        }

        void OnGUI()
        {
            if (Show && Texture2D)
            {
                var gameClientRect = WindowUtils.GetGameClientRect();
                GUI.depth = -100;
                GUI.DrawTexture(new Rect(Rect.xMin - gameClientRect.xMin, Rect.yMin - gameClientRect.yMin, Texture2D.width, Texture2D.height), Texture2D);
            }
        }

        #region Capture thread
        void Awake()
        {
            CaptureThread = new Thread(new ThreadStart(CaptureLoop));
            CaptureThread.Start();
        }

        void OnDestory()
        {
            if (CaptureThread != null && CaptureThread.IsAlive)
            {
                CaptureThread.Interrupt();
                CaptureThread.Join();
            }
            Graphics?.Dispose();
            Bitmap?.Dispose();
            FlippedGraphics?.Dispose();
            FlippedBitmap?.Dispose();
            if (Texture2D) Destroy(Texture2D);
        }

        private void CaptureLoop()
        {
            while (IsRunning)
            {
                var targetWindowHandle = WindowUtils.GetGameChildWindows().Where(TargetWindowHandle).FirstOrDefault();
                if (CurrentWindowHandle != targetWindowHandle)
                {
                    CurrentWindowHandle = targetWindowHandle;
                    var gameClientRect = WindowUtils.GetGameClientRect();
                    var dialogRect = WindowUtils.GetWindowRect(targetWindowHandle);

                    var dialogWidth = (int)dialogRect.width;
                    var dialogHeight = (int)dialogRect.height;

                    var gameWindowCenterX = (int)gameClientRect.x + (int)gameClientRect.width / 2;
                    var gameWindowCenterY = (int)gameClientRect.y + (int)gameClientRect.height / 2;

                    var newX = gameWindowCenterX - dialogWidth / 2;
                    var newY = gameWindowCenterY - dialogHeight / 2;

                    WindowUtils.NativeMethods.SetWindowPos(targetWindowHandle, IntPtr.Zero, newX, newY, dialogWidth, dialogHeight, 0);
                }
                var captureResult = Capture(CurrentWindowHandle);
                if (captureResult != null)
                {
                    Monitor.Enter(this);
                    Show = true;
                    TexturePixelData = captureResult.Item1;
                    TextureSize = captureResult.Item2;
                    Monitor.Exit(this);
                }
                else
                {
                    Monitor.Enter(this);
                    Show = false;
                    Monitor.Exit(this);
                }
                Thread.Sleep(10);
            }
        }

        private Tuple<byte[], Size> Capture(IntPtr targetWindowHandle)
        {
            if (targetWindowHandle == IntPtr.Zero) return null;
            Rect = TargetWindowRect(targetWindowHandle);
            if (Rect.width <= 0 || Rect.height <= 0) return null;
            var size = new Size((int)Rect.width, (int)Rect.height);

            if (Bitmap == null || Bitmap.Width != size.Width || Bitmap.Height != size.Height || !Texture2D)
            {
                Graphics?.Dispose();
                Bitmap?.Dispose();
                FlippedGraphics?.Dispose();
                FlippedBitmap?.Dispose();
                Bitmap = new Bitmap(size.Width, size.Height);
                Graphics = System.Drawing.Graphics.FromImage(Bitmap);
                FlippedBitmap = new Bitmap(size.Width, size.Height);
                FlippedGraphics = System.Drawing.Graphics.FromImage(FlippedBitmap);
            }

            Graphics.CopyFromScreen((int)Rect.xMin, (int)Rect.yMin, 0, 0, size, CopyPixelOperation.SourceCopy);
            FlippedGraphics.DrawImage(Bitmap, 0, Bitmap.Height, Bitmap.Width, -Bitmap.Height);

            var bitmapData = FlippedBitmap.LockBits(new System.Drawing.Rectangle(0, 0, FlippedBitmap.Width, FlippedBitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var length = Math.Abs(bitmapData.Stride) * bitmapData.Height;
            var pixelData = new byte[length];
            Marshal.Copy(bitmapData.Scan0, pixelData, 0, length);
            FlippedBitmap.UnlockBits(bitmapData);

            return new Tuple<byte[], Size>(pixelData, size);
        }
        #endregion
    }
}
