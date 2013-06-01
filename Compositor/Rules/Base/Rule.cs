using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PitchBase;
using Compositor.Levels;

namespace Compositor.Rules
{
    interface Rule<T>
    {
        void Init(T me);
        bool IsApplicable();
        double Apply(Note n);
    }
}
