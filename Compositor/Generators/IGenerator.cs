using System;
using System.Collections.Generic;
using Compositor.Levels;

namespace Compositor.Generators
{
    public interface IGenerator
    {
        int Generate(uint length);
        int Generate(uint length, Func<int, bool> callback);

        int GetSeed();

        List<Voice> GetNotes();
    }
}
