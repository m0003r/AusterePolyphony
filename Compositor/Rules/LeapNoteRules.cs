using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Compositor.Levels;

namespace Compositor.Rules
{
    class AfterSmoothLeapRule : NoteRule
    {
        public override bool IsApplicable()
        {
            return Me.Leap.isSmooth;
        }

        public override double Apply(Note NextNote)
        {
            bool coDir = (NextNote.Leap.Upwards == Me.Leap.Upwards);

            if (NextNote.Leap.AbsDeg == 1) //если дальше плавный ход
                //плавный ход предпочитаем туда же
                return (coDir) ? 1 : 0.8;
            else
                return coDir ? CoDirRule(NextNote) : OppDirRule(NextNote);
        }

        private double OppDirRule(Note NextNote)
        {
            if ((Duration > 2) && (NextNote.Duration > 2))
                return 0.85; //от белой к белой также хорошо прыгать

            if ((Duration == 2) && (NextNote.Duration > 2))
                return 0.7; //от четверти к белой тоже хорошо

            return 0.5; //иначе не очень
        }

        private double CoDirRule(Note NextNote)
        {
            //не больше квинты — можно
            return (NextNote.Leap.AbsDeg <= 4) ? 0.4 : 0;
        }        
    }

    class AfterLeapLeapRule : NoteRule
    {
        public override bool IsApplicable()
        {
            return Me.Leap.isLeap;
        }
        public override double Apply(Note NextNote)
        {
            bool coDir = (NextNote.Leap.Upwards == Me.Leap.Upwards);

            if (NextNote.Leap.AbsDeg == 1) //если дальше плавный ход
            {
                if (coDir) //в том же направлении после не-больше-чем-квинты можно
                    return (Leap.AbsDeg <= 4) ? 0.6 : 0;
                else
                    return 1; //а в другую сторону очень даже хорошо
            }
            else
                return coDir ? CoDirRule(NextNote) : OppDirRule(NextNote);
        }

        private double OppDirRule(Note NextNote)
        {
            int prevLeap = Leap.AbsDeg;
            int nextLeap = NextNote.Leap.AbsDeg;

            //в другую сторону можно сразу же скакать, если на меньшее расстояние
            return (prevLeap > nextLeap) ? 0.6 : 0; 
        }

        private double CoDirRule(Note NextNote)
        {
            //оба не больше квинты и в сумме консонанс!
            //и то редко
            return (
                    (Leap.AbsDeg <= 4) &&
                    (NextNote.Leap.AbsDeg <= 4) &&
                    ((NextNote.Leap + Leap).Consonance)
                ) ? 0.3 : 0;
        }
    }
}
