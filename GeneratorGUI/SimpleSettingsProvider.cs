using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using Compositor.Rules.Base;

namespace GeneratorGUI
{
    class SimpleSettingsProvider: ISettingsProvider
    {
        public Dictionary<String, object> Settings { get; private set; }
        private readonly string _fname;

        public SimpleSettingsProvider(string fname)
        {
            Settings = new Dictionary<string, object>();
            _fname = fname;
            var split = new[] { '=' };


            try
            {
                var f = File.OpenText(fname);
                while (!f.EndOfStream)
                {
                    var line = f.ReadLine();
                    var splitted = line.Split(split, 3);
                    AddParam(splitted);
                }

                f.Close();
            }
            catch (FileNotFoundException)
            {
            }
        }

        private void AddParam(string[] splitted)
        {
            var name = splitted[0];
            var typeName = splitted[1];
            var value = splitted[2];

            object val;

            switch (typeName)
            {
                case "Int32":
                    val = int.Parse(value);
                    break;
                case "float":
                    val = float.Parse(value);
                    break;
                case "Boolean":
                    val = bool.Parse(value);
                    break;
                default:
                    throw new Exception("Invalid settings file");
            }

            Settings[name] = val;
        }

        public bool HasSetting(string rule, string param)
        {
            return Settings.ContainsKey(rule + "." + param);
        }

        public object GetSetting(string rule, string param)
        {
            return Settings[rule + "." + param];
        }

        public void SetSetting(string rule, string param, object value)
        {
            Settings[rule + "." + param] = value;
        }

        public void Save()
        {
            var f = new StreamWriter(_fname, false);

            foreach (var kv in Settings)
                f.WriteLine("{0}={1}={2}", kv.Key, kv.Value.GetType().Name, kv.Value);

            f.Close();
        }
    
    }
}
