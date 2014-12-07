using System;
using System.Linq;
using Compositor.Levels;
using Compositor.Rules.Base;

namespace Compositor.Rules.Melody
{
/***
 * Улучшенные правила скачка:
 * 
 * Сумма заполнений должна быть >= 2/3 величины скачка
 * С каждой стороны должно быть минимум 1/4 величины скачка (для скачков больше квинты)
 */

    class GravityRule : MelodyRule
    {
        //double desiredCenter;
        double _gravityPoint;
        double _gravityForce;

        public override bool _IsApplicable()
        {
            if (LastNote.Pitch == null)
                return false;

            //double actualCenter = Voice.NotesList.Sum(n => n.Pitch.Value * n.Duration) / (double)Voice.Time.Position;
            //double diff = desiredCenter - actualCenter;
            _gravityPoint = LastNote.Pitch.Value + LastNote.Uncomp;
            _gravityForce = LastNote.Uncomp;

            return Math.Abs(_gravityForce) > 2;
        }

        public override double Apply(Note nextNote)
        {
            var distance = nextNote.Pitch.Value - _gravityPoint;
            var force = distance * _gravityForce;
            // Console.WriteLine(" >>>>> Gravity to {0}: {1}", n.Pitch.Value, force);
            if (force < 0)
                return 1;
            if (force > 40)
                return 0.1;
            return 1 - force / 45.0;
        }
    }

    class LeapCompensation : MelodyRule
    {

        public override bool _IsApplicable()
        {
            return (LeapSmooth.Count > 0);
        }

        const int UncompDenySmooth = 6;
        const int UncompDenyLeap = 4;
        const int ReserveLimit = 8;

        //255167827
        //1679803255
        public override double Apply(Note nextNote)
        {

            // ход в сторону нескомпенсированности
            if (LastNote.Uncomp != 0)
                if (Math.Sign(nextNote.Leap.Degrees) == Math.Sign(LastNote.Uncomp))
                {
                    if (Math.Abs(LastNote.Uncomp) >= UncompDenyLeap)
                        return nextNote.Leap.IsLeap ? 0 : 1;
                    if (Math.Abs(LastNote.Uncomp) >= UncompDenySmooth)
                        return 0;
                }

            // ход против запаса или если запас 0
            if (Math.Sign(LastNote.Reserve) == Math.Sign(nextNote.Leap.Degrees))
                return Math.Abs(nextNote.Leap.Degrees + LastNote.Reserve) < ReserveLimit ? 1 : 0;

            if (nextNote.Leap.Degrees < 5)
                return 1;

            if (nextNote.Leap.Degrees == 5)
                return 0.4;

            return Math.Abs(nextNote.Leap.Degrees) < Math.Abs(LastNote.Reserve * 4) ? 1 : 0;
        }
    }

    class AfterLeapRules : MelodyRule
    {
        private LeapAfterCoSmooth _coSmooth;
        private LeapAfterOppSmooth _oppSmooth;
        private LeapAfterCoLeap _coLeap;
        private LeapAfterOppLeap _oppLeap;

        protected Note PreLast;

        protected override bool ApplyToCadenza
        {
            get { return true; }
        }

        private bool _lastCo;
        private bool _lastSm;

        public override void Init(Voice parent)
        {
            base.Init(parent);

            _coSmooth = new LeapAfterCoSmooth();
            _coLeap = new LeapAfterCoLeap();
            _oppSmooth = new LeapAfterOppSmooth();
            _oppLeap = new LeapAfterOppLeap();

            _coSmooth.Init(Voice);
            _coLeap.Init(Voice);
            _oppSmooth.Init(Voice);
            _oppLeap.Init(Voice);
        }

        public override bool _IsApplicable()
        {
            if (LastNote.Leap.IsSmooth)
                return false;
            if (Notes.Count < 2)
                return false;

            PreLast = Notes[Notes.Count - 2];

            _lastCo = (PreLast.Leap.Upwards == LastNote.Leap.Upwards);
            _lastSm = PreLast.Leap.IsSmooth;

            return _lastCo
                ? (_lastSm ? _coSmooth.IsApplicable() : _coLeap.IsApplicable())
                : (_lastSm ? _oppSmooth.IsApplicable() : _oppLeap.IsApplicable());
        }

        public override double Apply(Note nextNote)
        {
            if (_lastCo)
                return _lastSm ? _coSmooth.Apply(nextNote) : _coLeap.Apply(nextNote);

            return _lastSm ? _oppSmooth.Apply(nextNote) : _oppLeap.Apply(nextNote);
        }
    }

    class LeapAfterCoSmooth : MelodyRule
    {
        public override bool _IsApplicable()
        {
            return true;
        }

        public override double Apply(Note nextNote)
        {
            //должно быть противонаправленно
            return (nextNote.Leap.Upwards ^ LastNote.Leap.Upwards) ? 1 : 0; 
        }
    }

    class LeapAfterOppSmooth : MelodyRule
    {
        public override bool _IsApplicable()
        {
            //если скачок больше квинты
            return (LastNote.Leap.AbsDeg > 4);
        }

        public override double Apply(Note nextNote)
        {
            //то нельзя плавного хода в том же направлении
            bool co = (nextNote.Leap.Upwards == LastNote.Leap.Upwards);
            return (co && (Math.Abs(nextNote.Leap.Degrees) == -1)) ? 0 : 1;
        }
    }

    class LeapAfterCoLeap : MelodyRule
    {
        public override bool _IsApplicable()
        {
            return true;
        }

        public override double Apply(Note nextNote)
        {
            //только плавный ход назад
            bool backwards = (nextNote.Leap.Upwards ^ LastNote.Leap.Upwards);
            return (backwards && (nextNote.Leap.AbsDeg == 1)) ? 1 : 0;
        }
    }

    class LeapAfterOppLeap : MelodyRule
    {
        private int _leapsInRow;

        public override bool _IsApplicable()
        {
            _leapsInRow = CountLast(n => n.Leap.AbsDeg > 1);
            return true;
        }

        public override double Apply(Note nextNote)
        {
            //только плавный ход назад
            bool backwards = (nextNote.Leap.Upwards ^ LastNote.Leap.Upwards);
            bool leapPossible = (_leapsInRow < 3);

            return (backwards && ((nextNote.Leap.AbsDeg == 1) || leapPossible) ) ? 1 : 0;
        }
    }

}