﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Melody.Rules
{
    class AfterSmoothLeapRule : NoteRule
    {
        protected override bool _IsApplicable()
        {
            return Me.Leap.isSmooth;
        }

        public override double Apply(Note NextNote)
        {
            bool coDir = (NextNote.Leap.Upwards == Me.Leap.Upwards);

            if (Math.Abs(NextNote.Leap.Degrees) == 1) //если дальше плавный ход
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
            return (Math.Abs(NextNote.Leap.Degrees) <= 4) ? 0.4 : 0;
        }        
    }

    class AfterLeapLeapRule : NoteRule
    {
        protected override bool _IsApplicable()
        {
            return Me.Leap.isLeap;
        }
        public override double Apply(Note NextNote)
        {
            bool coDir = (NextNote.Leap.Upwards == Me.Leap.Upwards);

            if (Math.Abs(NextNote.Leap.Degrees) == 1) //если дальше плавный ход
            {
                if (coDir) //в том же направлении после не-больше-чем-квинты можно
                    return (Leap.Degrees <= 4) ? 0.6 : 0;
                else
                    return 1; //а в другую сторону очень даже хорошо
            }
            else
                return coDir ? CoDirRule(NextNote) : OppDirRule(NextNote);
        }

        private double OppDirRule(Note NextNote)
        {
            int prevLeap = Math.Abs(Leap.Degrees);
            int nextLeap = Math.Abs(NextNote.Leap.Degrees);

            //в другую сторону можно сразу же скакать, если на меньшее расстояние
            return (prevLeap > nextLeap) ? 0.6 : 0; 
        }

        private double CoDirRule(Note NextNote)
        {
            //оба не больше квинты и в сумме консонанс!
            //и то редко
            return (
                    (Math.Abs(Leap.Degrees) <= 4) &&
                    (Math.Abs(NextNote.Leap.Degrees) <= 4) &&
                    ((NextNote.Leap + Leap).Consonance)
                ) ? 0.3 : 0;
        }
    }
}
