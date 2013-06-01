using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Compositor.Rules;
using PitchBase;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("RulesTest")]
namespace Compositor.Levels
{
    using NotesList = List<Note>;
    using LSList = List<LeapOrSmooth>;

    public class LeapOrSmooth
    {
        public bool isLeap;
        public bool isSmooth { get { return !isLeap; } }
        public Time TimeStart;
        public int Duration;
        public Time TimeEnd { get { return TimeStart + Duration; } }
        public int NotesCount { get { return notes.Count; } }
        public Interval Interval;

        public bool Upwards { get { return Interval.Upwards; } }

        private NotesList notes;

        public LeapOrSmooth(Note a, Note b)
        {
            TimeStart = a.TimeStart;
            Duration = (b.TimeEnd - TimeStart).Beats;
            isLeap = b.Leap.isLeap;
            Interval = b.Pitch - a.Pitch;

            notes = new NotesList();
            notes.Add(a);
            notes.Add(b);
        }

        public bool CanAdd(Note n)
        {
            return ((n.Leap.isLeap == isLeap) && (n.Leap.Upwards == Upwards));
        }

        public void Add(Note n)
        {
            if (!CanAdd(n))
                throw new Exception("Invalid adding to LeapOrSmooth");

            Interval = Interval + n.Leap;
            Duration += n.Duration;
            notes.Add(n);
        }

        public void Delete()
        {
            if (NotesCount <= 2)
                throw new Exception("Cannot delete from two-note LeapOrSmooth");

            Note n = notes.Last();
            Interval = Interval - n.Leap;
            Duration -= n.Duration;

            notes.RemoveAt(NotesCount - 1);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append(isLeap ? "Leap(" : "Smooth(");
            sb.Append(NotesCount);
            sb.Append("/");
            sb.Append(Duration);
            sb.Append(") ");
            sb.Append(Interval.ToString());

            return sb.ToString();
        }
    }

    [Rule(typeof(CadenzaRule))]
    [Rule(typeof(TrillRule))]
    [Rule(typeof(SyncopaRule))]
    [Rule(typeof(BreveRule))]
    [Rule(typeof(TritoneRule))]
    [Rule(typeof(PeakRule))]
    [Rule(typeof(PeakRule2))]
    [Rule(typeof(ManyQuartersRule))]
    [Rule(typeof(DottedHalveRestrictionRule))]
    [Rule(typeof(AfterLeapRules))]
    [Rule(typeof(DenyTwoNoteSequence))]
    [Rule(typeof(DenyTwoNoteRhytmicSequence))]
    [Rule(typeof(DenySequence))]
    [Rule(typeof(GravityRule))]
    [Rule(typeof(LimitGamming))]
    [Rule(typeof(LeapCompensation))]
    [Rule(typeof(DenyStrongNotesRepeat))]

    public class Melody : RuledLevel<Melody>, IEnumerable<Note>, IEnumerable<KeyValuePair<int, Pitch>> , IEnumerable
    {
        public Clef Clef { get; private set; }
        public Modus Modus { get; private set; }
        public Time Time { get; private set; }

        public int Reserve { get; private set; }
        public int Uncomp { get; private set; }

        internal NotesList notes;
        internal LSList leapsmooth;
        internal List<Pitch> Diapason { get; private set; }

        Dictionary<Note, double> firstNoteFreqs;

        internal uint DesiredLength;

        public NotesList Notes { get { return new NotesList(notes); } }
        public LSList LeapSmooth { get { return new LSList(leapsmooth); } }

        internal Pitch Higher;
        internal Pitch Lower;

        public Melody(Clef Clef, Modus Modus, Time Time)
        {
            this.Clef = Clef;
            this.Modus = Modus;
            this.Time = Time;

            notes = new NotesList();
            leapsmooth = new LSList();
            Reserve = Uncomp = 0;

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
            firstNoteFreqs = new Dictionary<Note, double>();

            List<Pitch> pi = Diapason.FindAll(p => ((p.Degree % 7 == 4) || (p.Degree % 7 == 0)));

            foreach (Pitch p in pi)
            {
                firstNoteFreqs[new Note(p, Time, 4)] = 0.5;
                firstNoteFreqs[new Note(p, Time, 6)] = 0.7;
                firstNoteFreqs[new Note(p, Time, 8)] = 1;
                firstNoteFreqs[new Note(p, Time, 12)] = 1;
            }
        }

        private void FirstNote()
        {
            Freqs = new Dictionary<Note, double>(firstNoteFreqs);
        }

        internal void RemoveLast(bool ban = true)
        {
            Note n = notes.Last();
            Time -= n.Duration;
            notes.RemoveAt(notes.Count - 1);
            
            if (ban)
            {
                banNote(n);
            }

            updateLeapsSmooth(true);
            updateUncomp();
        }

        private void banNote(Note n)
        {
            if (notes.Count > 0)
            {
                notes.Last().ban(n);
                Freqs = notes.Last().Freqs;
            }
            else
            {
                firstNoteFreqs[n] = 0;
                FirstNote();
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

        private void updateFreqs()
        {
            if (notes.Count > 0)
                notes.Last().UpdateFreqs(Freqs);
            else
                firstNoteFreqs = Freqs;
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

            /*if (adjust > strength)
                return adjust;

            else
                return strength + (strength - adjust) / 8;*/
        }

        internal double getStrengthIf(Note next)
        {
            return calculateStrength(notes[notes.Count - 2], notes[notes.Count - 1], next);
        }

        private double rhytmicStopStrength(int prevDur, int currDur)
        {
            double k = (currDur - prevDur) / prevDur;
            return (k > 1.5) ? k * 0.8 : 1;

        }

        private double calculateStrength(Note prev, Note curr, Note next)
        {
            const double basicStrength = 1;
            const double changeDirectionAdjust = 1.5;
            const double leapAdjustCoefficent = 0.02;
            const double offBeatAdjust = 0.7;

            double Strength = basicStrength;// (curr.Duration >= 8) ? 1 : 0.5;

            if (next != null)
            {
                if (curr.Leap.Upwards ^ next.Leap.Upwards)
                    Strength = adjustStrength(Strength, changeDirectionAdjust);

                if (next.Leap.AbsDeg > 1)
                    Strength = adjustStrength(Strength, changeDirectionAdjust + leapAdjustCoefficent * next.Leap.AbsDeg);
            }

            /* if (curr.Duration == prev.Duration)
            {
                Strength *= 0.8;
                if ((next != null) && (next.Duration == curr.Duration))
                    Strength *= 0.65;
            }

            */
            if (curr.Leap.AbsDeg > 1)
                Strength = adjustStrength(Strength, changeDirectionAdjust + leapAdjustCoefficent * curr.Leap.AbsDeg);

            if ((curr.TimeStart.Beats % 4) != 0)
                Strength *= offBeatAdjust;

            if (prev.Duration < curr.Duration)
                Strength *= rhytmicStopStrength(prev.Duration, curr.Duration);

            /*double durCoeff = (double)curr.Duration / (double)prev.Duration;
            if (durCoeff < 1)
                durCoeff = 1 / durCoeff;

            if (durCoeff > 2)
                Strength *= (1 + durCoeff / 6);*/

            return Strength;
        }

        public void AddNote(Note n)
        {
            updateFreqs();
            notes.Add(n);            
            filtered = false;
            Time += n.Duration;

            updateStrenghts();
            updateLeapsSmooth();
            updateUncomp();
        }

        private bool CoSign(int a, int b)
        {
            return (Math.Sign(a) == Math.Sign(b));
        }


        private void updateUncomp()
        {
            Reserve = Uncomp = 0;

            foreach (var ls in LeapSmooth)
            {
                int deg = ls.Interval.Degrees;

                if (ls.isSmooth)
                    updateUncompIfSmooth(deg);
                else
                    updateUncompIfLeap(deg);

            }
            Notes.Last().Uncomp = Uncomp;
            Notes.Last().Reserve = Reserve;

        }

        private void updateUncompIfLeap(int deg)
        {
            if (Reserve != 0)
                if (CoSign(Reserve, deg))
                    Uncomp += Reserve;
                else
                {
                    if (Math.Abs(Reserve) >= Math.Abs(deg * 2))
                        Reserve += deg * 2;
                    else
                        Reserve = 0;
                }

            Uncomp += deg;
        }

        private void updateUncompIfSmooth(int deg)
        {
            if ((Reserve == 0) || (CoSign(Reserve, deg)))
                Reserve += deg;
            else
                if (Math.Abs(Reserve) >= Math.Abs(deg * 2))
                    Reserve += deg * 2;
                else
                    Reserve = deg + Reserve / 2;

            if (Uncomp != 0)
            {
                if (CoSign(Uncomp, deg))
                    Uncomp += deg / 2;
                else
                    if (Math.Abs(Uncomp) <= Math.Abs(deg * 2))
                        Uncomp = 0;
                    else
                        Uncomp += deg * 2;

            }
        }

        private void updateLeapsSmooth(bool delete = false)
        {
            if (notes.Count < 2)
                return;

            LeapOrSmooth ls;


            if (delete)
            {
                if (leapsmooth.Count == 0)
                    return;

                if (leapsmooth.Last().NotesCount == 2)
                {
                    leapsmooth.RemoveAt(leapsmooth.Count - 1);
                    return;
                }

                leapsmooth.Last().Delete();

                return;
            }

            if (notes.Count == 2)
            {
                ls = new LeapOrSmooth(notes[0], notes[1]);
                leapsmooth.Add(ls);
                return;
            }

            ls = leapsmooth.Last();
            var lastNote = notes.Last();

            if (ls.CanAdd(lastNote))
                ls.Add(lastNote);
            else
            {
                ls = new LeapOrSmooth(notes[notes.Count - 2], lastNote);
                leapsmooth.Add(ls);
            }
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
