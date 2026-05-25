using VertexOrcamentos.Services.Pedidos;

namespace VertexOrcamentos.Api;

public static class PedidosEndpoints
{
    public static IEndpointRouteBuilder MapPedidosEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/pedidos").WithTags("Pedidos");

        group.MapGet("/", (PedidoService service, string? busca, string? status, DateTime? data, CancellationToken cancellationToken) =>
            service.ListarAsync(busca, status, data, cancellationToken));

        group.MapGet("/resumo", (PedidoService service, CancellationToken cancellationToken) =>
            service.ObterResumoAsync(cancellationToken));

        group.MapPost("/", async (PedidoService service, PedidoFormModel model, CancellationToken cancellationToken) =>
        {
            await service.CriarAsync(model, cancellationToken);
            return Results.Created();
        });

        group.MapPatch("/{id:guid}/status", async (PedidoService service, Guid id, AtualizarStatusModel model, CancellationToken cancellationToken) =>
        {
            await service.AtualizarStatusAsync(id, model.Status, cancellationToken);
            return Results.NoContent();
        });

        group.MapDelete("/{id:guid}", async (PedidoService service, Guid id, CancellationToken cancellationToken) =>
        {
            await service.ExcluirAsync(id, cancellationToken);
            return Results.NoContent();
        });

        return app;
    }
}

// modelo auxiliar só para receber o status via JSON
public record AtualizarStatusModel(string Status);