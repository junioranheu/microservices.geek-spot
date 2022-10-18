using Microsoft.AspNetCore.Mvc;
using Usuarios.Application.Common.Interfaces.Authentication;
using Usuarios.Domain.DTO;
using Utils.Enums;
using static Utils.Biblioteca;

namespace Usuarios.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AutenticarController : BaseController<AutenticarController>
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IAutenticarService _autenticarService;

        public AutenticarController(IWebHostEnvironment webHostEnvironment, IAutenticarService autenticarService)
        {
            _webHostEnvironment = webHostEnvironment;
            _autenticarService = autenticarService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UsuarioDTO>> Login(UsuarioSenhaDTO dto)
        {
            var authResultado = await _autenticarService.Login(dto);
            return Ok(authResultado);
        }

        [HttpPost("registrar")]
        public async Task<ActionResult<UsuarioDTO>> Registrar(UsuarioSenhaDTO dto)
        {
            var authResultado = await _autenticarService.Registrar(dto);

            try
            {
                if (!String.IsNullOrEmpty(dto.Foto))
                {
                    var file = Base64ToImage(dto.Foto);
                    string caminhoNovaImagem = await UparImagem(file, authResultado.Foto, GetDescricaoEnum(CaminhoUploadEnum.FotoPerfilUsuario), "", _webHostEnvironment);
                    authResultado.Foto = caminhoNovaImagem ?? "";
                }
            }
            catch (Exception)
            {

            }

            return Ok(authResultado);
        }

        [HttpPost("refreshToken")]
        public async Task<ActionResult<UsuarioDTO>> RefreshToken(UsuarioSenhaDTO dto)
        {
            var authResultado = await _autenticarService.RefreshToken(dto.Token, dto.RefreshToken);
            return Ok(authResultado);
        }
    }
}
