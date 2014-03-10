using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compositor.Rules
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RuleAttribute : System.Attribute 
    {
        private static List<Type> AppliedRules = new List<Type>();

        public static List<Type> GetAllRules()
        {
            return AppliedRules;
        }

        public Type Value { get; set; }

        public RuleAttribute(Type value)
        {
            if (typeof(Rule).IsAssignableFrom(value))
                AppliedRules.Add(value);
            
            this.Value = value;
        }
    }
}