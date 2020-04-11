using System;
using System.Collections.Generic;
using System.Linq;
using LauPas.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Common.Tests
{
    [TestClass]
    public abstract class BaseTest
    {
//        protected Mock<IProcessHelper> ProcessMock { get; set; }
        private readonly MockRepository mockRepository = new MockRepository(MockBehavior.Default);
        private readonly List<Mock> mocks = new List<Mock>();

        protected void StartAllServices()
        {
            var starter = Starter.Create().AddAssembly(this.GetType().Assembly);
            this.Start(starter);
        }

        protected void StartAllServices<TModule>() where TModule : IModule
        {
            var starter = Starter.Create().AddAssembly(this.GetType().Assembly).AddModule<TModule>();
            this.Start(starter);
        }
        
        protected void StartAllServices<TModule1, TModule2>() where TModule1 : IModule where TModule2 : IModule
        {
            var starter = Starter.Create().AddAssembly(this.GetType().Assembly).AddModule<TModule1>().AddModule<TModule2>();
            this.Start(starter);
        }
        
        private void Start(IStarterBuilder starter)
        {
            Environment.SetEnvironmentVariable("VERBOSE", "true");
            starter.Build(this.Arguments.ToArray(), collection =>
            {
                foreach (var mock in this.mocks)
                {
                    collection
                        .Where(r => r.ServiceType == mock.GetType().GenericTypeArguments[0])
                        .ToList()
                        .ForEach(c => { collection.Remove(c); });
                    collection.AddSingleton(mock.GetType().GenericTypeArguments[0], mock.Object);
                }
            });
        }

        protected Mock<T> RegisterMock<T>() where T : class
        {
            var mock = this.CreateMock<T>();
            this.mocks.Add(mock);
            return mock;
        }

        protected Mock<T> CreateMock<T>() where T : class
        {
            var mock = this.mockRepository.Create<T>();
            return mock;
        }

        protected List<string> Arguments { get; set; } = new List<string>();

    }
}