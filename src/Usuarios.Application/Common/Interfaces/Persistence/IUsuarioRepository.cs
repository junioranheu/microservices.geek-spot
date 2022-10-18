using Usuarios.Domain.DTO;

namespace Usuarios.Application.Common.Interfaces.Persistence
{
    public interface IUsuarioRepository
    {
        Task<UsuarioDTO>? Adicionar(UsuarioSenhaDTO dto);
        Task<UsuarioDTO>? Atualizar(UsuarioSenhaDTO dto);
        Task<List<UsuarioDTO>>? GetTodos();
        Task<UsuarioDTO>? GetById(int id);
        Task<UsuarioSenhaDTO>? GetByEmailOuUsuarioSistema(string? email, string? nomeUsuarioSistema);
        Task<UsuarioSenhaDTO>? GetByEmailOuUsuarioSistemaDiferenteDoMeu(int usuarioId, string? email, string? nomeUsuarioSistema);
        Task? AtualizarFoto(int usuarioId, string foto);
        Task<string>? AtualizarCodigoVerificacao(int usuarioId);
        Task<UsuarioDTO>? VerificarConta(string codigoVerificacao);
        Task<UsuarioDTO>? AtualizarDadosLojinha(int usuarioId, UsuarioDTO dto);
        Task<UsuarioDTO>? AtualizarDadosPessoais(int usuarioId, UsuarioSenhaDTO dto);
        Task<UsuarioDTO>? AtualizarDadosEndereco(int usuarioId, UsuarioDTO dto);
        Task<UsuarioDTO>? DesativarConta(int usuarioId, UsuarioSenhaDTO dto);
        Task<AtualizarSenhaDTO>? AtualizarSenha(int usuarioId, AtualizarSenhaDTO dto);
        Task<UsuarioDTO>? EmailRecuperarSenha(UsuarioDTO dto);
        Task<AtualizarSenhaDTO>? AtualizarSenhaRecuperar(AtualizarSenhaDTO dto);
        Task<UsuarioDTO>? EmailVerificarConta(int usuarioId);
    }
}
