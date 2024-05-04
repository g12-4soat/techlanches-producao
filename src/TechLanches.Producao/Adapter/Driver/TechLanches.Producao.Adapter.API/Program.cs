using Polly;
using Polly.Extensions.Http;
using TechLanches.Producao.Adapter.API.Configuration;
using TechLanches.Producao.Adapter.API.Constantes;
using TechLanches.Producao.Adapter.API.Options;
using TechLanches.Producao.Adapter.FilaPedidos;
using TechLanches.Producao.Adapter.FilaPedidos.Health;
using TechLanches.Producao.Adapter.FilaPedidos.Options;
using TechLanches.Producao.Adapter.RabbitMq.Options;
using TechLanches.Producao.Application.Constantes;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true, true)
    .AddEnvironmentVariables();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

//Add cognito auth
builder.Services.Configure<AuthenticationCognitoOptions>(builder.Configuration.GetSection("Authentication"));
builder.Services.Configure<WorkerOptions>(builder.Configuration.GetSection("Worker"));
builder.Services.Configure<RabbitOptions>(builder.Configuration.GetSection("RabbitMQ"));

//Criar uma politica de retry (tente 3x, com timeout de 3 segundos)
var retryPolicy = HttpPolicyExtensions.HandleTransientHttpError()
                  .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(retryAttempt));

//Registrar httpclient

builder.Services.AddHttpClient(Constantes.API_PEDIDO, httpClient =>
{
    httpClient.BaseAddress = new Uri(builder.Configuration.GetSection($"TechLanchesPedido:BaseUrl").Value);
}).AddPolicyHandler(retryPolicy);

builder.Services.AddMemoryCache();

builder.Services.AddAuthenticationConfig();

//Setting Swagger
builder.Services.AddSwaggerConfiguration();

//DI Abstraction
builder.Services.AddDependencyInjectionConfiguration();

builder.Services.AddHealthCheckConfig(builder.Configuration);

builder.Services.AddHostedService<FilaPedidosHostedService>();
builder.Services.AddHostedService<TcpHealthProbeService>();

var app = builder.Build();

app.AddCustomMiddlewares();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSwaggerConfiguration();

app.AddHealthCheckEndpoint();

app.UseMapEndpointsConfiguration();

app.UseStaticFiles();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.Run();
