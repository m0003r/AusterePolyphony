using System;
using Compositor.Rules.Base;
using PitchBase;

namespace Compositor.Levels
{
    public class NoteRest : RuledLevel
    {
        public Time TimeStart { get; protected set; }
        public Time TimeEnd { get { return TimeStart + Duration; } }
        public int Duration { get; protected set; }

        internal void UpdateFreqs(FreqsDict freqs)
        {
            Freqs = freqs;
        }

        public override void AddVariants(bool dumpResult = false)
        {
            throw new NotImplementedException();
        }
    }
}