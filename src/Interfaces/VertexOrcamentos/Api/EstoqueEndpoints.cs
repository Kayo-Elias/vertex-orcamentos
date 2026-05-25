using VertexOrcamentos.Services.Estoque;

namespace VertexOrcamentos.Api;

public static class EstoqueEndpoints
{
    public static IEndpointRouteBuilder MapEstoqueEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/estoque").WithTags("Estoque");

        group.MapGet("/", (EstoqueService service, string? busca, CancellationToken cancellationToken) =>
            service.ListarAsync(busca, cancellationToken));

        group.MapGet("/resumo", (EstoqueService service, CancellationToken cancellationToken) =>
            service.ObterResumoAsync(cancellationToken));

        group.MapGet("/{id:guid}", async (EstoqueService service, Guid id, CancellationToken cancellationToken) =>
        {
            var produto = await service.ObterAsync(id, cancellationToken);
            return produto is null ? Results.NotFound() : Results.Ok(produto);
        });

        group.MapPost("/", async (EstoqueService service, ProdutoFormModel model, CancellationToken cancellationToken) =>
        {
            await service.CriarAsync(model, cancellationToken);
            return Results.Created();
        });

        group.MapPut("/{id:guid}", async (EstoqueService service, Guid id, ProdutoFormModel model, CancellationToken cancellationToken) =>
        {
            await service.AtualizarAsync(id, model, cancellationToken);
            return Results.NoContent();
        });

        group.MapDelete("/{id:guid}", async (EstoqueService service, Guid id, CancellationToken cancellationToken) =>
        {
            await service.ExcluirAsync(id, cancellationToken);
            return Results.NoContent();
        });

        return app;
    }
}