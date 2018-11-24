using System.IO;
using Xunit;

namespace Brewd.ServiceDiscovery.Tests
{
    public class ParserTests
    {
        [Fact]
        public void CanParse()
        {
            var buffer = File.ReadAllBytes(@"dump.dat");
            var service = AvailableService.Parse(buffer);
            
            Assert.NotNull(service);

            Assert.Equal("power-01", service.DeviceId);
            Assert.Equal(1, service.DeviceType);
            Assert.Equal((uint)1, service.NumberOfCapabilities);
            Assert.Equal((uint)1, service.ProtocolVersion);
            Assert.Equal("192.168.2.30", service.IpAddress.ToString());
            Assert.Equal("84F3EBA73955", service.MacAddress.ToString());
        }
    }
}
