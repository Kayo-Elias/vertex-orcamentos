namespace VertexOrcamentos.Data.Entities;

public sealed class OrcamentoEntity
{
    public Guid Id { get; set; }

    public Guid UsuarioId { get; set; }

    public Guid ClienteId { get; set; }

    public Guid ProdutoId { get; set; }

    public string Codigo { get; set; } = string.Empty;

    public string Status { get; set; } = "Pendente";

    public decimal ValorTotal { get; set; }

    public string CondicaoPagamento { get; set; } = "A vista";

    public int ValidadeDias { get; set; } = 15;

    public decimal Frete { get; set; }

    public string Observacoes { get; set; } = string.Empty;

    public DateTimeOffset DataCriacao { get; set; }

    public DateTimeOffset? DataModificacao { get; set; }

    public UsuarioEntity Usuario { get; set; } = null!;

    public PessoaEntity Cliente { get; set; } = null!;

    public ProdutoEntity Produto { get; set; } = null!;

    public List<OrcamentoProdutoEntity> Produtos { get; set; } = [];
}
