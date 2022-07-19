using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class Employee
    {
        [Column("EmployeeId")] //especificará que la propiedad Id se asignará con un nombre diferente en la base de datos
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Employee name is a required field.")] //declara la propiedad como obligatoria y está aquí para fines de validación
        [MaxLength(30, ErrorMessage = "Maximum length for the Name is 30 characters.")] //define su longitud máxima y está aquí para fines de validación
        public string Name { get; set; }
        [Required(ErrorMessage = "Age is a required field.")]
        public int Age { get; set; }
        [Required(ErrorMessage = "Position is a required field.")]
        [MaxLength(20, ErrorMessage = "Maximum length for the Position is 20 characters.")]
        public string Position { get; set; }


        [ForeignKey(nameof(Company))]
        public Guid CompanyId { get; set; }
        public Company Company { get; set; } //propiedades de navegación: sirven para definir la relación entre nuestros modelos
    }
}
