using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Usuarios.Application.Common.Interfaces.Persistence;
using Usuarios.Domain.DTO;
using Utils.Enums;
using static Utils.Biblioteca;

namespace Usuarios.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : BaseController<UsuariosController>
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IUsuarioRepository _usuarios;

        public UsuariosController(IWebHostEnvironment webHostEnvironment, IUsuarioRepository usuarioRepository)
        {
            _webHostEnvironment = webHostEnvironment;
            _usuarios = usuarioRepository;
        }

        [HttpPut("atualizar")]
        [Authorize]
        public async Task<ActionResult<UsuarioDTO>> Atualizar(UsuarioSenhaDTO dto)
        {
            var isMesmoUsuario = await IsUsuarioSolicitadoMesmoDoToken(dto.UsuarioId);

            if (!isMesmoUsuario)
            {
                UsuarioDTO erro = new()
                {
                    Erro = true,
                    CodigoErro = (int)CodigoErrosEnum.NaoAutorizado,
                    MensagemErro = GetDescricaoEnum(CodigoErrosEnum.NaoAutorizado)
                };

                return erro;
            }

            var usuario = await _usuarios.Atualizar(dto);
            return Ok(usuario);
        }

        [HttpGet("todos")]
        public async Task<ActionResult<List<UsuarioDTO>>> GetTodos()
        {
            var itens = await _usuarios.GetTodos();

            if (itens == null)
            {
                return NotFound();
            }

            return itens;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioDTO>> GetById(int id)
        {
            var byId = await _usuarios.GetById(id);

            if (byId == null)
            {
                return NotFound();
            }

            return byId;
        }

        [HttpPut("verificarConta/{codigoVerificacao}")]
        public async Task<ActionResult<UsuarioDTO>> VerificarConta(string codigoVerificacao)
        {
            var usuario = await _usuarios.VerificarConta(codigoVerificacao);
            return usuario;
        }

        [HttpPut("atualizarDadosLojinha")]
        [Authorize]
        public async Task<ActionResult<UsuarioDTO>> AtualizarDadosLojinha(UsuarioDTO dto)
        {
            int usuarioLogadoId = Convert.ToInt32(User?.FindFirstValue(ClaimTypes.NameIdentifier));
            var usuario = await _usuarios.AtualizarDadosLojinha(usuarioLogadoId, dto);

            // Atualizar físicamente a foto de perfil do usuário;
            try
            {
                if (!String.IsNullOrEmpty(dto.Foto))
                {
                    var file = Base64ToImage(dto.Foto);
                    await UparImagem(file, usuario.Foto, GetDescricaoEnum(CaminhoUploadEnum.FotoPerfilUsuario), usuario.FotoAnterior, _webHostEnvironment);
                }
            }
            catch (Exception)
            {

            }

            // Atualizar físicamente a capa da lojinha;
            try
            {
                if (!String.IsNullOrEmpty(dto.UsuariosInformacoes?.LojinhaImagemCapa))
                {
                    var file = Base64ToImage(dto.UsuariosInformacoes.LojinhaImagemCapa);
                    await UparImagem(file, usuario.UsuariosInformacoes.LojinhaImagemCapa, GetDescricaoEnum(CaminhoUploadEnum.CapaLojinha), usuario.UsuariosInformacoes.LojinhaImagemCapaAnterior, _webHostEnvironment);
                }
            }
            catch (Exception)
            {

            }

            return Ok(usuario);
        }

        [HttpPut("atualizarDadosPessoais")]
        [Authorize]
        public async Task<ActionResult<UsuarioDTO>> AtualizarDadosPessoais(UsuarioSenhaDTO dto)
        {
            int usuarioLogadoId = Convert.ToInt32(User?.FindFirstValue(ClaimTypes.NameIdentifier));
            var usuario = await _usuarios.AtualizarDadosPessoais(usuarioLogadoId, dto);

            return Ok(usuario);
        }

        [HttpPut("atualizarDadosEndereco")]
        [Authorize]
        public async Task<ActionResult<UsuarioDTO>> AtualizarDadosEndereco(UsuarioDTO dto)
        {
            int usuarioLogadoId = Convert.ToInt32(User?.FindFirstValue(ClaimTypes.NameIdentifier));
            var usuario = await _usuarios.AtualizarDadosEndereco(usuarioLogadoId, dto);

            return Ok(usuario);
        }

        [HttpPut("desativarConta")]
        [Authorize]
        public async Task<ActionResult<UsuarioDTO>> DesativarConta(UsuarioSenhaDTO dto)
        {
            int usuarioLogadoId = Convert.ToInt32(User?.FindFirstValue(ClaimTypes.NameIdentifier));
            var usuario = await _usuarios.DesativarConta(usuarioLogadoId, dto);

            return Ok(usuario);
        }

        [HttpPut("atualizarSenha")]
        [Authorize]
        public async Task<ActionResult<AtualizarSenhaDTO>> AtualizarSenha(AtualizarSenhaDTO dto)
        {
            int usuarioLogadoId = Convert.ToInt32(User?.FindFirstValue(ClaimTypes.NameIdentifier));
            var usuario = await _usuarios.AtualizarSenha(usuarioLogadoId, dto);

            return Ok(usuario);
        }

        [HttpPost("emailRecuperarSenha")]
        public async Task<ActionResult<UsuarioDTO>> EmailRecuperarSenha(UsuarioDTO dto)
        {
            var usuario = await _usuarios.EmailRecuperarSenha(dto);

            return Ok(usuario);
        }

        [HttpPut("atualizarSenhaRecuperar")]
        public async Task<ActionResult<AtualizarSenhaDTO>> AtualizarSenhaRecuperar(AtualizarSenhaDTO dto)
        {
            var usuario = await _usuarios.AtualizarSenhaRecuperar(dto);

            return Ok(usuario);
        }

        [HttpPost("emailVerificarConta")]
        [Authorize]
        public async Task<ActionResult<UsuarioDTO>> EmailVerificarConta()
        {
            int usuarioLogadoId = Convert.ToInt32(User?.FindFirstValue(ClaimTypes.NameIdentifier));
            var usuario = await _usuarios.EmailVerificarConta(usuarioLogadoId);

            return Ok(usuario);
        }
    }
}
