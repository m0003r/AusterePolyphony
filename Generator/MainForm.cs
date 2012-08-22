using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Melody;
using Notes;

namespace Generator
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void makeButton_Click(object sender, EventArgs e)
        {
            Modus m;
            MelodyGenerator mg;
            Clef c = (Clef)(clefList.SelectedIndex - 1);

            int noteStart = startNotes.SelectedIndex;

            switch (modiList.SelectedIndex)
            {
                case 0: m = Modus.Ionian(noteStart); break;
                case 1: m = Modus.Dorian(noteStart); break;
                case 2: m = Modus.Phrygian(noteStart); break;
                case 3: m = Modus.Lydian(noteStart); break;
                case 4: m = Modus.Mixolydian(noteStart); break;
                case 5: m = Modus.Aeolian(noteStart); break;
                default: return;
            }

            mg = new MelodyGenerator(c, m, Time.Create(perfectTime.Checked));

            mg.Generate((uint)barsCount.Value);

            outputArea.Text = makePrelude(m);
            foreach (Note n in mg.Notes)
            {
                outputArea.Text += (n.ToString() + " ");
            }
            outputArea.Text += "\r\n}\r\n\\midi {\r\n\\context {\r\n" +
                                "\\Score\r\n" +
                                "tempoWholesPerMinute = #(ly:make-moment 72 1)\r\n" +
                                " }\r\n}\r\n\\layout { }\r\n}";

        }

        private string makePrelude(Modus m)
        {
            Pitch p = new Pitch(0, m);

            string o = "\\version \"2.15.41\"\r\n\r\n\\language \"deutsch\"\r\n\\score {\r\n" +
                       "\\new Voice \\with {\r\n" +
                       "\\remove \"Note_heads_engraver\" \r\n\\consists \"Completion_heads_engraver\"\r\n" +
                       "\\remove \"Rest_engraver\"\r\n\\consists \"Completion_rest_engraver\"\r\n  }\r\n" +
                       "{\r\n\\set Staff.midiInstrument = #\"synth voice\"\r\n\\key ";

            o += (p.StringForm + " ");

            switch (modiList.SelectedIndex)
            {
                case 0: o += "\\ionian"; break;
                case 1: o += "\\dorian"; break;
                case 2: o += "\\phrygian"; break;
                case 3: o += "\\lydian"; break;
                case 4: o += "\\mixolydian"; break;
                case 5: o += "\\aeolian"; break;
            }

            switch (clefList.SelectedIndex)
            {
                case 0: o += "\r\n\\clef treble"; break;
                case 1: o += "\r\n\\clef soprano"; break;
                case 2: o += "\r\n\\clef mezzosoprano"; break;
                case 3: o += "\r\n\\clef alto"; break;
                case 4: o += "\r\n\\clef tenor"; break;
                case 5: o += "\r\n\\clef baritone"; break;
                case 6: o += "\r\n\\clef bass"; break;
            }

            o += "\r\n\\time " + (perfectTime.Checked ? "3" : "4") + "/2\r\n\r\n ";

            return o;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            modiList.SelectedIndex = 1;
            startNotes.SelectedIndex = 2;
            clefList.SelectedIndex = 0;
        }
    }
}
