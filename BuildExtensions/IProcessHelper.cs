namespace LauPas
{
    public interface IProcessHelper
    {
        int Run(string command, string args = null, string workingDirectory = "", bool throwOnError = true, bool noEcho = false);
        string Read(string command, string args = null, string workingDirectory = "", bool throwOnError = true, bool noEcho = false);
    }
}