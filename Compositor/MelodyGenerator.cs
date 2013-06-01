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
        public Melody Melody { get; private set; }
        public int Seed { get; private set; }
        public int StepLimit { get; private set; }

        IChooseNextStrategy chooseStrategy;

        const double MinimumAccumulatedFrequency = 0.3;
        const double MinimumNoteFrequencyAllowed = 0.02;

        public MelodyGenerator(Clef Clef, Modus Modus, Time Time, int seed = 0, int stepLimit = 50000, IChooseNextStrategy Strategy = null)
        {
            StepLimit = stepLimit;
            Melody = new Melody(Clef, Modus, Time);
            if (Strategy == null)
            {
                SetSeed(seed);
                chooseStrategy = new DefaultNextStrategy(this.Seed);
            }
            else
                chooseStrategy = Strategy;
        }

        private void SetSeed(int givenSeed)
        {
            if (givenSeed == 0)
                this.Seed = (int)DateTime.Now.Ticks;
            else
                this.Seed = givenSeed;
        }

        public int Generate(uint Length)
        {
            uint DesiredLength = Length * (uint)Melody.Time.Beats * 4;
            int steps = 0;

            Melody.DesiredLength = DesiredLength;

            while ((Melody.Time.Position < DesiredLength) && (steps < StepLimit))
            {
                Step();
                steps++;
            }

            return steps;
        }

        private void Step()
        {
            Melody.Filter();
            double max = Melody.Freqs.Max(kv => (kv.Value > MinimumNoteFrequencyAllowed) ? kv.Value : 0);

            if (max > MelodyGenerator.MinimumAccumulatedFrequency) //должно быть что-то приличное!
                chooseNextNote();
            else
                Melody.RemoveLast();
        }

        private void chooseNextNote()
        {
            Note next = chooseStrategy.ChooseNext(Melody.Freqs.Where(kv => kv.Value > MinimumNoteFrequencyAllowed));
            Melody.AddNote(next);
        }
    }
}


