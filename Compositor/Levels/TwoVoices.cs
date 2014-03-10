using System;
using System.Collections.Generic;
using System.Linq;
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
    [Rule(typeof(SusPassRule1))]
    [Rule(typeof(SusPassRule2))]


    public class TwoVoices : RuledLevel<TwoVoices, TwoNotes>
    {
        public Melody Voice1 { get; private set; }
        public Melody Voice2 { get; private set; }

        public Dictionary<TwoNotes, double> FirstFreqs;

        internal List<TwoNotes> Twonotes;

        public Time Time { get; private set; }

        public TwoVoices(Clef clef1, Clef clef2, Modus modus, Time time)
        {
            Time = time;

            Voice1 = new Melody(clef1, modus, time);
            Voice2 = new Melody(clef2, modus, time);

            Twonotes = new List<TwoNotes>();
            FirstFreqs = CombineFreqs(Voice1.Freqs, Voice2.Freqs);

            var keys = new List<TwoNotes>(FirstFreqs.Keys);
            foreach (var key in keys.Where(key => key.Interval.ModDeg == 3))
                FirstFreqs[key] = 0;

            FirstNote();
            Filtered = true;
        }

        protected override void AddVariants(bool dumpResult = false)
        {
            var filtered1 = Voice1.Filter(dumpResult);
            var filtered2 = Voice2.Filter(dumpResult);

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

        private void AddTwoNotesVariants(Dictionary<Note, double> filtered1, Dictionary<Note, double> filtered2)
        {
            Freqs = CombineFreqs(filtered1, filtered2);
        }

        private void AddToVoice1Variants(Dictionary<Note, double> filtered1, Note l2)
        {
            var f2 = new Dictionary<Note, double>();
            f2[l2] = 1;
            Freqs = CombineFreqs(filtered1, f2);
        }

        private void AddToVoice2Variants(Note l1, Dictionary<Note, double> filtered2)
        {
            var f1 = new Dictionary<Note, double>();
            f1[l1] = 1;
            Freqs = CombineFreqs(f1, filtered2);
        }

        private double GetCombinedFreq(double f1, double f2)
        {
            return Math.Sqrt(f1*f2);
        }

        private Dictionary<TwoNotes, double> CombineFreqs(Dictionary<Note, double> f1, Dictionary<Note, double> f2)
        {
            var result = new Dictionary<TwoNotes, double>();
            foreach (var n1 in f1)
                foreach (var n2 in f2)
                    result.Add(new TwoNotes(n1.Key, n2.Key), GetCombinedFreq(n1.Value, n2.Value));            
            

            return result;
        }


        internal void AddTwoNotes(TwoNotes next)
        {
            if (Twonotes.Count > 0)
                Twonotes.Last().Freqs = Freqs;

            Twonotes.Add(next);

            if ((Voice1.notes.Count == 0) || !(Voice1.EndsWith(next.Note1)))
                Voice1.AddNote(next.Note1);
            if ((Voice2.notes.Count == 0) || !(Voice2.EndsWith(next.Note2)))
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
                    newLast.Freqs[removed] = 0;

                Freqs = newLast.Freqs;

                Time = newLast.Note1.TimeEnd;
                if (Time.Position < newLast.Note2.TimeEnd.Position)
                    Time = newLast.Note2.TimeEnd;

            }
            else if (Twonotes.Count == 1)
            {
                Voice1.RemoveLast(false);
                Voice2.RemoveLast(false);
                var removed = Twonotes.Last(); 
                Twonotes.RemoveAt(0);
                FirstFreqs[removed] = 0;

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
    }
}
