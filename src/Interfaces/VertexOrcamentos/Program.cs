using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MudBlazor.Services;
using VertexOrcamentos.Api;
using VertexOrcamentos.Components;
using VertexOrcamentos.Data;
using VertexOrcamentos.Services.Auth;
using VertexOrcamentos.Services.Clientes;
using VertexOrcamentos.Services.Estoque;
using VertexOrcamentos.Services.Orcamentos;
using VertexOrcamentos.Services.Pedidos;
using VertexOrcamentos.Services.Relatorios;
using VertexOrcamentos.Services.Usuarios;

var builder = WebApplication.CreateBuilder(args);

// Add MudBlazor services
builder.Services.AddMudServices();

// Add HttpClient
builder.Services.AddScoped<HttpClient>(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5189") });

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<RelatorioService>();
builder.Services.AddScoped<ClienteService>();
builder.Services.AddScoped<EstoqueService>();
builder.Services.AddScoped<PedidoService>();
builder.Services.AddScoped<OrcamentoService>();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// habilita geração do Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Vertex Orçamentos API",
        Version = "v1",
        Description = "API do sistema de gestão comercial Vertex Orçamentos — UNIPÊ 2026"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

// ativa o Swagger em qualquer ambiente
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Vertex Orçamentos v1");
    c.RoutePrefix = "swagger";
});

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapUsuariosEndpoints();
app.MapRelatoriosEndpoints();

// endpoints dos outros módulos
app.MapClientesEndpoints();
app.MapEstoqueEndpoints();
app.MapPedidosEndpoints();
app.MapOrcamentosEndpoints();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();