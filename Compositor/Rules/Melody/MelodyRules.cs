using System;
using System.Collections.Generic;
using System.Linq;
using Compositor.Levels;
using Compositor.Rules.Base;
using PitchBase;

namespace Compositor.Rules.Melody
{
    [Param("SoftLimit", typeof(int), 6, "Беспроблемное значение")]
    [Param("HardLimit", typeof(int), 8, "Нельзя никогда так")]
    class DenyGamming : MelodyRule
    {
        private int _gammingCount;
        private readonly int _gammingSoftLimit;
        private readonly int _gammingHardLimit;

        public DenyGamming()
        {
            _gammingSoftLimit = (int)Settings["SoftLimit"];
            _gammingHardLimit = (int)Settings["HardLimit"];
        }

        public override bool _IsApplicable()
        {
            _gammingCount = Notes.Reverse<Note>().TakeWhile(n => n.Leap.IsSmooth).Count();

            return (_gammingCount > _gammingSoftLimit);
        }

        public override double Apply(Note nextNotes)
        {
            return (_gammingHardLimit - Math.Abs(_gammingCount - nextNotes.Leap.AbsDeg)) / (double)_gammingHardLimit;
        }
    }

    class CadenzaRule : MelodyRule
    {
        private int _lastBarStart;

        public override void Init(Levels.Melody parent)
        {
            base.Init(parent);
            _lastBarStart = (int)Melody.DesiredLength - Time.Beats * 4;
        }

        public override bool _IsApplicable()
        {
            return (Time.Position >= _lastBarStart - Time.Beats * 4); //если мы не в предпоследнем такте
        }

        public override double Apply(Note nextNotes)
        {
            if (nextNotes.TimeEnd.Position < _lastBarStart)
                return 1;
            if (nextNotes.TimeEnd.Position == _lastBarStart)
            {
                return ((nextNotes.Pitch.Degree == 1) || (nextNotes.Pitch.Degree == 6)) ? 1 : 0;
                    
            }
            if (nextNotes.TimeStart.Position == _lastBarStart)
                //нельзя ничего в последнем такте кроме долгой тоники
                return ((nextNotes.Duration == Time.Beats * 4) && (nextNotes.Pitch.Degree == 0) &&
                          (nextNotes.Leap.AbsDeg == 1)) ? 1 : 0;  //достигаемой плавным ходом 
                    

            if (nextNotes.TimeEnd.Position > _lastBarStart) //нельзя залиговку к последнему такту
                return 0;
            return 1;
        }
    }

    class BreveRule : MelodyRule
    {
        private int _lastBar;

        public override void Init(Levels.Melody parent)
        {
            base.Init(parent);
            _lastBar = (int)Melody.DesiredLength - Time.Beats * 4;
        }

        public override bool _IsApplicable()
        {
            return (Time.Position < _lastBar); //если мы не в предпоследнем такте
        }

        public override double Apply(Note nextNotes)
        {
            return (nextNotes.Duration == 16) ? 0 : 1;
        }
    }
    /*********************************************************************/

    class TrillRule : MelodyRule
    {
        private List<Note> _last;

        public override bool _IsApplicable()
        {
            if (Notes.Count < 3)
                return false;

            _last = GetLast(3);

            return (_last[0].Pitch == _last[2].Pitch);
        }

        public override double Apply(Note nextNotes)
        {
            return (nextNotes.Pitch == _last[1].Pitch) ? 0 : 1;
        }
    }

    /*********************************************************************/

    //запрещаем писать половину после нечетверти и двух четвертей с сильной доли
    // -1329812680 !!!
    class DenyPolka : MelodyRule
    {
        private List<Note> _last;

        public override bool _IsApplicable()
        {
            if (Notes.Count == 2)
            {
                _last = GetLast(2);
                _last.Insert(0, new Note(_last[1].Pitch, _last[1].TimeStart, 4));
            }
            else if (Notes.Count > 2)
            {
                _last = GetLast(3);
            }
            else
                return false;

            return ((_last[1].TimeStart.StrongTime) &&
                (_last[1].Duration == 2) &&
                (_last[2].Duration == 2) &&
                (_last[0].Duration != 2));
        }

        public override double Apply(Note nextNotes)
        {
            if (nextNotes.Duration > 3)
                return 0;

            if (nextNotes.Duration == 8)
                return (nextNotes.TimeStart.Beat == 4) ? 0 : 1;

            return 1;
        }
    }

    /*********************************************************************/

    /*
     * Мелодическое обнаружение тритона:
     * запрещает брать звук тритона на долгим, если 
     * другой звук тритона был мелодической вершиной
     */

    /*class TritoneRule : MelodyRule
    {
        public override bool _IsApplicable()
        {
            return (Lower.isTritone || Higher.isTritone);
        }

        public override double Apply(Note n)
        {
            if (!n.Pitch.isTritone)
                return 1;

            double koeff = (1 - n.Duration / 16.0);
            return n.TimeStart.strongTime ? koeff / 2 : koeff;
        }
    }*/

    /*********************************************************************/

    class PeakRule : MelodyRule
    {
        private bool _denyUp;
        private bool _denyDown;
        Pitch _lastPitch;

        public override bool _IsApplicable()
        {
            _lastPitch = LastNote.Pitch;

            //запрещаем вниз, если мы на текущей верхней мелодической вершине            
            _denyDown = (_lastPitch == Higher);
            // или если нижняя — звук тритона, а мы на другом
            _denyDown |= ((_lastPitch.IsTritone) && (Lower.IsTritone) && (_lastPitch != Lower));

            //и аналогично вверх
            _denyUp = (_lastPitch == Lower);
            _denyUp |= ((_lastPitch.IsTritone) && (Higher.IsTritone) && (_lastPitch != Higher));

            return true; //мы всегда можем запретить стоять на прошлой вершине долго
        }

        public override double Apply(Note nextNotes)
        {
            if (_denyDown && (nextNotes.Pitch < _lastPitch))
                return 0;
            if (_denyUp && (nextNotes.Pitch > _lastPitch))
                return 0;
            if ((nextNotes.Pitch == Lower) || (nextNotes.Pitch == Higher))
            {
                double k = (nextNotes.Duration > 4) ? Math.Max(0.2, 1 - (Math.Log(nextNotes.Duration, 2) - 2) * 0.5) : 1;
                if (nextNotes.TimeStart.Beat % 8 == 0)
                    k *= 0.7;
                return k;
            }
            return 1;
        }
    }

    //474605227
    class PeakRule2 : MelodyRule
    {
        LeapOrSmooth _last;

        public override bool _IsApplicable()
        {
            if (LeapSmooth.Count < 2)
                return false;

            _last = LeapSmooth.Last();

            return (LeapSmooth[LeapSmooth.Count - 2].Interval.AbsDeg == _last.Interval.AbsDeg);
        }

        public override double Apply(Note nextNotes)
        {
            return _last.CanAdd(nextNotes) ? 1 : 0;
        }

    }

    class ManyQuartersRule : MelodyRule
    {
        int _quarterCount;
        int _lastTime = -1;

        public override bool _IsApplicable()
        {
            if (_lastTime != LastNote.TimeStart.Position)
            {
                _lastTime = LastNote.TimeStart.Position;
                if (LastNote.Duration > 2)
                    _quarterCount = 0;
                else
                    _quarterCount++;
            }

            return (_quarterCount >= 6);
        }

        public override double Apply(Note nextNotes)
        {
            if (nextNotes.Duration > 2)
                return 1;

            switch (_quarterCount)
            {
                case 6: return 0.4;
                case 7: return 0.1;
                default: return 0;
            }
        }
    }

    //хорошо бы после половинки с точкой и четверти продолжать движение четвертями
    class DottedHalveRestrictionRule : MelodyRule
    {
        public override bool _IsApplicable()
        {
            if (Notes.Count < 2)
                return false;

            List<Note> last = GetLast(2);
            return ((last[0].Duration == 6) && (last[1].Duration == 2));
        }

        public override double Apply(Note nextNotes)
        {
            if (nextNotes.Duration == 2)
                return 2; //неслыханная рекомендация!

            return (nextNotes.Duration == 6) ? 0.5 : 0.7;
        }
    }

    class DenyTwoNoteSequence : MelodyRule
    {
        Pitch _deniedPitch;
        int _deniedDuration;

        public override bool _IsApplicable()
        {
            int checkedInterval;

            _deniedPitch = null;

            if (Notes.Count < 3)
                return false;

            var last = GetLast(3);

            checkedInterval = (last[1].Pitch - last[0].Pitch).Degrees;
            if ((last[2].Pitch - last[1].Pitch).Degrees != checkedInterval)
            {
                _deniedPitch = last[2].Pitch + checkedInterval;
                _deniedDuration = last[1].Duration;
                return true;
            }

            return false;
        }

        public override double Apply(Note nextNotes)
        {
            double frq = 1;

            if (nextNotes.Duration == _deniedDuration)
                frq *= 0.5;

            if (_deniedPitch == nextNotes.Pitch)
                frq *= 0.1;

            return frq;
        }
    }

    class DenyTwoNoteRhytmicSequence : MelodyRule
    {
        int _deniedDuration;
        int _patternDuration;

        public override bool _IsApplicable()
        {
            if (Notes.Count < 3)
                return false;

            List<Note> last = GetLast(3);

            if ((last[0].Duration) == (last[2].Duration))
            {
                _deniedDuration = last[1].Duration;
                _patternDuration = last[0].Duration + _deniedDuration;

                return true;
            }

            return false;
        }

        public override double Apply(Note nextNotes)
        {
            if (nextNotes.Duration == _deniedDuration)
                return ((_patternDuration % 8) == 0) ? 0.1 : 0.9;

            return 1;
        }
    }

    //TODO: seed = -967466835, -450168169

    class DenySequence : MelodyRule
    {
        class SequencePattern
        {
            private List<int> Tones { get { return _tones.GetRange(_startedAt, Length); } }
            public int Length { get; private set; }

            private readonly List<int> _tones;

            readonly int _startedAt;
            readonly int _previousStartedAt;

            public SequencePattern(Levels.Melody m, int notes)
            {
                if (notes > m.NoteCount - 3)
                    throw new IndexOutOfRangeException();

                _tones = new List<int>();

                int melodyLength = m.Time.Position;
                _startedAt = m[m.NoteCount - notes].TimeStart.Position;
                _previousStartedAt = m[m.NoteCount - notes - 1].TimeStart.Position;
                int baseShift = melodyLength - _previousStartedAt;

                if (_startedAt < baseShift)
                    throw new IndexOutOfRangeException();

                Length = 0;
                foreach (var subNote in (IEnumerable<KeyValuePair<int, Pitch>>)m)
                {
                    _tones.Add(subNote.Value.Value);
                    if (subNote.Key > _startedAt)
                        Length++;
                }
            }

            /*public void Extend()
            {
                startedAt -= ShiftStep;
                Length += ShiftStep;
            }*/

            private double Difference()
            {
                int comparedLength = 0;
                double average, diffAccumulator = 0;
                var diffs = new List<int>();

                var myEnum = Tones.GetEnumerator();
                var otherEnum = _tones.GetRange(_previousStartedAt - Length, Length).GetEnumerator();

                while (myEnum.MoveNext() && otherEnum.MoveNext())
                {
                    diffs.Add(Math.Abs(myEnum.Current - otherEnum.Current));
                    comparedLength++;
                } 

                average = diffs.Average();

                diffAccumulator += diffs.Sum(diff => Math.Pow((diff - average), 2));

                double rawSpread = Math.Sqrt(diffAccumulator / comparedLength);
                double spread = rawSpread;

                if (average > 5) //это уже линеарная имитация...
                    spread *= 1.5;

                return spread;
            }

            public Dictionary<int, double> UndesiredNotes()
            {
                var result = new Dictionary<int, double>();

                /*for (int shift = 1; baseShift + shift < startedAt - 1; shift++)
                {*/
                    int nextPitch = _tones[_startedAt - 1];
                    result[nextPitch] = Difference();
                //}

                return result;
            }
        }

        Dictionary<int, double> _undesiredPitches;

        const int MaxSequenceLength = 24;

        public override bool _IsApplicable()
        {
            if (Melody.NoteCount < 5)
                return false;

            int currGrabbedNotes = 2;
            bool tooLong = false;

            _undesiredPitches = new Dictionary<int,double>();

            do
            {
                try
                {
                    var pattern = new SequencePattern(Melody, currGrabbedNotes);
                    if (pattern.Length > MaxSequenceLength)
                        tooLong = true;
                    else
                        foreach (KeyValuePair<int, double> kv in pattern.UndesiredNotes())
                            if (_undesiredPitches.ContainsKey(kv.Key))
                                _undesiredPitches[kv.Key] = Math.Min(_undesiredPitches[kv.Key], kv.Value);
                            else
                                _undesiredPitches[kv.Key] = kv.Value;
                    currGrabbedNotes++;
                }
                catch (IndexOutOfRangeException)
                {
                    tooLong = true;
                }
            }
            while (!tooLong);

            return (_undesiredPitches.Count > 0);
        }



        public override double Apply(Note nextNotes)
        {
            int tone = nextNotes.Pitch.Value;

            if (!_undesiredPitches.ContainsKey(tone)) return 1;

            var calculated = _undesiredPitches[tone];
            return Math.Min(1, calculated);
        }
    }

    // 1245604329
    class DenyStrongNotesRepeat : MelodyRule
    {
        const double MinimumRequiredForApply = 1.5;

        public override bool _IsApplicable()
        {
            if (Notes.Count < 3)
                return false;

            return true;
        }

        private IEnumerable<Note> FindLastEquiv(Note n)
        {
            return Notes.Where(f => (
                (f.Pitch.Degree == n.Pitch.Degree) && 
                ((n.TimeStart - f.TimeStart).Bar <= 3) &&
                ((n.TimeStart - f.TimeStart).Position > 6) &&
                (f.Strength >= MinimumRequiredForApply)
              ));
        }

        public override double Apply(Note nextNotes)
        {
            double lastStrength = Melody.GetStrengthIf(nextNotes);
            if (lastStrength < MinimumRequiredForApply)
                return 1;

            double freq = 1;
            foreach (Note f in FindLastEquiv(nextNotes))
            {
                int distance = (nextNotes.TimeStart - f.TimeStart).Position;
                if (f.TimeStart.Beat == nextNotes.TimeStart.Beat)
                    freq *= 0.7;
                if (f.Strength + lastStrength > MinimumRequiredForApply + distance / 8.0)
                    freq *= 0.2;
            }
            
            return freq;
        }
    }

    class TooManyEightsRule : MelodyRule
    {
        private Time _lastEightPosition;

        public override bool _IsApplicable()
        {
            var lastEight = Notes.FindLast(n => n.Duration == 1);
            if (lastEight == null) return false;

            _lastEightPosition = lastEight.TimeStart;
            return true;
        }

        public override double Apply(Note nextNotes)
        {
            const double safeDistance = 4 * 16;

            if (nextNotes.Duration != 1)
                return 1;

            var distance = nextNotes.TimeStart - _lastEightPosition;

            return distance.Beats > safeDistance ? 1 : Math.Pow(distance.Beats/safeDistance, 3);
        }
    }

    class TritoneRule : MelodyRule
    {
        const double MinimumRequiredForApply = 1.1;

        IEnumerable<Note> _lastTritones;

        public override bool _IsApplicable()
        {
            if (Notes.Count > 1)
            {
                _lastTritones = FindLastTritone();
                return true;
            }
            return false;
        }

        private IEnumerable<Note> FindLastTritone()
        {
            /*if (Notes.Count == 6)
            {
                double f = 1;
            }*/

            return Notes.Where(f => (
                f.Pitch.IsTritone &&
                //(f.Pitch.isTritoneLow ^ n.Pitch.isTritoneLow) &&
                ((LastNote.TimeEnd - f.TimeStart).Bar <= 3) &&
                ((LastNote.TimeEnd - f.TimeStart).Position > 6)
                // (f.Strength >= minimumRequiredForApply)
              ));
        }

        public override double Apply(Note nextNotes)
        {
            if (!LastNote.Pitch.IsTritone)
                return 1;

            double lastStrength = Melody.GetStrengthIf(nextNotes);
            if (lastStrength < MinimumRequiredForApply)
                return 1;

            double freq = 1;
            foreach (Note f in _lastTritones)
            {
                int distance = (nextNotes.TimeStart - f.TimeStart).Position;
                if (f.TimeStart.Beat == nextNotes.TimeStart.Beat)
                    freq *= 0.7;
                if (f.Strength + lastStrength > MinimumRequiredForApply + distance / 16.0)
                    freq *= 0.2;
            }
            
            return freq;
        }
    }

    class TritoneRule2 : MelodyRule
    {
        public override bool _IsApplicable()
        {
            return LeapSmooth.Count >= 1 && LeapSmooth.Last().Interval.IsTritone;
        }

        public override double Apply(Note nextNotes)
        {
            return LeapSmooth.Last().CanAdd(nextNotes) ? 1 : 0;
        }
    }

    class AfterQuarterNewRule : MelodyRule
    {
        bool _allowLeap;

        public override bool _IsApplicable()
        {
            if (LastNote.Duration != 2)
                return false;

            if (Melody.NoteCount == 1)
                _allowLeap = false;
            else
            {
                var prelast = Notes[Melody.NoteCount - 2];
                _allowLeap = ((prelast.Duration == 6) && (LastNote.Leap.Degrees == -1));
            }

            return true;
        }

        public override double Apply(Note nextNotes)
        {
            return (_allowLeap || (nextNotes.Leap.IsSmooth)) ? 1 : 0;
        }
    }
}

