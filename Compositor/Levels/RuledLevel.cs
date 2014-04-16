using System;
using System.Collections.Generic;
using System.Linq;
using Compositor.Rules.Base;

namespace Compositor.Levels
{
    public interface IDeniable
    {
        IRule DeniedRule { get; set; }
        bool IsBanned { get; set; }
    }

    public abstract class RuledLevel : IFilterable
    {
        internal List<IRule> Rules;

        protected bool Filtered;
        public FreqsDict Freqs { get; protected set; }

        private const double MinimumFrequency = 0.01;

        protected RuledLevel()
        {
            Filtered = false;

            if (Rules == null)
            {
                Rules = new List<IRule>();
                AddRules();
            }

            Freqs = new FreqsDict();
        }

        private void AddRules()
        {
            var attrs = (RuleAttribute[])Attribute.GetCustomAttributes(GetType(), typeof(RuleAttribute));

            var expectedInterface = typeof(IRule);

            foreach (var t in attrs)
            {
                var ruleClass = t.Value;
                var interfaces = ruleClass.GetInterfaces();

                if (!interfaces.Contains(expectedInterface)) continue;

                var instance = (IRule)Activator.CreateInstance(ruleClass);
                Rules.Add(instance);
            }
        }

        public abstract void AddVariants();

        public FreqsDict Filter()
        {
            if (Filtered)
                return Freqs;

            AddVariants();

            var toFilter =
                from kv in Freqs.ToList()
                where kv.Value >= MinimumFrequency
                select kv.Key;

            Timer.Start("filter");

            Rules
                //cause strage problems
                //.AsParallel().ForAll(r =>
                .ForEach(r =>
            {
                r.Init(this);
                if (r.IsApplicable())
                    toFilter
                        //cause strage problems
                        //.AsParallel().ForAll(n =>
                        .ToList().ForEach(n =>
                    {
                        if (!(Freqs[n] >= MinimumFrequency)) return;

                        var freq = r.Apply(n);
#if TRACE_RULES
                        Console.WriteLine("Rule {0} to note {1} (@ {2}) = {3:F}",
                            r.GetType().Name, n.ToString(), n.TimeStart.Position, freq);*/
#endif 
                        Freqs[n] *= freq;

                        if (Math.Abs(freq) < MinimumFrequency)
                        {
                            n.DeniedRule = r;
                        }
                    });
            });

            Timer.Stop("filter");


            Filtered = true;
            return Freqs;
        }

        public void Ban(IDeniable what)
        {
            if (!Freqs.ContainsKey(what)) return;

            Freqs[what] = 0;
            what.IsBanned = true;
        }

        public IRule DeniedRule { get; set; }
        public bool IsBanned { get; set; }
    }
}
