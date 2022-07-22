using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
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
        [HttpGet("{id}", Name = "GetEmployeeForCompany")]
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



        [HttpPost]
        public IActionResult CreateEmployeeForCompany(Guid companyId, [FromBody] EmployeeForCreationDto employee)
        {
            if (employee == null)
            {
                _logger.LogError("EmployeeForCreationDto object sent from client is null.");
                return BadRequest("EmployeeForCreationDto object is null");
            }
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the EmployeeForCreationDto object");
                return UnprocessableEntity(ModelState);
            }
            var company = _repository.Company.GetCompany(companyId, trackChanges: false);
            if (company == null)
            {
                _logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");
                return NotFound();
            }
            var employeeEntity = _mapper.Map<Employee>(employee);
            _repository.Employee.CreateEmployeeForCompany(companyId, employeeEntity);
            _repository.Save();
            var employeeToReturn = _mapper.Map<EmployeeDto>(employeeEntity);
            return CreatedAtRoute("GetEmployeeForCompany", new { companyId, id = employeeToReturn.Id }, employeeToReturn);
            //la declaración de devolución, que ahora tiene dos parámetros para el objeto anónimo.
        }




        [HttpPut("{id}")]
        public IActionResult UpdateEmployeeForCompany(Guid companyId, Guid id, [FromBody] EmployeeForUpdateDto employee)
        {
            if (employee == null)
            {
                _logger.LogError("EmployeeForUpdateDto object sent from client is null.");
                return BadRequest("EmployeeForUpdateDto object is null");
            }
            var company = _repository.Company.GetCompany(companyId, trackChanges: false);
            if (company == null)
            {
                _logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");
                return NotFound();
            }

            //El parámetro trackChanges se establece en true para employeeEntity. Eso es porque queremos que EF Core realice un seguimiento de los cambios 
            //en esta entidad. Esto significa que tan pronto como cambiemos cualquier propiedad en esta entidad, EF Core establecerá el estado de esa entidad 
            //en Modificado

            var employeeEntity = _repository.Employee.GetEmployee(companyId, id, trackChanges: true);
            if (employeeEntity == null)
            {
                _logger.LogInfo($"Employee with id: {id} doesn't exist in the database.");
                return NotFound();
            }
            _mapper.Map(employee, employeeEntity);// cambiando así el estado del objeto employeeEntity a Modificado
            //Debido a que nuestra entidad tiene un estado modificado, basta con llamar al método Save sin ninguna acción de actualización adicional.
            //Tan pronto como llamemos al método Guardar, nuestra entidad se actualizará en la base de datos.
            _repository.Save();
            return NoContent();// devolvemos el estado 204 NoContent.
        }



        [HttpDelete("{id}")]
        public IActionResult DeleteEmployeeForCompany(Guid companyId, Guid id)//Recopilamos el companyId de la ruta raíz y el id del empleado del argumento pasado
        {
            var company = _repository.Company.GetCompany(companyId, trackChanges: false);
            if (company == null)//Tenemos que comprobar si la empresa existe
            {
                _logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");
                return NotFound();
            }
            var employeeForCompany = _repository.Employee.GetEmployee(companyId, id, trackChanges: false); // verificamos la entidad del empleado
            if (employeeForCompany == null)
            {
                _logger.LogInfo($"Employee with id: {id} doesn't exist in the database.");
                return NotFound();
            }
            //eliminamos a nuestro empleado
            _repository.Employee.DeleteEmployee(employeeForCompany);
            _repository.Save();//Guardamos cambios
            return NoContent();//devolvemos el método NoContent(), que devuelve el código de estado 204 Sin contenido (No Content).
        }
    }
}
