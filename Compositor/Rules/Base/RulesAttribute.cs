using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compositor.Rules
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple=true)]
    class RuleAttribute : System.Attribute 
    {
        public Type Value { get; set; }

        public RuleAttribute(Type value)
        {
            this.Value = value;
        }
    }
}