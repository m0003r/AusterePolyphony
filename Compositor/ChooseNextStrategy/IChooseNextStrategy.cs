using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Compositor.Levels;

namespace Compositor
{
    public interface IChooseNextStrategy
    {
        Note ChooseNext(IEnumerable<KeyValuePair<Note, double>> Freqs);
    }
}
