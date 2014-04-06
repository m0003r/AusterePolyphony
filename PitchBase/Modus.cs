using System;
using System.Linq;

namespace PitchBase
{
    //ПЕРВАЯ СТУПЕНЬ В ЛЮБОМ СЛУЧАЕ В БОЛЬШОЙ ОКТАВЕ
    public class Modus
    {
        private readonly IntervalAlt[] _alt;
        public int NoteStart { get; private set; }
        private readonly int _keysDelta;
        public string Name { get; private set; }

        //Количество знаков в ладу
        public int Keys
        {
            get
            {
                int k = (NoteStart * 7 + _keysDelta) % 12;
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
                int r = (_keysDelta * -4) % 7;
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
                int ds = ((Keys - _keysDelta) * 4) % 7;
                if (ds < 0)
                    ds += 7;
                return ds;
            }
        }

        private static readonly int[] IonianData = { 0, 1, 1, 0, 0, 1, 1 };
        private static readonly int[] DorianData = { 0, 1, -1, 0, 0, 1, -1 };
        private static readonly int[] PhrygianData = { 0, -1, -1, 0, 0, -1, -1 };
        private static readonly int[] LydianData = {0, 1, 1, 2, 0, 1, 1};
        private static readonly int[] MixolydianData = { 0, 1, 1, 0, 0, 1, -1 };
        private static readonly int[] AeolianData = { 0, 1, -1, 0, 0, -1, -1 };

        private Modus(int[] alt, int n, int keysDelta, string name)
        {
            if (alt.Length != 7)
                throw new ArgumentException();

            _alt = alt.Select(x => (IntervalAlt)(x)).ToArray();
            NoteStart = n;
            _keysDelta = keysDelta;
            Name = name;
        }

        public static Modus Ionian(int start = 0)
        {
            return new Modus(IonianData, start, 0, "ionian");
        }

        public static Modus Dorian(int start = 0) // -> -2
        {
            return new Modus(DorianData, start, -2, "dorian");
        }

        public static Modus Phrygian(int start = 0) // -> -4
        {
            return new Modus(PhrygianData, start, -4, "phrygian");
        }

        public static Modus Lydian(int start = 0) // -> +1
        {
            return new Modus(LydianData, start, 1, "lydian");
        }

        public static Modus Mixolydian(int start = 0) // -> -1
        {
            return new Modus(MixolydianData, start, -1, "mixolydian");
        }

        public static Modus Aeolian(int start = 0) // -> -3
        {
            return new Modus(AeolianData, start, -3, "aeolian");
        }

        internal Interval BaseToDegree(uint degree)
        {
            switch (degree)
            {
                case 0: return new Interval(IntervalType.Prima, _alt[0]);
                case 1: return new Interval(IntervalType.Secunda, _alt[1]);
                case 2: return new Interval(IntervalType.Tertia, _alt[2]);
                case 3: return new Interval(IntervalType.Quarta, _alt[3]);
                case 4: return new Interval(IntervalType.Quinta, _alt[4]);
                case 5: return new Interval(IntervalType.Sexta, _alt[5]);
                case 6: return new Interval(IntervalType.Septima, _alt[6]);
                case 7: return new Interval(IntervalType.Octava, _alt[0]);
                default: return new Interval(IntervalType.Prima, _alt[0]);
            }
        }

        public override string ToString()
        {
            return String.Format("{0} {1}", new Pitch(0, this).StringForm, Name);
        }
    }
}
