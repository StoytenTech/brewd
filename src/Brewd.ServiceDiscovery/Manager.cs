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

            return Task.WhenAll(receive, monitor).ContinueWith(t =>
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

                var service = ServiceParser.Parse(datagram);
                if (service != null)
                {
                    _serviceRegistry.AddOrUpdate(service);
                }
            }
        }
    }
}