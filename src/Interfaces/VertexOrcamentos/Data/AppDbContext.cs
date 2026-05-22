using Microsoft.EntityFrameworkCore;
using VertexOrcamentos.Data.Entities;

namespace VertexOrcamentos.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<PessoaEntity> Pessoas => Set<PessoaEntity>();

    public DbSet<UsuarioEntity> Usuarios => Set<UsuarioEntity>();

    public DbSet<ProdutoEntity> Produtos => Set<ProdutoEntity>();

    public DbSet<OrcamentoEntity> Orcamentos => Set<OrcamentoEntity>();

    public DbSet<OrcamentoProdutoEntity> OrcamentoProdutos => Set<OrcamentoProdutoEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("pessoas");

        modelBuilder.Entity<PessoaEntity>(entity =>
        {
            entity.ToTable("tb_pessoa", "pessoas");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.Cpf).HasColumnName("cpf").HasMaxLength(14).IsRequired();
            entity.Property(x => x.PrimeiroNome).HasColumnName("primeiro_nome").HasMaxLength(20).IsRequired();
            entity.Property(x => x.Telefone).HasColumnName("telefone").HasMaxLength(20).IsRequired();
            entity.Property(x => x.Email).HasColumnName("email").HasMaxLength(120).IsRequired();
            entity.Property(x => x.Endereco).HasColumnName("endereco").HasMaxLength(100).IsRequired();
            entity.Property(x => x.TipoPessoa).HasColumnName("tipo_pessoa").HasMaxLength(20).IsRequired();
            entity.Property(x => x.Status).HasColumnName("status").HasMaxLength(20).IsRequired();
            entity.Property(x => x.DataCriacao).HasColumnName("data_criacao").IsRequired();
            entity.Property(x => x.DataModificacao).HasColumnName("data_modificacao");
        });

        modelBuilder.Entity<UsuarioEntity>(entity =>
        {
            entity.ToTable("tb_usuario", "pessoas");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.PessoaId).HasColumnName("id_pessoa").IsRequired();
            entity.Property(x => x.Login).HasColumnName("login").HasMaxLength(36).IsRequired();
            entity.Property(x => x.Senha).HasColumnName("senha").HasMaxLength(50).IsRequired();
            entity.Property(x => x.Cargo).HasColumnName("cargo").HasMaxLength(36).IsRequired();
            entity.Property(x => x.TentativasRestantes).HasColumnName("tentativas_restantes").IsRequired();
            entity.Property(x => x.MaxTentativasSegundos).HasColumnName("max_tentativas_segundos").IsRequired();
            entity.Property(x => x.DataHoraUltimoAcesso).HasColumnName("datahora_ultimo_acesso").IsRequired();
            entity.Property(x => x.UsuarioAtivo).HasColumnName("usuario_ativo").IsRequired();
            entity.Property(x => x.DataCriacao).HasColumnName("data_criacao").IsRequired();
            entity.Property(x => x.DataModificacao).HasColumnName("data_modificacao");

            entity.HasOne(x => x.Pessoa)
                .WithOne(x => x.Usuario)
                .HasForeignKey<UsuarioEntity>(x => x.PessoaId);

            entity.HasIndex(x => x.Login).IsUnique();
        });

        modelBuilder.Entity<ProdutoEntity>(entity =>
        {
            entity.ToTable("tb_produto", "estoque");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.NomeProduto).HasColumnName("nome_produto").HasMaxLength(80).IsRequired();
            entity.Property(x => x.QuantidadeEstoque).HasColumnName("quantidade_estoque").HasPrecision(10, 2).IsRequired();
            entity.Property(x => x.UnidadeMedida).HasColumnName("unidade_medida").HasMaxLength(20).IsRequired();
            entity.Property(x => x.ValorUnitario).HasColumnName("valor_unitario").HasPrecision(10, 2).IsRequired();
            entity.Property(x => x.CategoriaProduto).HasColumnName("categoria_produto").HasMaxLength(20).IsRequired();
            entity.Property(x => x.ProdutoAtivo).HasColumnName("produto_ativo").IsRequired();
            entity.Property(x => x.DataCriacao).HasColumnName("data_criacao").IsRequired();
            entity.Property(x => x.DataModificacao).HasColumnName("data_modificacao");
        });

        modelBuilder.Entity<OrcamentoEntity>(entity =>
        {
            entity.ToTable("tb_orcamento", "documentos");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.UsuarioId).HasColumnName("id_usuario").IsRequired();
            entity.Property(x => x.ClienteId).HasColumnName("id_cliente").IsRequired();
            entity.Property(x => x.ProdutoId).HasColumnName("id_produto").IsRequired();
            entity.Property(x => x.Codigo).HasColumnName("codigo").HasMaxLength(20).IsRequired();
            entity.Property(x => x.Status).HasColumnName("status").HasMaxLength(20).IsRequired();
            entity.Property(x => x.ValorTotal).HasColumnName("valor_total").HasPrecision(12, 2).IsRequired();
            entity.Property(x => x.CondicaoPagamento).HasColumnName("condicao_pagamento").HasMaxLength(50).IsRequired();
            entity.Property(x => x.ValidadeDias).HasColumnName("validade_dias").IsRequired();
            entity.Property(x => x.Frete).HasColumnName("frete").HasPrecision(10, 2).IsRequired();
            entity.Property(x => x.Observacoes).HasColumnName("observacoes").HasMaxLength(300).IsRequired();
            entity.Property(x => x.DataCriacao).HasColumnName("data_criacao").IsRequired();
            entity.Property(x => x.DataModificacao).HasColumnName("data_modificacao");

            entity.HasOne(x => x.Usuario).WithMany(x => x.Orcamentos).HasForeignKey(x => x.UsuarioId);
            entity.HasOne(x => x.Cliente).WithMany(x => x.OrcamentosCliente).HasForeignKey(x => x.ClienteId);
            entity.HasOne(x => x.Produto).WithMany().HasForeignKey(x => x.ProdutoId);
        });

        modelBuilder.Entity<OrcamentoProdutoEntity>(entity =>
        {
            entity.ToTable("tb_orcamento_produto", "documentos");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.OrcamentoId).HasColumnName("id_orcamento").IsRequired();
            entity.Property(x => x.ProdutoId).HasColumnName("id_produto").IsRequired();
            entity.Property(x => x.Quantidade).HasColumnName("quantidade").HasPrecision(10, 2).IsRequired();
            entity.Property(x => x.ValorUnitario).HasColumnName("valor_unitario").HasPrecision(10, 2).IsRequired();
            entity.Property(x => x.DescontoPercentual).HasColumnName("desconto_percentual").HasPrecision(5, 2).IsRequired();
            entity.Property(x => x.Observacao).HasColumnName("observacao").HasMaxLength(200).IsRequired();
            entity.Property(x => x.DataCriacao).HasColumnName("data_criacao").IsRequired();
            entity.Property(x => x.DataModificacao).HasColumnName("data_modificacao");

            entity.HasOne(x => x.Orcamento).WithMany(x => x.Produtos).HasForeignKey(x => x.OrcamentoId);
            entity.HasOne(x => x.Produto).WithMany(x => x.OrcamentoProdutos).HasForeignKey(x => x.ProdutoId);
        });
    }
}
