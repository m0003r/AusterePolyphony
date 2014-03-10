using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compositor.Rules
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class ParamAttribute : System.Attribute
    {
        public string ParamName;
        public Type Type;
        public object Default;
        public string Desc;

        public ParamAttribute(string name, Type type, object def, string desc)
        {
            if ( !type.IsInstanceOfType(def))
                throw new ArgumentException();

            this.ParamName = name;
            this.Type = type;
            this.Default = def;
            this.Desc = desc;
        }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    class RuleNameAttribute : System.Attribute
    {
        public string ParamName { get; set; }
        public RuleNameAttribute(string name)
        {
            name = ParamName;
        }
    }


}
