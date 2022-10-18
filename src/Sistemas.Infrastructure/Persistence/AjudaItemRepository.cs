using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sistemas.Application.Common.Interfaces.Persistence;
using Sistemas.Domain.DTO;
using Sistemas.Domain.Entities;
using Sistemas.Infrastructure.Data;

namespace Sistemas.Infrastructure.Persistence
{
    public class AjudaItemRepository : IAjudaItemRepository
    {
        public readonly Context _context;
        private readonly IMapper _map;

        public AjudaItemRepository(Context context, IMapper map)
        {
            _context = context;
            _map = map;
        }

        public async Task? Adicionar(AjudaItemDTO dto)
        {
            AjudaItem ajudaItem = _map.Map<AjudaItem>(dto);

            _context.Add(ajudaItem);
            await _context.SaveChangesAsync();
        }

        public async Task? Atualizar(AjudaItemDTO dto)
        {
            AjudaItem ajudaItem = _map.Map<AjudaItem>(dto);

            _context.Update(ajudaItem);
            await _context.SaveChangesAsync();
        }

        public async Task? Deletar(int id)
        {
            var dados = await _context.AjudasItens.FindAsync(id);

            if (dados == null)
            {
                throw new Exception("Registro com o id " + id + " não foi encontrado");
            }

            _context.AjudasItens.Remove(dados);
            await _context.SaveChangesAsync();
        }

        public async Task<List<AjudaItemDTO>>? GetTodos()
        {
            var todos = await _context.AjudasItens.
                        Include(at => at.AjudasTopicos).
                        Where(i => i.IsAtivo == true).
                        OrderBy(t => t.Titulo).AsNoTracking().ToListAsync();

            List<AjudaItemDTO> dto = _map.Map<List<AjudaItemDTO>>(todos);
            return dto;
        }

        public async Task<AjudaItemDTO>? GetById(int id)
        {
            var itens = await _context.AjudasItens.
                        Include(u => u.AjudasTopicos).
                        Where(ai => ai.AjudaItemId == id && ai.IsAtivo == true).AsNoTracking().FirstOrDefaultAsync();

            AjudaItemDTO dto = _map.Map<AjudaItemDTO>(itens);
            return dto;
        }

        public async Task<List<AjudaItemDTO>>? GetByAjudaTopicoId(int ajudaTopicoId)
        {
            var itens = await _context.AjudasItens.
                        Include(u => u.AjudasTopicos).
                        Where(at => at.AjudaTopicoId == ajudaTopicoId && at.IsAtivo == true).AsNoTracking().ToListAsync();

            List<AjudaItemDTO> dto = _map.Map<List<AjudaItemDTO>>(itens);
            return dto;
        }

        public async Task<List<AjudaItemDTO>>? GetByQuery(string query)
        {
            if (String.IsNullOrEmpty(query))
            {
                return null;
            }

            var itens = await _context.AjudasItens.
                        Include(u => u.AjudasTopicos).
                        Where(i => i.IsAtivo == true && (i.Titulo.Contains(query) || i.ConteudoHtml.Contains(query))).
                        AsNoTracking().ToListAsync();

            List<AjudaItemDTO> dto = _map.Map<List<AjudaItemDTO>>(itens);
            return dto;
        }
    }
}
