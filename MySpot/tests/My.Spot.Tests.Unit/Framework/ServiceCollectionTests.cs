﻿using Microsoft.Extensions.DependencyInjection;

namespace My.Spot.Tests.Unit.Framework
{
    public class ServiceCollectionTests
    {

        [Fact]
        public void test()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IMessenger, Messenger>();
            serviceCollection.AddTransient<IMessenger,Messenger>();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var messenger = serviceProvider.GetService<IMessenger>();
        }

        private interface IMessenger
        {

            void Send();
        }
        private class Messenger : IMessenger
        {
            private readonly Guid _id = Guid.NewGuid();
            public void Send() => Console.WriteLine($"Sending a message... [{_id}]");
        }
    }
}
