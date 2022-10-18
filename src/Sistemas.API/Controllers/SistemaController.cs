using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sistemas.Application.Common.Interfaces.Persistence;
using Sistemas.Domain.DTO;

namespace Sistemas.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SistemaController : ControllerBase
    {
        private readonly ISistemaRepository _sistemaRepository;

        public SistemaController(ISistemaRepository sistemaRepository)
        {
            _sistemaRepository = sistemaRepository;
        }

        [HttpGet("todosEstados")]
        public async Task<ActionResult<List<EstadoDTO>>> GetTodosEstados()
        {
            var todos = await _sistemaRepository.GetTodosEstados();
            return Ok(todos);
        }

        [HttpGet("todasCidadesByEstadoId/{estadoId}")]
        public async Task<ActionResult<List<CidadeDTO>>> GetTodasCidades(int estadoId)
        {
            var todos = await _sistemaRepository.GetTodasCidades(estadoId);
            return Ok(todos);
        }

        [HttpGet("listaDataAtual")]
        [Authorize]
        public List<DateTime> GetListaDataAtual()
        {
            var lista = _sistemaRepository.GetListaDataAtual();
            return lista;
        }
    }
}
