﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PitchBase;
using Compositor.Rules;

namespace Compositor.Levels
{
    public class TwoNotes : IDeniable
    {
        public Note Note1;
        public Note Note2;

        public Rule DeniedRule { get; set; }
        public bool isBanned { get; set; }

        public TwoNotes(Note Note1, Note Note2)
        {
            isBanned = false;
            DeniedRule = null;
            this.Note1 = Note1;
            this.Note2 = Note2;
        }

        public Time Time { get { return (Note1.TimeEnd.Beats > Note2.TimeEnd.Beats) ? Note1.TimeEnd : Note2.TimeEnd; } }
    }

    public class TwoVoices : RuledLevel<TwoVoices, TwoNotes>
    {
        public Melody Voice1 { get; private set; }
        public Melody Voice2 { get; private set; }

        private Melody lastAdded;

        internal List<TwoNotes> twonotes;

        public TwoVoices(Clef Clef1, Clef Clef2, Modus Modus, Time Time)
        {
            Voice1 = new Melody(Clef1, Modus, Time);
            Voice2 = new Melody(Clef2, Modus, Time);

            twonotes = new List<TwoNotes>();
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

                int diff = l1.TimeEnd.Beats - l2.TimeEnd.Beats;
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
            twonotes.Add(next);

            if (next.Note1 != Voice1.Notes.Last())
                Voice1.AddNote(next.Note1);
            if (next.Note2 != Voice2.Notes.Last())
                Voice1.AddNote(next.Note1);

        }

        internal void setLength(uint lengthInBeats)
        {
            Voice1.setLength(lengthInBeats);
            Voice2.setLength(lengthInBeats);
        }

        public int NoteCount { get { return twonotes.Count; } }

        internal void RemoveLast()
        {
            throw new NotImplementedException();
        }

        internal void FirstNote()
        {
            throw new NotImplementedException();
        }

        public Time Time { get { return NoteCount > 0 ? twonotes.Last().Time : Time.Create(Voice1.Time.perfectus); } }
    }
}