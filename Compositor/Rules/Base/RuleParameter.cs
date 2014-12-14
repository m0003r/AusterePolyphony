using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compositor.Rules.Base
{
    public class RuleParameter
    {
        public string Name;

        public Type Type;
        private object _value;

        public object DefaultValue;
        public object Value {
            get { return _value; }
            set {
                if (Type.IsInstanceOfType(value))
                    _value = value;
                else
                    throw new ArgumentException("Invalid value type");
            }
        }

        public string Description;

        public RuleParameter(RuleParamAttribute p)
        {
            Name = p.ParamName;
            Type = p.Type;
            DefaultValue = p.Default;
            _value = p.Default;
            Description = p.Desc;
        }
    }

}
