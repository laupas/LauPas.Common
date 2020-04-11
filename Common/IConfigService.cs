namespace LauPas.Common
{
    /// <summary>
    /// The ConfigService
    /// </summary>
    public interface IConfigService
    {
        /// <summary>
        /// Gets a config value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Get<T>(string key, T defaultValue = default(T));

        /// <summary>
        /// Sets the location of the configfile.
        /// </summary>
        /// <param name="configFile"></param>
        void SetConfigFile(string configFile);

        /// <summary>
        /// Adds an value to the configService.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        void SetValue<T>(string key, T value);
    }
}