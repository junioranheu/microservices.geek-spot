using AutoMapper;
using System.Security.Claims;
using Usuarios.Application.Common.Interfaces.Authentication;
using Usuarios.Application.Common.Interfaces.Persistence;
using Usuarios.Domain.DTO;
using Utils.Domain.Enums;
using Utils.Entities;
using Utils.Enums;
using static Utils.Biblioteca;

namespace Usuarios.Application.Services.Authentication
{
    public class AutenticarService : IAutenticarService
    {
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IMapper _map;

        public AutenticarService(IJwtTokenGenerator jwtTokenGenerator, IUsuarioRepository usuarioRepository, IRefreshTokenRepository refreshTokenRepository, IMapper map)
        {
            _jwtTokenGenerator = jwtTokenGenerator;
            _usuarioRepository = usuarioRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _map = map;
        }

        public async Task<UsuarioDTO> Login(UsuarioSenhaDTO dto)
        {
            // #1 - Verificar se o usuário existe;
            var usuario = await _usuarioRepository.GetByEmailOuUsuarioSistema(dto?.Email, dto?.NomeUsuarioSistema);

            if (usuario is null)
            {
                UsuarioDTO erro = new() { Erro = true, CodigoErro = (int)CodigoErrosEnum.UsuarioNaoEncontrado, MensagemErro = GetDescricaoEnum(CodigoErrosEnum.UsuarioNaoEncontrado) };
                return erro;
            }

            // #2 - Verificar se a senha está correta;
            if (usuario.Senha != Criptografar(dto?.Senha))
            {
                UsuarioDTO erro = new() { Erro = true, CodigoErro = (int)CodigoErrosEnum.UsuarioSenhaIncorretos, MensagemErro = GetDescricaoEnum(CodigoErrosEnum.UsuarioSenhaIncorretos) };
                return erro;
            }

            // #3 - Verificar se o usuário está ativo;
            if (!usuario.IsAtivo)
            {
                UsuarioDTO erro = new() { Erro = true, CodigoErro = (int)CodigoErrosEnum.ContaDesativada, MensagemErro = GetDescricaoEnum(CodigoErrosEnum.ContaDesativada) };
                return erro;
            }

            // #4 - Criar token JWT;
            var token = _jwtTokenGenerator.GerarToken(usuario, null);
            usuario.Token = token;

            // #5 - Gerar refresh token;
            var refreshToken = _jwtTokenGenerator.GerarRefreshToken();
            usuario.RefreshToken = refreshToken;

            RefreshTokenDTO novoRefreshToken = new()
            {
                RefToken = refreshToken,
                UsuarioId = usuario.UsuarioId,
                DataRegistro = HorarioBrasilia()
            };

            await _refreshTokenRepository.Adicionar(novoRefreshToken);

            // #6 - Converter de UsuarioSenhaDTO para UsuarioDTO;
            UsuarioDTO usuarioDTO = _map.Map<UsuarioDTO>(usuario);

            return usuarioDTO;
        }

        public async Task<UsuarioDTO> Registrar(UsuarioSenhaDTO dto)
        {
            // #1 - Verificar se o usuário já existe com o e-mail ou nome de usuário do sistema informados. Se existir, aborte;
            var verificarUsuario = await _usuarioRepository.GetByEmailOuUsuarioSistema(dto?.Email, dto?.NomeUsuarioSistema);

            if (verificarUsuario is not null)
            {
                UsuarioDTO erro = new() { Erro = true, CodigoErro = (int)CodigoErrosEnum.UsuarioExistente, MensagemErro = GetDescricaoEnum(CodigoErrosEnum.UsuarioExistente) };
                return erro;
            }

            // #2.1 - Verificar requisitos gerais;
            if (dto?.NomeCompleto?.Length < 3 || dto?.NomeUsuarioSistema?.Length < 3)
            {
                UsuarioDTO erro = new() { Erro = true, CodigoErro = (int)CodigoErrosEnum.RequisitosNome, MensagemErro = GetDescricaoEnum(CodigoErrosEnum.RequisitosNome) };
                return erro;
            }

            // #2.2 - Verificar e-mail;
            if (!ValidarEmail(dto?.Email))
            {
                UsuarioDTO erro = new() { Erro = true, CodigoErro = (int)CodigoErrosEnum.EmailInvalido, MensagemErro = GetDescricaoEnum(CodigoErrosEnum.EmailInvalido) };
                return erro;
            }

            // #2.3 - Verificar requisitos de senha;
            var validarSenha = ValidarSenha(dto?.Senha, dto?.NomeCompleto, dto?.NomeUsuarioSistema, dto?.Email);
            if (!validarSenha.Item1)
            {
                UsuarioDTO erro = new() { Erro = true, CodigoErro = (int)CodigoErrosEnum.RequisitosSenhaNaoCumprido, MensagemErro = validarSenha.Item2 };
                return erro;
            }

            // #3.1 - Gerar código de verificação para usar no processo de criação e no envio de e-mail;
            string codigoVerificacao = GerarStringAleatoria(6, true);

            // #3.2 - Criar usuário;
            var novoUsuario = new UsuarioSenhaDTO
            {
                NomeCompleto = dto?.NomeCompleto,
                Email = dto?.Email,
                NomeUsuarioSistema = dto?.NomeUsuarioSistema,
                Senha = Criptografar(dto?.Senha),
                UsuarioTipoId = (int)UsuarioTipoEnum.Usuario,
                Foto = "",
                DataRegistro = HorarioBrasilia(),
                DataOnline = HorarioBrasilia(),
                IsAtivo = true,
                IsPremium = false,
                IsVerificado = false,
                CodigoVerificacao = codigoVerificacao,
                ValidadeCodigoVerificacao = HorarioBrasilia().AddHours(24),
                HashUrlTrocarSenha = "",
                ValidadeHashUrlTrocarSenha = DateTime.MinValue
            };

            UsuarioDTO usuarioAdicionado = await _usuarioRepository.Adicionar(novoUsuario);

            // #4 - Automaticamente atualizar o valor da Foto com um valor padrão após criar o novo usuário e adicionar ao ovjeto novoUsuario;
            string nomeNovaFoto = $"{usuarioAdicionado.UsuarioId}{GerarStringAleatoria(5, true)}.webp";
            await _usuarioRepository.AtualizarFoto(usuarioAdicionado.UsuarioId, nomeNovaFoto);
            novoUsuario.Foto = nomeNovaFoto;

            // #5 - Adicionar ao objeto novoUsuario o id do novo usuário;
            novoUsuario.UsuarioId = usuarioAdicionado.UsuarioId;

            // #6 - Criar token JWT;
            var token = _jwtTokenGenerator.GerarToken(novoUsuario, null);
            novoUsuario.Token = token;

            // #7 - Gerar refresh token;
            var refreshToken = _jwtTokenGenerator.GerarRefreshToken();
            novoUsuario.RefreshToken = refreshToken;

            RefreshTokenDTO novoRefreshToken = new()
            {
                RefToken = refreshToken,
                UsuarioId = usuarioAdicionado.UsuarioId,
                DataRegistro = HorarioBrasilia()
            };

            await _refreshTokenRepository.Adicionar(novoRefreshToken);

            // #8 - Converter de UsuarioSenhaDTO para UsuarioDTO;
            UsuarioDTO usuarioDTO = _map.Map<UsuarioDTO>(novoUsuario);

            // #9 - Enviar e-mail de verificação de conta;
            try
            {
                if (!String.IsNullOrEmpty(usuarioDTO?.Email) && !String.IsNullOrEmpty(usuarioDTO?.NomeCompleto) && !String.IsNullOrEmpty(codigoVerificacao))
                {
                    usuarioDTO.IsEmailVerificacaoContaEnviado = await EnviarEmailVerificacaoConta(usuarioDTO?.Email, usuarioDTO?.NomeCompleto, codigoVerificacao);
                }
            }
            catch (Exception)
            {
                usuarioDTO.IsEmailVerificacaoContaEnviado = false;
            }

            return usuarioDTO;
        }

        public async Task<UsuarioDTO> RefreshToken(string token, string refreshToken)
        {
            var principal = _jwtTokenGenerator.GetInfoTokenExpirado(token);
            int usuarioId = Convert.ToInt32(principal?.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).FirstOrDefault());

            var refreshTokenSalvoAnteriormente = await _refreshTokenRepository.GetRefreshTokenByUsuarioId(usuarioId);

            if (refreshTokenSalvoAnteriormente != refreshToken)
            {
                UsuarioDTO erro = new() { Erro = true, CodigoErro = (int)CodigoErrosEnum.RefreshTokenInvalido, MensagemErro = GetDescricaoEnum(CodigoErrosEnum.RefreshTokenInvalido) };
                return erro;
            }

            var novoToken = _jwtTokenGenerator.GerarToken(null, principal?.Claims);
            var novoRefreshToken = _jwtTokenGenerator.GerarRefreshToken();

            // Criar novo registro com o novo refresh token gerado;
            RefreshTokenDTO novoRefreshTokenDTO = new()
            {
                RefToken = novoRefreshToken,
                UsuarioId = usuarioId,
                DataRegistro = HorarioBrasilia()
            };

            await _refreshTokenRepository.Adicionar(novoRefreshTokenDTO);

            // Retornar o novo token e o novo refresh token;
            UsuarioDTO dto = new()
            {
                Token = novoToken,
                RefreshToken = novoRefreshToken
            };

            return dto;
        }

        public static async Task<bool> EnviarEmailVerificacaoConta(string emailTo, string nomeUsuario, string codigoVerificacao)
        {
            string assunto = "Verifique sua conta do GeekSpot";
            string nomeArquivo = GetDescricaoEnum(EmailEnum.VerificarConta);

            List<EmailDadosReplace> listaDadosReplace = new()
            {
                new EmailDadosReplace { Key = "Url", Value = $"{CaminhoFront()}/usuario/verificar-conta/{codigoVerificacao}"},
                new EmailDadosReplace { Key = "NomeUsuario", Value = nomeUsuario }
            };

            bool resposta = await EnviarEmail(emailTo, assunto, nomeArquivo, listaDadosReplace);
            return resposta;
        }
    }
}
