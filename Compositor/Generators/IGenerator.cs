using System;
using System.Collections.Generic;
using Compositor.Levels;

namespace Compositor.Generators
{
    public struct GenerationInfo
    {
        public int Steps;
        public int Duration;
        public int Rollback;

        public GenerationInfo(int steps, int rollback, int duration)
        {
            Steps = steps;
            Duration = duration;
            Rollback = rollback;
        }
    }

    public interface IGenerator
    {
        int Generate(uint length);
        int Generate(uint length, Func<GenerationInfo, bool> callback);

        int GetSeed();

        List<Voice> GetNotes();
    }
}
