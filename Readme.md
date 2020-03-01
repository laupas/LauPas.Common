# LauPas.Common
Contains a Starter for an IOC Container and a simple ConfigService

# LauPas.BuildExtensions
Contains some helpers for build scripts

## Starter
```
static void Main(string[] args)
{
    Starter
        .Create()
        .AddAssembly<BuildExtensions>() // option 1
        .AddAssembly(this) // option 2
        .Build(args);

    var service = Starter.Get.Resolve<T>();
}
```


## ConfigService
Can read values from a yaml file or from environment variables
A value from the config file can be overwritten from the commandline

``` c#
var configService = Starter.Get.Resolve<IConfigService>();
configService.SetConfigFile("config.yml");

// Can be overwritten with setting the environment variable "KEY1"
var value1 = configService.Get<string>("key1");

// Can be overwritten with setting the environment variable "KEY2"
var value2 = configService.Get<string>("key2", "defaultValue");
```

config.yml:
``` yaml
---
key1: value1
key2: value2
```

## How to get started
Create a new net core console project (for ex: build.csproj in folder build)
```
using System;
using LauPas.Common;
using Microsoft.Extensions.Logging;

using static Bullseye.Targets;
using static LauPas.BuildExtensions;

namespace Build
{
    static class Build
    {
        static void Main(string[] args)
        {
            Target("default", DependsOn(new string[]
            {
                "init",
                "restore",
                "build", 
                "test", 
                "pack"
            }));

            var buildNumber = string.Empty;

            Target("init", () =>
            {
                if (IsEnvVariableExisting("GITHUB_RUN_NUMBER"))
                {
                    Logger.LogInformation($"BuildAgent run");
                    SetConfigFile("BuildConfig.yml");
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
                var nugetOrgToken = GetConfigValue<string>("NUGET_ORG_API_KEY");

                RunProcess("dotnet", $"pack . --no-build -c Release -p:Version={buildNumber}", "./..");
                RunProcess("dotnet", $"nuget push ./**/*.nupkg --no-symbols true -s https://api.nuget.org/v3/index.json -k {nugetOrgToken}", "./..");
            });
            
            RunTargetAndExit(args);
        }
    }
}
```