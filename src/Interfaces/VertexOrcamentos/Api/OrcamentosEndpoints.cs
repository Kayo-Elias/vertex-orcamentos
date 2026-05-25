using VertexOrcamentos.Services.Orcamentos;

namespace VertexOrcamentos.Api;

public static class OrcamentosEndpoints
{
    public static IEndpointRouteBuilder MapOrcamentosEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orcamentos").WithTags("Orcamentos");

        group.MapGet("/resumo", (OrcamentoService service, CancellationToken cancellationToken) =>
            service.ObterResumoAsync(cancellationToken));

        group.MapGet("/codigo", (OrcamentoService service) =>
            Results.Ok(new { Codigo = service.CriarCodigo() }));

        group.MapPost("/", async (OrcamentoService service, OrcamentoRequestModel model, CancellationToken cancellationToken) =>
        {
            await service.SalvarAsync(model.Cabecalho, model.Itens, cancellationToken);
            return Results.Created();
        });

        return app;
    }
}

// modelo auxiliar para receber cabeçalho + itens juntos no POST
public record OrcamentoRequestModel(OrcamentoCabecalhoModel Cabecalho, List<ItemOrcamentoModel> Itens);