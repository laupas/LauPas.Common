using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Serialization.NamingConventions;

namespace LauPas.Common
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Search keys in a list with several cases
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public static string FindEntryWithUndefinedCasesInList(this IEnumerable<string> keys, string keyName)
        {
            var keyNameToSearch = keyName;
            var enumerable = keys.ToList();
            if (enumerable.Any(k => k == keyNameToSearch))
            {
                return keyNameToSearch;
            }

            keyNameToSearch = keyName.ToLowerInvariant();
            if (enumerable.Any(k => k == keyNameToSearch))
            {
                return keyNameToSearch;
            }

            keyNameToSearch = keyName.ToUpperInvariant();
            if (enumerable.Any(k => k == keyNameToSearch))
            {
                return keyNameToSearch;
            }

            keyNameToSearch = UnderscoredNamingConvention.Instance.Apply(keyName);
            if (enumerable.Any(k => k == keyNameToSearch))
            {
                return keyNameToSearch;
            }

            keyNameToSearch = PascalCaseNamingConvention.Instance.Apply(keyName);
            if (enumerable.Any(k => k == keyNameToSearch))
            {
                return keyNameToSearch;
            }

            foreach (var key in enumerable)
            {
                if (key.ToUpperInvariant() == keyName)
                {
                    return key;
                }
            }

            return string.Empty;
        }
    }
}