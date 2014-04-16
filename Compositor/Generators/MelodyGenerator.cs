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
        public Voice Voice { get; private set; }
        public int Seed { get; private set; }
        public int StepLimit { get; private set; }

        readonly IChooseNextStrategy _chooseStrategy;

        const double MinimumAccumulatedFrequency = 0.1;
        const double MinimumNoteFrequencyAllowed = 0.02;

        public List<Voice> GetNotes()
        {
            var res = new List<Voice> {Voice};
            return res;
        }

        public int GetSeed()
        {
            return Seed;
        }        

        public MelodyGenerator(Clef clef, Modus modus, Time time, int seed = 0, int stepLimit = 50000, IChooseNextStrategy strategy = null)
        {
            StepLimit = stepLimit;
            Voice = new Voice(clef, modus, time);
            if (strategy == null)
            {
                SetSeed(seed);
                _chooseStrategy = new DefaultNextStrategy(Seed);
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
            uint lengthInBeats = length * (uint)Voice.Time.Beats * 4;
            int steps = 0;

            Voice.SetLength(lengthInBeats);

            try
            {
                while ((Voice.Time.Position < lengthInBeats) && (steps < StepLimit))
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
            Voice.Filter(dumpResult);
            double max = Voice.Freqs.Max(kv => (kv.Value > MinimumNoteFrequencyAllowed) ? kv.Value : 0);

            if (max > MinimumAccumulatedFrequency) //должно быть что-то приличное!
                ChooseNextNote(dumpResult);
            else
            {
                if (Voice.NoteCount > 0)
                    Voice.RemoveLast();
                else
                    Voice.FirstNote();
            }
        }

        private void ChooseNextNote(bool dumpResult = false)
        {
            var possibleNext = Voice.Freqs.Where(kv => kv.Value > MinimumNoteFrequencyAllowed).OrderBy(kv => kv.Key);

            if (dumpResult)
            {
                var sb = new StringBuilder();
                foreach (var kv in Voice.Freqs)
                {
                    sb.AppendFormat("{0} => {1}; ", kv.Key, kv.Value);
                }
                Console.WriteLine(sb);
            }

            var next = (Note) _chooseStrategy.ChooseNext(possibleNext);

            Voice.AddNote(next);
        }
    }
}


