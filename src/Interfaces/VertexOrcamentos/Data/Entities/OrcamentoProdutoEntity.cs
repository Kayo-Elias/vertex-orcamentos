namespace VertexOrcamentos.Data.Entities;

public sealed class OrcamentoProdutoEntity
{
    public Guid Id { get; set; }

    public Guid OrcamentoId { get; set; }

    public Guid ProdutoId { get; set; }

    public decimal Quantidade { get; set; }

    public decimal ValorUnitario { get; set; }

    public decimal DescontoPercentual { get; set; }

    public string Observacao { get; set; } = string.Empty;

    public DateTimeOffset DataCriacao { get; set; }

    public DateTimeOffset? DataModificacao { get; set; }

    public OrcamentoEntity Orcamento { get; set; } = null!;

    public ProdutoEntity Produto { get; set; } = null!;
}
