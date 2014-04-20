using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Compositor.Generators;
using Compositor.Rules.Base;
using Compositor.Rules.Melody;
using PitchBase;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("RulesTest")]
namespace Compositor.Levels
{
    using NotesList = List<Note>;

    public enum VoiceType
    {
        Single,
        Top,
        Middle,
        Bass
    }

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
    [Rule(typeof(DenyRestsRule))]

    public class Voice : RuledLevel, IEnumerable<Note>, IEnumerable<KeyValuePair<int, Pitch>>
    {
        public Clef Clef { get; private set; }
        public Modus Modus { get; private set; }
        public Time Time { get; private set; }
        public VoiceType Type { get; private set; }

        internal NotesList NotesList;
        internal LeapSmoothList LeapSmooths;
        internal List<Pitch> Diapason { get; private set; }

        FreqsDict _firstNoteFreqs;

        internal uint DesiredLength;

        public NotesList Notes { get { return new NotesList(NotesList); } }
        public LeapSmoothList LeapSmooth { get { return new LeapSmoothList(LeapSmooths); } }

        internal Pitch Higher;
        internal Pitch Lower;

        private ImitationSettings _imitationSettings;
        private Voice _imitationSource;
        private Note _startPause;

        public Voice(Clef clef, Modus modus, Time time, VoiceType type)
        {
            Clef = clef;
            Modus = modus;
            Time = time;
            Type = type;

            NotesList = new NotesList();
            LeapSmooths = new LeapSmoothList();

            time.Position = 0;
            SetupDiapason(clef, modus);
            InitFirstNoteFreqs();
            SetFreqsToFirst();
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

        private void InitFirstNoteFreqs()
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

        internal void SetFreqsToFirst()
        {
            Freqs = new FreqsDict(_firstNoteFreqs);
        }

        public void SetMirroring(ImitationSettings settings, Voice sourceVoice)
        {
            if (settings == null)
            {
                InitFirstNoteFreqs();
                SetFreqsToFirst();
                return;
            }

            _imitationSettings = settings;
            _imitationSource = sourceVoice;
            _firstNoteFreqs = new FreqsDict();
            _startPause = new Note(null, Time, settings.Delay);
            _firstNoteFreqs[_startPause] = 1;

            SetFreqsToFirst();
        }

        internal void RemoveLast(bool ban = true)
        {
            var n = NotesList.Last();
            Time -= n.Duration;
            NotesList.RemoveAt(NotesList.Count - 1);

            if (_imitationSettings != null)
                //if (Time.Position <= _imitationSettings.Range)
                {
                    Filtered = false;
                    if (NoteCount > 0)
                        NotesList.Last().Filtered = false;
                }

            if (NotesList.Count > 0)
            {
                if (ban)
                    NotesList.Last().Ban(n);

                Freqs = NotesList.Last().Freqs;
            }
            else
            {
                _firstNoteFreqs[n] = 0;
                SetFreqsToFirst();
            }

            LeapSmooths.DeleteLast();
            UpdateLastNoteIndices();
        }


        public override void AddVariants()
        {
            if (NoteCount == 0)
                SetFreqsToFirst();
            else
            {
                var lastNote = Notes.Last();

                if (lastNote.Diapason == null)
                    lastNote.Diapason = Diapason;

                Freqs = lastNote.Filter();

                if (_imitationSettings == null || Time.Position >= _imitationSettings.Range)
                    return;

                //TODO: it must be RULE!
                var sourceNote = _imitationSource[NoteCount - 1];

                foreach (var key in Freqs.Keys.ToList())
                {
                    var note = key as Note;
                    if (note == null)
                        continue;

                    var interval = note.Pitch - sourceNote.Pitch;
                    if (interval.AbsDeg == _imitationSettings.Interval.AbsDeg && note.Duration == sourceNote.Duration)
                        continue;

                    Freqs[key] = 0;
                    key.IsBanned = true;
                }
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

            var currNumber = 1;

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

        private static double CalculateStrength(Note prev, Note curr, Note next)
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
        }


        private void UpdateLeapsSmooth()
        {
            if (NotesList.Count < 2)
                return;

            LeapSmooths.Add(NotesList[NotesList.Count - 2], NotesList.Last());
            UpdateLastNoteIndices();
        }

        private void UpdateLastNoteIndices()
        {
            if (Notes.Count <= 0) return;

            Notes.Last().Uncomp = LeapSmooths.Uncomp;
            Notes.Last().Reserve = LeapSmooths.Reserve;
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
