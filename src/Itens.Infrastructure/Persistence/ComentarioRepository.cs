using AutoMapper;
using Itens.Application.Common.Interfaces.Persistence;
using Itens.Domain.DTO;
using Itens.Domain.Entities;
using Itens.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using static Utils.Biblioteca;

namespace Itens.Infrastructure.Persistence
{
    public class ComentarioRepository : IComentarioRepository
    {
        public readonly Context _context;
        private readonly IMapper _map;

        public ComentarioRepository(Context context, IMapper map)
        {
            _context = context;
            _map = map;
        }

        public async Task? Adicionar(ComentarioDTO dto)
        {
            Comentario comentario = _map.Map<Comentario>(dto);

            _context.Add(comentario);
            await _context.SaveChangesAsync();
        }

        public async Task? Atualizar(ComentarioDTO dto)
        {
            Comentario comentario = _map.Map<Comentario>(dto);

            _context.Update(comentario);
            await _context.SaveChangesAsync();
        }

        public async Task? Deletar(int id)
        {
            var dados = await _context.Comentarios.FindAsync(id);

            if (dados == null)
            {
                throw new Exception("Registro com o id " + id + " não foi encontrado");
            }

            _context.Comentarios.Remove(dados);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ComentarioDTO>>? GetTodos()
        {
            var todos = await _context.Comentarios.
                        Include(i => i.Itens).ThenInclude(u => u.Usuarios).
                        Include(u => u.Usuarios).
                        Where(i => i.IsAtivo == true).
                        OrderBy(d => d.DataMensagem).AsNoTracking().ToListAsync();

            List<ComentarioDTO> dto = _map.Map<List<ComentarioDTO>>(todos);
            return dto;
        }

        public async Task<ComentarioDTO>? GetById(int id)
        {
            var byId = await _context.Comentarios.
                        Include(i => i.Itens).ThenInclude(u => u.Usuarios).
                        Include(u => u.Usuarios).
                        Where(i => i.IsAtivo == true && i.ComentarioId == id).
                        OrderBy(d => d.DataMensagem).AsNoTracking().FirstOrDefaultAsync();

            ComentarioDTO dto = _map.Map<ComentarioDTO>(byId);
            return dto;
        }

        public async Task<List<ComentarioDTO>>? GetByItemId(int itemId)
        {
            var itens = await _context.Comentarios.
                        Include(i => i.Itens).ThenInclude(u => u.Usuarios).
                        Include(u => u.Usuarios).
                        Where(i => i.ItemId == itemId && i.IsAtivo == true).
                        OrderBy(d => d.DataMensagem).AsNoTracking().ToListAsync();

            List<ComentarioDTO> dto = _map.Map<List<ComentarioDTO>>(itens);
            return dto;
        }

        public async Task? ResponderComentario(ComentarioDTO dto)
        {
            Comentario comentario = _map.Map<Comentario>(dto);

            var comentarioBd = await _context.Comentarios.FindAsync(comentario.ComentarioId);

            if (comentarioBd is null)
            {
                throw new Exception("Registro com o id " + comentario.ComentarioId + " não foi encontrado");
            }

            comentarioBd.Resposta = comentario.Resposta;
            comentarioBd.DataResposta = HorarioBrasilia();

            _context.Update(comentarioBd);
            await _context.SaveChangesAsync();
        }
    }
}
