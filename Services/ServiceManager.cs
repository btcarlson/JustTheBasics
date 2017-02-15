using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace JustTheBasics
{
    public class ServiceManager
    {
        private List<IService> _registeredServices = new List<IService>();
        private DiscordSocketClient _client;

        public ServiceManager(DiscordSocketClient client)
        {
            _client = client;

            var allServices = Assembly.GetEntryAssembly().ExportedTypes
                .Where(x => x.GetInterfaces().Contains(typeof(IService)));

            foreach (var s in allServices)
            {
                var constructor = s.GetConstructors().FirstOrDefault(x 
                    => x.GetParameters().Count() == 1 && x.GetParameters().First().ParameterType == typeof(DiscordSocketClient));

                if (constructor == null)
                    throw new InvalidOperationException("IServices must have a single ctor that accepts only a DiscordSocketClient as a parameter");

                var service = (IService)constructor.Invoke(new object[1] { client });

                service.PreEnable(_client).GetAwaiter().GetResult();
                service.IsEnabled = true;

                _registeredServices.Add(service);
            }
        }

        public T GetService<T>() where T : class, IService
            => _registeredServices.FirstOrDefault(x => x is T) as T;

        public async Task<bool> TryDisable<T>() where T : class, IService
        {
            var service = GetService<T>();

            if (!service.IsEnabled)
                return false;

            await service.PreDisable(_client);
            return true;
        }

        public async Task<bool> TryEnable<T>() where T : class, IService
        {
            var service = GetService<T>();

            if (service.IsEnabled)
                return false;

            await service.PreEnable(_client);
            return true;
        }
    }
}