namespace VertexOrcamentos.Data.Entities;

public sealed class ProdutoEntity
{
    public Guid Id { get; set; }

    public string NomeProduto { get; set; } = string.Empty;

    public decimal QuantidadeEstoque { get; set; }

    public string UnidadeMedida { get; set; } = string.Empty;

    public decimal ValorUnitario { get; set; }

    public string CategoriaProduto { get; set; } = string.Empty;

    public bool ProdutoAtivo { get; set; } = true;

    public DateTimeOffset DataCriacao { get; set; }

    public DateTimeOffset? DataModificacao { get; set; }

    public List<OrcamentoProdutoEntity> OrcamentoProdutos { get; set; } = [];
}
