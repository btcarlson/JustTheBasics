using Discord.Commands;
using Discord.WebSocket;
using NLog;
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
        private List<ServiceBase> _registeredServices = new List<ServiceBase>();
        private DiscordSocketClient _client;
        private Logger _logger = LogManager.GetLogger("ServiceManager");

        public ServiceManager(DiscordSocketClient client)
        {
            _client = client;

            var allServices = Assembly.GetEntryAssembly().ExportedTypes
                .Where(x => x.GetTypeInfo().BaseType == typeof(ServiceBase));

            foreach (var s in allServices)
            {
                var constructor = s.GetConstructors().FirstOrDefault(c => c.GetParameters().Count() == 0);

                var service = (ServiceBase)constructor.Invoke(new object[0]);

                service.TryEnable().GetAwaiter().GetResult();

                _registeredServices.Add(service);

                _logger.Info($"Registered and Enabled Service {s.Name}");
            }
        }

        public TService GetService<TService>() where TService : ServiceBase
            => _registeredServices.FirstOrDefault(x => x is TService) as TService;

        public async Task<bool> TryEnable<TService>() where TService : ServiceBase
            => await (_registeredServices.FirstOrDefault(x => x is TService) as TService).TryEnable();

        public async Task<bool> TryDisable<TService>() where TService : ServiceBase
            => await (_registeredServices.FirstOrDefault(x => x is TService) as TService).TryDisable();
    }
}