using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class Company
    {

        [Column("CompanyId")] //especificará que la propiedad Id se asignará con un nombre diferente en la base de datos
        public Guid Id { get; set; } //GUID: Tipo de dato id unico a nivel global
        [Required(ErrorMessage = "El nombre de la empresa es un campo obligatorio.")] //declara la propiedad como obligatoria y está aquí para fines de validación
        [MaxLength(60, ErrorMessage = "La longitud máxima del nombre es de 60 caracteres.")] //define su longitud máxima y está aquí para fines de validación
        public string Name { get; set; }
        [Required(ErrorMessage = "La dirección de la empresa es un campo obligatorio.")]
        [MaxLength(60, ErrorMessage = "La longitud máxima de la dirección es de 60 caracteres")]
        public string Address { get; set; }
        public string Country { get; set; }
        public ICollection<Employee> Employees { get; set; } //propiedad de navegación: sirven para definir la relación entre nuestros modelos
    }
}
