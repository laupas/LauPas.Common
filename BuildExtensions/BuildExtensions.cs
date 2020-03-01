using System;
using LauPas.Common;
using Microsoft.Extensions.Logging;
using static Bullseye.Targets;

namespace LauPas
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class BuildExtensions
    {
        public static ILogger Logger { get; private set; }

        public static void RunTargetAndExit(string[] args)
        {
            Starter
                .Create()
                .AddAssembly<BuildExtensions>()
                .Build(args);
            
            Logger = Starter.Get.Resolve<ILoggerFactory>().CreateLogger("Build");

            if (args.Length == 0)
            {
                RunTargetsAndExit(new[] {"default"});
            }
            else
            {
                RunTargetsAndExit(new[] {args[0]});
            }
        }

        public static bool IsEnvVariableExisting(string name)
        {
            var value = Environment.GetEnvironmentVariable(name.ToUpperInvariant());
            return !string.IsNullOrEmpty(value);
        }
        
        public static T Resolve<T>()
        {
            return Starter.Get.Resolve<T>();
        }

        public static T GetConfigValue<T>(string key, T defaultValue = default(T))
        {
            return Resolve<IConfigService>().Get(key, defaultValue);
        }

        public static void SetConfigFile(string configFile)
        {
            Resolve<IConfigService>().SetConfigFile(configFile);
        }

        public static void RunProcess(string command, string args = null, string workingDirectory = null, bool throwOnError = true, bool noEcho = false)
        {
            Resolve<IProcessHelper>().Run(command, args, workingDirectory, throwOnError, noEcho);
        }

        public static string ReadProcess(string command, string args = null, string workingDirectory = null, bool throwOnError = true, bool noEcho = false)
        {
            return Resolve<IProcessHelper>().Read(command, args, workingDirectory, throwOnError, noEcho);
        }

        public void LogInformation(string message)
        {
            Logger.LogInformation(message);
        }
    }
}