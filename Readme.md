# LauPas.Common
Contains a Starter for an IOC Container and a simple ConfigService

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

# LauPas.Azure
Contains some helpers for Azure

## Azure Vault
Can be used to read values from a Azure Vault


``` c#
var vaultService = Starter.Get.Resolve<IAzureVault>();
var secret = await vaultService.GetSecretAsync("secretName");
```


needed vaules in config.yml:
``` yaml
---
clientid: client id
clientsecret: client secret
vaulturl: url to the vault service
```

# LauPas.AnsibleVault


## Install
[Nuget.org](https://www.nuget.org/packages/AnsibleVault/)

```
dotnet add package AnsibleVault
```

## Encode
```
var input = "$ANSIBLE_VAULT;1.1;AES256" + Environment.NewLine +
            "33643238353032653563323633343066643261306333613130316266323135663864303465333063" + Environment.NewLine +
            "3030393132643338353237366462346634666566376262610a323838616631383065323030326565" + Environment.NewLine +
            "32353839323366323139303232343032366236373064613933663062663830616330353631646462" + Environment.NewLine +
            "3738643964393831650a383261633135376166626437356234643764363537363533393637343164" + Environment.NewLine +
            "64643231323565396135353962633966623361343030623464633436666432353462";

IAnsibleVault ansibleVault = new AnsibleVault()
var value = ansibleVault.Decode("1234", input);
```

## Decode
```
IAnsibleVault ansibleVault = new AnsibleVault()
var vaultValue = ansibleVault.Encode("1234", "abcd");
```

# LauPas.BuildExtensions
Contains some helpers for build scripts

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