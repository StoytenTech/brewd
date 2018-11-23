using System.Net;

namespace Brewd.ServiceDiscovery
{
    public class ServiceDiscoveryConfiguration
    {
        public IPAddress MulticastAddress { get; set; }
        public int port { get; set; }
    }
}