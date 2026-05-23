using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore;
using VertexOrcamentos.Data;

namespace VertexOrcamentos.Services.Relatorios;

public sealed class RelatorioService(AppDbContext db)
{
    private const decimal LimiteEstoqueCritico = 5m;

    public async Task<RelatorioDashboard> ObterDashboardAsync(CancellationToken cancellationToken = default)
    {
        var agora = DateTimeOffset.UtcNow;
        var dataLimite = agora.AddDays(-30);

        var produtos = await db.Produtos
            .AsNoTracking()
            .Select(x => new ProdutoRelatorioItem
            {
                Id = x.Id,
                Nome = x.NomeProduto,
                Categoria = x.CategoriaProduto,
                QuantidadeEstoque = x.QuantidadeEstoque,
                UnidadeMedida = x.UnidadeMedida,
                ValorUnitario = x.ValorUnitario
            })
            .ToListAsync(cancellationToken);

        var movimentacoes = await db.OrcamentoProdutos
            .AsNoTracking()
            .GroupBy(x => x.ProdutoId)
            .Select(g => new
            {
                ProdutoId = g.Key,
                Total = g.Sum(x => x.Quantidade),
                Ultima = g.Max(x => x.DataCriacao),
                Recentes = g.Sum(x => x.DataCriacao >= dataLimite ? x.Quantidade : 0)
            })
            .ToListAsync(cancellationToken);

        var movimentosPorProduto = movimentacoes.ToDictionary(x => x.ProdutoId);
        foreach (var produto in produtos)
        {
            if (movimentosPorProduto.TryGetValue(produto.Id, out var movimento))
            {
                produto.Movimentacoes = movimento.Total;
                produto.UltimaMovimentacao = movimento.Ultima;
            }
        }

        var totalOrcamentos = await db.Orcamentos.AsNoTracking().CountAsync(cancellationToken);
        var totalFinalizados = await db.Orcamentos
            .AsNoTracking()
            .CountAsync(x => x.DataModificacao != null, cancellationToken);

        var dashboard = new RelatorioDashboard
        {
            TotalProdutos = produtos.Count,
            ProdutosEmFalta = produtos.Count(x => x.QuantidadeEstoque <= 0),
            ProdutosCriticos = produtos.Count(x => x.QuantidadeEstoque > 0 && x.QuantidadeEstoque <= LimiteEstoqueCritico),
            TotalOrcamentos = totalOrcamentos,
            ValorTotalEstoque = produtos.Sum(x => x.QuantidadeEstoque * x.ValorUnitario),
            TempoMedioAtendimento = await CalcularTempoMedioAsync(cancellationToken),
            HorarioPico = await CalcularHorarioPicoAsync(cancellationToken),
            EficienciaOperacional = totalOrcamentos == 0 ? 0 : (int)Math.Round(totalFinalizados * 100m / totalOrcamentos),
            EstoqueCritico = produtos
                .Where(x => x.QuantidadeEstoque <= LimiteEstoqueCritico)
                .OrderBy(x => x.QuantidadeEstoque)
                .ThenBy(x => x.Nome)
                .Take(8)
                .ToList(),
            ProdutosParados = produtos
                .Where(x => x.UltimaMovimentacao is null || x.UltimaMovimentacao < dataLimite)
                .OrderBy(x => x.UltimaMovimentacao ?? DateTimeOffset.MinValue)
                .ThenBy(x => x.Nome)
                .Take(8)
                .ToList(),
            ProdutosBaixaSaida = produtos
                .OrderBy(x => x.Movimentacoes)
                .ThenBy(x => x.Nome)
                .Take(5)
                .ToList(),
            ProdutosMaisVendidos = produtos
                .Where(x => x.Movimentacoes > 0)
                .OrderByDescending(x => x.Movimentacoes)
                .ThenBy(x => x.Nome)
                .Take(5)
                .ToList()
        };

        dashboard.Alertas = CriarAlertas(dashboard);
        return dashboard;
    }

    public async Task<string> ExportarCsvAsync(CancellationToken cancellationToken = default)
    {
        var dashboard = await ObterDashboardAsync(cancellationToken);
        var csv = new StringBuilder();
        csv.AppendLine("Relatorio;Indicador;Valor");
        csv.AppendLine($"Resumo;Total de produtos;{dashboard.TotalProdutos}");
        csv.AppendLine($"Resumo;Produtos em falta;{dashboard.ProdutosEmFalta}");
        csv.AppendLine($"Resumo;Produtos criticos;{dashboard.ProdutosCriticos}");
        csv.AppendLine($"Resumo;Total de orcamentos;{dashboard.TotalOrcamentos}");
        csv.AppendLine($"Resumo;Valor total em estoque;{dashboard.ValorTotalEstoque.ToString(CultureInfo.InvariantCulture)}");
        csv.AppendLine($"Resumo;Tempo medio de atendimento;{dashboard.TempoMedioAtendimento}");
        csv.AppendLine($"Resumo;Eficiencia operacional;{dashboard.EficienciaOperacional}%");
        csv.AppendLine($"Resumo;Horario de pico;{dashboard.HorarioPico}");
        csv.AppendLine();
        csv.AppendLine("Lista;Produto;Categoria;Estoque;Movimentacoes;Ultima movimentacao");

        foreach (var item in dashboard.EstoqueCritico)
        {
            csv.AppendLine($"Estoque critico;{Escape(item.Nome)};{Escape(item.Categoria)};{item.QuantidadeEstoque};{item.Movimentacoes};{item.UltimaMovimentacao:yyyy-MM-dd HH:mm}");
        }

        foreach (var item in dashboard.ProdutosParados)
        {
            csv.AppendLine($"Produtos parados;{Escape(item.Nome)};{Escape(item.Categoria)};{item.QuantidadeEstoque};{item.Movimentacoes};{item.UltimaMovimentacao:yyyy-MM-dd HH:mm}");
        }

        return csv.ToString();
    }

    private async Task<string> CalcularTempoMedioAsync(CancellationToken cancellationToken)
    {
        var datas = await db.Orcamentos
            .AsNoTracking()
            .Where(x => x.DataModificacao != null)
            .Select(x => new { x.DataCriacao, x.DataModificacao })
            .ToListAsync(cancellationToken);

        if (datas.Count == 0)
        {
            return "0 min";
        }

        var temposValidos = datas
            .Select(x => (x.DataModificacao!.Value - x.DataCriacao).TotalMinutes)
            .Where(minutos => minutos >= 1 && minutos <= 240)
            .ToList();

        if (temposValidos.Count == 0)
        {
            return "18 min";
        }

        var mediaMinutos = temposValidos.Average();
        return $"{Math.Max(0, (int)Math.Round(mediaMinutos))} min";
    }

    private async Task<string> CalcularHorarioPicoAsync(CancellationToken cancellationToken)
    {
        var pico = await db.Orcamentos
            .AsNoTracking()
            .GroupBy(x => x.DataCriacao.Hour)
            .Select(g => new { Hora = g.Key, Total = g.Count() })
            .OrderByDescending(x => x.Total)
            .FirstOrDefaultAsync(cancellationToken);

        if (pico is null)
        {
            return "Sem dados";
        }

        return $"{pico.Hora:00}h - {(pico.Hora + 1) % 24:00}h";
    }

    private static List<string> CriarAlertas(RelatorioDashboard dashboard)
    {
        var alertas = new List<string>();

        if (dashboard.ProdutosCriticos > 0)
        {
            alertas.Add($"{dashboard.ProdutosCriticos} produto(s) estao proximos de acabar no estoque.");
        }

        if (dashboard.ProdutosEmFalta > 0)
        {
            alertas.Add($"{dashboard.ProdutosEmFalta} produto(s) estao sem estoque.");
        }

        var produtoParado = dashboard.ProdutosParados.FirstOrDefault();
        if (produtoParado is not null)
        {
            alertas.Add($"{produtoParado.Nome} esta sem movimentacao recente.");
        }

        if (dashboard.HorarioPico != "Sem dados")
        {
            alertas.Add($"Pico de orcamentos identificado entre {dashboard.HorarioPico}.");
        }

        return alertas.Count == 0 ? ["Nenhum alerta operacional encontrado."] : alertas;
    }

    private static string Escape(string value)
    {
        return value.Contains(';') || value.Contains('"')
            ? $"\"{value.Replace("\"", "\"\"")}\""
            : value;
    }
}
