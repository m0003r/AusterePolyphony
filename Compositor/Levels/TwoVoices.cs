using System;
using System.Collections.Generic;
using System.Linq;
using Compositor.Generators;
using Compositor.Rules.Base;
using Compositor.Rules.TwoVoices;
using PitchBase;

namespace Compositor.Levels
{

    [Rule(typeof(DistanceRule))]
    
    [Rule(typeof(ConsonantesSimult))]
    [Rule(typeof(LinearDiss))]
    [Rule(typeof(AfterDiss))]
    [Rule(typeof(SecondaSeptimaResolution))]
    [Rule(typeof(DenyParallelConsonantes))]
    [Rule(typeof(DenyStraightToConsonans))]
    [Rule(typeof(DenyHiddenParallelConsonantes))]

    [Rule(typeof(ComplementRule))]
    [Rule(typeof(ComplementRule2))]
    [Rule(typeof(DenyCrossing))]
    [Rule(typeof(SusPassTakeRule))]
    [Rule(typeof(SusPassResolutionRule))]


    public class TwoVoices : RuledLevel
    {
        public Voice Voice1 { get; private set; }
        public Voice Voice2 { get; private set; }

        public FreqsDict FirstFreqs;

        internal List<TwoNotes> Twonotes;

        public Time Time { get; private set; }


        public TwoVoices(Clef clef1, Clef clef2, Modus modus, Time time)
        {
            Time = time;

            Voice1 = new Voice(clef1, modus, time, VoiceType.Top);
            Voice2 = new Voice(clef2, modus, time, VoiceType.Bass);

            Twonotes = new List<TwoNotes>();
            GenerateFirstFreqs();

            var keys = new List<IDeniable>(FirstFreqs.Keys);
            foreach (var key in keys.Where(key => ((TwoNotes)key).Interval.ModDeg == 3))
                FirstFreqs[key] = 0;

            FirstNote();
            Filtered = true;
        }

        private void GenerateFirstFreqs()
        {
            FirstFreqs = CombineFreqs(Voice1.Freqs, Voice2.Freqs);
        }

        public override void AddVariants()
        {
            var filtered1 = Voice1.Filter();
            var filtered2 = Voice2.Filter();

            if (Voice1.NoteCount == 0)
            {
                AddTwoNotesVariants(filtered1, filtered2);
            }
            else
            {
                var l1 = Voice1.Notes.Last();
                var l2 = Voice2.Notes.Last();

                int diff = l1.TimeEnd.Position - l2.TimeEnd.Position;
                if (diff == 0)
                    AddTwoNotesVariants(filtered1, filtered2);
                else if (diff < 0)
                    AddToVoice1Variants(filtered1, l2);
                else
                    AddToVoice2Variants(l1, filtered2);
            }

        }

        private void AddTwoNotesVariants(FreqsDict filtered1, FreqsDict filtered2)
        {
            Freqs = CombineFreqs(filtered1, filtered2);
        }

        private void AddToVoice1Variants(FreqsDict filtered1, Note l2)
        {
            var f2 = new FreqsDict();
            f2[l2] = 1;
            Freqs = CombineFreqs(filtered1, f2);
        }

        private void AddToVoice2Variants(Note l1, FreqsDict filtered2)
        {
            var f1 = new FreqsDict();
            f1[l1] = 1;
            Freqs = CombineFreqs(f1, filtered2);
        }

        private static double GetCombinedFreq(double f1, double f2)
        {
            return Math.Sqrt(f1*f2);
        }

        private static FreqsDict CombineFreqs(FreqsDict f1, FreqsDict f2)
        {
            var result = new FreqsDict();
            foreach (var n1 in f1)
                foreach (var n2 in f2)
                    result.Add(new TwoNotes((Note)n1.Key, (Note)n2.Key), GetCombinedFreq(n1.Value, n2.Value));            
            

            return result;
        }


        internal void AddTwoNotes(TwoNotes next)
        {
            if (Twonotes.Count > 0)
                Twonotes.Last().Freqs = Freqs;

            Twonotes.Add(next);

            if ((Voice1.NotesList.Count == 0) || !(Voice1.EndsWith(next.Note1)))
                Voice1.AddNote(next.Note1);
            if ((Voice2.NotesList.Count == 0) || !(Voice2.EndsWith(next.Note2)))
                Voice2.AddNote(next.Note2);

            Filtered = false;

            Time = next.Note1.TimeEnd;
            if (Time.Position < next.Note2.TimeEnd.Position)
                Time = next.Note2.TimeEnd;
        }

        internal void SetLength(uint lengthInBeats)
        {
            Voice1.SetLength(lengthInBeats);
            Voice2.SetLength(lengthInBeats);
        }

        public int NoteCount { get { return Twonotes.Count; } }

        internal void RemoveLast(bool ban = true)
        {
            if (Twonotes.Count > 1)
            {
                var removed = Twonotes.Last();
                Twonotes.RemoveAt(Twonotes.Count - 1);
                var newLast = Twonotes.Last();

                if (removed.Note1 != newLast.Note1)
                    Voice1.RemoveLast(false);
                if (removed.Note2 != newLast.Note2)
                    Voice2.RemoveLast(false);

                if (ban)
                {
                    newLast.Freqs[removed] = 0;
                    removed.IsBanned = true;
                }

                Freqs = newLast.Freqs;

                Time = newLast.Note1.TimeEnd;
                if (Time < newLast.Note2.TimeEnd)
                    Time = newLast.Note2.TimeEnd;

            }
            else if (Twonotes.Count == 1)
            {
                Voice1.RemoveLast(false);
                Voice2.RemoveLast(false);
                var removed = Twonotes.Last(); 
                Twonotes.RemoveAt(0);
                FirstFreqs[removed] = 0;

                if (ban)
                    removed.IsBanned = true;

                FirstNote();

                Time.Position = 0;
            }

        }

        internal void FirstNote()
        {
            Freqs = FirstFreqs;
        }

        internal bool Finished()
        {
            return (Voice1.Finished() && Voice2.Finished());
        }

        public void SetMirroring(Voice source, Voice dest, ImitationSettings imitationSettings)
        {
            dest.SetMirroring(imitationSettings, source);
            GenerateFirstFreqs();
        }
    }
}
