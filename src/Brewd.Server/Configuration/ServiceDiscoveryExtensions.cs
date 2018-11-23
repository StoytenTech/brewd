using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Brewd.ServiceDiscovery;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Brewd.Server.Configuration
{
    public static class ServiceDiscoveryExtensions
    {
        public static void AddServiceDiscovery(this IServiceCollection services)
        {
            services.AddSingleton(new ServiceDiscoveryConfiguration
            {
                MulticastAddress = IPAddress.Parse("239.100.0.1"),
                port = 5900
            });
            
            services.AddSingleton<ServiceRegistry>();
            services.AddSingleton<Manager>();
            
            services.AddHostedService<ServiceDiscoveryAdapter>();
        }

        private class ServiceDiscoveryAdapter : IHostedService
        {
            private readonly Manager _manager;
            private CancellationTokenSource _executingCancellationToken;
            private Task _executingTask;

            public ServiceDiscoveryAdapter(Manager manager)
            {
                _manager = manager;
            }

            public Task StartAsync(CancellationToken cancellationToken)
            {
                _executingCancellationToken = new CancellationTokenSource();
                _executingTask = _manager.Start(_executingCancellationToken.Token);

                return Task.CompletedTask;
            }

            public Task StopAsync(CancellationToken cancellationToken)
            {
                _executingCancellationToken.Cancel();
                return _executingTask;
            }
        }
    }
}