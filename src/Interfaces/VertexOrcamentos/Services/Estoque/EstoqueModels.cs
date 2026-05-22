using System.ComponentModel.DataAnnotations;

namespace VertexOrcamentos.Services.Estoque;

public sealed class ProdutoListItem
{
    public Guid Id { get; set; }
    public string Codigo => Id.ToString()[..8].ToUpperInvariant();
    public string Categoria { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public decimal Quantidade { get; set; }
    public string Unidade { get; set; } = "un";
    public bool Ativo { get; set; }
}

public sealed class ProdutoFormModel
{
    public Guid? Id { get; set; }

    [Required]
    [MaxLength(80)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Categoria { get; set; } = "Perifericos";

    [Required]
    [MaxLength(20)]
    public string Unidade { get; set; } = "un";

    public decimal Valor { get; set; }

    public decimal Quantidade { get; set; }

    public bool Ativo { get; set; } = true;
}

public sealed class EstoqueResumo
{
    public int TotalProdutos { get; set; }
    public int ProdutosAtivos { get; set; }
    public int BaixoEstoque { get; set; }
    public decimal ValorTotal { get; set; }
}
