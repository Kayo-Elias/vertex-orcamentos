namespace VertexOrcamentos.Services.Orcamentos;

public sealed class OrcamentoCabecalhoModel
{
    public Guid? IdBanco { get; set; }
    public string Id { get; set; } = string.Empty;
    public string Vendedor { get; set; } = string.Empty;
    public Guid? ClienteId { get; set; }
    public string ClienteNome { get; set; } = string.Empty;
    public string ClienteCpfCnpj { get; set; } = string.Empty;
    public string ClienteTelefone { get; set; } = string.Empty;
    public string ClienteEmail { get; set; } = string.Empty;
    public string CondicaoPagamento { get; set; } = "A vista";
    public int Validade { get; set; } = 15;
    public decimal Frete { get; set; }
    public string Observacoes { get; set; } = string.Empty;
}

public sealed class ItemOrcamentoModel
{
    public Guid ProdutoId { get; set; }
    public string ProdutoNome { get; set; } = string.Empty;
    public string Unidade { get; set; } = "un";
    public decimal Quantidade { get; set; } = 1;
    public decimal ValorUnitario { get; set; }
    public decimal Desconto { get; set; }
    public string Observacao { get; set; } = string.Empty;
    public decimal TotalItem => ValorUnitario * Quantidade * (1 - Desconto / 100);
}

public sealed class OrcamentoResumoModel
{
    public int OrcamentosCriados { get; set; }
    public int ProdutosEstoque { get; set; }
}
