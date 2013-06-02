using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PitchBase;
using Compositor.Rules;

namespace Compositor.Levels
{
    public class TwoNotes : IDeniable
    {
        public Note Note1;
        public Note Note2;

        public Rule DeniedRule { get; set; }
        public bool isBanned { get; set; }
    }

    public class TwoVoices : RuledLevel<TwoVoices, TwoNotes>
    {
        public Melody Voice1 { get; private set; }
        public Melody Voice2 { get; private set; }

        private Melody lastAdded;

        public TwoVoices(Clef Clef1, Clef Clef2, Modus Modus, Time Time)
        {
            Voice1 = new Melody(Clef1, Modus, Time);
            Voice2 = new Melody(Clef2, Modus, Time);
        }

        protected override void AddVariants(bool dumpResult = false)
        {
            throw new NotImplementedException();
        }
    }
}
