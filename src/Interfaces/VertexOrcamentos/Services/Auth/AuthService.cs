using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using VertexOrcamentos.Data;

namespace VertexOrcamentos.Services.Auth;

public sealed class AuthService(AppDbContext db)
{
    public async Task<AuthUser?> LoginAsync(string login, string senha, CancellationToken cancellationToken = default)
    {
        var loginNormalizado = login.Trim().ToLower();
        var senhaHash = HashSenha(senha);

        var usuario = await db.Usuarios
            .Include(x => x.Pessoa)
            .FirstOrDefaultAsync(x => x.Login.ToLower() == loginNormalizado && x.UsuarioAtivo, cancellationToken);

        if (usuario is null || usuario.Senha != senhaHash)
        {
            return null;
        }

        usuario.DataHoraUltimoAcesso = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(cancellationToken);

        return new AuthUser
        {
            Id = usuario.Id,
            Nome = usuario.Pessoa.PrimeiroNome,
            Login = usuario.Login,
            Cargo = usuario.Cargo
        };
    }

    private static string HashSenha(string senha)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(senha));
        return Convert.ToBase64String(bytes);
    }
}

public sealed class AuthUser
{
    public Guid Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Login { get; set; } = string.Empty;

    public string Cargo { get; set; } = string.Empty;
}
