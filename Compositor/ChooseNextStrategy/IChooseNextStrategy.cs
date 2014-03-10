using System.Collections.Generic;

namespace Compositor.ChooseNextStrategy
{
    public interface IChooseNextStrategy<T>
    {
        T ChooseNext(IEnumerable<KeyValuePair<T, double>> freqs);
    }
}
