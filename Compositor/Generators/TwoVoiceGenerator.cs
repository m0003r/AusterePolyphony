using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Compositor.ChooseNextStrategy;
using Compositor.Levels;
using PitchBase;

namespace Compositor.Generators
{
    public class ImitationSettings
    {
        public bool TopFirst;
        public int Delay;
        public int Interval;
        public int Range;

        public ImitationSettings(int delay, IntervalType interval, int range)
        {
            Delay = delay;
            Interval = (int)interval;
            Range = range;
        }
    }

    public class TwoVoiceGenerator : IGenerator
    {
        public TwoVoices Melodies { get; private set; }
        public int Seed { get; private set; }
        public int StepLimit { get; private set; }

        
        readonly IChooseNextStrategy _chooseStrategy;
        private int _rollback;

        const double MinimumAccumulatedFrequency = 0.1;
        const double MinimumNoteFrequencyAllowed = 0.02;

        public List<Voice> GetNotes()
        {
            var res = new List<Voice> {Melodies.Voice1, Melodies.Voice2};
            return res;
        }

        public int GetSeed()
        {
            return Seed;
        }        

        public TwoVoiceGenerator(Clef clef1, Clef clef2, Modus modus, Time time, int seed = 0, int stepLimit = 50000, IChooseNextStrategy strategy = null)
        {
            StepLimit = stepLimit;
            Melodies = new TwoVoices(clef1, clef2, modus, time);
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

        public void SetMirroring(bool topFirst, ImitationSettings settings)
        {
            if (topFirst)
                Melodies.SetMirroring(Melodies.Voice1, Melodies.Voice2, settings);
            else
                Melodies.SetMirroring(Melodies.Voice2, Melodies.Voice1, settings);
        }

        public int Generate(uint length, Func<GenerationInfo, bool> callback)
        {
            var lengthInBeats = length * (uint)Melodies.Time.Beats * 4;
            _rollback = 0;
            var steps = 0;

            Melodies.SetLength(lengthInBeats);

            try
            {
                while ((!Melodies.Finished()) && (steps < StepLimit))
                {
                    if (Melodies.Time.Position > lengthInBeats)
                        Melodies.RemoveLast();
                    else
                        Step();

                    steps++;
                    if (callback != null)
                        callback(new GenerationInfo(steps, _rollback, Melodies.Time.Position));

                }
            }
            catch (StopGeneration) { } //-202107752
            catch (ArgumentOutOfRangeException) { }

            return steps;
        }

        private void Step()
        {
            Melodies.Filter();
            //double max = Melodies.Freqs.Max(kv => (kv.Value > MinimumNoteFrequencyAllowed) ? kv.Value : 0);
            var max = Melodies.Freqs.Max(kv => kv.Value);

            if (max > MinimumAccumulatedFrequency) //должно быть что-то приличное!
                ChooseNextNote();
            else
            {
                _rollback++;
                if (Melodies.NoteCount > 0)
                    Melodies.RemoveLast();
                else
                    Melodies.SetFirstNoteFreqs();
            }
        }

        private void ChooseNextNote()
        {
            var possibleNext = Melodies.Freqs.Where(kv => kv.Value > MinimumNoteFrequencyAllowed);//.OrderBy(kv => kv.Key);
#if TRACE
            var sb = new StringBuilder();
            foreach (var kv in Melodies.Freqs)
            {
                sb.AppendFormat("{0} => {1}; ", kv.Key, kv.Value);
            }
            Console.WriteLine(sb);
            Console.WriteLine();
#endif
            var next = (TwoNotes)_chooseStrategy.ChooseNext(possibleNext);

            Melodies.AddTwoNotes(next);
        }
    }
}


