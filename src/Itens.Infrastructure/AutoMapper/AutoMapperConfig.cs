using AutoMapper;
using Itens.Domain.DTO;
using Itens.Domain.Entities;

namespace Itens.Infrastructure.AutoMapper
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            // Item;
            CreateMap<ItemTipo, ItemTipoDTO>().ReverseMap();
            CreateMap<Item, ItemDTO>().ReverseMap();
            CreateMap<ItemImagem, ItemImagemDTO>().ReverseMap();

            // Comentário;
            CreateMap<Comentario, ComentarioDTO>().ReverseMap();
        }
    }
}
