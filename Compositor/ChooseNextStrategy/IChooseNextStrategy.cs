using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Compositor.Levels;

namespace Compositor
{
    public interface IChooseNextStrategy<T>
    {
        T ChooseNext(IEnumerable<KeyValuePair<T, double>> Freqs);
    }
}
