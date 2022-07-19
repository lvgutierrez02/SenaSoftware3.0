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
            _logger.LogInfo("Aquí hay un mensaje de información del controlador.");
            _logger.LogDebug("Aquí está el mensaje de depuración del controlador.");
            _logger.LogWarn("Aquí hay un mensaje de advertencia del controlador.");
            _logger.LogError("Aquí hay un mensaje de error del controlador.");
            return new string[] { "value1", "value2" };
        }
    }
}