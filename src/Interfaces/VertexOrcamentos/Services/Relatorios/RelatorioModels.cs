namespace VertexOrcamentos.Services.Relatorios;

public sealed class RelatorioDashboard
{
    public int TotalProdutos { get; set; }

    public int ProdutosEmFalta { get; set; }

    public int ProdutosCriticos { get; set; }

    public int TotalOrcamentos { get; set; }

    public decimal ValorTotalEstoque { get; set; }

    public string TempoMedioAtendimento { get; set; } = "0 min";

    public int EficienciaOperacional { get; set; }

    public string HorarioPico { get; set; } = "Sem dados";

    public List<ProdutoRelatorioItem> EstoqueCritico { get; set; } = [];

    public List<ProdutoRelatorioItem> ProdutosParados { get; set; } = [];

    public List<ProdutoRelatorioItem> ProdutosBaixaSaida { get; set; } = [];

    public List<ProdutoRelatorioItem> ProdutosMaisVendidos { get; set; } = [];

    public List<string> Alertas { get; set; } = [];
}

public sealed class ProdutoRelatorioItem
{
    public Guid Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Categoria { get; set; } = string.Empty;

    public decimal QuantidadeEstoque { get; set; }

    public string UnidadeMedida { get; set; } = string.Empty;

    public decimal Movimentacoes { get; set; }

    public DateTimeOffset? UltimaMovimentacao { get; set; }

    public decimal ValorUnitario { get; set; }
}
