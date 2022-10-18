using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Usuarios.Application.Common.Interfaces.Persistence;
using Usuarios.Domain.DTO;
using Utils.Enums;
using static Utils.Biblioteca;

namespace Usuarios.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosSeguirController : BaseController<UsuariosSeguirController>
    {
        private readonly IUsuarioSeguirRepository _itemUsuarioSeguirRepository;

        public UsuariosSeguirController(IUsuarioSeguirRepository itemUsuarioSeguirRepository)
        {
            _itemUsuarioSeguirRepository = itemUsuarioSeguirRepository;
        }

        [HttpPost("adicionar")]
        [Authorize]
        public async Task<ActionResult<UsuarioSeguirDTO>> Adicionar(UsuarioSeguirDTO dto)
        {
            var isMesmoUsuario = await IsUsuarioSolicitadoMesmoDoToken(dto.UsuarioSeguidorId);

            if (!isMesmoUsuario)
            {
                UsuarioSeguirDTO erro = new() { Erro = true, CodigoErro = (int)CodigoErrosEnum.NaoAutorizado, MensagemErro = GetDescricaoEnum(CodigoErrosEnum.NaoAutorizado) };
                return erro;
            }

            bool isJaSigo = await GetIsJaSigoEsseUsuario(dto.UsuarioSeguidoId);

            if (isJaSigo)
            {
                UsuarioSeguirDTO erro = new() { Erro = true, CodigoErro = (int)CodigoErrosEnum.UsuarioJaSegue, MensagemErro = GetDescricaoEnum(CodigoErrosEnum.UsuarioJaSegue) };
                return erro;
            }

            await _itemUsuarioSeguirRepository.Adicionar(dto);
            return Ok(true);
        }

        [HttpDelete("deletar/{usuarioSeguidoId}")]
        [Authorize]
        public async Task<ActionResult<UsuarioSeguirDTO>> Deletar(int usuarioSeguidoId)
        {
            bool isJaSigo = await GetIsJaSigoEsseUsuario(usuarioSeguidoId);

            if (!isJaSigo)
            {
                UsuarioSeguirDTO erro = new() { Erro = true, CodigoErro = (int)CodigoErrosEnum.UsuarioNaoExiste, MensagemErro = GetDescricaoEnum(CodigoErrosEnum.UsuarioNaoExiste) };
                return erro;
            }

            int usuarioLogadoId = Convert.ToInt32(User?.FindFirstValue(ClaimTypes.NameIdentifier));
            await _itemUsuarioSeguirRepository.Deletar(usuarioSeguidoId, usuarioLogadoId);
            return Ok(true);
        }

        [HttpGet("todosSeguidoresByUsuarioSeguidoId/{usuarioSeguidoId}")]
        public async Task<ActionResult<List<UsuarioSeguirDTO>>> GetTodosSeguidoresByUsuarioSeguidoId(int usuarioSeguidoId)
        {
            var todosSeguidoresByUsuarioId = await _itemUsuarioSeguirRepository.GetTodosSeguidoresByUsuarioSeguidoId(usuarioSeguidoId);

            if (todosSeguidoresByUsuarioId == null)
            {
                return NotFound();
            }

            return Ok(todosSeguidoresByUsuarioId);
        }

        [HttpGet("isJaSigoEsseUsuario/{usuarioSeguidoId}")]
        [Authorize]
        public async Task<bool> GetIsJaSigoEsseUsuario(int usuarioSeguidoId)
        {
            int usuarioLogadoId = Convert.ToInt32(User?.FindFirstValue(ClaimTypes.NameIdentifier));
            var isJaSigoEsseUsuario = await _itemUsuarioSeguirRepository.GetIsJaSigoEsseUsuario(usuarioSeguidoId, usuarioLogadoId);

            return isJaSigoEsseUsuario;
        }
    }
}
