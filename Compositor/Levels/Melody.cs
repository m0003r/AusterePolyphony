using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Compositor.Helpers;
using Compositor.Rules;
using PitchBase;

namespace Compositor.Levels
{
    using NotesList = List<Note>;

    [Rule(typeof(CadenzaRule))]
    [Rule(typeof(TrillRule))]
    [Rule(typeof(SyncopaRule))]
    [Rule(typeof(TritoneRule))]
    [Rule(typeof(PeakRule))]
    [Rule(typeof(ManyQuartersRule))]
    [Rule(typeof(DottedHalveRestrictionRule))]
    [Rule(typeof(AfterLeapRules))]
    [Rule(typeof(DenyTwoNoteSequence))]
    [Rule(typeof(DenyTwoNoteRhytmicSequence))]
    [Rule(typeof(DenySequence))]
    [Rule(typeof(AverageHeight))]
    [Rule(typeof(DenyStrongNotesRepeat))]

    public class Melody : RuledLevel<Melody>, IEnumerable<Note>, IEnumerable<KeyValuePair<int, Pitch>>, IEnumerable
    {
        public Clef Clef { get; private set; }
        public Modus Modus { get; private set; }
        public Time Time { get; private set; }

        internal NotesList notes;
        internal List<Pitch> Diapason { get; private set; }

        Dictionary<Note, double> protoFreqs;

        internal uint DesiredLength;

        public NotesList Notes { get { return new NotesList(notes); } }

        internal Pitch Higher;
        internal Pitch Lower;

        public Melody(Clef Clef, Modus Modus, Time Time)
        {
            this.Clef = Clef;
            this.Modus = Modus;
            this.Time = Time;

            notes = new NotesList();

            Time.Position = 0;
            SetupDiapason(Clef, Modus);
            InitFirstNote();
            FirstNote();
        }

        private void SetupDiapason(Clef Clef, Modus Modus)
        {
            PitchFactory pf = new PitchFactory(Modus, Clef);
            Diapason = pf.Pitches;
            Lower = Diapason.Last();
            Higher = Diapason.First();
        }

        private void InitFirstNote()
        {
            protoFreqs = new Dictionary<Note, double>();

            List<Pitch> pi = Diapason.FindAll(
                delegate(Pitch p)
                {
                    return ((p.Degree % 7 == 4) || (p.Degree % 7 == 0));
                }
            );

            foreach (Pitch p in pi)
            {
                protoFreqs[new Note(p, Time, 4)] = 0.5;
                protoFreqs[new Note(p, Time, 6)] = 0.7;
                protoFreqs[new Note(p, Time, 8)] = 1;
                protoFreqs[new Note(p, Time, 12)] = 1;
            }
        }

        private void FirstNote()
        {
            Freqs = new Dictionary<Note, double>(protoFreqs);
        }

        internal void RemoveLast(bool ban = true)
        {
            Note n = notes.Last();
            Time -= n.Duration;
            notes.RemoveAt(notes.Count - 1);
            
            if (ban)
            {
                if (notes.Count > 0)
                {
                    notes.Last().ban(n);
                    Freqs = notes.Last().Freqs;
                }
                else
                {
                    protoFreqs[n] = 0;
                    FirstNote();
                }
            }
        }

        protected override void AddVariants()
        {
            if (Notes.Count == 0)
                FirstNote();

            else
            {
                if (Notes.Last().Diapason == null)
                    Notes.Last().Diapason = Diapason;

                Freqs = Notes.Last().Filter();
            }
        }

        private void AdjustFreqs()
        {
            if (notes.Count > 0)
                notes.Last().AdjustFreqs(Freqs);
            else
                protoFreqs = Freqs;
        }

        private void updateStrenghts()
        {
            if (notes.Count < 3)
            {
                notes[0].Strength = 2;
                return;
            }

            Note prev, curr, next;
            int currNumber = 1;

            prev = notes[0]; curr = notes[1]; next = notes[2];
            while (currNumber < notes.Count - 1)
            {
                curr.Strength = calculateStrength(prev, curr, next);

                currNumber++;
                prev = curr;
                curr = next;
                next = notes[currNumber];
            }

            notes[currNumber].Strength = calculateStrength(curr, next, null);
        }

        private double adjustStrength(double strength, double adjust)
        {
            return strength * adjust;

            if (adjust > strength)
                return adjust;

            else
                return strength + (strength - adjust) / 8;
        }

        internal double getStrengthIf(Note next)
        {
            return calculateStrength(notes[notes.Count - 2], notes[notes.Count - 1], next);
        }

        private double calculateStrength(Note prev, Note curr, Note next)
        {
            double Strength = 1;// (curr.Duration >= 8) ? 1 : 0.5;

            if (next != null)
            {
                if (curr.Leap.Upwards ^ next.Leap.Upwards)
                    Strength = adjustStrength(Strength, 1.5);

                if (next.Leap.AbsDeg > 1)
                    Strength = adjustStrength(Strength, 1.5 + 0.02 * next.Leap.AbsDeg);
            }

            /* if (curr.Duration == prev.Duration)
            {
                Strength *= 0.8;
                if ((next != null) && (next.Duration == curr.Duration))
                    Strength *= 0.65;
            }

            */
            if (curr.Leap.AbsDeg > 1)
                Strength = adjustStrength(Strength, 1.5 + 0.02 * curr.Leap.AbsDeg);

            if ((curr.TimeStart.Beats % 4) != 0)
                Strength *= 0.7;

            /*double durCoeff = (double)curr.Duration / (double)prev.Duration;
            if (durCoeff < 1)
                durCoeff = 1 / durCoeff;

            if (durCoeff > 2)
                Strength *= (1 + durCoeff / 6);*/

            return Strength;
        }

        internal void AddNote(Note n)
        {
            AdjustFreqs();
            notes.Add(n);            
            filtered = false;
            Time += n.Duration;

            updateStrenghts();
        }

        public Note this[int i] { get { return notes[i]; } }

        public int NoteCount { get { return notes.Count; } }

        IEnumerator<Note> IEnumerable<Note>.GetEnumerator()
        {
            return Notes.GetEnumerator();
        }

        IEnumerator<KeyValuePair<int, Pitch>> IEnumerable<KeyValuePair<int, Pitch>>.GetEnumerator()
        {
            return new Melody.SubNoteIterator(this);
        }

        public IEnumerable GetLast(int count)
        {
            if (count > NoteCount)
                throw new IndexOutOfRangeException();

            SubNoteIterator iter = new SubNoteIterator(this, NoteCount - count);
            iter.Reset();
            do
                yield return iter.Current;
            while (iter.MoveNext());
        }
        
        public IEnumerator GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }


        class SubNoteIterator : IEnumerator<KeyValuePair<int, Pitch>>, IEnumerator
        {
            private Melody melody;

            private int NoteNumber;
            private int subPosition;
            private int position;

            private int minNoteNumber;
            private int maxNoteNumber;

            public SubNoteIterator(Melody melody)
                : this(melody, 0, melody.NoteCount) { }

            public SubNoteIterator(Melody melody, int start)
                : this(melody, start, melody.NoteCount) { }

            public SubNoteIterator(Melody melody, int start, int end)
            {
                this.melody = melody;
                this.minNoteNumber = start;
                this.maxNoteNumber = end;

                if (this.minNoteNumber > melody.NoteCount)
                    start = melody.NoteCount;
                if (this.maxNoteNumber < melody.NoteCount)
                    end = melody.NoteCount;

                Reset();
            }
            
            public void Reset()
            {
                NoteNumber = minNoteNumber;
                subPosition = -1;
                position = (maxNoteNumber > minNoteNumber) ? melody[minNoteNumber].TimeStart.Position : 0;
            }

            public bool MoveNext()
            {

                subPosition++;
                position++;
                if (subPosition < melody[NoteNumber].Duration)
                    return true;
                else
                {
                    NoteNumber++;
                    subPosition = 0;
                    if (NoteNumber < melody.NoteCount)
                        return true;
                }

                return false;
            }

            public void Dispose() { }

            object IEnumerator.Current { get { return Current; } }

            public KeyValuePair<int, Pitch> Current { get { return new KeyValuePair<int, Pitch>(position, melody[NoteNumber].Pitch); } }
        }
    }
}
