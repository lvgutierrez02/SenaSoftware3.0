using Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Seguimiento.APi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly IRepositoryManager _repository;
        public WeatherForecastController(ILoggerManager logger, IRepositoryManager repository)
        {
            _logger = logger;
            _repository = repository;
        }
        [HttpGet]
        public IEnumerable<string> Get()
        {
            _logger.LogInfo("Here is info message from the controller.");
            _logger.LogDebug("Here is debug message from the controller.");
            _logger.LogWarn("Here is warn message from the controller.");
            _logger.LogError("Here is error message from the controller.");

            //_repository.Company.AnyMethodFromCompanyRepository(); 
            //_repository.Employee.AnyMethodFromEmployeeRepository();
            return new string[] { "value1", "value2" };
        }
    }
}
