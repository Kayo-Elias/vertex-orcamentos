using System.ComponentModel.DataAnnotations;

namespace VertexOrcamentos.Services.Usuarios;

public sealed class UsuarioListItem
{
    public Guid Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Login { get; set; } = string.Empty;

    public string Cargo { get; set; } = string.Empty;

    public bool Ativo { get; set; }

    public DateTimeOffset CriadoEm { get; set; }

    public bool IsAdmin => Cargo.Equals("Admin", StringComparison.OrdinalIgnoreCase)
        || Cargo.Equals("Administrador", StringComparison.OrdinalIgnoreCase);
}

public sealed class UsuarioFormModel
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "Informe o nome.")]
    [MaxLength(20, ErrorMessage = "O banco permite ate 20 caracteres para o nome.")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o CPF.")]
    [MaxLength(14, ErrorMessage = "O CPF deve ter ate 14 caracteres.")]
    public string Cpf { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o telefone.")]
    [MaxLength(20, ErrorMessage = "O telefone deve ter ate 20 caracteres.")]
    public string Telefone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o email.")]
    [EmailAddress(ErrorMessage = "Informe um email valido.")]
    [MaxLength(120, ErrorMessage = "O email deve ter ate 120 caracteres.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o endereco.")]
    [MaxLength(100, ErrorMessage = "O endereco deve ter ate 100 caracteres.")]
    public string Endereco { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o login.")]
    [MaxLength(36, ErrorMessage = "O login deve ter ate 36 caracteres.")]
    public string Login { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Senha { get; set; } = string.Empty;

    [Required(ErrorMessage = "Selecione o cargo.")]
    [MaxLength(36)]
    public string Cargo { get; set; } = "Funcionario";

    public bool Ativo { get; set; } = true;
}
