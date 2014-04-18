using System.Linq;
using System.Threading;
using Compositor.Generators;
using GeneratorGUI.Properties;
using PitchBase;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Timer = Compositor.Timer;

namespace GeneratorGUI
{
    public partial class MainForm : Form
    {
        public string FName = "";
        private IGenerator _generator;

        private readonly SettingsForm _settingsForm;
        private int _steps;
        private Thread _generatorThread;

        public MainForm()
        {
            InitializeComponent();
            _settingsForm = new SettingsForm();
        }

        private void makeButton_Click(object sender, EventArgs e)
        {
            FName = "";
            RunGenerator();
        }

        private void RunGenerator()
        {
            var clefs = (from int index in clefList.SelectedIndices select index - 1).ToList();


            InitGenerator(clefs, startNotes.SelectedIndex, perfectTime.Checked, (int) randSeedDD.Value, (int) maxSteps.Value);
            generationProgressBar.Maximum = (int) maxSteps.Value;
            generationProgressBar.Minimum = 0;
            generationProgressBar.Value = 0;


            _generatorThread = new Thread(Generate);
            _generatorThread.Start();
        }

        private void Generate()
        {
            Timer.Flush("filter");
            Timer.Start("generator");

            Invoke(new Action<bool>(b => makeButton.Enabled = b), false);
            _steps = _generator.Generate((uint) barsCount.Value, s =>
            {
                if (s%50 == 0)
                    Invoke(new Action<int>(i => generationProgressBar.Value = i), s);
                return true;
            });
            Console.WriteLine(Resources.FilteringGenerationTime, Timer.Total("filter"), Timer.Stop("generator"));
            Console.WriteLine(Resources.TotalSteps, _steps);

            if (_steps == maxSteps.Value)
            {
                var dr = MessageBox.Show(Resources.CantFinish, Resources.Error, MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation);
                if (dr == DialogResult.Retry)
                    Invoke(new Action(RunGenerator));
                if (dr == DialogResult.Cancel)
                    Invoke(new Action(PrepareOutput));
            }
            else
            {
                Invoke(new Action<int>(i => generationProgressBar.Value = i), (int)maxSteps.Value);
                Invoke(new Action(PrepareOutput));
            }
            Invoke(new Action<bool>(b => makeButton.Enabled = b), true);
        }


        private void InitGenerator(IList<int> clefIndices, int noteStart, bool perfect, int seed, int stepLimit)
        {
            Modus modus;

            switch (modiList.SelectedIndex)
            {
                case 0: modus = Modus.Ionian(noteStart); break;
                case 1: modus = Modus.Dorian(noteStart); break;
                case 2: modus = Modus.Phrygian(noteStart); break;
                case 3: modus = Modus.Lydian(noteStart); break;
                case 4: modus = Modus.Mixolydian(noteStart); break;
                case 5: modus = Modus.Aeolian(noteStart); break;
                default: return;
            }

            _generator = null;
            GC.Collect();

            var time = Time.Create(perfect);
            if (clefIndices.Count == 1)
            {
                var clef = (Clef)clefIndices[0];

                _generator = new VoiceGenerator(clef, modus, time, seed, stepLimit);
            }

            if (clefIndices.Count == 2)
            {
                Clef c1 = (Clef)clefIndices[0], c2 = (Clef)clefIndices[1];
                _generator = new TwoVoiceGenerator(c1, c2, modus, time, seed, stepLimit);

                if (!imitationSettingsBox.Enabled) return;

                ((TwoVoiceGenerator)_generator).SetMirroring(
                    ImitationTopFirst.Checked,
                    new ImitationSettings(
                        (int) imitationDelay.Value * 4,
                        new Interval((IntervalType)imitationInterval.SelectedIndex),
                        (int) imitationRange.Value*time.BarLength));
            }

        }

        private void PrepareOutput()
        {
            outputArea.Text = LilyOutput.Lily(_generator);

            //GenerateGraph();
            engraveButton.Enabled = true;
        }

/*
        private void GenerateGraph()
        {
            var generator = _generator as VoiceGenerator;
            if (generator != null)
            {
                gvOut.Text = generator.GenerationGraph();
                drawGraphButton.Enabled = true;
            }
        }
*/

        private void MainForm_Load(object sender, EventArgs e)
        {
            modiList.SelectedIndex = 1;
            startNotes.SelectedIndex = 2;
            clefList.SelectedIndex = 0;
            imitationInterval.SelectedIndex = 4;
        }

        private void SaveLily()
        {
            CheckOutDirectory();
            if (FName == "")
                FName = "gen_" + DateTime.Now.Ticks;
            using (var outfile = new StreamWriter("out\\" + FName + ".ly"))
            {
                outfile.Write(outputArea.Text);
            }
        }

        private static void CheckOutDirectory()
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
            var p = new Process
            {
                StartInfo =
                {
                    UseShellExecute = true,
                    FileName = "lilypond",
                    Arguments = FName + ".ly",
                    WorkingDirectory = Directory.GetCurrentDirectory() + "\\out"
                },
                EnableRaisingEvents = true
            };

            p.Exited += EngraveCompleted;
            p.Start();
        }

        private void EngraveCompleted(object sender, EventArgs e)
        {
            var s = (Process)sender;
            if (s.ExitCode != 0) return;

            Process.Start("out\\" + FName + ".pdf");
            EnablePlay();
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

        private void GvCompleted(object sender, EventArgs e)
        {
            var s = (Process)sender;
            if (s.ExitCode == 0)
                Process.Start("out\\" + FName + "_graph.pdf");
        }


/*
        private void SaveGraph()
        {
            CheckOutDirectory();
            if (FName == "")
                FName = "gen_" + DateTime.Now.Ticks;

            using (var outfile = new StreamWriter("out\\" + FName + ".gv"))
                outfile.Write(gvOut.Text);
        }
        private void drawGraphButton_Click(object sender, EventArgs e)
        {
            SaveGraph();
            DrawGraph();
        }

        private void DrawGraph()
        {
            var cmdline = "-Tpdf " + FName + ".gv -o" + FName + "_graph.pdf";
            var p = new Process
            {
                StartInfo = {UseShellExecute = true, FileName = "dot", Arguments = cmdline},
                EnableRaisingEvents = true
            };

            p.Exited += GvCompleted;
            p.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory() + "\\out";
            p.Start();
        }
*/

        private void playButton_Click(object sender, EventArgs e)
        {
            Process.Start("out\\" + FName + ".mid");
        }

        private void settingsButton_Click(object sender, EventArgs e)
        {
            _settingsForm.ShowDialog();
        }

        private void UpdateImitationSettingsEnabled(object sender, EventArgs e)
        {
            imitationSettingsBox.Enabled = (clefList.SelectedIndices.Count == 2) && (imitationEnabled.Checked);
        }
    }
}
