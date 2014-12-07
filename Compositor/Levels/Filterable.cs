using System;
using System.Collections.Generic;
using Compositor.Rules.Base;

namespace Compositor.Levels
{
    public interface IFilterable : IDeniable
    {
        FreqsDict Freqs { get; }
        void AddVariants();
        FreqsDict Filter();
        void Ban(IDeniable what);
    }
}