using Itens.Application.Common.Interfaces.Persistence;
using Itens.Domain.DTO;
using Microsoft.AspNetCore.Mvc;
using Utils.Enums;
using Utils.Filters;

namespace Itens.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItensTiposController : ControllerBase
    {
        private readonly IItemTipoRepository _itemTipoRepository;

        public ItensTiposController(IItemTipoRepository itemTipoRepository)
        {
            _itemTipoRepository = itemTipoRepository;
        }

        [HttpPost("adicionar")]
        [CustomAuthorize(UsuarioTipoEnum.Administrador)]
        public async Task<ActionResult<bool>> Adicionar(ItemTipoDTO dto)
        {
            await _itemTipoRepository.Adicionar(dto);
            return Ok(true);
        }

        [HttpPut("atualizar")]
        [CustomAuthorize(UsuarioTipoEnum.Administrador)]
        public async Task<ActionResult<bool>> Atualizar(ItemTipoDTO dto)
        {
            await _itemTipoRepository.Atualizar(dto);
            return Ok(true);
        }

        [HttpDelete("deletar/{id}")]
        [CustomAuthorize(UsuarioTipoEnum.Administrador)]
        public async Task<ActionResult<bool>> Deletar(int id)
        {
            await _itemTipoRepository.Deletar(id);
            return Ok(true);
        }

        [HttpGet("todos")]
        public async Task<ActionResult<List<ItemTipoDTO>>> GetTodos()
        {
            var todos = await _itemTipoRepository.GetTodos();
            return Ok(todos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemTipoDTO>> GetById(int id)
        {
            var byId = await _itemTipoRepository.GetById(id);

            if (byId == null)
            {
                return NotFound();
            }

            return Ok(byId);
        }
    }
}
