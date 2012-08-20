using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Notes
{
    public class Degree
    {
        int Number;
        int Acc;
        int keys;
        

        private static String[] Naturals = { "c", "d", "e", "f", "g", "a", "h" };
        private static String[] Sharps = { "cis", "dis", "eis", "fis", "gis", "ais", "his" };
        private static String[] Flats = { "ces", "des", "es", "fes", "ges", "as", "b" };
        private static String[] DoubleSharps = { "cisis", "disis", "eisis", "fisis", "gisis", "aisis", "bisis" };
        private static String[] DoubleFlats = { "ceses", "deses", "eses", "feses", "geses", "ases", "beses" };

        private Degree(int number, int acc)
        {
            Number = number;
            Acc = acc;
        }

        // это ужасная музыкальная математика
        // мы жульничаем, прибавляя знаки, чтобы вышел мажор
        // и потом вычисляем, сколько ступеней надо добавить
        public Degree(Pitch n)
        {
            Number = (int)n.Pitch;
            Acc = 0;
            keys = (n.PitchModus.noteStart * 7 + n.PitchModus.keysDelta) % 12;

            Number += n.PitchModus.notesDelta;
            if (keys > 6)
                keys -= 12;
            if (keys < -6)
                keys += 12;
        }

        public String resolve()
        {
            int fAcc = Acc;
            int auxAcc;
            int diatNum;
            int inScaleNum = Number % 7;

            auxAcc = 6;

            auxAcc = (auxAcc - 2 * inScaleNum) % 7;

            if (auxAcc <= 0)
                auxAcc += 7;

            if (keys < 0)
                auxAcc = 8 - auxAcc;

            if (auxAcc <= Math.Abs(keys))
                fAcc += keys / Math.Abs(keys);

            diatNum = (inScaleNum + keys * 4 ) % 7;

            if (diatNum < 0)
                diatNum += 7;

            switch (fAcc)
            {
                case -2: return DoubleFlats[diatNum];
                case -1: return Flats[diatNum];
                case 0: return Naturals[diatNum];
                case 1: return Sharps[diatNum];
                case 2: return DoubleSharps[diatNum];
                default: return "";
            }
        }

        public static String operator -(Degree a, Degree g)
        {
            int o;

            o = a.Number - g.Number;
            o = (int)Math.Round((double)o / 7.0);

            return OctaveShift(o);
        }

        public static String OctaveShift(int o)
        {
            if (o > 0)
                return new String('\'', o);
            if (o < 0)
                return new String(',', -o);
            return "";
        }
    }
}
