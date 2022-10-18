using Sistemas.Domain.DTO;

namespace Sistemas.Application.Common.Interfaces.Persistence
{
    public interface ISistemaRepository
    {
        Task<List<EstadoDTO>>? GetTodosEstados();
        Task<List<CidadeDTO>>? GetTodasCidades(int estadoId);
        List<DateTime>? GetListaDataAtual();
    }
}
