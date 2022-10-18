using AutoMapper;
using Sistemas.Domain.DTO;
using Sistemas.Domain.Entities;

namespace Sistemas.Infrastructure.AutoMapper
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            // Outros; 
            CreateMap<AjudaTopico, AjudaTopicoDTO>().ReverseMap();
            CreateMap<AjudaItem, AjudaItemDTO>().ReverseMap();

            // Logradouro;
            CreateMap<Estado, EstadoDTO>().ReverseMap();
            CreateMap<Cidade, CidadeDTO>().ReverseMap();
        }
    }
}
