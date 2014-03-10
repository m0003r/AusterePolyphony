﻿using System.Linq;
using Compositor.Levels;
using Compositor.Rules.Base;

namespace Compositor.Rules.TwoVoices
{
    class ComplementRule : TwoVoicesRule
    {
        private const int MinSimultCount = 2;
        private const int MaxSimultCount = 4;

        int _simultCount;

        public override bool _IsApplicable()
        {
            _simultCount = Notes.Reverse<TwoNotes>().TakeWhile(n => n.Simult).Count();
            return (_simultCount > MinSimultCount);
        }

        public override double Apply(TwoNotes nextNotes)
        {
            if (nextNotes.Simult)
                return (double)(MaxSimultCount - _simultCount) / (MaxSimultCount - MinSimultCount);

            return 1;
        }
    }

    class ComplementRule2 : TwoVoicesRule
    {
        private const double MinSimultProportion = 0.4;
        private const double MaxSimultProportion = 0.8;

        double _simultProportion;

        public override bool _IsApplicable()
        {
            int simultCount = Notes.Count(n => n.Simult);
            _simultProportion = (double)simultCount / Notes.Count;
            return (simultCount > MinSimultProportion);
        }

        public override double Apply(TwoNotes nextNotes)
        {
            if (nextNotes.Simult)
                return (MaxSimultProportion - _simultProportion) / (MaxSimultProportion - MinSimultProportion);

            return 1;
        }
    }
}