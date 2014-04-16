using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Compositor.Rules.Base;
using Compositor.Rules.Melody;
using PitchBase;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("RulesTest")]
namespace Compositor.Levels
{
    using NotesList = List<Note>;
    using LSList = List<LeapOrSmooth>;

    [Rule(typeof(CadenzaRule))]
    [Rule(typeof(DenyGamming))]
    [Rule(typeof(TrillRule))]
    [Rule(typeof(DenyPolka))]
    [Rule(typeof(BreveRule))]
    [Rule(typeof(TritoneRule))]
    [Rule(typeof(TritoneRule2))]
    [Rule(typeof(PeakRule))]
    [Rule(typeof(PeakRule2))]
    [Rule(typeof(ManyQuartersRule))]
    [Rule(typeof(DottedHalveRestrictionRule))]
    [Rule(typeof(AfterLeapRules))]
    [Rule(typeof(DenyTwoNoteSequence))]
    [Rule(typeof(DenyTwoNoteRhytmicSequence))]
    [Rule(typeof(DenySequence))]
    [Rule(typeof(GravityRule))]
    [Rule(typeof(LeapCompensation))]
    [Rule(typeof(DenyStrongNotesRepeat))]
    [Rule(typeof(AfterQuarterNewRule))]
    [Rule(typeof(TooManyEightsRule))]

    public class Voice : RuledLevel, IEnumerable<Note>, IEnumerable<KeyValuePair<int, Pitch>>
    {
        public Clef Clef { get; private set; }
        public Modus Modus { get; private set; }
        public Time Time { get; private set; }

        public int Reserve { get; private set; }
        public int Uncomp { get; private set; }

        internal NotesList NotesList;
        internal LSList Leapsmooth;
        internal List<Pitch> Diapason { get; private set; }

        FreqsDict _firstNoteFreqs;

        internal uint DesiredLength;

        public NotesList Notes { get { return new NotesList(NotesList); } }
        public LSList LeapSmooth { get { return new LSList(Leapsmooth); } }

        internal Pitch Higher;
        internal Pitch Lower;

        public Voice(Clef clef, Modus modus, Time time)
        {
            Clef = clef;
            Modus = modus;
            Time = time;

            NotesList = new NotesList();
            Leapsmooth = new LSList();
            Reserve = Uncomp = 0;

            time.Position = 0;
            SetupDiapason(clef, modus);
            InitFirstNote();
            FirstNote();
        }

        public void SetLength(uint desiredLength)
        {
            DesiredLength = desiredLength;
        }

        private void SetupDiapason(Clef clef, Modus modus)
        {
            var pf = new PitchFactory(modus, clef);
            Diapason = pf.Pitches;
            Lower = Diapason.Last();
            Higher = Diapason.First();
        }

        private void InitFirstNote()
        {
            _firstNoteFreqs = new FreqsDict();

            var pi = Diapason.FindAll(p => ((p.Degree == 4) || (p.Degree == 0)));

            foreach (var p in pi)
            {
                var k = (p.Degree == 0) ? 1 : 0.5;

                _firstNoteFreqs[new Note(p, Time, 4)] = 0.5 * k;
                _firstNoteFreqs[new Note(p, Time, 6)] = k;
                _firstNoteFreqs[new Note(p, Time, 8)] = k;
                _firstNoteFreqs[new Note(p, Time, 12)] = 0.7 *k;
            }
        }

        internal void FirstNote()
        {
            Freqs = new FreqsDict(_firstNoteFreqs);
        }

        internal void RemoveLast(bool ban = true)
        {
            var n = NotesList.Last();
            Time -= n.Duration;
            NotesList.RemoveAt(NotesList.Count - 1);
            
            if (NotesList.Count > 0)
            {
                if (ban)
                    NotesList.Last().Ban(n);

                Freqs = NotesList.Last().Freqs;
            }
            else
            {
                _firstNoteFreqs[n] = 0;
                FirstNote();
            }

            UpdateLeapsSmooth(true);
            UpdateUncomp();
        }


        public override void AddVariants(bool dumpResult = false)
        {
            if (Notes.Count == 0)
                FirstNote();

            else
            {
                if (Notes.Last().Diapason == null)
                    Notes.Last().Diapason = Diapason;

                Freqs = Notes.Last().Filter(dumpResult);
            }
        }

        private void UpdateFreqs()
        {
            if (NotesList.Count > 0)
                NotesList.Last().UpdateFreqs(Freqs);
            else
                _firstNoteFreqs = Freqs;
        }

        private void UpdateStrenghts()
        {
            if (NotesList.Count < 3)
            {
                NotesList[0].Strength = 2;
                return;
            }

            int currNumber = 1;

            var prev = NotesList[0];
            var curr = NotesList[1];
            var next = NotesList[2];
            while (currNumber < NotesList.Count - 1)
            {
                curr.Strength = CalculateStrength(prev, curr, next);

                currNumber++;
                prev = curr;
                curr = next;
                next = NotesList[currNumber];
            }

            NotesList[currNumber].Strength = CalculateStrength(curr, next, null);
        }

        private static double AdjustStrength(double strength, double adjust)
        {
            return strength * adjust;

            /*if (adjust > strength)
                return adjust;

            else
                return strength + (strength - adjust) / 8;*/
        }

        internal double GetStrengthIf(Note next)
        {
            return CalculateStrength(NotesList[NotesList.Count - 2], NotesList[NotesList.Count - 1], next);
        }

        private static double RhytmicStopStrength(int prevDur, int currDur)
        {
            double k = (currDur - prevDur) / (double)prevDur;
            return (k > 1.5) ? k * 0.8 : 1;

        }

        private double CalculateStrength(Note prev, Note curr, Note next)
        {
            const double basicStrength = 1;
            const double changeDirectionAdjust = 1.5;
            const double leapAdjustCoefficent = 0.02;
            const double offBeatAdjust = 0.7;

            double strength = basicStrength;// (curr.Duration >= 8) ? 1 : 0.5;

            if (next != null)
            {
                if (curr.Leap.Upwards ^ next.Leap.Upwards)
                    strength = AdjustStrength(strength, changeDirectionAdjust);

                if (next.Leap.AbsDeg > 1)
                    strength = AdjustStrength(strength, changeDirectionAdjust + leapAdjustCoefficent * next.Leap.AbsDeg);
            }

            /* if (curr.Duration == prev.Duration)
            {
                Strength *= 0.8;
                if ((next != null) && (next.Duration == curr.Duration))
                    Strength *= 0.65;
            }

            */
            if (curr.Leap.AbsDeg > 1)
                strength = AdjustStrength(strength, changeDirectionAdjust + leapAdjustCoefficent * curr.Leap.AbsDeg);

            if ((curr.TimeStart.Beats % 4) != 0)
                strength *= offBeatAdjust;

            if (prev.Duration < curr.Duration)
                strength *= RhytmicStopStrength(prev.Duration, curr.Duration);

            /*double durCoeff = (double)curr.Duration / (double)prev.Duration;
            if (durCoeff < 1)
                durCoeff = 1 / durCoeff;

            if (durCoeff > 2)
                Strength *= (1 + durCoeff / 6);*/

            return strength;
        }

        public void AddNote(Note n)
        {
            UpdateFreqs();
            NotesList.Add(n);            
            Filtered = false;
            Time += n.Duration;

            UpdateStrenghts();
            UpdateLeapsSmooth();
            UpdateUncomp();
        }

        private bool CoSign(int a, int b)
        {
            return (Math.Sign(a) == Math.Sign(b));
        }


        private void UpdateUncomp()
        {
            Reserve = 0;
            Uncomp = 0;

            foreach (var ls in LeapSmooth)
            {
                int deg = ls.Interval.Degrees;

                if (ls.IsSmooth)
                    UpdateUncompIfSmooth(deg);
                else
                    UpdateUncompIfLeap(deg);

            }

            if (Notes.Count > 0)
            {
                Notes.Last().Uncomp = Uncomp;
                Notes.Last().Reserve = Reserve;
            }

        }

        private void UpdateUncompIfLeap(int deg)
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

        private void UpdateUncompIfSmooth(int deg)
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

        private void UpdateLeapsSmooth(bool delete = false)
        {
            if ((NotesList.Count < 2) && (!delete))
                return;

            LeapOrSmooth ls;


            if (delete)
            {
                if (Leapsmooth.Count == 0)
                    return;

                if (Leapsmooth.Last().NotesCount == 2)
                {
                    Leapsmooth.RemoveAt(Leapsmooth.Count - 1);
                    return;
                }

                Leapsmooth.Last().Delete();

                return;
            }

            if (NotesList.Count == 2)
            {
                ls = new LeapOrSmooth(NotesList[0], NotesList[1]);
                Leapsmooth.Add(ls);
                return;
            }

            ls = Leapsmooth.Last();
            var lastNote = NotesList.Last();

            if (ls.CanAdd(lastNote))
                ls.Add(lastNote);
            else
            {
                ls = new LeapOrSmooth(NotesList[NotesList.Count - 2], lastNote);
                Leapsmooth.Add(ls);
            }
        }

        public Note this[int i] { get { return NotesList[i]; } }

        public int NoteCount { get { return NotesList.Count; } }

        public bool EndsWith(Note n)
        {
            return NotesList.Count != 0 && NotesList.Last() == n;
        }

        IEnumerator<Note> IEnumerable<Note>.GetEnumerator()
        {
            return Notes.GetEnumerator();
        }

        IEnumerator<KeyValuePair<int, Pitch>> IEnumerable<KeyValuePair<int, Pitch>>.GetEnumerator()
        {
            return new SubNoteIterator(this);
        }

        public IEnumerable GetLast(int count)
        {
            if (count > NoteCount)
                throw new IndexOutOfRangeException();

            var iter = new SubNoteIterator(this, NoteCount - count);
            iter.Reset();
            do
                yield return iter.Current;
            while (iter.MoveNext());
        }
        
        
        /*public IEnumerator GetEnumerator()
        {
            return GetEnumerator();
        }*/
        

        class SubNoteIterator : IEnumerator<KeyValuePair<int, Pitch>>
        {
            private readonly Voice _voice;

            private int _noteNumber;
            private int _subPosition;
            private int _position;

            private readonly int _minNoteNumber;
            private readonly int _maxNoteNumber;

            public SubNoteIterator(Voice voice)
                : this(voice, 0, voice.NoteCount) { }

            public SubNoteIterator(Voice voice, int start)
                : this(voice, start, voice.NoteCount) { }

            private SubNoteIterator(Voice voice, int start, int end)
            {
                _voice = voice;
                _minNoteNumber = start;
                _maxNoteNumber = end;

                Reset();
            }
            
            public void Reset()
            {
                _noteNumber = _minNoteNumber;
                _subPosition = -1;
                _position = (_maxNoteNumber > _minNoteNumber) ? _voice[_minNoteNumber].TimeStart.Position : 0;
            }

            public bool MoveNext()
            {

                _subPosition++;
                _position++;
                if (_subPosition < _voice[_noteNumber].Duration)
                    return true;

                _noteNumber++;
                _subPosition = 0;

                return _noteNumber < _voice.NoteCount;
            }

            public void Dispose() { }

            object IEnumerator.Current { get { return Current; } }

            public KeyValuePair<int, Pitch> Current { get { return new KeyValuePair<int, Pitch>(_position, _voice[_noteNumber].Pitch); } }
        }

        internal bool Finished()
        {
            return (Time.Position == DesiredLength);
        }

        public IEnumerator GetEnumerator()
        {
            return NotesList.GetEnumerator();
        }
    }
}
