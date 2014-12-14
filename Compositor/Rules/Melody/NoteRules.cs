using Compositor.Levels;
using Compositor.Rules.Base;

namespace Compositor.Rules.Melody
{
    class StableOnDownBeatRule : NoteRule
    {
        public override bool IsApplicable()
        {
            return ((Time + Duration).Beat == 0);
        }

        public override double Apply(Note nextNotes)
        {
            switch (nextNotes.Pitch.Degree)
            {
                case 0: return 1 + nextNotes.Duration / 16.0; //увеличиваем вероятность длинной первой или пятой ступени на сильную долю
                case 4: return 1 + nextNotes.Duration / 32.0;
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

        public override double Apply(Note nextNotes)
        {
            return (nextNotes.Duration == Time.BarLength) ? 0 : 1;
        }
    }

    /***********************************************************/

    class EightRestrictionsAfterRule : NoteRule
    {
        public override bool IsApplicable()
        {
            return (Duration == 1);
        }

        public override double Apply(Note nextNotes)
        {
            return (nextNotes.Duration > 4) ? 0 : 0.5;
        }
    }

    /***********************************************************/

    class EightRestrictionsBeforeRule : NoteRule
    {
        public override bool IsApplicable()
        {
            return ((Duration > 2) || (Leap.IsLeap));
        }

        public override double Apply(Note nextNotes)
        {
            if (nextNotes.Duration != 1) return 1;

            switch (Duration)
            {
                case 2:
                    return 0.45;
                case 6:
                    return 0.9;
            }
            return 0;
        }
    }
}
