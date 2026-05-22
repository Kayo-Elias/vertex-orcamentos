using Microsoft.EntityFrameworkCore;
using VertexOrcamentos.Data;
using VertexOrcamentos.Data.Entities;

namespace VertexOrcamentos.Services.Pedidos;

public sealed class PedidoService(AppDbContext db)
{
    public async Task<List<PedidoListItem>> ListarAsync(string? busca = null, string? status = null, DateTime? data = null, CancellationToken cancellationToken = default)
    {
        var query = db.Orcamentos.AsNoTracking().Include(x => x.Cliente).AsQueryable();

        if (!string.IsNullOrWhiteSpace(busca))
        {
            var termo = busca.Trim().ToLower();
            query = query.Where(x => x.Codigo.ToLower().Contains(termo) || x.Cliente.PrimeiroNome.ToLower().Contains(termo));
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(x => x.Status == status);
        }

        if (data.HasValue)
        {
            var inicio = new DateTimeOffset(data.Value.Date, TimeSpan.Zero);
            var fim = inicio.AddDays(1);
            query = query.Where(x => x.DataCriacao >= inicio && x.DataCriacao < fim);
        }

        return await query
            .OrderByDescending(x => x.DataCriacao)
            .Select(x => new PedidoListItem
            {
                Id = x.Id,
                Codigo = x.Codigo,
                Cliente = x.Cliente.PrimeiroNome,
                Valor = x.ValorTotal,
                Status = x.Status,
                Data = x.DataCriacao
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<PedidoResumo> ObterResumoAsync(CancellationToken cancellationToken = default)
    {
        var pedidos = db.Orcamentos.AsNoTracking();
        return new PedidoResumo
        {
            Total = await pedidos.CountAsync(cancellationToken),
            Pagos = await pedidos.CountAsync(x => x.Status == "Pago", cancellationToken),
            Pendentes = await pedidos.CountAsync(x => x.Status == "Pendente", cancellationToken),
            Cancelados = await pedidos.CountAsync(x => x.Status == "Cancelado", cancellationToken)
        };
    }

    public async Task AtualizarStatusAsync(Guid id, string status, CancellationToken cancellationToken = default)
    {
        var pedido = await db.Orcamentos.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new InvalidOperationException("Pedido nao encontrado.");

        pedido.Status = NormalizarStatus(status);
        pedido.DataModificacao = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task ExcluirAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var pedido = await db.Orcamentos.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new InvalidOperationException("Pedido nao encontrado.");

        pedido.Status = "Cancelado";
        pedido.DataModificacao = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task CriarAsync(PedidoFormModel model, CancellationToken cancellationToken = default)
    {
        var produto = await db.Produtos.FirstOrDefaultAsync(x => x.Id == model.ProdutoId && x.ProdutoAtivo, cancellationToken)
            ?? throw new InvalidOperationException("Produto nao encontrado.");

        var usuario = await db.Usuarios.FirstAsync(cancellationToken);
        var quantidade = Math.Max(1, model.Quantidade);
        var total = quantidade * produto.ValorUnitario;
        var agora = DateTimeOffset.UtcNow;

        var pedido = new OrcamentoEntity
        {
            Codigo = $"PED-{agora:yyyyMMddHHmmss}",
            ClienteId = model.ClienteId,
            UsuarioId = usuario.Id,
            ProdutoId = produto.Id,
            Status = NormalizarStatus(model.Status),
            ValorTotal = total,
            CondicaoPagamento = "Cartao",
            ValidadeDias = 7,
            Frete = 0,
            Observacoes = "Pedido criado pela tela de pedidos.",
            DataCriacao = agora
        };

        pedido.Produtos.Add(new OrcamentoProdutoEntity
        {
            ProdutoId = produto.Id,
            Quantidade = quantidade,
            ValorUnitario = produto.ValorUnitario,
            DescontoPercentual = 0,
            Observacao = string.Empty,
            DataCriacao = agora
        });

        if (pedido.Status == "Pago")
        {
            produto.QuantidadeEstoque = Math.Max(0, produto.QuantidadeEstoque - quantidade);
        }

        db.Orcamentos.Add(pedido);
        await db.SaveChangesAsync(cancellationToken);
    }

    private static string NormalizarStatus(string status)
    {
        return status switch
        {
            "Pago" => "Pago",
            "Cancelado" => "Cancelado",
            _ => "Pendente"
        };
    }
}
