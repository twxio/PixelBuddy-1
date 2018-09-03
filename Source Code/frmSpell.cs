using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Helpers;
using Rotation;

namespace PixelBuddy
{
    public partial class frmSpell : Form
    {
        private readonly Color color;
        private readonly CombatRoutine combatRoutine;
        private Keys key;
        private readonly POINT point;
        
        public frmSpell(Color color, POINT point, CombatRoutine combatRoutine)
        {
            this.color = color;
            this.point = point;
            this.combatRoutine = combatRoutine;
            InitializeComponent();
        }

        private void cmdAddSpell_Click(object sender, EventArgs e)
        {
            var spell = new Spell(color, key, point, txtSpellName.Text);
            combatRoutine.Spells.Add(spell);

            Log.Write("Added Spell: [" + spell.Name + "] => Keybind: [" + spell.Key + "]");
            Close();
        }

        private void frmSpell_Load(object sender, EventArgs e)
        {
            txtKey.KeyDown += TxtKey_KeyDown;
        }

        private void TxtKey_KeyDown(object sender, KeyEventArgs e)
        {
            key = e.KeyCode;
        }
    }
}