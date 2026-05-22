using Microsoft.EntityFrameworkCore;
using VertexOrcamentos.Data;
using VertexOrcamentos.Data.Entities;

namespace VertexOrcamentos.Services.Estoque;

public sealed class EstoqueService(AppDbContext db)
{
    private const decimal LimiteBaixoEstoque = 5m;

    public async Task<List<ProdutoListItem>> ListarAsync(string? busca = null, CancellationToken cancellationToken = default)
    {
        var query = db.Produtos.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(busca))
        {
            var termo = busca.Trim().ToLower();
            query = query.Where(x =>
                x.NomeProduto.ToLower().Contains(termo)
                || x.CategoriaProduto.ToLower().Contains(termo)
                || x.UnidadeMedida.ToLower().Contains(termo));
        }

        return await query
            .OrderBy(x => x.NomeProduto)
            .Select(x => new ProdutoListItem
            {
                Id = x.Id,
                Nome = x.NomeProduto,
                Categoria = x.CategoriaProduto,
                Unidade = x.UnidadeMedida,
                Valor = x.ValorUnitario,
                Quantidade = x.QuantidadeEstoque,
                Ativo = x.ProdutoAtivo
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<EstoqueResumo> ObterResumoAsync(CancellationToken cancellationToken = default)
    {
        var produtos = db.Produtos.AsNoTracking();
        return new EstoqueResumo
        {
            TotalProdutos = await produtos.CountAsync(cancellationToken),
            ProdutosAtivos = await produtos.CountAsync(x => x.ProdutoAtivo, cancellationToken),
            BaixoEstoque = await produtos.CountAsync(x => x.ProdutoAtivo && x.QuantidadeEstoque <= LimiteBaixoEstoque, cancellationToken),
            ValorTotal = await produtos.SumAsync(x => x.QuantidadeEstoque * x.ValorUnitario, cancellationToken)
        };
    }

    public async Task<ProdutoFormModel?> ObterAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await db.Produtos.AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new ProdutoFormModel
            {
                Id = x.Id,
                Nome = x.NomeProduto,
                Categoria = x.CategoriaProduto,
                Unidade = x.UnidadeMedida,
                Valor = x.ValorUnitario,
                Quantidade = x.QuantidadeEstoque,
                Ativo = x.ProdutoAtivo
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task CriarAsync(ProdutoFormModel model, CancellationToken cancellationToken = default)
    {
        db.Produtos.Add(new ProdutoEntity
        {
            NomeProduto = model.Nome.Trim(),
            CategoriaProduto = model.Categoria.Trim(),
            UnidadeMedida = model.Unidade.Trim(),
            ValorUnitario = model.Valor,
            QuantidadeEstoque = model.Quantidade,
            ProdutoAtivo = model.Ativo,
            DataCriacao = DateTimeOffset.UtcNow
        });

        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task AtualizarAsync(Guid id, ProdutoFormModel model, CancellationToken cancellationToken = default)
    {
        var produto = await db.Produtos.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new InvalidOperationException("Produto nao encontrado.");

        produto.NomeProduto = model.Nome.Trim();
        produto.CategoriaProduto = model.Categoria.Trim();
        produto.UnidadeMedida = model.Unidade.Trim();
        produto.ValorUnitario = model.Valor;
        produto.QuantidadeEstoque = model.Quantidade;
        produto.ProdutoAtivo = model.Ativo;
        produto.DataModificacao = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task ExcluirAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var produto = await db.Produtos.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new InvalidOperationException("Produto nao encontrado.");

        var usado = await db.OrcamentoProdutos.AnyAsync(x => x.ProdutoId == id, cancellationToken);
        if (usado)
        {
            produto.ProdutoAtivo = false;
            produto.DataModificacao = DateTimeOffset.UtcNow;
        }
        else
        {
            db.Produtos.Remove(produto);
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}
