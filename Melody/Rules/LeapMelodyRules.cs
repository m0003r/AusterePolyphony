using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Notes;

namespace Melody.Rules
{
    using NotesList = List<Note>;

    class AfterLeapRules : MelodyRule
    {
        public AfterLeapRules(MelodyGenerator parent)
            : base(parent) { }

        private void InitSubRules()
        {
            if (subRulesReady)
                return;

            CoSmooth = new LeapAfterCoSmooth(_p);
            CoLeap = new LeapAfterCoLeap(_p);
            OppSmooth = new LeapAfterOppSmooth(_p);
            OppLeap = new LeapAfterOppLeap(_p);

            subRulesReady = true;
        }

        private bool subRulesReady = false;

        private LeapAfterCoSmooth CoSmooth;
        private LeapAfterOppSmooth OppSmooth;
        private LeapAfterCoLeap CoLeap;
        private LeapAfterOppLeap OppLeap;

        protected Note PreLast;

        private bool lastCo;
        private bool lastSm;

        public override bool IsApplicable()
        {
            if (LastNote.Leap.isSmooth)
                return false;
            if (Notes.Count < 2)
                return false;

            PreLast = Notes[Notes.Count - 2];
            InitSubRules();

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

    class LeapAfterCoSmooth : AfterLeapRules
    {
        public LeapAfterCoSmooth(MelodyGenerator parent)
            : base(parent) { }

        public override bool IsApplicable()
        {
            return true;
        }

        public override double Apply(Note n)
        {
            //должно быть противонаправленно
            return (n.Leap.Upwards ^ LastNote.Leap.Upwards) ? 1 : 0; 
        }
    }

    class LeapAfterOppSmooth : AfterLeapRules
    {
        public LeapAfterOppSmooth(MelodyGenerator parent)
            : base(parent) { }

        public override bool IsApplicable()
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

    class LeapAfterCoLeap : AfterLeapRules
    {
        public LeapAfterCoLeap(MelodyGenerator parent)
            : base(parent) { }

        public override bool IsApplicable()
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

    class LeapAfterOppLeap : AfterLeapRules
    {
        public LeapAfterOppLeap(MelodyGenerator parent)
            : base(parent) { }

        private int leapsInRow = 0;

        public override bool IsApplicable()
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