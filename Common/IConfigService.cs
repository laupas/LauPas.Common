namespace LauPas.Common
{
    public interface IConfigService
    {
        T Get<T>(string key, T defaultValue = default(T));
        void SetConfigFile(string configFile);
    }
}