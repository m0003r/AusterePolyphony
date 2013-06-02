using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PitchBase;
using Compositor.Levels;

namespace Compositor.Rules
{
    interface Rule<T, N> : Rule
    {
        void Init(T me);
        double Apply(N n);
    }

    public interface Rule
    {
        bool IsApplicable();
    }
}
