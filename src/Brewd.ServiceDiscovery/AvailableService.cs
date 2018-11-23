using System.Net;
using System.Net.NetworkInformation;

namespace Brewd.ServiceDiscovery
{
    public class AvailableService
    {
        /*
         char     type[7];
         uint8_t  version;
         uint8_t  mac[6];
         uint32_t ip;
         uint16_t device_type;
         char     device_id[33];
         uint8_t  capabilities;
         */
        
        public uint ProtocolVersion { get; internal set; }
        
        public ushort DeviceType { get; internal set; }
        public string DeviceId { get; internal set; }
        public uint NumberOfCapabilities { get; internal set; }
        
        internal byte[] MacAddress { get; set; }
        internal long IpAddress { get; set; }
        
        public PhysicalAddress GetMacAddress() => new PhysicalAddress(MacAddress);
        public IPAddress GetIpAddress() => new IPAddress(IpAddress);

    }
}