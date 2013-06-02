using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Compositor.Levels;

namespace Compositor.Rules
{
    class StableOnDownBeatRule : NoteRule
    {
        public override bool IsApplicable()
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
        public override bool IsApplicable()
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
        public override bool IsApplicable()
        {
            return (Duration == 1);
        }

        public override double Apply(Note NextNote)
        {
            return (NextNote.Duration > 4) ? 0 : 0.5;
        }
    }

    /***********************************************************/

    class EightRestrictionsBeforeRule : NoteRule
    {
        public override bool IsApplicable()
        {
            return ((Duration > 2) || (Leap.isLeap));
        }

        public override double Apply(Note NextNote)
        {
            if (NextNote.Duration == 1)
            {
                if (Duration == 2) return 0.05;
                if (Duration == 6) return 0.5;
                return 0;
            }
            return 1;
        }
    }
}
