using Brewd.ServiceDiscovery;
using Microsoft.AspNetCore.Mvc;

namespace Brewd.Server
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ServicesController : ControllerBase
    {
        private readonly ServiceRegistry _serviceRegistry;

        public ServicesController(ServiceRegistry serviceRegistry)
        {
            _serviceRegistry = serviceRegistry;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var services = _serviceRegistry.GetServices();

            return new OkObjectResult(services);
        }
    }
}