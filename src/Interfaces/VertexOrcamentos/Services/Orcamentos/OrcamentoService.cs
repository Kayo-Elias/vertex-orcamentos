using Microsoft.EntityFrameworkCore;
using VertexOrcamentos.Data;
using VertexOrcamentos.Data.Entities;

namespace VertexOrcamentos.Services.Orcamentos;

public sealed class OrcamentoService(AppDbContext db)
{
    public async Task<OrcamentoResumoModel> ObterResumoAsync(CancellationToken cancellationToken = default)
    {
        return new OrcamentoResumoModel
        {
            OrcamentosCriados = await db.Orcamentos.CountAsync(cancellationToken),
            ProdutosEstoque = await db.Produtos.CountAsync(x => x.ProdutoAtivo, cancellationToken)
        };
    }

    public string CriarCodigo()
    {
        return $"ORC-{DateTime.Now:yyyyMMddHHmmss}";
    }

    public async Task SalvarAsync(OrcamentoCabecalhoModel cabecalho, List<ItemOrcamentoModel> itens, CancellationToken cancellationToken = default)
    {
        if (cabecalho.ClienteId is null)
        {
            throw new InvalidOperationException("Selecione um cliente cadastrado.");
        }

        if (itens.Count == 0)
        {
            throw new InvalidOperationException("Adicione pelo menos um produto ao orcamento.");
        }

        var usuario = await db.Usuarios.AsNoTracking().FirstAsync(cancellationToken);
        var primeiroProdutoId = itens[0].ProdutoId;
        var agora = DateTimeOffset.UtcNow;
        var subtotal = itens.Sum(x => x.TotalItem);
        var total = subtotal + cabecalho.Frete;

        var orcamento = new OrcamentoEntity
        {
            Codigo = string.IsNullOrWhiteSpace(cabecalho.Id) ? CriarCodigo() : cabecalho.Id,
            UsuarioId = usuario.Id,
            ClienteId = cabecalho.ClienteId.Value,
            ProdutoId = primeiroProdutoId,
            Status = "Pendente",
            ValorTotal = total,
            CondicaoPagamento = cabecalho.CondicaoPagamento.Trim(),
            ValidadeDias = cabecalho.Validade,
            Frete = cabecalho.Frete,
            Observacoes = cabecalho.Observacoes.Trim(),
            DataCriacao = agora
        };

        foreach (var item in itens)
        {
            orcamento.Produtos.Add(new OrcamentoProdutoEntity
            {
                ProdutoId = item.ProdutoId,
                Quantidade = item.Quantidade,
                ValorUnitario = item.ValorUnitario,
                DescontoPercentual = item.Desconto,
                Observacao = item.Observacao.Trim(),
                DataCriacao = agora
            });
        }

        db.Orcamentos.Add(orcamento);
        await db.SaveChangesAsync(cancellationToken);
        cabecalho.IdBanco = orcamento.Id;
        cabecalho.Id = orcamento.Codigo;
    }
}
