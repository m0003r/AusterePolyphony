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

    public class TwoVoiceGenerator : IGenerator
    {
        public TwoVoices Melodies { get; private set; }
        public int Seed { get; private set; }
        public int StepLimit { get; private set; }

        IChooseNextStrategy<TwoNotes> chooseStrategy;

        const double MinimumAccumulatedFrequency = 0.1;
        const double MinimumNoteFrequencyAllowed = 0.02;

        public List<Melody> GetNotes()
        {
            var res = new List<Melody>();
            res.Add(Melodies.Voice1);
            res.Add(Melodies.Voice2);
            return res;
        }

        public int GetSeed()
        {
            return Seed;
        }        

        public TwoVoiceGenerator(Clef Clef1, Clef Clef2, Modus Modus, Time Time, int seed = 0, int stepLimit = 50000, IChooseNextStrategy<TwoNotes> Strategy = null)
        {
            StepLimit = stepLimit;
            Melodies = new TwoVoices(Clef1, Clef2, Modus, Time);
            if (Strategy == null)
            {
                SetSeed(seed);
                chooseStrategy = new DefaultNextStrategy<TwoNotes>(this.Seed);
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
            uint lengthInBeats = Length * (uint)Melodies.Time.Beats * 4;
            int steps = 0;

            Melodies.setLength(lengthInBeats);

            try
            {
                while ((Melodies.Time.Position < lengthInBeats) && (steps < StepLimit))
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
            Melodies.Filter(dumpResult);
            double max = Melodies.Freqs.Max(kv => (kv.Value > MinimumNoteFrequencyAllowed) ? kv.Value : 0);

            if (max > TwoVoiceGenerator.MinimumAccumulatedFrequency) //должно быть что-то приличное!
                chooseNextNote(dumpResult);
            else
            {
                if (Melodies.NoteCount > 0)
                    Melodies.RemoveLast();
                else
                    Melodies.FirstNote();
            }
        }

        private void chooseNextNote(bool dumpResult = false)
        {
            var possibleNext = Melodies.Freqs.Where(kv => kv.Value > MinimumNoteFrequencyAllowed).OrderBy(kv => kv.Key);

            if (dumpResult)
            {
                var sb = new StringBuilder();
                foreach (var kv in Melodies.Freqs)
                {
                    sb.AppendFormat("{0} => {1}; ", kv.Key.ToString(), kv.Value);
                }
                Console.WriteLine(sb);
            }

            TwoNotes next = chooseStrategy.ChooseNext(possibleNext);

            Melodies.AddTwoNotes(next);
        }
    }
}


