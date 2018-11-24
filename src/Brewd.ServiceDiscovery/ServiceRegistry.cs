using System;
using System.Collections.Generic;
using System.Linq;

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
