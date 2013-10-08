using System.Linq;

using Compositor.Levels;

namespace Compositor.Rules
{
    class ComplementRule : TwoVoicesRule
    {
        static int MinSimultCount = 2;
        static int MaxSimultCount = 4;

        int SimultCount;

        public override bool _IsApplicable()
        {
            SimultCount = Notes.Reverse<TwoNotes>().TakeWhile(n => n.Simult).Count();
            return (SimultCount > MinSimultCount);
        }

        public override double Apply(TwoNotes NextNotes)
        {
            if (NextNotes.Simult)
                return (double)(MaxSimultCount - SimultCount) / (MaxSimultCount - MinSimultCount);

            return 1;
        }
    }

    class ComplementRule2 : TwoVoicesRule
    {
        static double MinSimultProportion = 0.4;
        static double MaxSimultProportion = 0.8;

        double SimultProportion;

        public override bool _IsApplicable()
        {
            int SimultCount = Notes.Count(n => n.Simult);
            SimultProportion = (double)SimultCount / Notes.Count;
            return (SimultCount > MinSimultProportion);
        }

        public override double Apply(TwoNotes NextNotes)
        {
            if (NextNotes.Simult)
                return (MaxSimultProportion - SimultProportion) / (MaxSimultProportion - MinSimultProportion);

            return 1;
        }
    }
}
