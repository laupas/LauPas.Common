namespace LauPas.AnsibleVault
{
    public interface IAnsibleVault
    {
        string Decode(string password, string input);
        string Encode(string password, string input);
    }
}