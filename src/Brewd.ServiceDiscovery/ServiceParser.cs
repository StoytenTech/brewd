using System;
using System.IO;
using System.Text;

namespace Brewd.ServiceDiscovery
{
    internal class ServiceParser
    {
        private const string MessageType = "STDISC";

        public static AvailableService Parse(byte[] buffer)
        {
            using(var stream = new MemoryStream(buffer))
            using (var reader = new BinaryReader(stream, Encoding.ASCII))
            {
                var header = reader.ReadChars(7);
                var s = header.ToStringWithoutTerminator();
                
                if (!string.Equals(MessageType, s))
                {
                    Console.WriteLine("Got non-STDISC message");
                    return null;
                }

                return new AvailableService
                {
                    ProtocolVersion = reader.ReadByte(),
                    MacAddress = reader.ReadBytes(6),
                    IpAddress = reader.ReadUInt32(),
                    DeviceType = reader.ReadUInt16(),
                    DeviceId = reader.ReadChars(33).ToStringWithoutTerminator(),
                    NumberOfCapabilities = reader.ReadByte()
                };
            }
        }
    }
}