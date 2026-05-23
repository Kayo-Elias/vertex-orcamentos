using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using VertexOrcamentos.Data;
using VertexOrcamentos.Data.Entities;

namespace VertexOrcamentos.Services.Usuarios;

public sealed class UsuarioService(AppDbContext db)
{
    public async Task<List<UsuarioListItem>> ListarAsync(string? busca = null, CancellationToken cancellationToken = default)
    {
        var query = db.Usuarios
            .AsNoTracking()
            .Include(x => x.Pessoa)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(busca))
        {
            var termo = busca.Trim().ToLower();
            query = query.Where(x =>
                x.Login.ToLower().Contains(termo)
                || x.Cargo.ToLower().Contains(termo)
                || x.Pessoa.PrimeiroNome.ToLower().Contains(termo)
                || x.Pessoa.Email.ToLower().Contains(termo));
        }

        return await query
            .OrderByDescending(x => x.Cargo == "Admin")
            .ThenBy(x => x.Pessoa.PrimeiroNome)
            .Select(x => new UsuarioListItem
            {
                Id = x.Id,
                Nome = x.Pessoa.PrimeiroNome,
                Email = x.Pessoa.Email,
                Login = x.Login,
                Cargo = x.Cargo,
                Ativo = x.UsuarioAtivo,
                CriadoEm = x.DataCriacao
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<UsuarioFormModel?> ObterAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await db.Usuarios
            .AsNoTracking()
            .Include(x => x.Pessoa)
            .Where(x => x.Id == id)
            .Select(x => new UsuarioFormModel
            {
                Id = x.Id,
                Nome = x.Pessoa.PrimeiroNome,
                Cpf = x.Pessoa.Cpf,
                Telefone = x.Pessoa.Telefone,
                Email = x.Pessoa.Email,
                Endereco = x.Pessoa.Endereco,
                Login = x.Login,
                Cargo = x.Cargo,
                Ativo = x.UsuarioAtivo
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<UsuarioListItem> CriarAsync(UsuarioFormModel model, CancellationToken cancellationToken = default)
    {
        await ValidarLoginUnicoAsync(model.Login, null, cancellationToken);

        if (string.IsNullOrWhiteSpace(model.Senha))
        {
            throw new InvalidOperationException("Informe uma senha para criar o usuario.");
        }

        var agora = DateTimeOffset.UtcNow;
        var pessoa = new PessoaEntity
        {
            Cpf = model.Cpf.Trim(),
            PrimeiroNome = model.Nome.Trim(),
            Telefone = model.Telefone.Trim(),
            Email = model.Email.Trim(),
            Endereco = model.Endereco.Trim(),
            DataCriacao = agora
        };

        var usuario = new UsuarioEntity
        {
            Pessoa = pessoa,
            Login = model.Login.Trim(),
            Senha = HashSenha(model.Senha),
            Cargo = NormalizarCargo(model.Cargo),
            TentativasRestantes = 3,
            MaxTentativasSegundos = 300,
            DataHoraUltimoAcesso = agora,
            UsuarioAtivo = model.Ativo,
            DataCriacao = agora
        };

        db.Usuarios.Add(usuario);
        await db.SaveChangesAsync(cancellationToken);

        return new UsuarioListItem
        {
            Id = usuario.Id,
            Nome = pessoa.PrimeiroNome,
            Email = pessoa.Email,
            Login = usuario.Login,
            Cargo = usuario.Cargo,
            Ativo = usuario.UsuarioAtivo,
            CriadoEm = usuario.DataCriacao
        };
    }

    public async Task AtualizarAsync(Guid id, UsuarioFormModel model, CancellationToken cancellationToken = default)
    {
        var usuario = await db.Usuarios
            .Include(x => x.Pessoa)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new InvalidOperationException("Usuario nao encontrado.");

        await ValidarLoginUnicoAsync(model.Login, id, cancellationToken);

        var novoCargo = NormalizarCargo(model.Cargo);

        var agora = DateTimeOffset.UtcNow;
        usuario.Pessoa.PrimeiroNome = model.Nome.Trim();
        usuario.Pessoa.Cpf = model.Cpf.Trim();
        usuario.Pessoa.Telefone = model.Telefone.Trim();
        usuario.Pessoa.Email = model.Email.Trim();
        usuario.Pessoa.Endereco = model.Endereco.Trim();
        usuario.Pessoa.DataModificacao = agora;

        usuario.Login = model.Login.Trim();
        usuario.Cargo = novoCargo;
        usuario.UsuarioAtivo = model.Ativo;
        usuario.DataModificacao = agora;

        if (!string.IsNullOrWhiteSpace(model.Senha))
        {
            usuario.Senha = HashSenha(model.Senha);
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task InativarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var usuario = await db.Usuarios.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new InvalidOperationException("Usuario nao encontrado.");

        usuario.UsuarioAtivo = false;
        usuario.DataModificacao = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(cancellationToken);
    }

    private async Task ValidarLoginUnicoAsync(string login, Guid? usuarioId, CancellationToken cancellationToken)
    {
        var loginNormalizado = login.Trim().ToLower();
        var existe = await db.Usuarios.AnyAsync(x =>
            x.Login.ToLower() == loginNormalizado && (!usuarioId.HasValue || x.Id != usuarioId.Value), cancellationToken);

        if (existe)
        {
            throw new InvalidOperationException("Ja existe um usuario com esse login.");
        }
    }

    private static string NormalizarCargo(string cargo)
    {
        return cargo.Equals("Admin", StringComparison.OrdinalIgnoreCase)
            || cargo.Equals("Administrador", StringComparison.OrdinalIgnoreCase)
            ? "Admin"
            : "Funcionario";
    }

    private static string HashSenha(string senha)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(senha));
        return Convert.ToBase64String(bytes);
    }
}
