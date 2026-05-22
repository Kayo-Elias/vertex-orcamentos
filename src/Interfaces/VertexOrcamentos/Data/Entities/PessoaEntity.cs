namespace VertexOrcamentos.Data.Entities;

public sealed class PessoaEntity
{
    public Guid Id { get; set; }

    public string Cpf { get; set; } = string.Empty;

    public string PrimeiroNome { get; set; } = string.Empty;

    public string Telefone { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Endereco { get; set; } = string.Empty;

    public string TipoPessoa { get; set; } = "Cliente";

    public string Status { get; set; } = "Ativo";

    public DateTimeOffset DataCriacao { get; set; }

    public DateTimeOffset? DataModificacao { get; set; }

    public UsuarioEntity? Usuario { get; set; }

    public List<OrcamentoEntity> OrcamentosCliente { get; set; } = [];
}
