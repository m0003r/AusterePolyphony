using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PitchBase;
using Compositor.Levels;
using Compositor.Helpers;

namespace Compositor
{
    interface Rule<T>
    {
        void Init(T me);
        bool IsApplicable();
        double Apply(Note n);
    }
}
