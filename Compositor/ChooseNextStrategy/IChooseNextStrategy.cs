using System.Collections.Generic;
using Compositor.Levels;

namespace Compositor.ChooseNextStrategy
{
    public interface IChooseNextStrategy
    {
        IDeniable ChooseNext(IEnumerable<KeyValuePair<IDeniable, double>> freqs);
    }
}
