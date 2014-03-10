using System;
using System.Collections.Generic;

namespace Compositor.Rules.Base
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RuleAttribute : Attribute 
    {
        private static readonly List<Type> AppliedRules = new List<Type>();

        public static List<Type> GetAllRules()
        {
            return AppliedRules;
        }

        public Type Value { get; set; }

        public RuleAttribute(Type value)
        {
            if (typeof(IRule).IsAssignableFrom(value))
                AppliedRules.Add(value);
            
            Value = value;
        }
    }
}