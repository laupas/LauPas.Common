using Microsoft.Extensions.Logging;

using static Bullseye.Targets;
using static LauPas.BuildExtensions;

namespace Build
{
    static class Build
    {
        private static bool push;
        static void Main(string[] args)
        {
            Target("default", DependsOn(new string[]
            {
                "init",
                "restore",
                "build", 
                "test", 
                "pack",
                "push"
            }));

            var buildNumber = string.Empty;

            Target("init", () =>
            {
                if (IsEnvVariableExisting("GITHUB_RUN_NUMBER"))
                {
                    Logger.LogInformation($"BuildAgent run");
                    SetConfigFile("BuildConfig.yml");
                    push = true;
                }
                else
                {
                    Logger.LogInformation($"Local run");
                    SetConfigFile(".BuildConfig.yml");
                }
                
                var versionPrefix = GetConfigValue("VERSION_PREFIX", "0.0");
                var runNumber = GetConfigValue("GITHUB_RUN_NUMBER", "0-local");
                buildNumber = $"{versionPrefix}.{runNumber}";
                Logger.LogInformation($"BuildNumber: {buildNumber}");
            });

            Target("restore", DependsOn("init"), () =>
            {
                RunProcess("dotnet", $"restore", workingDirectory: "./..");
            });

            Target("build", DependsOn("init"), () =>
            {
                RunProcess("dotnet", $"build -c Release -p:Version={buildNumber}", "./..");
            });

            Target("test", DependsOn("init"), () =>
            {
                RunProcess("dotnet", $"test -c Release --no-build", "./..");
            });

            Target("pack", DependsOn("init"), () =>
            {
                RunProcess("dotnet", $"pack . --no-build -c Release -p:Version={buildNumber}", "./..");
            });

            Target("push", DependsOn("init"), () =>
            {
                var nugetOrgToken = GetConfigValue<string>("NUGET_ORG_API_KEY");
                if (push)
                {
                    RunProcess("dotnet", $"nuget push ./**/*.nupkg --no-symbols true -s https://api.nuget.org/v3/index.json -k {nugetOrgToken}", "./..");
                }
                else
                {
                    Logger.LogWarning("No push because local run.");
                }
            });

            RunTargetAndExit(args);
        }
    }
}

