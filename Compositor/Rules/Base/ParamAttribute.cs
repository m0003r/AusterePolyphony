using System;

namespace Compositor.Rules.Base
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class ParamAttribute : Attribute
    {
        public string ParamName;
        public Type Type;
        public object Default;
        public string Desc;

        public ParamAttribute(string name, Type type, object def, string desc)
        {
            if ( !type.IsInstanceOfType(def))
                throw new ArgumentException();

            ParamName = name;
            Type = type;
            Default = def;
            Desc = desc;
        }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    class RuleNameAttribute : Attribute
    {
        public string ParamName { get; set; }
        public RuleNameAttribute(string name)
        {
            ParamName = name;
        }
    }


}
