using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;
using Helpers;
using Rotation;

namespace PixelBuddy
{
    public partial class frmMain : Form
    {
        private readonly CombatRoutine combatRoutine = new CombatRoutine();
        private KeyboardHook hook;

        public frmMain()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Log.Initialize(richTextBox1, this);
            Shown += Form1_Shown;
            Closing += Form1_Closing;

        }

        private void Form1_Closing(object sender, CancelEventArgs e)
        {
            hook?.Dispose();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            hook = new KeyboardHook();
            hook.RegisterHotKey(Helpers.ModifierKeys.Ctrl, Keys.None, "Get Pixel Color");
            
            hook.KeyPressed += Hook_KeyPressed;
            combatRoutine.Load();
        }

        private void Hook_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            POINT point;
            Mouse.GetCursorPos(out point);
            Pixels.ScreenToClient(Globals.Process.MainWindowHandle, ref point);
            var color = Pixels.GetPixelColor(point);
            
            frmSpell f = new frmSpell(color, point, combatRoutine);
            f.Show();
        }


        private void cmdStartStop_Click(object sender, EventArgs e)
        {
            if (combatRoutine.Spells.Count == 0)
            {
                Log.Write("Add atleast 1 spell.", Color.Red);
                return;
            }
                
            if (combatRoutine.State == CombatRoutine.RotationState.Stopped)
            {
                combatRoutine.Start();

                if (combatRoutine.State != CombatRoutine.RotationState.Running) return;

                cmdStartStop.Text = "Stop";
                cmdStartStop.BackColor = Color.Salmon;
            }
            else
            {
                combatRoutine.Pause();
                cmdStartStop.Text = "Start";
                cmdStartStop.BackColor = Color.LightGreen;
            }
        }
        
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            combatRoutine.Save();
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            combatRoutine.LoadFromFile();
        }
    }
}