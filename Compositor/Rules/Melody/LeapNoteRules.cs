using Compositor.Levels;
using Compositor.Rules.Base;

namespace Compositor.Rules.Melody
{
    class AfterSmoothLeapRule : NoteRule
    {
        public override bool IsApplicable()
        {
            return Me.Leap.IsSmooth;
        }

        public override double Apply(Note nextNotes)
        {
            var coDir = (nextNotes.Leap.Upwards == Me.Leap.Upwards);

            return nextNotes.Leap.AbsDeg == 1
                ? ((coDir) ? 1 : 0.8)
                : (coDir ? CoDirRule(nextNotes) : OppDirRule(nextNotes));
        }

        private double OppDirRule(Note nextNote)
        {
            if (Duration > 2)
            {
                return nextNote.Duration > 2 ? 0.85 : 0.7;
            }

            if (Duration == 2)
                return (Leap.Upwards) ? 0 : 0.5;

            return 0; //иначе не очень
        }

        private double CoDirRule(Note nextNote)
        {
            //не больше квинты — можно
            return (nextNote.Leap.AbsDeg <= 4) ? 0.4 : 0;
        }        
    }

    class AfterLeapLeapRule : NoteRule
    {
        public override bool IsApplicable()
        {
            return Me.Leap.IsLeap;
        }
        public override double Apply(Note nextNotes)
        {
            var coDir = (nextNotes.Leap.Upwards == Me.Leap.Upwards);

            //а в другую сторону очень даже хорошо
            if (nextNotes.Leap.AbsDeg != 1) return coDir ? CoDirRule(nextNotes) : OppDirRule(nextNotes);

            if (coDir) //в том же направлении после не-больше-чем-квинты можно
                return (Leap.AbsDeg <= 4) ? 0.7 : 0;

            return 1;
        }

        private double OppDirRule(Note nextNote)
        {
            int prevLeap = Leap.AbsDeg;
            int nextLeap = nextNote.Leap.AbsDeg;

            //в другую сторону можно сразу же скакать, если на меньшее расстояние
            return (prevLeap > nextLeap) ? 0.7 : 0; 
        }

        private double CoDirRule(Note nextNote)
        {
            //оба не больше квинты и в сумме консонанс!
            //и то редко
            return (
                    (Leap.AbsDeg <= 4) &&
                    (nextNote.Leap.AbsDeg <= 4) &&
                    ((nextNote.Leap + Leap).Consonance)
                ) ? 0.4 : 0;
        }
    }
}
