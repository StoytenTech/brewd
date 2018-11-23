using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;

namespace Brewd.ServiceDiscovery
{
    public class ServiceRegistry
    {
        private readonly Dictionary<string, ServiceWrapper> _services = new Dictionary<string, ServiceWrapper>();

        public IEnumerable<AvailableService> GetServices() => _services.Values.Select(s => s.Service);
        
        public void AddOrUpdate(AvailableService service)
        {
            _services[service.DeviceId] = new ServiceWrapper(service);
        }

        public void Scavenge()
        {
            _services.Values
                .Where(wrapper => wrapper.Expired(DateTime.Now))
                .Select(w => w.Service.DeviceId)
                .ToList()
                .ForEach(s => _services.Remove(s));
        }
        
        public void PrintServices()
        {
            Console.WriteLine("==================");
            Console.WriteLine("AVAILABLE SERVICES");
            Console.WriteLine("==================");

            foreach (var service in GetServices())
            {
                var macAddress = new PhysicalAddress(service.MacAddress);
                var ipAddress = new IPAddress(service.IpAddress);
                
                Console.WriteLine($"Device Id:    {service.DeviceId}");
                Console.WriteLine($"Protocol:     {service.ProtocolVersion}");
                Console.WriteLine($"Device Type:  {service.DeviceType}");
                Console.WriteLine($"IP Address:   {service.GetIpAddress()}");
                Console.WriteLine($"MAC Address:  {service.GetMacAddress()}");
                Console.WriteLine($"Capabilities: {service.NumberOfCapabilities}");
                
                Console.WriteLine("--------------------------------------------");
                Console.WriteLine();
            }
        }

        private class ServiceWrapper
        {
            public DateTime LastSeen { get; }
            public AvailableService Service { get; }

            public bool Expired(DateTime now) => now - LastSeen >= TimeSpan.FromMinutes(1);
            
            public ServiceWrapper(AvailableService service)
            {
                LastSeen = DateTime.Now;
                Service = service;
            }
        }
    }
}
