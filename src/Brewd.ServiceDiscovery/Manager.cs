using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Brewd.ServiceDiscovery
{
    public class Manager
    {
        private readonly IPAddress _multicastAddress;
        private readonly int _port;
        
        private readonly ServiceRegistry _serviceRegistry;

        public Manager(ServiceDiscoveryConfiguration configuration, ServiceRegistry serviceRegistry)
        {
            _multicastAddress = configuration.MulticastAddress;
            _port = configuration.port;
            
            _serviceRegistry = serviceRegistry;
        }

        public Task Start(CancellationToken cancellationToken)
        {
            var udpClient = new UdpClient(_port);
            udpClient.JoinMulticastGroup(_multicastAddress);

            var buffer = new BufferBlock<byte[]>();

            var receive = Receive(udpClient, buffer, cancellationToken);
            var monitor = Monitor(buffer, cancellationToken);
            var scavenger = Scavenger(cancellationToken);

            return Task.WhenAll(receive, monitor, scavenger).ContinueWith(t =>
            {
                udpClient.DropMulticastGroup(_multicastAddress);
                buffer.Complete();
                
                udpClient.Close();
                udpClient.Dispose();
            });
        }

        private static async Task Receive(UdpClient client, ITargetBlock<byte[]> buffer, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var datagram = await client.ReceiveAsync();
                await buffer.SendAsync(datagram.Buffer, cancellationToken);
            }
        }
        
        private async Task Monitor(ISourceBlock<byte[]> buffer, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (!await buffer.OutputAvailableAsync(cancellationToken)) 
                    continue;
                
                var datagram = await buffer.ReceiveAsync(cancellationToken);

                if (AvailableService.TryParse(datagram, out var service))
                {
                    _serviceRegistry.AddOrUpdate(service);
                }
            }
        }

        private async Task Scavenger(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                _serviceRegistry.Scavenge();
                await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
            }
        }
    }
}