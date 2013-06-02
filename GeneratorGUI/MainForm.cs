using System;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;


using Compositor;
using Compositor.Levels;
using PitchBase;

using GeneratorGUI.Properties;
using System.Text;

namespace GeneratorGUI
{
    public partial class MainForm : Form
    {
        public string fname = "";
        private MelodyGenerator Generator;

        public MainForm()
        {
            InitializeComponent();
        }

        private void makeButton_Click(object sender, EventArgs e)
        {
            Compositor.Timer.Flush("filter");
            Compositor.Timer.Start("generator");
            InitGenerator(clefList.SelectedIndex - 1, startNotes.SelectedIndex, perfectTime.Checked, (int)randSeedDD.Value, (int)maxSteps.Value);
            int steps = Generator.Generate((uint)barsCount.Value);
            Console.WriteLine("Total filtering time: {0}\nTotal generation time: {1}", Compositor.Timer.Total("filter"), Compositor.Timer.Stop("generator"));
            Console.WriteLine("Total steps: {0}\n", steps);
            prepareOutput();

            //dumpLeapsSmooth();
        }

        private void dumpLeapsSmooth()
        {
            foreach (var ls in Generator.Melody.LeapSmooth)
            {
                Console.WriteLine(" * {0}", ls.ToString());
            };

        }

        private void InitGenerator(int ClefIndex, int noteStart, bool perfect, int seed, int stepLimit)
        {
            fname = "";
            Modus Modus;
            Clef Clef = (Clef)ClefIndex;


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

            Generator = new MelodyGenerator(Clef, Modus, Time.Create(perfect), seed, stepLimit);
        }

        private void prepareOutput()
        {
            outputArea.Text = Generator.Lily();
            gvOut.Text = Generator.GenerationGraph();

            engraveButton.Enabled = true;
            drawGraphButton.Enabled = true;
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
                Process.Start("out\\" + fname + ".pdf");
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
    }
}
