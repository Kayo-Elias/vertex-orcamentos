using System.Text;
using VertexOrcamentos.Services.Relatorios;

namespace VertexOrcamentos.Api;

public static class RelatoriosEndpoints
{
    public static IEndpointRouteBuilder MapRelatoriosEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/relatorios").WithTags("Relatorios");

        group.MapGet("/", (RelatorioService service, CancellationToken cancellationToken) =>
            service.ObterDashboardAsync(cancellationToken));

        group.MapGet("/exportar", async (RelatorioService service, CancellationToken cancellationToken) =>
        {
            var csv = await service.ExportarCsvAsync(cancellationToken);
            return Results.File(
                Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(csv)).ToArray(),
                "text/csv",
                $"relatorio-vertex-{DateTime.UtcNow:yyyyMMddHHmm}.csv");
        });

        return app;
    }
}
