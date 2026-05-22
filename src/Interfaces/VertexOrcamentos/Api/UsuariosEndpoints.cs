using VertexOrcamentos.Services.Usuarios;

namespace VertexOrcamentos.Api;

public static class UsuariosEndpoints
{
    public static IEndpointRouteBuilder MapUsuariosEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/usuarios").WithTags("Usuarios");

        group.MapGet("/", (UsuarioService service, string? busca, CancellationToken cancellationToken) =>
            service.ListarAsync(busca, cancellationToken));

        group.MapGet("/{id:guid}", async (UsuarioService service, Guid id, CancellationToken cancellationToken) =>
        {
            var usuario = await service.ObterAsync(id, cancellationToken);
            return usuario is null ? Results.NotFound() : Results.Ok(usuario);
        });

        group.MapPost("/", async (UsuarioService service, UsuarioFormModel model, CancellationToken cancellationToken) =>
        {
            var usuario = await service.CriarAsync(model, cancellationToken);
            return Results.Created($"/api/usuarios/{usuario.Id}", usuario);
        });

        group.MapPut("/{id:guid}", async (UsuarioService service, Guid id, UsuarioFormModel model, CancellationToken cancellationToken) =>
        {
            await service.AtualizarAsync(id, model, cancellationToken);
            return Results.NoContent();
        });

        group.MapDelete("/{id:guid}", async (UsuarioService service, Guid id, CancellationToken cancellationToken) =>
        {
            await service.InativarAsync(id, cancellationToken);
            return Results.NoContent();
        });

        return app;
    }
}
