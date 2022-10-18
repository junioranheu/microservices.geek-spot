using Itens.Application.Common.Interfaces.Persistence;
using Itens.Domain.DTO;
using Microsoft.AspNetCore.Mvc;
using Utils.Enums;
using Utils.Filters;

namespace Itens.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItensController : ControllerBase
    {
        private readonly IItemRepository _itemRepository;

        public ItensController(IItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }

        [HttpPost("adicionar")]
        [CustomAuthorize(UsuarioTipoEnum.Administrador)]
        public async Task<ActionResult<bool>> Adicionar(ItemDTO dto)
        {
            await _itemRepository.Adicionar(dto);
            return Ok(true);
        }

        [HttpPut("atualizar")]
        [CustomAuthorize(UsuarioTipoEnum.Administrador)]
        public async Task<ActionResult<bool>> Atualizar(ItemDTO dto)
        {
            await _itemRepository.Atualizar(dto);
            return Ok(true);
        }

        [HttpDelete("deletar/{id}")]
        [CustomAuthorize(UsuarioTipoEnum.Administrador)]
        public async Task<ActionResult<bool>> Deletar(int id)
        {
            await _itemRepository.Deletar(id);
            return Ok(true);
        }

        [HttpGet("todos")]
        public async Task<ActionResult<List<ItemDTO>>> GetTodos()
        {
            var todos = await _itemRepository.GetTodos();
            return Ok(todos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDTO>> GetById(int id)
        {
            var byId = await _itemRepository.GetById(id);

            if (byId == null)
            {
                return NotFound();
            }

            return Ok(byId);
        }

        [HttpGet("byItemTipoId/{itemTipoId}")]
        public async Task<ActionResult<List<ItemDTO>>> GetByItemTipoId(int itemTipoId)
        {
            var byItemTipoId = await _itemRepository.GetByItemTipoId(itemTipoId);

            if (byItemTipoId == null)
            {
                return NotFound();
            }

            return Ok(byItemTipoId);
        }

        [HttpGet("byUsuarioId/{usuarioId}")]
        public async Task<ActionResult<List<ItemDTO>>> GetByUsuarioId(int usuarioId)
        {
            var byUsuarioId = await _itemRepository.GetByUsuarioId(usuarioId);

            if (byUsuarioId == null)
            {
                return NotFound();
            }

            return Ok(byUsuarioId);
        }

        [HttpGet("listaItensGroupByUsuario")]
        public async Task<ActionResult<List<List<ItemDTO>>>> GetListaItensGroupByUsuario()
        {
            var listaItensGroupByUsuario = await _itemRepository.GetListaItensGroupByUsuario();

            if (listaItensGroupByUsuario == null)
            {
                return NotFound();
            }

            return Ok(listaItensGroupByUsuario);
        }
    }
}

