namespace LauPas.AnsibleVault
{
    /// <summary>
    /// AnsibleVault
    /// </summary>
    public interface IAnsibleVault
    {
        /// <summary>
        /// Decode an Ansible Vault value to a string
        /// </summary>
        /// <param name="password">The password</param>
        /// <param name="input">The input to decode</param>
        /// <returns></returns>
        string Decode(string password, string input);
        
        /// <summary>
        /// Encode a string to an Ansible Vault string
        /// </summary>
        /// <param name="password">The password</param>
        /// <param name="input">The value to encode</param>
        /// <returns></returns>
        string Encode(string password, string input);
    }
}