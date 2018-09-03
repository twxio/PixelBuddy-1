using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Helpers
{
    public static class Pixels
    {
        private static readonly object thisLock = new object();
        private static readonly Bitmap screenPixel = new Bitmap(1, 1);

        [DllImport("gdi32.dll")]
        private static extern int BitBlt(IntPtr srchDC, int srcX, int srcY, int srcW, int srcH, IntPtr desthDC, int destX, int destY, int op);

        [DllImport("user32.dll")]
        public static extern bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);


        public static Color GetPixelColor(POINT p)
        {
            if (Globals.Process == null)
            {
                Log.Write("Failed to connect to process 'Wow-64', will return Orange.", Color.Red);
                return Color.Orange;
            }
            
            lock (thisLock) // We lock the bitmap "screenPixel" here to avoid it from being accessed by multiple threads at the same time and crashing
            {
                try
                {
                    using (var gdest = Graphics.FromImage(screenPixel))
                    {
                        using (var gsrc = Graphics.FromHwnd(Globals.Process.MainWindowHandle))
                        {
                            var hSrcDC = gsrc.GetHdc();
                            var hDC = gdest.GetHdc();
                            BitBlt(hDC, 0, 0, 1, 1, hSrcDC, p.X, p.Y, (int) CopyPixelOperation.SourceCopy);
                            gdest.ReleaseHdc();
                            gsrc.ReleaseHdc();
                        }
                    }
                    var temp = screenPixel.GetPixel(0, 0);

                    //Log.Write($"XY = {p.X},{p.Y} RGB = {temp.R},{temp.G},{temp.B}");

                    return temp;
                }
                catch (Exception ex)
                {
                    Log.Write("Failed to find pixel color from screen, this is usually due to wow closing while", Color.Red);
                    Log.Write("attempting to find the pixel color", Color.Red);
                    Log.Write("Error Details: " + ex.Message, Color.Red);

                    return Color.Orange; // Orange cause nothing currently uses it
                }
            }
        }
    }
}