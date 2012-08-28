using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Resources;
using System.Reflection;


using Melody;
using Notes;

using Generator.Properties;

namespace Generator
{
    public partial class MainForm : Form
    {
        public string fname = "";
        private MelodyGenerator mg;
        ResourceManager resman;

        public MainForm()
        {
            InitializeComponent();
        }

        private void makeButton_Click(object sender, EventArgs e)
        {
            fname = "";
            Modus m;
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

            mg = new MelodyGenerator(c, m, Time.Create(perfectTime.Checked), (int)randSeedDD.Value);

            mg.Generate((uint)barsCount.Value);

            outputArea.Text = formatLily(m, c);


            gvOut.Text = mg.GenerationGraph();

            saveLily.Enabled = true;
            engrave.Enabled = true;
            saveGV.Enabled = true;
            graphViz.Enabled = true;

        }

        private string formatLily(Modus m, Clef c)
        {
            Pitch p = new Pitch(0, m);

            string key = (p.StringForm + " ");
            string modus, clef, time;
            string notes = "";

            switch (modiList.SelectedIndex)
            {
                case 0: modus = "\\ionian"; break;
                case 1: modus = "\\dorian"; break;
                case 2: modus = "\\phrygian"; break;
                case 3: modus = "\\lydian"; break;
                case 4: modus = "\\mixolydian"; break;
                case 5: modus = "\\aeolian"; break;
                default: modus = "\\major"; break;
            }

            switch (clefList.SelectedIndex)
            {
                case 0: clef = "treble"; break;
                case 1: clef = "soprano"; break;
                case 2: clef = "mezzosoprano"; break;
                case 3: clef = "alto"; break;
                case 4: clef = "tenor"; break;
                case 5: clef = "baritone"; break;
                case 6: clef = "bass"; break;
                default: clef = "treble"; break;
            }

            time = (perfectTime.Checked ? "3" : "4") + "/2";

            foreach (Note n in mg.Notes)
            {
                notes += (n.ToString() + " ");
            }

            string format = Resources.ScoreTemplate;
            return string.Format(format, mg.seed, key, modus, clef, time, notes);
            
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            modiList.SelectedIndex = 1;
            startNotes.SelectedIndex = 2;
            clefList.SelectedIndex = 0;
        }

        private void saveLily_Click(object sender, EventArgs e)
        {
            if (fname == "")
                fname = "gen_" + DateTime.Now.Ticks.ToString();
            using (StreamWriter outfile = new StreamWriter("out\\" + fname + ".ly"))
            {
                outfile.Write(outputArea.Text);
            }
        }

        private void engrave_Click(object sender, EventArgs e)
        {
            saveLily_Click(sender, e);
            Process p = new Process();
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.FileName = "lilypond";
            p.StartInfo.Arguments = fname + ".ly";
            p.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory() + "\\out";
            p.EnableRaisingEvents = true;
            p.Exited += new EventHandler(engraveCompleted);
            p.Start();
        }

        private void engraveCompleted(object sender, EventArgs e)
        {
            Process s = (Process)sender;
            if (s.ExitCode == 0)
                Process.Start("out\\" + fname + ".pdf");
        }

        private void saveGV_Click(object sender, EventArgs e)
        {
            if (fname == "")
                fname = "gen_" + DateTime.Now.Ticks.ToString();
            using (StreamWriter outfile = new StreamWriter("out\\" + fname + ".gv"))
            {
                outfile.Write(gvOut.Text);
            }
        }

        private void graphViz_Click(object sender, EventArgs e)
        {
            saveGV_Click(sender, e);
            string cmdline = "-Tsvg " + fname + ".gv -o" + fname + ".svg";
            Process p = new Process();
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.FileName = "dot";
            p.StartInfo.Arguments = cmdline;
            p.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory() + "\\out";
            p.Start();
        }
    }
}
