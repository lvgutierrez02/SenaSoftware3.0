using Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Seguimiento.APi
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        public WeatherForecastController(ILoggerManager logger)
        {
            _logger = logger;
        }
        [HttpGet]
        public IEnumerable<string> Get()
        {
            _logger.LogInfo("Aqu� hay un mensaje de informaci�n del controlador.");
            _logger.LogDebug("Aqu� est� el mensaje de depuraci�n del controlador.");
            _logger.LogWarn("Aqu� hay un mensaje de advertencia del controlador.");
            _logger.LogError("Aqu� hay un mensaje de error del controlador.");
            return new string[] { "value1", "value2" };
        }
    }
}