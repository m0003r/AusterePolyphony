using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PitchBase;

namespace Compositor.Levels
{
    public static class LeapsData
    {
        public static readonly double Tertia = 0.8;
        public static readonly double Quarta = 0.7;
        public static readonly double Quinta = 0.6;
        public static readonly double OctavaUp = 0.5;
        public static readonly double OctavaDown = 0.2;
        public static readonly double SextaUp = 0.1;
    }

    static class NoteSequencerExtension
    {
        public static double DurationCoeff(this Note me, int Duration)
        {
            if (Duration < 2)
                return 0.5;  //восьмые
            if (Duration > 6)
                return 1 - (Math.Log(Duration, 2) - 3) * 0.8;
            return 1;
        }

        public static double allowPitchAfterAt(this Note me, Pitch p2, Time t)
        {
            if (me.Pitch == p2)
                return 0; //не бывает никада!

            Interval diff = p2 - me.Pitch;

            if ((t.Beat % 4 != 0) || (me.Duration == 1)) // нельзя неплавные ходы внутри метрической доли или после восьмых
                return (diff.Type == IntervalType.Secunda) ? 1 : 0;

            switch (diff.Type)
            {
                case IntervalType.Secunda: return 1;
                case IntervalType.Tertia: return LeapsData.Tertia;
                case IntervalType.Quarta: return (diff.Alteration == IntervalAlt.Natural) ? LeapsData.Quarta : 0;
                case IntervalType.Quinta: return (diff.Alteration == IntervalAlt.Natural) ? LeapsData.Quinta : 0;
                case IntervalType.Octava: return diff.Upwards ? LeapsData.OctavaUp : LeapsData.OctavaDown;
            }

            if ((diff.Semitones == 8) && (diff.Upwards)) // малая секста вверх
                return LeapsData.SextaUp;

            return 0;
        }

        public static Note[] GenerateDurations(this Note me, Pitch p, Time t)
        {
            List<Note> v = new List<Note>();
            List<int> durVar = new List<int>();

            if (t.Beat % 2 == 1)
                durVar.Add(1);
            else
            {
                if (t.allowEight)
                    durVar.Add(1);
                durVar.Add(2); //четверти можно везде

                if (t.Beat % 4 == 0) //каждую половину
                {
                    durVar.Add(4); // половины
                    durVar.Add(6); // половины с точкой
                    if (me.Duration > 1) //если не восьмая
                        durVar.Add(8); // и целые
                }

                if (me.Duration > 1) //после восьмушек всю это белоту нельзя
                {
                    if (t.Beat == 0) // на первую долю
                    {
                        durVar.Add(12); // целые с точкой
                        durVar.Add(16); // и бревисы
                    }
                    if (t.Beats == 3) // в трёхдольном размере
                    {
                        if (t.Beat == 0)
                            durVar.Add(10); // половинку с точкой + половинку на сильную долю
                        if (t.Beat == 4)
                            durVar.Add(12); // целую с точкой на вторую долю
                    }
                    else
                    {
                        if (t.Beat == 8) // в четырёхдольном в середине такта
                        {
                            durVar.Add(12); // целую с точкой
                            durVar.Add(16); // и бревис
                        }
                    }
                }
            }

            foreach (int dur in durVar)
            {
                v.Add(new Note(p, t, dur, me));
            }

            return v.ToArray();
        }
    }
}
