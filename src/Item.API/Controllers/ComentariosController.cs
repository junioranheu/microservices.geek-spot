using Itens.Application.Common.Interfaces.Persistence;
using Itens.Domain.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Utils.Enums;
using Utils.Filters;
using static Utils.Biblioteca;

namespace Itens.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComentariosController : BaseController<ComentariosController>
    {
        private readonly IComentarioRepository _comentarioRepository;
        private readonly IItemRepository _itemRepository;

        public ComentariosController(IComentarioRepository comentarioRepository, IItemRepository itemRepository)
        {
            _comentarioRepository = comentarioRepository;
            _itemRepository = itemRepository;
        }

        [HttpPost("adicionar")]
        [Authorize]
        public async Task<ActionResult<bool>> Adicionar(ComentarioDTO dto)
        {
            dto.UsuarioId = Convert.ToInt32(User?.FindFirstValue(ClaimTypes.NameIdentifier));
            await _comentarioRepository.Adicionar(dto);
            return Ok(true);
        }

        [HttpPut("atualizar")]
        [CustomAuthorize(UsuarioTipoEnum.Administrador)]
        public async Task<ActionResult<bool>> Atualizar(ComentarioDTO dto)
        {
            await _comentarioRepository.Atualizar(dto);
            return Ok(true);
        }

        [HttpDelete("deletar/{id}")]
        [CustomAuthorize(UsuarioTipoEnum.Administrador)]
        public async Task<ActionResult<bool>> Deletar(int id)
        {
            await _comentarioRepository.Deletar(id);
            return Ok(true);
        }

        [HttpGet("todos")]
        [CustomAuthorize(UsuarioTipoEnum.Administrador)]
        public async Task<ActionResult<List<ComentarioDTO>>> GetTodos()
        {
            var todos = await _comentarioRepository.GetTodos();
            return Ok(todos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ComentarioDTO>> GetById(int id)
        {
            var byId = await _comentarioRepository.GetById(id);

            if (byId == null)
            {
                return NotFound();
            }

            return Ok(byId);
        }

        [HttpGet("byItemId/{itemId}")]
        public async Task<ActionResult<List<ComentarioDTO>>> GetByItemId(int itemId)
        {
            var byItemId = await _comentarioRepository.GetByItemId(itemId);

            if (byItemId == null)
            {
                return NotFound();
            }

            return Ok(byItemId);
        }

        [HttpPut("responderComentario")]
        [Authorize]
        public async Task<ActionResult<ComentarioDTO>> ResponderComentario(ComentarioDTO dto)
        {
            // Buscar o usuario dono do item em questão que vem do parâmetro dto;
            var item = await _itemRepository.GetById(dto.ItemId);

            if (item is null)
            {
                ComentarioDTO erro = new()
                {
                    Erro = true,
                    CodigoErro = (int)CodigoErrosEnum.NaoEncontrado,
                    MensagemErro = GetDescricaoEnum(CodigoErrosEnum.NaoEncontrado)
                };

                return erro;
            }

            int idUsuarioDonoItem = item.UsuarioId > 0 ? item.UsuarioId : 0;
            var isMesmoUsuario = await IsUsuarioSolicitadoMesmoDoToken(idUsuarioDonoItem);

            if (!isMesmoUsuario)
            {
                ComentarioDTO erro = new()
                {
                    Erro = true,
                    CodigoErro = (int)CodigoErrosEnum.NaoAutorizado,
                    MensagemErro = GetDescricaoEnum(CodigoErrosEnum.NaoAutorizado)
                };

                return erro;
            }

            await _comentarioRepository.ResponderComentario(dto);
            return Ok(true);
        }
    }
}
