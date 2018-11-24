using System;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

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
        private const string MessageType = "STDISC";
        
        private byte[] _macAddress;
        private long   _ipAddress;
        
        public uint   ProtocolVersion      { get; private set; }
        public ushort DeviceType           { get; private set; }
        public string DeviceId             { get; private set; }
        public uint   NumberOfCapabilities { get; private set; }
        
        public PhysicalAddress MacAddress => new PhysicalAddress(_macAddress);
        public IPAddress       IpAddress  => new IPAddress(_ipAddress);

        public static bool TryParse(byte[] buffer, out AvailableService service)
        {
            service = Parse(buffer);
            return service != null;
        }

        public static AvailableService Parse(byte[] buffer)
        {
            using(var stream = new MemoryStream(buffer))
            using (var reader = new BinaryReader(stream, Encoding.ASCII))
            {
                var header = reader.ReadChars(7).ToStringWithoutTerminator();
                
                if (!string.Equals(MessageType, header))
                {
                    Console.WriteLine("Got non-STDISC message");
                    return null;
                }

                return new AvailableService
                {
                    ProtocolVersion = reader.ReadByte(),
                    _macAddress = reader.ReadBytes(6),
                    _ipAddress = reader.ReadUInt32(),
                    DeviceType = reader.ReadUInt16(),
                    DeviceId = reader.ReadChars(33).ToStringWithoutTerminator(),
                    NumberOfCapabilities = reader.ReadByte()
                };
            }
        }
    }
}