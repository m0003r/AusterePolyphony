using Compositor.Levels;
using Compositor.Rules;
using PitchBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compositor
{
    using NotesFreq = Dictionary<Note, double>;
    using NotesList = List<Note>;

    public class StopGeneration : Exception
    {

    }

    public class MelodyGenerator : IGenerator
    {
        public Melody Melody { get; private set; }
        public int Seed { get; private set; }
        public int StepLimit { get; private set; }

        IChooseNextStrategy<Note> chooseStrategy;

        const double MinimumAccumulatedFrequency = 0.1;
        const double MinimumNoteFrequencyAllowed = 0.02;

        public List<Melody> GetNotes()
        {
            var res = new List<Melody>();
            res.Add(Melody);
            return res;
        }

        public int GetSeed()
        {
            return Seed;
        }        

        public MelodyGenerator(Clef Clef, Modus Modus, Time Time, int seed = 0, int stepLimit = 50000, IChooseNextStrategy<Note> Strategy = null)
        {
            StepLimit = stepLimit;
            Melody = new Melody(Clef, Modus, Time);
            if (Strategy == null)
            {
                SetSeed(seed);
                chooseStrategy = new DefaultNextStrategy<Note>(this.Seed);
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
            uint lengthInBeats = Length * (uint)Melody.Time.Beats * 4;
            int steps = 0;

            Melody.setLength(lengthInBeats);

            try
            {
                while ((Melody.Time.Position < lengthInBeats) && (steps < StepLimit))
                {
                    Step();
                    steps++;
                }
            }
            catch (StopGeneration)
            {

            }

            return steps;
        }

        private void Step(bool dumpResult = false)
        {
            Melody.Filter(dumpResult);
            double max = Melody.Freqs.Max(kv => (kv.Value > MinimumNoteFrequencyAllowed) ? kv.Value : 0);

            if (max > MelodyGenerator.MinimumAccumulatedFrequency) //должно быть что-то приличное!
                chooseNextNote(dumpResult);
            else
            {
                if (Melody.NoteCount > 0)
                    Melody.RemoveLast();
                else
                    Melody.FirstNote();
            }
        }

        private void chooseNextNote(bool dumpResult = false)
        {
            var possibleNext = Melody.Freqs.Where(kv => kv.Value > MinimumNoteFrequencyAllowed).OrderBy(kv => kv.Key);

            if (dumpResult)
            {
                var sb = new StringBuilder();
                foreach (var kv in Melody.Freqs)
                {
                    sb.AppendFormat("{0} => {1}; ", kv.Key.ToString(), kv.Value);
                }
                Console.WriteLine(sb);
            }

            Note next = chooseStrategy.ChooseNext(possibleNext);

            Melody.AddNote(next);
        }
    }
}


