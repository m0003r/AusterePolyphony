using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Notes
{
    public enum IntervalType
    {
        Prima = 0,
        Secunda = 1,
        Tertia = 2,
        Quarta = 3,
        Quinta = 4,
        Sexta = 5,
        Septima = 6,
        Octava = 7,
        Nona = 8,
        Decima = 9,
        Undecima = 10,
        Duodecima = 11
    }

    public static class IntervalTypeExtensionMethods
    {
        public static double ToSemitones(this IntervalType a)
        {
            uint degrees = (uint)a % 7;
            uint shift = ((uint)a / 7) * 12;

            switch (degrees)
            {
                case 0: return shift;
                case 1: return shift + 1.5;
                case 2: return shift + 3.5;
                case 3: return shift + 5;
                case 4: return shift + 7;
                case 5: return shift + 8.5;
                case 6: return shift + 10.5;
                default: return shift;
            }
        }

        public static IntervalAlt CalcAlteration(this IntervalType me, int realSemitones)
        {
            double semitones = (realSemitones - me.ToSemitones());
            int altV = (int)(semitones * 2);
            return (IntervalAlt)altV;
        }

        public static bool isLeap(this IntervalType me)
        {
            return ((int)me > 1);
        }

        public static bool isCont(this IntervalType me)
        {
            return !me.isLeap();
        }
    }
}

