using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Compositor.ChooseNextStrategy;
using Compositor.Levels;
using PitchBase;

namespace Compositor.Generators
{
    public class StopGeneration : Exception
    {

    }

    public class MelodyGenerator : IGenerator
    {
        public Melody Melody { get; private set; }
        public int Seed { get; private set; }
        public int StepLimit { get; private set; }

        readonly IChooseNextStrategy<Note> _chooseStrategy;

        const double MinimumAccumulatedFrequency = 0.1;
        const double MinimumNoteFrequencyAllowed = 0.02;

        public List<Melody> GetNotes()
        {
            var res = new List<Melody> {Melody};
            return res;
        }

        public int GetSeed()
        {
            return Seed;
        }        

        public MelodyGenerator(Clef clef, Modus modus, Time time, int seed = 0, int stepLimit = 50000, IChooseNextStrategy<Note> strategy = null)
        {
            StepLimit = stepLimit;
            Melody = new Melody(clef, modus, time);
            if (strategy == null)
            {
                SetSeed(seed);
                _chooseStrategy = new DefaultNextStrategy<Note>(Seed);
            }
            else
                _chooseStrategy = strategy;
        }

        private void SetSeed(int givenSeed)
        {
            if (givenSeed == 0)
                Seed = (int)DateTime.Now.Ticks;
            else
                Seed = givenSeed;
        }

        public int Generate(uint length)
        {
            return Generate(length, null);
        }

        public int Generate(uint length, Func<int, bool> callback)
        {
            uint lengthInBeats = length * (uint)Melody.Time.Beats * 4;
            int steps = 0;

            Melody.SetLength(lengthInBeats);

            try
            {
                while ((Melody.Time.Position < lengthInBeats) && (steps < StepLimit))
                {
                    Step();
                    steps++;
                    if (callback != null)
                        callback(steps);
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

            if (max > MinimumAccumulatedFrequency) //должно быть что-то приличное!
                ChooseNextNote(dumpResult);
            else
            {
                if (Melody.NoteCount > 0)
                    Melody.RemoveLast();
                else
                    Melody.FirstNote();
            }
        }

        private void ChooseNextNote(bool dumpResult = false)
        {
            var possibleNext = Melody.Freqs.Where(kv => kv.Value > MinimumNoteFrequencyAllowed).OrderBy(kv => kv.Key);

            if (dumpResult)
            {
                var sb = new StringBuilder();
                foreach (var kv in Melody.Freqs)
                {
                    sb.AppendFormat("{0} => {1}; ", kv.Key, kv.Value);
                }
                Console.WriteLine(sb);
            }

            Note next = _chooseStrategy.ChooseNext(possibleNext);

            Melody.AddNote(next);
        }
    }
}


