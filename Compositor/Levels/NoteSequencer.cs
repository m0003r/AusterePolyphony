using System;
using System.Collections.Generic;
using System.Linq;
using PitchBase;

namespace Compositor.Levels
{
    public static class LeapsData
    {
        public static readonly double Tertia = 0.8;
        public static readonly double Quarta = 0.8;
        public static readonly double Quinta = 0.8;
        public static readonly double OctavaUp = 0.8;
        public static readonly double OctavaDown = 0.5;
        public static readonly double SextaUp = 0.4;
    }

    static class NoteSequencerExtension
    {
        public static double DurationCoeff(this Note me, int duration)
        {
            if (duration < 2)
                return 0.5;  //восьмые
            if (duration > 6)
                return 1 - (Math.Log(duration, 2) - 3) * 0.8;
            return 1;
        }

        public static double GetPitchFreqAfter(this Note me, Pitch p2, Time t)
        {
            if (me.Pitch == p2)
                return 0; //не бывает никада!

            if (null == p2)
                return (t.Beat%2 == 0) ? 1 : 0;

            if (me.Pitch == null)
                return 1;

            var diff = p2 - me.Pitch;

            if (me.Duration == 1) // нельзя неплавные ходы после восьмых
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

        public static Note[] GenerateNotes(this Note me, Pitch p, Time t)
        {
            var durVar = new List<int>();

            if (t.Beat % 2 == 1) // только восьмые
                durVar.Add(1);
            else
            {
                GenerateOnQuarter(me, t, durVar);
            }

            return durVar.Select(dur => new Note(p, t, dur, me)).ToArray();
        }

        private static void GenerateOnQuarter(Note me, Time t, List<int> durVar)
        {
            if (t.AllowEight)
                durVar.Add(1);

            durVar.Add(2); //четверти можно везде

            if (t.Beat % 4 == 0) //каждую половину
                GenerateOnHalf(me, durVar);

            if (t.Beat == 0) // на первую долю
            {
                durVar.Add(12); // целые с точкой
                durVar.Add(16); // и бревисы
            }

            //Imperfectus
            if (t.Beats != 3)
            {
                if (t.Beat != 8) return;

                durVar.Add(12); // целую с точкой
                durVar.Add(16); // и бревис

                return;
            }

            //Perfectus
            switch (t.Beat)
            {
                case 0:
                    durVar.Add(10); // половинку с точкой + половинку на сильную долю
                    break;
                case 4:
                    durVar.Add(12); // целую с точкой на вторую долю
                    break;
            }
        }

        private static void GenerateOnHalf(Note me, List<int> durVar)
        {
            durVar.Add(4); // половины
            durVar.Add(6); // половины с точкой
            //if (me.Duration > 1) //если не восьмая
                durVar.Add(8); // и целые
        }
    }
}
