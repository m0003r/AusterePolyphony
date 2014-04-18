﻿using System;
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
            _gammingSoftLimit = 6;//(int)Settings["SoftLimit"];
            _gammingHardLimit = 8;//(int)Settings["HardLimit"];
        }

        public override bool _IsApplicable()
        {
            _gammingCount = Notes.Reverse<Note>().TakeWhile(n => n.Leap.IsSmooth).Count();

            return (_gammingCount > _gammingSoftLimit);
        }

        public override double Apply(Note nextNote)
        {
            return (_gammingHardLimit - Math.Abs(_gammingCount - nextNote.Leap.AbsDeg)) / (double)_gammingHardLimit;
        }
    }

    class CadenzaRule : MelodyRule
    {
        private int _lastBarStart;

        protected override bool ApplyToRests { get { return true; } }

        public override void Init(Voice parent)
        {
            base.Init(parent);
            _lastBarStart = (int)Voice.DesiredLength - Time.Beats * 4;
        }

        public override bool _IsApplicable()
        {
            return (Time.Position >= _lastBarStart - Time.Beats * 4); //если мы не в предпоследнем такте
        }

        public override double Apply(Note nextNote)
        {
            if (nextNote.TimeEnd.Position < _lastBarStart)
                return 1;

            if (nextNote.Pitch == null)
                return 0;

            if (nextNote.TimeEnd.Position == _lastBarStart)
                return (nextNote.Pitch.Degree == 1 || nextNote.Pitch.Degree == 6) ? 1 : 0;

            if (nextNote.TimeStart.Position == _lastBarStart)
                //нельзя ничего в последнем такте кроме долгой тоники
                return (nextNote.Duration == Time.Beats * 4 &&
                        nextNote.Pitch.Degree == 0 &&
                        nextNote.Leap.AbsDeg == 1)
                        ? 1 : 0;  //достигаемой плавным ходом 
                    

            return nextNote.TimeEnd.Position > _lastBarStart ? 0 : 1;
        }
    }

    class BreveRule : MelodyRule
    {
        private int _lastBar;

        public override void Init(Voice parent)
        {
            base.Init(parent);
            _lastBar = (int)Voice.DesiredLength - Time.Beats * 4;
        }

        public override bool _IsApplicable()
        {
            return (Time.Position < _lastBar); //если мы не в предпоследнем такте
        }

        public override double Apply(Note nextNote)
        {
            return (nextNote.Duration == 16) ? 0 : 1;
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

        public override double Apply(Note nextNote)
        {
            return (nextNote.Pitch == _last[1].Pitch) ? 0 : 1;
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

        public override double Apply(Note nextNote)
        {
            if (nextNote.Duration > 3)
                return 0;

            if (nextNote.Duration == 8)
                return (nextNote.TimeStart.Beat == 4) ? 0 : 1;

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
            if (_lastPitch == null)
                return false;
            //запрещаем вниз, если мы на текущей верхней мелодической вершине            
            _denyDown = (_lastPitch == Higher);
            // или если нижняя — звук тритона, а мы на другом
            _denyDown |= ((_lastPitch.IsTritone) && (Lower.IsTritone) && (_lastPitch != Lower));

            //и аналогично вверх
            _denyUp = (_lastPitch == Lower);
            _denyUp |= ((_lastPitch.IsTritone) && (Higher.IsTritone) && (_lastPitch != Higher));

            return true; //мы всегда можем запретить стоять на прошлой вершине долго
        }

        public override double Apply(Note nextNote)
        {
            if (_denyDown && (nextNote.Pitch < _lastPitch))
                return 0;
            if (_denyUp && (nextNote.Pitch > _lastPitch))
                return 0;
            if ((nextNote.Pitch != Lower) && (nextNote.Pitch != Higher)) return 1;

            var k = (nextNote.Duration > 4) ? Math.Max(0.2, 1 - (Math.Log(nextNote.Duration, 2) - 2) * 0.5) : 1;
            if (nextNote.TimeStart.Beat % 8 == 0)
                k *= 0.7;

            return k;
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

        public override double Apply(Note nextNote)
        {
            return _last.CanAdd(nextNote) ? 1 : 0;
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

        public override double Apply(Note nextNote)
        {
            if (nextNote.Duration > 2)
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

        public override double Apply(Note nextNote)
        {
            if (nextNote.Duration == 2)
                return 2; //неслыханная рекомендация!

            return (nextNote.Duration == 6) ? 0.5 : 0.7;
        }
    }

    class DenyTwoNoteSequence : MelodyRule
    {
        Pitch _deniedPitch;
        int _deniedDuration;

        public override bool _IsApplicable()
        {
            _deniedPitch = null;

            if (Notes.Count < 3)
                return false;

            var last = GetLast(3);

            int checkedInterval = (last[1].Pitch - last[0].Pitch).Degrees;
            if ((last[2].Pitch - last[1].Pitch).Degrees != checkedInterval)
            {
                _deniedPitch = last[2].Pitch + checkedInterval;
                _deniedDuration = last[1].Duration;
                return true;
            }

            return false;
        }

        public override double Apply(Note nextNote)
        {
            double frq = 1;

            if (nextNote.Duration == _deniedDuration)
                frq *= 0.5;

            if (_deniedPitch == nextNote.Pitch)
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

            var last = GetLast(3);

            if ((last[0].Duration) != (last[2].Duration)) return false;

            _deniedDuration = last[1].Duration;
            _patternDuration = last[0].Duration + _deniedDuration;

            return true;
        }

        public override double Apply(Note nextNote)
        {
            if (nextNote.Duration == _deniedDuration)
                return ((_patternDuration % 8) == 0) ? 0.1 : 0.9;

            return 1;
        }
    }

    //TODO: seed = -967466835, -450168169

    // 1245604329
    class DenyStrongNotesRepeat : MelodyRule
    {
        const double MinimumStrengthForApply = 1.5;

        public override bool _IsApplicable()
        {
            return Notes.Count >= 3;
        }

        private IEnumerable<Note> FindLastEquiv(Note n)
        {
            return Notes.Where(found => (
                found.Pitch != null &&
                found.Pitch.Degree == n.Pitch.Degree && 
                (n.TimeStart - found.TimeStart).Bar <= 3 &&
                (n.TimeStart - found.TimeStart).Position > 6 &&
                found.Strength >= MinimumStrengthForApply
              ));
        }

        public override double Apply(Note nextNote)
        {
            var lastStrength = Voice.GetStrengthIf(nextNote);
            if (lastStrength < MinimumStrengthForApply)
                return 1;

            double freq = 1;
            foreach (var f in FindLastEquiv(nextNote))
            {
                var distance = (nextNote.TimeStart - f.TimeStart).Position;
                if (f.TimeStart.Beat == nextNote.TimeStart.Beat)
                    freq *= 0.7;
                if (f.Strength + lastStrength > MinimumStrengthForApply + distance / 8.0)
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

        public override double Apply(Note nextNote)
        {
            const double safeDistance = 4 * 16;

            if (nextNote.Duration != 1)
                return 1;

            var distance = nextNote.TimeStart - _lastEightPosition;

            return distance.Beats > safeDistance ? 1 : Math.Pow(distance.Beats/safeDistance, 3);
        }
    }

    class TritoneRule : MelodyRule
    {
        const double MinimumStrengthForApply = 1.1;

        IEnumerable<Note> _lastTritones;

        public override bool _IsApplicable()
        {
            if (Notes.Count <= 1) return false;

            _lastTritones = FindLastTritone();
            return true;
        }

        private IEnumerable<Note> FindLastTritone()
        {
            /*if (NotesList.Count == 6)
            {
                double f = 1;
            }*/

            return Notes.Where(found => (
                found.Pitch != null &&
                found.Pitch.IsTritone &&
                //(f.Pitch.isTritoneLow ^ n.Pitch.isTritoneLow) &&
                ((LastNote.TimeEnd - found.TimeStart).Bar <= 3) &&
                ((LastNote.TimeEnd - found.TimeStart).Position > 6)
                // (f.Strength >= minimumRequiredForApply)
              ));
        }

        public override double Apply(Note nextNote)
        {
            /*if (!LastNote.Pitch.IsTritone)
                return 1;*/

            var lastStrength = Voice.GetStrengthIf(nextNote);
            if (lastStrength < MinimumStrengthForApply)
                return 1;

            double freq = 1;
            foreach (var f in _lastTritones)
            {
                var distance = (nextNote.TimeStart - f.TimeStart).Position;
                if (f.TimeStart.Beat == nextNote.TimeStart.Beat)
                    freq *= 0.7;
                if (f.Strength + lastStrength > MinimumStrengthForApply + distance / 16.0)
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

        public override double Apply(Note nextNote)
        {
            return LeapSmooth.Last().CanAdd(nextNote) ? 1 : 0;
        }
    }

    class AfterQuarterNewRule : MelodyRule
    {
        bool _allowLeap;

        public override bool _IsApplicable()
        {
            if (LastNote.Duration != 2)
                return false;

            if (Voice.NoteCount == 1)
                _allowLeap = false;
            else
            {
                var prelast = Notes[Voice.NoteCount - 2];
                _allowLeap = ((prelast.Duration == 6) && (LastNote.Leap.Degrees == -1));
            }

            return true;
        }

        public override double Apply(Note nextNote)
        {
            return (_allowLeap || (nextNote.Leap.IsSmooth)) ? 1 : 0;
        }
    }

    class DenyRestsRule : MelodyRule
    {
        protected override bool ApplyToRests
        {
            get { return true; }
        }

        public override bool _IsApplicable()
        {
            return true; //Voice.Type == VoiceType.Single;
        }

        public override double Apply(Note nextNote)
        {
            return nextNote.Pitch == null ? 0 : 1;
        }
    }
}

