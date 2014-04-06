using Compositor.Rules.Base;

namespace Compositor.Levels
{
    public interface IFilterable : IDeniable
    {
        FreqsDict Freqs { get; }
        void AddVariants(bool dumpResult = false);
        FreqsDict Filter(bool dumpResult = false);
        void Ban(IDeniable what);
    }
}