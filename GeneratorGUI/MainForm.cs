using Compositor;
using PitchBase;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace GeneratorGUI
{
    public partial class MainForm : Form
    {
        public string fname = "";
        private IGenerator Generator;

        public MainForm()
        {
            InitializeComponent();
        }

        private void makeButton_Click(object sender, EventArgs e)
        {

            var clefs = new List<int>();
            foreach (int index in clefList.SelectedIndices)
                clefs.Add(index - 1);

            Compositor.Timer.Flush("filter");
            Compositor.Timer.Start("generator");

            InitGenerator(clefs, startNotes.SelectedIndex, perfectTime.Checked, (int)randSeedDD.Value, (int)maxSteps.Value);
            int steps = Generator.Generate((uint)barsCount.Value);
            Console.WriteLine("Total filtering time: {0}\nTotal generation time: {1}", Compositor.Timer.Total("filter"), Compositor.Timer.Stop("generator"));
            Console.WriteLine("Total steps: {0}\n", steps);
            prepareOutput();

        }


        private void InitGenerator(List<int> ClefIndices, int noteStart, bool perfect, int seed, int stepLimit)
        {
            Modus Modus;

            switch (modiList.SelectedIndex)
            {
                case 0: Modus = Modus.Ionian(noteStart); break;
                case 1: Modus = Modus.Dorian(noteStart); break;
                case 2: Modus = Modus.Phrygian(noteStart); break;
                case 3: Modus = Modus.Lydian(noteStart); break;
                case 4: Modus = Modus.Mixolydian(noteStart); break;
                case 5: Modus = Modus.Aeolian(noteStart); break;
                default: return;
            }

            Generator = null;

            if (ClefIndices.Count == 1)
            {
                Clef Clef = (Clef)ClefIndices[0];

                GC.Collect();
                Generator = new MelodyGenerator(Clef, Modus, Time.Create(perfect), seed, stepLimit);
            }
            if (ClefIndices.Count == 2)
            {
                Clef c1 = (Clef)ClefIndices[0], c2 = (Clef)ClefIndices[1];
                Generator = new TwoVoiceGenerator(c1, c2, Modus, Time.Create(perfect), seed, stepLimit);
            }

        }

        private void prepareOutput()
        {
            outputArea.Text = LilyOutput.Lily(Generator);
            if (Generator is MelodyGenerator)
            {
                gvOut.Text = ((MelodyGenerator)Generator).GenerationGraph();
                drawGraphButton.Enabled = true;
            }

            engraveButton.Enabled = true;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            modiList.SelectedIndex = 1;
            startNotes.SelectedIndex = 2;
            clefList.SelectedIndex = 0;
        }

        private void SaveLily()
        {
            checkOutDirectory();
            if (fname == "")
                fname = "gen_" + DateTime.Now.Ticks.ToString();
            using (StreamWriter outfile = new StreamWriter("out\\" + fname + ".ly"))
            {
                outfile.Write(outputArea.Text);
            }
        }

        private void checkOutDirectory()
        {
            if (!Directory.Exists("out"))
                Directory.CreateDirectory("out");
        }

        private void engraveButton_Click(object sender, EventArgs e)
        {
            SaveLily();
            Engrave();
        }

        private void Engrave()
        {
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
            {
                Process.Start("out\\" + fname + ".pdf");
                EnablePlay();
            }
        }

        private void EnablePlay()
        {
            if (playButton.InvokeRequired)
            {
                Action act = EnablePlay;
                playButton.Invoke(act);
            }
            else
                playButton.Enabled = true;
        }

        private void gvCompleted(object sender, EventArgs e)
        {
            Process s = (Process)sender;
            if (s.ExitCode == 0)
                Process.Start("out\\" + fname + "_graph.pdf");
        }


        private void SaveGraph()
        {
            checkOutDirectory();
            if (fname == "")
                fname = "gen_" + DateTime.Now.Ticks.ToString();
            using (StreamWriter outfile = new StreamWriter("out\\" + fname + ".gv"))
            {
                outfile.Write(gvOut.Text);
            }
        }

        private void drawGraphButton_Click(object sender, EventArgs e)
        {
            SaveGraph();
            DrawGraph();
        }

        private void DrawGraph()
        {
            string cmdline = "-Tpdf " + fname + ".gv -o" + fname + "_graph.pdf";
            Process p = new Process();
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.FileName = "dot";
            p.StartInfo.Arguments = cmdline;
            p.EnableRaisingEvents = true;
            p.Exited += new EventHandler(gvCompleted);
            p.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory() + "\\out";
            p.Start();
        }

        private void playButton_Click(object sender, EventArgs e)
        {
            Process.Start("out\\" + fname + ".mid");
        }
    }
}
