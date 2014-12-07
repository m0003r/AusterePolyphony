using Compositor.Rules.Base;
using PitchBase;

namespace Compositor.Levels
{
    public interface IFilterable : IDeniable
    {
        FreqsDict Freqs { get; }
        void AddVariants();
        FreqsDict Filter();
        void Ban(IDeniable what);
    }

    public interface ITemporal
    {
        Time TimeStart { get; }
        bool Equals(ITemporal obj);
    }
}