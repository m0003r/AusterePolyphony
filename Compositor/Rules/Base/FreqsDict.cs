using System;
using System.Collections.Generic;
using Compositor.Levels;

namespace Compositor.Rules.Base
{
    [Serializable]
    public class FreqsDict : Dictionary<IDeniable, double>
    {
        public FreqsDict(FreqsDict existing) : base(existing) { }
        public FreqsDict() { }
    }
}
