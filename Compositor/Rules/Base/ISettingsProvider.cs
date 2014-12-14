namespace Compositor.Rules.Base
{
    public interface ISettingsProvider
    {
        bool HasSetting(string rule, string param);
        object GetSetting(string rule, string param);
        void SetSetting(string rule, string param, object value);
    }
}