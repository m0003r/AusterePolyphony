using System;
using System.Collections.Generic;

namespace Compositor
{
    public interface IGenerator
    {
        int Generate(uint Length);

        int GetSeed();

        List<Compositor.Levels.Melody> GetNotes();
    }
}
