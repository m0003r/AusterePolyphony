using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PitchBase
{
    //ПЕРВАЯ СТУПЕНЬ В ЛЮБОМ СЛУЧАЕ В БОЛЬШОЙ ОКТАВЕ
    public class Modus
    {
        private IntervalAlt[] alt;
        public int NoteStart { get; private set; }
        private int KeysDelta;

        //Количество знаков в ладу
        public int Keys
        {
            get
            {
                int k = (NoteStart * 7 + KeysDelta) % 12;
                if (k > 6)
                    return k - 12;
                if (k <= -6)
                    return k + 12;
                return k;
            }
        }

        //Насколько первая ступень лада отличается от первой ступени параллельного мажора
        public int NotesDelta
        {
            get
            {
                int r = (KeysDelta * -4) % 7;
                if (r < 0)
                    r += 7;
                return r;
            }
        }

        //диатоническая нота, с которой начинается лад
        public int DiatonicStart
        {
            get
            {
                int ds = ((Keys - KeysDelta) * 4) % 7;
                if (ds < 0)
                    ds += 7;
                return ds;
            }
        }

        private static int[] ionian = {0, 1, 1, 0, 0, 1, 1};
        private static int[] dorian = { 0, 1, -1, 0, 0, 1, -1 };
        private static int[] phrygian = { 0, -1, -1, 0, 0, -1, -1 };
        private static int[] lydian = { 0, 1, 1, 2, 0, 1, 1 };
        private static int[] mixolydian = { 0, 1, 1, 0, 0, 1, -1 };
        private static int[] aeolian = { 0, 1, -1, 0, 0, -1, -1 };

        private Modus(int[] alt, int n, int keysDelta)
        {
            if (alt.Length != 7)
                throw new ArgumentException();

            this.alt = alt.Select<int, IntervalAlt>(x => (IntervalAlt)(x)).ToArray<IntervalAlt>();
            NoteStart = n;
            this.KeysDelta = keysDelta;
        }

        public static Modus Ionian(int start = 0)
        {
            return new Modus(ionian, start, 0);
        }

        public static Modus Dorian(int start = 0) // -> -2
        {
            return new Modus(dorian, start, -2);
        }

        public static Modus Phrygian(int start = 0) // -> -4
        {
            return new Modus(phrygian, start, -4);
        }

        public static Modus Lydian(int start = 0) // -> +1
        {
            return new Modus(lydian, start, 1);
        }

        public static Modus Mixolydian(int start = 0) // -> -1
        {
            return new Modus(mixolydian, start, -1);
        }

        public static Modus Aeolian(int start = 0) // -> -3
        {
            return new Modus(aeolian, start, -3);
        }

        internal Interval baseToDegree(uint degree)
        {
            switch (degree)
            {
                case 0: return new Interval(IntervalType.Prima, alt[0]);
                case 1: return new Interval(IntervalType.Secunda, alt[1]);
                case 2: return new Interval(IntervalType.Tertia, alt[2]);
                case 3: return new Interval(IntervalType.Quarta, alt[3]);
                case 4: return new Interval(IntervalType.Quinta, alt[4]);
                case 5: return new Interval(IntervalType.Sexta, alt[5]);
                case 6: return new Interval(IntervalType.Septima, alt[6]);
                case 7: return new Interval(IntervalType.Octava, alt[0]);
                default: return new Interval(IntervalType.Prima, alt[0]);
            } 
        }
    }
}
