using System.ComponentModel.DataAnnotations;

namespace VertexOrcamentos.Services.Pedidos;

public sealed class PedidoListItem
{
    public Guid Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Cliente { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public string Status { get; set; } = "Pendente";
    public DateTimeOffset Data { get; set; }
}

public sealed class PedidoResumo
{
    public int Total { get; set; }
    public int Pagos { get; set; }
    public int Pendentes { get; set; }
    public int Cancelados { get; set; }
}

public sealed class PedidoFormModel
{
    public Guid? Id { get; set; }

    [Required]
    public Guid ClienteId { get; set; }

    [Required]
    public Guid ProdutoId { get; set; }

    public decimal Quantidade { get; set; } = 1;

    [Required]
    public string Status { get; set; } = "Pendente";
}
