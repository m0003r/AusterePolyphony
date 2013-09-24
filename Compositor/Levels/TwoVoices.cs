using System;
using System.Collections.Generic;
using System.Linq;

using PitchBase;
using Compositor.Rules;

namespace Compositor.Levels
{
    [Rule(typeof(ConsonantesSimult))]
    [Rule(typeof(DenyParallelConsonantes))]
    [Rule(typeof(DenyStraightToConsonans))]
    public class TwoVoices : RuledLevel<TwoVoices, TwoNotes>
    {
        public Melody Voice1 { get; private set; }
        public Melody Voice2 { get; private set; }

        public Dictionary<TwoNotes, double> firstFreqs;

        internal List<TwoNotes> twonotes;

        public TwoVoices(Clef Clef1, Clef Clef2, Modus Modus, Time Time)
        {
            Voice1 = new Melody(Clef1, Modus, Time);
            Voice2 = new Melody(Clef2, Modus, Time);

            twonotes = new List<TwoNotes>();
            firstFreqs = CombineFreqs(Voice1.Freqs, Voice2.Freqs);

            var keys = new List<TwoNotes>(firstFreqs.Keys);
            foreach (var key in keys)
                if (key.Interval.ModDeg == 3)
                    firstFreqs[key] = 0;

            FirstNote();
            filtered = true;
        }

        protected override void AddVariants(bool dumpResult = false)
        {
            var Filtered1 = Voice1.Filter(dumpResult);
            var Filtered2 = Voice2.Filter(dumpResult);

            if (Voice1.NoteCount == 0)
            {
                AddTwoNotesVariants(Filtered1, Filtered2);
            }
            else
            {
                var l1 = Voice1.Notes.Last();
                var l2 = Voice2.Notes.Last();

                int diff = l1.TimeEnd.Position - l2.TimeEnd.Position;
                if (diff == 0)
                    AddTwoNotesVariants(Filtered1, Filtered2);
                else if (diff < 0)
                    AddToVoice1Variants(Filtered1, l2);
                else
                    AddToVoice2Variants(l1, Filtered2);
            }

        }

        private void AddTwoNotesVariants(Dictionary<Note, double> Filtered1, Dictionary<Note, double> Filtered2)
        {
            Freqs = CombineFreqs(Filtered1, Filtered2);
        }

        private void AddToVoice1Variants(Dictionary<Note, double> Filtered1, Note l2)
        {
            var f2 = new Dictionary<Note, double>();
            f2[l2] = 1;
            Freqs = CombineFreqs(Filtered1, f2);
        }

        private void AddToVoice2Variants(Note l1, Dictionary<Note, double> Filtered2)
        {
            var f1 = new Dictionary<Note, double>();
            f1[l1] = 1;
            Freqs = CombineFreqs(f1, Filtered2);
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
            if (twonotes.Count > 0)
                twonotes.Last().Freqs = Freqs;

            twonotes.Add(next);

            if ((Voice1.notes.Count == 0) || !(Voice1.EndsWith(next.Note1)))
                Voice1.AddNote(next.Note1);
            if ((Voice2.notes.Count == 0) || !(Voice2.EndsWith(next.Note2)))
                Voice2.AddNote(next.Note2);

            filtered = false;
        }

        internal void setLength(uint lengthInBeats)
        {
            Voice1.setLength(lengthInBeats);
            Voice2.setLength(lengthInBeats);
        }

        public int NoteCount { get { return twonotes.Count; } }

        internal void RemoveLast(bool ban = true)
        {
            if (twonotes.Count > 1)
            {
                var removed = twonotes.Last();
                twonotes.RemoveAt(twonotes.Count - 1);
                var new_last = twonotes.Last();

                if (removed.Note1 != new_last.Note1)
                    Voice1.RemoveLast(false);
                if (removed.Note2 != new_last.Note2)
                    Voice2.RemoveLast(false);

                if (ban)
                    new_last.Freqs[removed] = 0;

                Freqs = new_last.Freqs;

            }
            else if (twonotes.Count == 1)
            {
                Voice1.RemoveLast(false);
                Voice2.RemoveLast(false);
                var removed = twonotes.Last(); 
                twonotes.RemoveAt(0);
                firstFreqs[removed] = 0;

                FirstNote();
            }
        }

        public Time Time { get { return NoteCount > 0 ? twonotes.Last().Time : Time.Create(Voice1.Time.perfectus); } }

        internal void FirstNote()
        {
            Freqs = firstFreqs;
        }
    }
}
