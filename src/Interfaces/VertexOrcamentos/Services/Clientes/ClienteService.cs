using Microsoft.EntityFrameworkCore;
using VertexOrcamentos.Data;
using VertexOrcamentos.Data.Entities;

namespace VertexOrcamentos.Services.Clientes;

public sealed class ClienteService(AppDbContext db)
{
    public async Task<List<ClienteListItem>> ListarAsync(string? busca = null, CancellationToken cancellationToken = default)
    {
        var query = db.Pessoas.AsNoTracking().Where(x => x.TipoPessoa == "Cliente");

        if (!string.IsNullOrWhiteSpace(busca))
        {
            var termo = busca.Trim().ToLower();
            query = query.Where(x =>
                x.PrimeiroNome.ToLower().Contains(termo)
                || x.Email.ToLower().Contains(termo)
                || x.Telefone.ToLower().Contains(termo)
                || x.Status.ToLower().Contains(termo)
                || x.Cpf.ToLower().Contains(termo));
        }

        return await query
            .OrderBy(x => x.PrimeiroNome)
            .Select(x => new ClienteListItem
            {
                Id = x.Id,
                Nome = x.PrimeiroNome,
                Cpf = x.Cpf,
                Email = x.Email,
                Telefone = x.Telefone,
                Endereco = x.Endereco,
                Status = x.Status
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<ClienteResumo> ObterResumoAsync(CancellationToken cancellationToken = default)
    {
        var clientes = db.Pessoas.AsNoTracking().Where(x => x.TipoPessoa == "Cliente");

        return new ClienteResumo
        {
            Total = await clientes.CountAsync(cancellationToken),
            Ativos = await clientes.CountAsync(x => x.Status == "Ativo", cancellationToken),
            Pendentes = await clientes.CountAsync(x => x.Status == "Pendente", cancellationToken),
            Bloqueados = await clientes.CountAsync(x => x.Status == "Bloqueado", cancellationToken)
        };
    }

    public async Task<ClienteFormModel?> ObterAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await db.Pessoas.AsNoTracking()
            .Where(x => x.Id == id && x.TipoPessoa == "Cliente")
            .Select(x => new ClienteFormModel
            {
                Id = x.Id,
                Nome = x.PrimeiroNome,
                Cpf = x.Cpf,
                Email = x.Email,
                Telefone = x.Telefone,
                Endereco = x.Endereco,
                Status = x.Status
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task CriarAsync(ClienteFormModel model, CancellationToken cancellationToken = default)
    {
        var agora = DateTimeOffset.UtcNow;
        db.Pessoas.Add(new PessoaEntity
        {
            PrimeiroNome = model.Nome.Trim(),
            Cpf = model.Cpf.Trim(),
            Email = model.Email.Trim(),
            Telefone = model.Telefone.Trim(),
            Endereco = model.Endereco.Trim(),
            TipoPessoa = "Cliente",
            Status = NormalizarStatus(model.Status),
            DataCriacao = agora
        });

        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task AtualizarAsync(Guid id, ClienteFormModel model, CancellationToken cancellationToken = default)
    {
        var cliente = await db.Pessoas.FirstOrDefaultAsync(x => x.Id == id && x.TipoPessoa == "Cliente", cancellationToken)
            ?? throw new InvalidOperationException("Cliente nao encontrado.");

        cliente.PrimeiroNome = model.Nome.Trim();
        cliente.Cpf = model.Cpf.Trim();
        cliente.Email = model.Email.Trim();
        cliente.Telefone = model.Telefone.Trim();
        cliente.Endereco = model.Endereco.Trim();
        cliente.Status = NormalizarStatus(model.Status);
        cliente.DataModificacao = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task ExcluirAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cliente = await db.Pessoas.FirstOrDefaultAsync(x => x.Id == id && x.TipoPessoa == "Cliente", cancellationToken)
            ?? throw new InvalidOperationException("Cliente nao encontrado.");

        var possuiOrcamento = await db.Orcamentos.AnyAsync(x => x.ClienteId == id, cancellationToken);
        if (possuiOrcamento)
        {
            cliente.Status = "Bloqueado";
            cliente.DataModificacao = DateTimeOffset.UtcNow;
        }
        else
        {
            db.Pessoas.Remove(cliente);
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    private static string NormalizarStatus(string status)
    {
        return status switch
        {
            "Pendente" => "Pendente",
            "Bloqueado" => "Bloqueado",
            _ => "Ativo"
        };
    }
}
