using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Seguimiento.API.ModelBinders;

namespace Seguimiento.API.Controllers
{
    [Route("api/companies")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        //Inyectamos los servicios de registro, autoMapper y el  repositorio dentro del constructor
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        public CompaniesController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet(Name = "GetCompanies"), Authorize]
        public IActionResult GetCompanies()
        {

            var companies = _repository.Company.GetAllCompanies(trackChanges: false);
            var companiesDto = _mapper.Map<IEnumerable<CompanyDto>>(companies);
            return Ok(companiesDto);

            //Eliminamos el try para utilizar el manejo de errores globales

            //try
            //{
            //    //obtener los datos de la clase de repositorio
            //    var companies = _repository.Company.GetAllCompanies(trackChanges: false); //Vamos a usarlo para mejorar el rendimiento de nuestras consultas de solo lectura
            //    //Consulta linq
            //    //var companiesDto = companies.Select(c => new CompanyDto { Id = c.Id, Name = c.Name, FullAddress = string.Join(' ', c.Address, c.Country) }).ToList();
            //    //consulta AutoMapper
            //    var companiesDto = _mapper.Map<IEnumerable<CompanyDto>>(companies);
            //    return Ok(companiesDto);// devuelve todas las empresas y también el código de estado 200, que significa OK

            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError($"Something went wrong in the {nameof(GetCompanies)} action {ex}");//registrar los mensajes 
            //    return StatusCode(500, "Internal server error");//Si ocurre una excepción, devolveremos el error interno del servidor con el código de estado 500.
            //}
        }


        [HttpGet(Name = "GetCompanies"), Authorize]
        public IActionResult GetCompany(Guid id) //buscamos una sola company de la base de datos
        {
            var company = _repository.Company.GetCompany(id, trackChanges: false);
            if (company == null)
            {
                //Si no existe, usamos el método NotFound para devolver un código de estado 404
                _logger.LogInfo($"Company with id: {id} doesn't exist in the database.");
                return NotFound();
            }
            else
            {
                // Si existe una company en la base de datos, simplemente la asignamos al tipo CompanyDto y se la devolvemos al cliente
                var companyDto = _mapper.Map<CompanyDto>(company);
                return Ok(companyDto);
            }
        }




        


        [HttpPost]
        public IActionResult CreateCompany([FromBody] CompanyForCreationDto company)//el parámetro de la empresa que proviene del cliente, lo recopilamos del cuerpo de la solicitud.
        {
            if (company == null)
            {
                _logger.LogError("CompanyForCreationDto object sent from client is null.");
                return BadRequest("CompanyForCreationDto object is null");
            }
            var companyEntity = _mapper.Map<Company>(company);
            _repository.Company.CreateCompany(companyEntity);// llamamos al método de repositorio para la creación 
            _repository.Save();// llamamos al método Save() para guardar la entidad en la base de datos.
            var companyToReturn = _mapper.Map<CompanyDto>(companyEntity);//asignamos la entidad de la empresa al objeto DTO de la empresa para devolverlo al cliente.
            return CreatedAtRoute("CompanyById", new { id = companyToReturn.Id }, companyToReturn);
            //CreatedAtRoute devolverá un código de estado 201, que significa Creado. Además, completará el cuerpo de la respuesta con el nuevo objeto de la empresa,
            //así como el atributo Ubicación dentro del encabezado de la respuesta con la dirección para recuperar esa empresa. Necesitamos proporcionar el nombre de la acción,
            //donde podemos recuperar la entidad creada.
        }


        [HttpGet("collection/({ids})", Name = "CompanyCollection")]
        public IActionResult GetCompanyCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
        {
            if (ids == null)
            {
                _logger.LogError("Parameter ids is null");
                return BadRequest("Parameter ids is null");
            }
            var companyEntities = _repository.Company.GetByIds(ids, trackChanges: false);
            if (ids.Count() != companyEntities.Count())
            {
                _logger.LogError("Some ids are not valid in a collection");
                return NotFound();
            }
            var companiesToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
            return Ok(companiesToReturn);
        }

        [HttpPost("collection")]
        public IActionResult CreateCompanyCollection([FromBody] IEnumerable<CompanyForCreationDto> companyCollection)
        {
            if (companyCollection == null)//verificamos si nuestra colección es nula
            {
                _logger.LogError("Company collection sent from client is null.");
                return BadRequest("Company collection is null");//devolvemos una solicitud incorrecta 
            }

            //mapeamos esa colección y guardamos todos los elementos de la colección en la base de datos
            var companyEntities = _mapper.Map<IEnumerable<Company>>(companyCollection);
            foreach (var company in companyEntities)
            {
                _repository.Company.CreateCompany(company);
            }
            _repository.Save();
            var companyCollectionToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
            //tomamos todos los ID como una cadena separada por comas y navegamos a la acción GET para obtener nuestras empresas creadas
            var ids = string.Join(",", companyCollectionToReturn.Select(c => c.Id));
            return CreatedAtRoute("CompanyCollection", new { ids }, companyCollectionToReturn);
        }


        [HttpDelete("{id}")]
        public IActionResult DeleteCompany(Guid id)
        {
            var company = _repository.Company.GetCompany(id, trackChanges: false);
            if (company == null)
            {
                _logger.LogInfo($"Company with id: {id} doesn't exist in the database.");
                return NotFound();
            }
            _repository.Company.DeleteCompany(company);
            _repository.Save();
            return NoContent();
        }
    }
}
