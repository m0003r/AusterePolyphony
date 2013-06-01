﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Notes
{
    public enum Clef
    {
        Treble = -1,
        Soprano = 0,
        Mezzo = 1,
        Alto = 2,
        Tenor = 3,
        Baritone = 4,
        Bass = 5
    }

    public class PitchFactory
    {
        private Modus _modus;
        private Clef _clef;

        public Modus Modus { get {return this._modus; } set { this._modus = value; Rebuild(); } }
        public Clef Clef { get {return this._clef; } set { this._clef = value; Rebuild(); } }

        public List<Pitch> Pitches;

        public PitchFactory(Modus Modus, Clef Clef)
        {
            this._modus = Modus;
            this._clef = Clef;

            Rebuild();
        }

        private void Rebuild()
        {
            Pitches = new List<Pitch>();
            for (int i = Low; i < High; i++)
            {
                Pitches.Add(new Pitch(i, _modus));
            }
        }

        private int Low
        {
            get {
                return -(int)_clef * 2 + 12 - _modus.DiatonicStart;
            }
        }

        private int High
        {
            get
            {
                return -(int)_clef * 2 + 25 - _modus.DiatonicStart;
            }
        }


    }
}
