using VertexOrcamentos.Services.Clientes;

namespace VertexOrcamentos.Api;

public static class ClientesEndpoints
{
    public static IEndpointRouteBuilder MapClientesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/clientes").WithTags("Clientes");

        group.MapGet("/", (ClienteService service, string? busca, CancellationToken cancellationToken) =>
            service.ListarAsync(busca, cancellationToken));

        group.MapGet("/resumo", (ClienteService service, CancellationToken cancellationToken) =>
            service.ObterResumoAsync(cancellationToken));

        group.MapGet("/{id:guid}", async (ClienteService service, Guid id, CancellationToken cancellationToken) =>
        {
            var cliente = await service.ObterAsync(id, cancellationToken);
            return cliente is null ? Results.NotFound() : Results.Ok(cliente);
        });

        group.MapPost("/", async (ClienteService service, ClienteFormModel model, CancellationToken cancellationToken) =>
        {
            await service.CriarAsync(model, cancellationToken);
            return Results.Created();
        });

        group.MapPut("/{id:guid}", async (ClienteService service, Guid id, ClienteFormModel model, CancellationToken cancellationToken) =>
        {
            await service.AtualizarAsync(id, model, cancellationToken);
            return Results.NoContent();
        });

        group.MapDelete("/{id:guid}", async (ClienteService service, Guid id, CancellationToken cancellationToken) =>
        {
            await service.ExcluirAsync(id, cancellationToken);
            return Results.NoContent();
        });

        return app;
    }
}