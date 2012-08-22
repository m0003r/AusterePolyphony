using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Notes
{
    public enum NoteState
    {
        LeapDown = -2,
        Down = -1,
        Unknown = 0,
        Up = 1,
        LeapUp = 2
    }
}
