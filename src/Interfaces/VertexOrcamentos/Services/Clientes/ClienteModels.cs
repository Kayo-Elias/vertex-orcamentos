using System.ComponentModel.DataAnnotations;

namespace VertexOrcamentos.Services.Clientes;

public sealed class ClienteListItem
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public string Status { get; set; } = "Ativo";
}

public sealed class ClienteFormModel
{
    public Guid? Id { get; set; }

    [Required]
    [MaxLength(20)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [MaxLength(14)]
    public string Cpf { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(120)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Telefone { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Endereco { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = "Ativo";
}

public sealed class ClienteResumo
{
    public int Total { get; set; }
    public int Ativos { get; set; }
    public int Pendentes { get; set; }
    public int Bloqueados { get; set; }
}
