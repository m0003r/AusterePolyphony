﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Melody
{
    public class SubNote
    {
        MelodyNote baseNote;

        public int Position { get; private set; }

        public SubNote(MelodyNote baseNote, int Position)
        {
            this.baseNote = baseNote;
            this.Position = Position;
        }

        public bool isBegin { get { return Position == 0; } }
        public bool isEnd { get { return Position == baseNote.Duration - 1; } }
    }
}
