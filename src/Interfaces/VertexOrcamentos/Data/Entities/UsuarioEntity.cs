namespace VertexOrcamentos.Data.Entities;

public sealed class UsuarioEntity
{
    public Guid Id { get; set; }

    public Guid PessoaId { get; set; }

    public string Login { get; set; } = string.Empty;

    public string Senha { get; set; } = string.Empty;

    public string Cargo { get; set; } = string.Empty;

    public int TentativasRestantes { get; set; }

    public int MaxTentativasSegundos { get; set; }

    public DateTimeOffset DataHoraUltimoAcesso { get; set; }

    public bool UsuarioAtivo { get; set; }

    public DateTimeOffset DataCriacao { get; set; }

    public DateTimeOffset? DataModificacao { get; set; }

    public PessoaEntity Pessoa { get; set; } = null!;

    public List<OrcamentoEntity> Orcamentos { get; set; } = [];
}
