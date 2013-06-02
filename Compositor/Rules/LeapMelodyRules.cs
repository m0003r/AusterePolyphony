using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PitchBase;
using Compositor.Levels;

namespace Compositor.Rules
{
    using NotesList = List<Note>;


/***
 * Улучшенные правила скачка:
 * 
 * Сумма заполнений должна быть >= 2/3 величины скачка
 * С каждой стороны должно быть минимум 1/4 величины скачка (для скачков больше квинты)
 */

    class GravityRule : MelodyRule
    {
        //double desiredCenter;
        double gravityPoint;
        double gravityForce;

        public override void Init(Melody parent)
        {
            base.Init(parent);
            //desiredCenter = Melody.Diapason.Average(p => p.Value);
        }

        public override bool _IsApplicable()
        {
            //double actualCenter = Melody.notes.Sum(n => n.Pitch.Value * n.Duration) / (double)Melody.Time.Position;
            //double diff = desiredCenter - actualCenter;
            gravityPoint = LastNote.Pitch.Value + LastNote.Uncomp;
            gravityForce = LastNote.Uncomp;

            return Math.Abs(gravityForce) > 2;
        }

        public override double Apply(Note n)
        {
            double distance = n.Pitch.Value - gravityPoint;
            double force = distance * gravityForce;
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
        public override double Apply(Note n)
        {

            // ход в сторону нескомпенсированности
            if (LastNote.Uncomp != 0)
                if (Math.Sign(n.Leap.Degrees) == Math.Sign(LastNote.Uncomp))
                {
                    if (Math.Abs(LastNote.Uncomp) >= UncompDenyLeap)
                        return n.Leap.isLeap ? 0 : 1;
                    if (Math.Abs(LastNote.Uncomp) >= UncompDenySmooth)
                        return 0;
                }

            // ход против запаса или если запас 0
            if (Math.Sign(LastNote.Reserve) != Math.Sign(n.Leap.Degrees))
            {
                if (n.Leap.Degrees < 5)
                    return 1;
                else
                    return Math.Abs(n.Leap.Degrees) < Math.Abs(LastNote.Reserve * 4) ? 1 : 0;
            }
            return Math.Abs(n.Leap.Degrees + LastNote.Reserve) < ReserveLimit ? 1 : 0;
        }
    }

    class AfterLeapRules : MelodyRule
    {
        private LeapAfterCoSmooth CoSmooth;
        private LeapAfterOppSmooth OppSmooth;
        private LeapAfterCoLeap CoLeap;
        private LeapAfterOppLeap OppLeap;

        protected Note PreLast;

        private bool lastCo;
        private bool lastSm;

        public override void Init(Melody parent)
        {
            base.Init(parent);

            CoSmooth = new LeapAfterCoSmooth();
            CoLeap = new LeapAfterCoLeap();
            OppSmooth = new LeapAfterOppSmooth();
            OppLeap = new LeapAfterOppLeap();

            CoSmooth.Init(Melody);
            CoLeap.Init(Melody);
            OppSmooth.Init(Melody);
            OppLeap.Init(Melody);
        }

        public override bool _IsApplicable()
        {
            if (LastNote.Leap.isSmooth)
                return false;
            if (Notes.Count < 2)
                return false;

            PreLast = Notes[Notes.Count - 2];

            lastCo = (PreLast.Leap.Upwards == LastNote.Leap.Upwards);
            lastSm = PreLast.Leap.isSmooth;

            if (lastCo)
                return lastSm ? CoSmooth.IsApplicable() : CoLeap.IsApplicable();
            else
                return lastSm ? OppSmooth.IsApplicable() : OppLeap.IsApplicable();
        }

        public override double Apply(Note n)
        {
            if (lastCo)
                return lastSm ? CoSmooth.Apply(n) : CoLeap.Apply(n);
            else
                return lastSm ? OppSmooth.Apply(n) : OppLeap.Apply(n);
        }
    }

    class LeapAfterCoSmooth : MelodyRule
    {
        public override bool _IsApplicable()
        {
            return true;
        }

        public override double Apply(Note n)
        {
            //должно быть противонаправленно
            return (n.Leap.Upwards ^ LastNote.Leap.Upwards) ? 1 : 0; 
        }
    }

    class LeapAfterOppSmooth : MelodyRule
    {
        public override bool _IsApplicable()
        {
            //если скачок больше квинты
            return (LastNote.Leap.AbsDeg > 4);
        }

        public override double Apply(Note n)
        {
            //то нельзя плавного хода в том же направлении
            bool co = (n.Leap.Upwards == LastNote.Leap.Upwards);
            return (co && (Math.Abs(n.Leap.Degrees) == -1)) ? 0 : 1;
        }
    }

    class LeapAfterCoLeap : MelodyRule
    {
        public override bool _IsApplicable()
        {
            return true;
        }

        public override double Apply(Note n)
        {
            //только плавный ход назад
            bool backwards = (n.Leap.Upwards ^ LastNote.Leap.Upwards);
            return (backwards && (n.Leap.AbsDeg == 1)) ? 1 : 0;
        }
    }

    class LeapAfterOppLeap : MelodyRule
    {
        private int leapsInRow = 0;

        public override bool _IsApplicable()
        {
            IEnumerable<Note> lastLeaps = Notes.Reverse<Note>().TakeWhile(n => n.Leap.AbsDeg > 1);
            leapsInRow = lastLeaps.Count();

            return true;
        }

        public override double Apply(Note n)
        {
            //только плавный ход назад
            bool backwards = (n.Leap.Upwards ^ LastNote.Leap.Upwards);
            bool leapPossible = (leapsInRow < 3);

            return (backwards && ((n.Leap.AbsDeg == 1) || leapPossible) ) ? 1 : 0;
        }
    }

}