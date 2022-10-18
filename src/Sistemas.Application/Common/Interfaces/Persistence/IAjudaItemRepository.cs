using Sistemas.Domain.DTO;

namespace Sistemas.Application.Common.Interfaces.Persistence
{
    public interface IAjudaItemRepository
    {
        Task? Adicionar(AjudaItemDTO dto);
        Task? Atualizar(AjudaItemDTO dto);
        Task? Deletar(int id);
        Task<List<AjudaItemDTO>>? GetTodos();
        Task<AjudaItemDTO>? GetById(int id);
        Task<List<AjudaItemDTO>>? GetByAjudaTopicoId(int ajudaTopicoId);
        Task<List<AjudaItemDTO>>? GetByQuery(string query);
    }
}
