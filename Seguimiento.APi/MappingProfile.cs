using AutoMapper;
using Entities.DataTransferObjects;
using Entities.Models;

namespace Seguimiento.API
{
    public class MappingProfile : Profile //MappingProfile debe heredar de la clase de perfil de AutoMapper
    {
        public MappingProfile()
        {
            CreateMap<Company, CompanyDto>() //usamos el método CreateMap donde especificamos el objeto de origen y el objeto de destino para mapear
                .ForMember(c => c.FullAddress,
                    opt => opt.MapFrom(x => string.Join(' ', x.Address, x.Country)));
            //Debido a que tenemos la propiedad FullAddress en nuestra clase DTO, que contiene tanto la direccióN (Address) como el país (Country) de la clase modelo,
            //debemos especificar reglas de mapeo adicionales con el método ForMember.

            CreateMap<Employee, EmployeeDto>();
        }
    }
}
