using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Seguimiento.API.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IAuthenticationManager _authenticationManager;
        public AuthenticationController(ILoggerManager logger, IMapper mapper, UserManager<User> userManager, IAuthenticationManager authManager)
        {
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
            _authenticationManager = authManager;   

        }


        [HttpPost]
        //[ServiceFilter(typeof(ValidationFilterAttribute))] 
        public async Task<IActionResult> RegisterUser([FromBody] UserForRegistrationDto userForRegistration)
        {
            var user = _mapper.Map<User>(userForRegistration);//asignamos el objeto DTO al objeto Usuario
            //llamamos al método CreateAsync para crear ese usuario específico en la base de datos.
            //El método CreateAsync guardará al usuario en la base de datos si la acción tiene éxito o devolverá mensajes de error.
            //Si devuelve mensajes de error, los agregamos al estado del modelo.
            var result = await _userManager.CreateAsync(user, userForRegistration.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }
                return BadRequest(ModelState);
            }
            await _userManager.AddToRolesAsync(user, userForRegistration.Roles); //si se crea un usuario, lo conectamos a sus roles, el predeterminado o los enviados desde el lado del cliente
            return StatusCode(201);//devolvemos 201 creado.
        }


        [HttpPost("login")] //[ServiceFilter(typeof(ValidationFilterAttribute))] 
        public async Task<IActionResult> Authenticate([FromBody] UserForAuthenticationDto user)
        {
            if (!await _authenticationManager.ValidateUser(user))
            {
                _logger.LogWarn($"{nameof(Authenticate)}: Authentication failed. Wrong user name or password.");
                return Unauthorized();//devolvemos la respuesta 401 no autorizada
            }
            return Ok(new { Token = await _authenticationManager.CreateToken() });// devolvemos nuestro token creado
        }
    }
}
