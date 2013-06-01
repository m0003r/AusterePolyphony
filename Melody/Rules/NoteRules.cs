﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Melody.Rules
{
    class StableOnDownBeatRule : NoteRule
    {
        protected override bool _IsApplicable()
        {
            return ((Time + Duration).Beat == 0);
        }

        public override double Apply(Note NextNote)
        {
            switch (NextNote.Pitch.Degree)
            {
                case 0: return 1 + NextNote.Duration / 16.0; //увеличиваем вероятность длинной первой или пятой ступени на сильную долю
                case 4: return 1 + NextNote.Duration / 32.0;
                default: return 1;
            }
        }
    }

    /************************************************************/

    class DenyDoubleBrevesRule : NoteRule
    {
        protected override bool _IsApplicable()
        {
            return (Me.Duration == Time.BarLength);
        }

        public override double Apply(Note NextNote)
        {
            return (NextNote.Duration == Time.BarLength) ? 0 : 1;
        }
    }

    /***********************************************************/

    class EightRestrictionsAfterRule : NoteRule
    {
        protected override bool _IsApplicable()
        {
            return (Me.Duration == 1);
        }

        public override double Apply(Note NextNote)
        {
            return (NextNote.Duration > 4) ? 0 : 1;
        }
    }

    /***********************************************************/

    class EightRestrictionsBeforeRule : NoteRule
    {
        protected override bool _IsApplicable()
        {
            return ((Duration > 2) || (Leap.isLeap));
        }

        public override double Apply(Note NextNote)
        {
            return (NextNote.Duration == 1) ? 0 : 1;
        }
    }
}
