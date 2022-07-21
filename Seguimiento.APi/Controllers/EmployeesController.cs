using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Seguimiento.API.Controllers
{
    //Como dijimos, un solo empleado no puede existir sin una entidad empresa
    //y esto es exactamente lo que estamos exponiendo a través de este URI.
    //Para obtener un empleado o empleados de la base de datos, debemos especificar el parámetro companyId
    [Route("api/companies/{companyId}/employees")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        //Inyectamos los servicios de registro, autoMapper y el  repositorio dentro del constructor
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        public EmployeesController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository; _logger = logger; _mapper = mapper;
        }



        [HttpGet]
        public IActionResult GetEmployeesForCompany(Guid companyId)
        {
            //Como puede ver, tenemos el parámetro companyId en nuestra acción y este parámetro se mapeará desde la ruta principal.
            //Por ese motivo, no lo colocamos en el atributo [HttpGet] como hicimos con la acción GetCompany.
            var company = _repository.Company.GetCompany(companyId, trackChanges: false);
            if (company == null)
            {
                _logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");
                return NotFound();
            }
            var employeesFromDb = _repository.Employee.GetEmployees(companyId, trackChanges: false);
            var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employeesFromDb);
            return Ok(employeesDto);
        }

        //api/companies/{companyId}/employees/{id}
        [HttpGet("{id}")]
        public IActionResult GetEmployeeForCompany(Guid companyId, Guid id)
        {
            var company = _repository.Company.GetCompany(companyId, trackChanges: false);
            if (company == null)
            {
                _logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");
                return NotFound();
            }
            var employeeDb = _repository.Employee.GetEmployee(companyId, id, trackChanges: false);
            if (employeeDb == null)
            {
                _logger.LogInfo($"Employee with id: {id} doesn't exist in the database.");
                return NotFound();
            }
            var employee = _mapper.Map<EmployeeDto>(employeeDb);
            return Ok(employee);
        }
    }
}
