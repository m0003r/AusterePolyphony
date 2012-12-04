using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PitchBase;
using Compositor.Rules;
using Compositor.Levels;

namespace Compositor
{
    using NotesFreq = Dictionary<Note, double>;
    using NotesList = List<Note>;

    public class MelodyGenerator
    {
        private Random rand;
        public Melody Melody { get; private set; }
        public int Seed { get; private set; }

        private const double MinimumAccumulatedFrequency = 0.3;

        public MelodyGenerator(Clef Clef, Modus Modus, Time Time, int seed = 0)
        {
            Melody = new Melody(Clef, Modus, Time);
            SetupRand(seed);
        }

        private void SetupRand(int seed)
        {
            if (seed == 0)
                this.Seed = (int)DateTime.Now.Ticks;
            else
                this.Seed = seed;

            rand = new Random(this.Seed);
        }

        public void Generate(uint Length)
        {
            uint DesiredLength = Length * (uint)Melody.Time.Beats * 4;
            int steps = 0;

            Melody.DesiredLength = DesiredLength;

            while ((Melody.Time.Position < DesiredLength) && (steps < 10000))
            {
                Step();
                steps++;
            }
        }

        private void Step()
        {
            Melody.Filter();
            double max = Melody.Freqs.Max(kv => (kv.Value > 0.01) ? kv.Value : 0);

            if (max > MelodyGenerator.MinimumAccumulatedFrequency) //должно быть что-то приличное!
            {
                chooseFrom(Melody.Freqs);
            }
            else
                Melody.RemoveLast();
        }

        private void chooseFrom(NotesFreq allowed)
        {
            double freqS = allowed.Sum(kv => kv.Value);
            double r = rand.NextDouble() * freqS;
            double accumulator = 0;

            foreach (KeyValuePair<Note, double> kv in allowed)
            {
                accumulator += kv.Value;
                if (kv.Value < 0.02) //более строги будем здесь
                    continue;
                if (accumulator > r)
                {
                    Melody.AddNote(kv.Key);
                    break;
                }
            }
        }
    }
}


