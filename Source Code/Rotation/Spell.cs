using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Input;
using Helpers;

namespace Rotation
{
    public class Spell
    {
        public Color Color;
        public Keys Key;
        public string Name;
        public POINT p;

        public Spell(Color color, Keys key, POINT p, string spellName)
        {
            Name = spellName;
            Color = color;
            Key = key;
            this.p = p;
        }

        public bool MustPress()
        {
            var color = Pixels.GetPixelColor(p);
            return color.R == Color.R && color.G == Color.G && color.B == Color.B;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private void KeyDown(Keys key)
        {
            SendMessage(Globals.Process.MainWindowHandle, 0x100, (int) key, 0);
        }

        private void KeyUp(Keys key)
        {
            SendMessage(Globals.Process.MainWindowHandle, 0x101, (int) key, 0);
        }

        private void KeyPressRelease(Keys key)
        {
            KeyDown(key);
            Thread.Sleep(50);
            KeyUp(key);
        }

        public void Press()
        {
            Log.Write("Casting: " + Name + " Key: " + Key);
            KeyPressRelease(Key);
        }
    }
}