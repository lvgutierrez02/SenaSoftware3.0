using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public class UserForAuthenticationDto
    {
        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "El password es requerido")]
        public string Password { get; set; }
    }
}
