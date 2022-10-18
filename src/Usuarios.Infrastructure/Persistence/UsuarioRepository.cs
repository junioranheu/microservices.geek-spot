using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Usuarios.Application.Common.Interfaces.Persistence;
using Usuarios.Application.Services.Authentication;
using Usuarios.Domain.DTO;
using Usuarios.Domain.Entities;
using Usuarios.Infrastructure.Data;
using Utils.Domain.Enums;
using Utils.Entities;
using Utils.Enums;
using static Utils.Biblioteca;

namespace Usuarios.Infrastructure.Persistence
{
    public class UsuarioRepository : IUsuarioRepository
    {
        public readonly Context _context;
        private readonly IMapper _map;

        public UsuarioRepository(Context context, IMapper map)
        {
            _map = map;
            _context = context;
        }

        public async Task<UsuarioDTO>? Adicionar(UsuarioSenhaDTO dto)
        {
            Usuario usuario = _map.Map<Usuario>(dto);

            _context.Add(usuario);
            await _context.SaveChangesAsync();

            UsuarioDTO usuarioDTO = _map.Map<UsuarioDTO>(usuario);
            return usuarioDTO;
        }

        public async Task<UsuarioDTO>? Atualizar(UsuarioSenhaDTO dto)
        {
            Usuario usuario = _map.Map<Usuario>(dto);
            UsuarioDTO usuarioDTO = _map.Map<UsuarioDTO>(dto);

            _context.Update(usuario);
            await _context.SaveChangesAsync();
            return usuarioDTO;
        }

        public async Task? Deletar(int id)
        {
            var dados = await _context.Usuarios.FindAsync(id);

            if (dados == null)
            {
                throw new Exception("Registro com o id " + id + " não foi encontrado");
            }

            _context.Usuarios.Remove(dados);
            await _context.SaveChangesAsync();
        }

        public async Task<List<UsuarioDTO>>? GetTodos()
        {
            var todos = await _context.Usuarios.
                        Include(ut => ut.UsuariosTipos).
                        Include(ui => ui.UsuariosInformacoes).
                        OrderBy(ui => ui.UsuarioId).AsNoTracking().ToListAsync();

            List<UsuarioDTO> dto = _map.Map<List<UsuarioDTO>>(todos);
            return dto;
        }

        public async Task<UsuarioDTO>? GetById(int id)
        {
            var byId = await _context.Usuarios.
                       Include(ut => ut.UsuariosTipos).
                       Include(ui => ui.UsuariosInformacoes).
                       Where(ui => ui.UsuarioId == id).AsNoTracking().FirstOrDefaultAsync();

            UsuarioDTO dto = _map.Map<UsuarioDTO>(byId);
            return dto;
        }

        public async Task<UsuarioSenhaDTO>? GetByEmailOuUsuarioSistema(string? email, string? nomeUsuarioSistema)
        {
            var byEmail = await _context.Usuarios.
                Include(ui => ui.UsuariosInformacoes).
                Where(e => e.Email == email).AsNoTracking().FirstOrDefaultAsync();

            if (byEmail is null)
            {
                var byNomeUsuario = await _context.Usuarios.
                                     Include(ui => ui.UsuariosInformacoes).
                                     Where(n => n.NomeUsuarioSistema == nomeUsuarioSistema).AsNoTracking().FirstOrDefaultAsync();

                UsuarioSenhaDTO dto1 = _map.Map<UsuarioSenhaDTO>(byNomeUsuario);
                return dto1;
            }

            UsuarioSenhaDTO dto2 = _map.Map<UsuarioSenhaDTO>(byEmail);
            return dto2;
        }

        public async Task<UsuarioSenhaDTO>? GetByEmailOuUsuarioSistemaDiferenteDoMeu(int usuarioId, string? email, string? nomeUsuarioSistema)
        {
            var byEmail = await _context.Usuarios.
                Include(ui => ui.UsuariosInformacoes).
                Where(e => e.Email == email && e.UsuarioId != usuarioId).AsNoTracking().FirstOrDefaultAsync();

            if (byEmail is null)
            {
                var byNomeUsuario = await _context.Usuarios.
                                     Include(ui => ui.UsuariosInformacoes).
                                     Where(n => n.NomeUsuarioSistema == nomeUsuarioSistema && n.UsuarioId != usuarioId).AsNoTracking().FirstOrDefaultAsync();

                UsuarioSenhaDTO dto1 = _map.Map<UsuarioSenhaDTO>(byNomeUsuario);
                return dto1;
            }

            UsuarioSenhaDTO dto2 = _map.Map<UsuarioSenhaDTO>(byEmail);
            return dto2;
        }

        public async Task? AtualizarFoto(int usuarioId, string foto)
        {
            var usuario = await _context.Usuarios.Where(ui => ui.UsuarioId == usuarioId).FirstOrDefaultAsync();
            usuario.Foto = foto;

            _context.Update(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task<string>? AtualizarCodigoVerificacao(int usuarioId)
        {
            var usuario = await _context.Usuarios.Where(ui => ui.UsuarioId == usuarioId).FirstOrDefaultAsync();
            string novoCodigoVerificacao = GerarStringAleatoria(6, true);

            usuario.CodigoVerificacao = novoCodigoVerificacao;
            usuario.ValidadeCodigoVerificacao = HorarioBrasilia().AddHours(24);

            _context.Update(usuario);
            await _context.SaveChangesAsync();

            return novoCodigoVerificacao;
        }

        public async Task<UsuarioDTO>? VerificarConta(string codigoVerificacao)
        {
            var usuario = await _context.Usuarios.Where(cv => cv.CodigoVerificacao == codigoVerificacao).AsNoTracking().FirstOrDefaultAsync();

            if (usuario is null)
            {
                UsuarioDTO erro = new() { Erro = true, CodigoErro = (int)CodigoErrosEnum.CodigoVerificacaoInvalido, MensagemErro = GetDescricaoEnum(CodigoErrosEnum.CodigoVerificacaoInvalido) };
                return erro;
            }

            if (HorarioBrasilia() > usuario.ValidadeCodigoVerificacao)
            {
                UsuarioDTO erro = new() { Erro = true, CodigoErro = (int)CodigoErrosEnum.CodigoExpirado, MensagemErro = GetDescricaoEnum(CodigoErrosEnum.CodigoExpirado) };
                return erro;
            }

            if (usuario.IsVerificado == true)
            {
                UsuarioDTO erro = new() { Erro = true, CodigoErro = (int)CodigoErrosEnum.ContaJaVerificada, MensagemErro = GetDescricaoEnum(CodigoErrosEnum.ContaJaVerificada) };
                return erro;
            }

            usuario.IsVerificado = true;
            _context.Update(usuario);
            await _context.SaveChangesAsync();

            UsuarioDTO dto = _map.Map<UsuarioDTO>(usuario);
            return dto;
        }

        public async Task<UsuarioDTO>? AtualizarDadosLojinha(int usuarioId, UsuarioDTO dto)
        {
            var byId = await _context.Usuarios.
                       Include(ut => ut.UsuariosTipos).
                       Include(ui => ui.UsuariosInformacoes).
                       Where(ui => ui.UsuarioId == usuarioId).AsNoTracking().FirstOrDefaultAsync();

            if (byId is null)
            {
                UsuarioDTO erro = new() { Erro = true, CodigoErro = (int)CodigoErrosEnum.FalhaAoAtualizarDados, MensagemErro = GetDescricaoEnum(CodigoErrosEnum.FalhaAoAtualizarDados) };
                return erro;
            }

            // Se o usuário não tiver dados na tabela UsuariosInformacoes, a classe deve ser instanciada;
            if (byId.UsuariosInformacoes is null)
            {
                byId.UsuariosInformacoes = new();
            }

            // "Segurar" dados anteriores;
            string fotoAnterior = byId?.Foto ?? "";
            string fotoCapaAnterior = byId?.UsuariosInformacoes?.LojinhaImagemCapa ?? "";

            // Atualizar dados;
            byId.Foto = !String.IsNullOrEmpty(dto.Foto) ? $"{usuarioId}{GerarStringAleatoria(5, true)}.webp" : "";
            byId.UsuariosInformacoes.LojinhaImagemCapa = !String.IsNullOrEmpty(dto.UsuariosInformacoes?.LojinhaImagemCapa) ? $"{usuarioId}{GerarStringAleatoria(5, true)}.webp" : "";
            byId.UsuariosInformacoes.LojinhaTitulo = dto.UsuariosInformacoes?.LojinhaTitulo;
            byId.UsuariosInformacoes.LojinhaDescricao = dto.UsuariosInformacoes?.LojinhaDescricao;
            byId.UsuariosInformacoes.DataUltimaAlteracao = HorarioBrasilia();

            _context.Update(byId);
            await _context.SaveChangesAsync();

            UsuarioDTO dtoRetorno = _map.Map<UsuarioDTO>(byId);
            dtoRetorno.FotoAnterior = fotoAnterior;
            dtoRetorno.UsuariosInformacoes.LojinhaImagemCapaAnterior = fotoCapaAnterior;

            return dtoRetorno;
        }

        public async Task<UsuarioDTO>? AtualizarDadosPessoais(int usuarioId, UsuarioSenhaDTO dto)
        {
            var byId = await _context.Usuarios.
                       Include(ut => ut.UsuariosTipos).
                       Include(ui => ui.UsuariosInformacoes).
                       Where(ui => ui.UsuarioId == usuarioId).AsNoTracking().FirstOrDefaultAsync();

            if (byId is null)
            {
                UsuarioDTO erro = new() { Erro = true, CodigoErro = (int)CodigoErrosEnum.FalhaAoAtualizarDados, MensagemErro = GetDescricaoEnum(CodigoErrosEnum.FalhaAoAtualizarDados) };
                return erro;
            }

            // #1 - Verificar se o usuário já existe com o e-mail ou nome de usuário do sistema informados. Se existir, aborte;
            var verificarUsuario = await GetByEmailOuUsuarioSistemaDiferenteDoMeu(usuarioId, dto?.Email, dto?.NomeUsuarioSistema);

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

            // #3.1 - Se o usuário não tiver dados na tabela UsuariosInformacoes, a classe deve ser instanciada;
            if (byId.UsuariosInformacoes is null)
            {
                byId.UsuariosInformacoes = new();
            }

            // #3.2 - Atualizar dados;
            byId.NomeCompleto = dto.NomeCompleto;
            byId.NomeUsuarioSistema = dto.NomeUsuarioSistema;
            byId.Email = dto.Email;
            byId.UsuariosInformacoes.DataAniversario = dto.UsuariosInformacoes?.DataAniversario > DateTime.MinValue ? dto.UsuariosInformacoes.DataAniversario : DateTime.MinValue;
            byId.UsuariosInformacoes.CPF = dto.UsuariosInformacoes?.CPF ?? "";
            byId.UsuariosInformacoes.Telefone = dto.UsuariosInformacoes?.Telefone ?? "";
            byId.UsuariosInformacoes.DataUltimaAlteracao = HorarioBrasilia();

            _context.Update(byId);
            await _context.SaveChangesAsync();

            UsuarioDTO dtoRetorno = _map.Map<UsuarioDTO>(byId);
            return dtoRetorno;
        }

        public async Task<UsuarioDTO>? AtualizarDadosEndereco(int usuarioId, UsuarioDTO dto)
        {
            var byId = await _context.Usuarios.
                       Include(ut => ut.UsuariosTipos).
                       Include(ui => ui.UsuariosInformacoes).
                       Where(ui => ui.UsuarioId == usuarioId).AsNoTracking().FirstOrDefaultAsync();

            if (byId is null)
            {
                UsuarioDTO erro = new() { Erro = true, CodigoErro = (int)CodigoErrosEnum.FalhaAoAtualizarDados, MensagemErro = GetDescricaoEnum(CodigoErrosEnum.FalhaAoAtualizarDados) };
                return erro;
            }

            // Se o usuário não tiver dados na tabela UsuariosInformacoes, a classe deve ser instanciada;
            if (byId.UsuariosInformacoes is null)
            {
                byId.UsuariosInformacoes = new();
            }

            // Atualizar dados;
            byId.UsuariosInformacoes.CEP = dto.UsuariosInformacoes?.CEP ?? "";
            byId.UsuariosInformacoes.Estado = dto.UsuariosInformacoes?.Estado ?? "";
            byId.UsuariosInformacoes.Cidade = dto.UsuariosInformacoes?.Cidade ?? "";
            byId.UsuariosInformacoes.Bairro = dto.UsuariosInformacoes?.Bairro ?? "";
            byId.UsuariosInformacoes.Rua = dto.UsuariosInformacoes?.Rua ?? "";
            byId.UsuariosInformacoes.NumeroResidencia = dto.UsuariosInformacoes?.NumeroResidencia ?? "";
            byId.UsuariosInformacoes.ReferenciaLocal = dto.UsuariosInformacoes?.ReferenciaLocal ?? "";
            byId.UsuariosInformacoes.DataUltimaAlteracao = HorarioBrasilia();

            _context.Update(byId);
            await _context.SaveChangesAsync();

            UsuarioDTO dtoRetorno = _map.Map<UsuarioDTO>(byId);
            return dtoRetorno;
        }

        public async Task<UsuarioDTO>? DesativarConta(int usuarioId, UsuarioSenhaDTO dto)
        {
            var usuarioBd = await _context.Usuarios.FindAsync(usuarioId);

            if (usuarioBd is null)
            {
                UsuarioDTO erro = new() { Erro = true, CodigoErro = (int)CodigoErrosEnum.NaoEncontrado, MensagemErro = GetDescricaoEnum(CodigoErrosEnum.NaoEncontrado) };
                return erro;
            }

            if (dto.Senha != Descriptografar(usuarioBd.Senha))
            {
                UsuarioDTO erro = new() { Erro = true, CodigoErro = (int)CodigoErrosEnum.UsuarioSenhaIncorretos, MensagemErro = GetDescricaoEnum(CodigoErrosEnum.UsuarioSenhaIncorretos) };
                return erro;
            }

            usuarioBd.IsAtivo = false;

            _context.Update(usuarioBd);
            await _context.SaveChangesAsync();

            UsuarioDTO usuarioDTO = _map.Map<UsuarioDTO>(usuarioBd);
            return usuarioDTO;
        }

        public async Task<AtualizarSenhaDTO>? AtualizarSenha(int usuarioId, AtualizarSenhaDTO dto)
        {
            var usuarioBd = await _context.Usuarios.FindAsync(usuarioId);

            if (usuarioBd is null)
            {
                AtualizarSenhaDTO erro = new() { Erro = true, CodigoErro = (int)CodigoErrosEnum.NaoEncontrado, MensagemErro = GetDescricaoEnum(CodigoErrosEnum.NaoEncontrado) };
                return erro;
            }

            // #1 - Verificar se a senha atual está correta;
            if (dto.SenhaAtual != Descriptografar(usuarioBd.Senha))
            {
                AtualizarSenhaDTO erro = new() { Erro = true, CodigoErro = (int)CodigoErrosEnum.SenhaIncorretaAoAtualizar, MensagemErro = GetDescricaoEnum(CodigoErrosEnum.SenhaIncorretaAoAtualizar) };
                return erro;
            }

            // #2 - Verificar se a nova senha coincide com a confirmação;
            if (dto.SenhaNova != dto.SenhaNovaConfirmacao)
            {
                AtualizarSenhaDTO erro = new() { Erro = true, CodigoErro = (int)CodigoErrosEnum.SenhasNaoCoincidem, MensagemErro = GetDescricaoEnum(CodigoErrosEnum.SenhasNaoCoincidem) };
                return erro;
            }

            // #3 - Verificar se a nova senha atende os requisitos mínimos;
            var validarSenha = ValidarSenha(dto?.SenhaNova, usuarioBd?.NomeCompleto, usuarioBd?.NomeUsuarioSistema, usuarioBd?.Email);
            if (!validarSenha.Item1)
            {
                AtualizarSenhaDTO erro = new() { Erro = true, CodigoErro = (int)CodigoErrosEnum.RequisitosSenhaNaoCumprido, MensagemErro = validarSenha.Item2 };
                return erro;
            }

            // #4 - Atualizar senha;
            usuarioBd.Senha = Criptografar(dto.SenhaNova);

            _context.Update(usuarioBd);
            await _context.SaveChangesAsync();

            AtualizarSenhaDTO semErro = new() { Erro = false, CodigoErro = (int)CodigoErrosEnum.OK, MensagemErro = GetDescricaoEnum(CodigoErrosEnum.OK) };
            return semErro;
        }

        public async Task<UsuarioDTO>? EmailRecuperarSenha(UsuarioDTO dto)
        {
            var usuarioBd = await _context.Usuarios.Where(e => e.Email == dto.Email).AsNoTracking().FirstOrDefaultAsync();

            if (usuarioBd is null)
            {
                UsuarioDTO erro = new() { Erro = true, CodigoErro = (int)CodigoErrosEnum.NaoEncontrado, MensagemErro = GetDescricaoEnum(CodigoErrosEnum.NaoEncontrado) };
                return erro;
            }

            // #1 - Verificar se conta está desativada;
            if (!usuarioBd.IsAtivo)
            {
                UsuarioDTO erro = new() { Erro = true, CodigoErro = (int)CodigoErrosEnum.ContaDesativada, MensagemErro = GetDescricaoEnum(CodigoErrosEnum.ContaDesativada) };
                return erro;
            }

            // #2 - Se a hash estiver válida, não envie outro e-mail;
            if (usuarioBd?.ValidadeHashUrlTrocarSenha >= HorarioBrasilia())
            {
                UsuarioDTO erro = new() { Erro = true, CodigoErro = (int)CodigoErrosEnum.EmailRecuperacaoJaEnviado, MensagemErro = GetDescricaoEnum(CodigoErrosEnum.EmailRecuperacaoJaEnviado) };
                return erro;
            }

            // #3 - Gerar hash de troca de senha;
            string hash = GerarHashUsuario(usuarioBd.UsuarioId);
            usuarioBd.HashUrlTrocarSenha = hash;
            usuarioBd.ValidadeHashUrlTrocarSenha = HorarioBrasilia().AddHours(1);

            _context.Update(usuarioBd);
            await _context.SaveChangesAsync();

            // #4 - Enviar e-mail;
            string assunto = "Recupere a senha da sua conta do GeekSpot";
            string nomeArquivo = GetDescricaoEnum(EmailEnum.RecuperarSenha);

            List<EmailDadosReplace> listaDadosReplace = new()
            {
                new EmailDadosReplace { Key = "Url", Value = $"{CaminhoFront()}/usuario/recuperar-senha/{hash}"},
                new EmailDadosReplace { Key = "NomeUsuario", Value = usuarioBd.NomeCompleto ?? "geek" }
            };

            bool resposta = await EnviarEmail(dto.Email, assunto, nomeArquivo, listaDadosReplace);

            UsuarioDTO semErro = new() { Erro = false, CodigoErro = (int)CodigoErrosEnum.OK, MensagemErro = GetDescricaoEnum(CodigoErrosEnum.OK) };
            return semErro;
        }

        public async Task<AtualizarSenhaDTO>? AtualizarSenhaRecuperar(AtualizarSenhaDTO dto)
        {
            var usuarioBd = await _context.Usuarios.
                            Where(h => h.HashUrlTrocarSenha == dto.Hash).AsNoTracking().FirstOrDefaultAsync();

            if (usuarioBd is null)
            {
                AtualizarSenhaDTO erro = new() { Erro = true, CodigoErro = (int)CodigoErrosEnum.HashRecuperacaoNaoExiste, MensagemErro = GetDescricaoEnum(CodigoErrosEnum.HashRecuperacaoNaoExiste) };
                return erro;
            }

            // #1 - Verificar se a nova senha coincide com a confirmação;
            if (dto.SenhaNova != dto.SenhaNovaConfirmacao)
            {
                AtualizarSenhaDTO erro = new() { Erro = true, CodigoErro = (int)CodigoErrosEnum.SenhasNaoCoincidem, MensagemErro = GetDescricaoEnum(CodigoErrosEnum.SenhasNaoCoincidem) };
                return erro;
            }

            // #2 - Verificar se o hash ainda é válido; 
            if (HorarioBrasilia() > usuarioBd.ValidadeHashUrlTrocarSenha)
            {
                AtualizarSenhaDTO erro = new() { Erro = true, CodigoErro = (int)CodigoErrosEnum.CodigoExpirado, MensagemErro = GetDescricaoEnum(CodigoErrosEnum.CodigoExpirado) };
                return erro;
            }

            // #3 - Verificar se a nova senha atende os requisitos mínimos;
            var validarSenha = ValidarSenha(dto?.SenhaNova, usuarioBd?.NomeCompleto, usuarioBd?.NomeUsuarioSistema, usuarioBd?.Email);
            if (!validarSenha.Item1)
            {
                AtualizarSenhaDTO erro = new() { Erro = true, CodigoErro = (int)CodigoErrosEnum.RequisitosSenhaNaoCumprido, MensagemErro = validarSenha.Item2 };
                return erro;
            }

            // #4 - Atualizar senha;
            usuarioBd.Senha = Criptografar(dto.SenhaNova);
            usuarioBd.HashUrlTrocarSenha = "";
            usuarioBd.ValidadeHashUrlTrocarSenha = DateTime.MinValue;

            _context.Update(usuarioBd);
            await _context.SaveChangesAsync();

            AtualizarSenhaDTO semErro = new() { Erro = false, CodigoErro = (int)CodigoErrosEnum.OK, MensagemErro = GetDescricaoEnum(CodigoErrosEnum.OK) };
            return semErro;
        }

        public async Task<UsuarioDTO>? EmailVerificarConta(int usuarioId)
        {
            var usuarioBd = await _context.Usuarios.FindAsync(usuarioId);

            if (usuarioBd is null)
            {
                UsuarioDTO erro = new() { Erro = true, CodigoErro = (int)CodigoErrosEnum.NaoEncontrado, MensagemErro = GetDescricaoEnum(CodigoErrosEnum.NaoEncontrado) };
                return erro;
            }

            // #1 - Verificar se conta já estava ativa;
            if (usuarioBd.IsVerificado)
            {
                UsuarioDTO erro = new() { Erro = true, CodigoErro = (int)CodigoErrosEnum.ContaJaVerificada, MensagemErro = GetDescricaoEnum(CodigoErrosEnum.ContaJaVerificada) };
                return erro;
            }

            // #2 - Se o código estiver válido, não envie outro e-mail;
            if (usuarioBd?.ValidadeCodigoVerificacao >= HorarioBrasilia())
            {
                UsuarioDTO erro = new() { Erro = true, CodigoErro = (int)CodigoErrosEnum.EmailValidacaoJaEnviado, MensagemErro = GetDescricaoEnum(CodigoErrosEnum.EmailValidacaoJaEnviado) };
                return erro;
            }

            // #3 - Gerar código de verificação e atualizar;
            string codigoVerificacao = GerarStringAleatoria(6, true);
            usuarioBd.CodigoVerificacao = codigoVerificacao;
            usuarioBd.ValidadeCodigoVerificacao = HorarioBrasilia().AddHours(24);

            _context.Update(usuarioBd);
            await _context.SaveChangesAsync();

            // #4 - Enviar e-mail de verificação de conta;
            try
            {
                if (!String.IsNullOrEmpty(usuarioBd?.Email) && !String.IsNullOrEmpty(usuarioBd?.NomeCompleto) && !String.IsNullOrEmpty(codigoVerificacao))
                {
                    await AutenticarService.EnviarEmailVerificacaoConta(usuarioBd?.Email, usuarioBd?.NomeCompleto, codigoVerificacao);
                }
            }
            catch (Exception)
            {
                UsuarioDTO erro = new() { Erro = true, CodigoErro = (int)CodigoErrosEnum.ContaNaoVerificadaComFalhaNoEnvioNovoEmailVerificacao, MensagemErro = GetDescricaoEnum(CodigoErrosEnum.ContaNaoVerificadaComFalhaNoEnvioNovoEmailVerificacao) };
                return erro;
            }

            UsuarioDTO semErro = new() { Erro = false, CodigoErro = (int)CodigoErrosEnum.OK, MensagemErro = GetDescricaoEnum(CodigoErrosEnum.OK) };
            return semErro;
        }
    }
}
