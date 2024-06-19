using Polly;
using Polly.Extensions.Http;
using TechLanches.Producao.Adapter.API.Configuration;
using TechLanches.Producao.Adapter.FilaPedidos;
using TechLanches.Producao.Adapter.FilaPedidos.Health;
using TechLanches.Producao.Adapter.RabbitMq.Options;
using TechLanches.Producao.Application.Constantes;
using TechLanches.Producao.Application.Options;
using TechLanches.Producao.Adapter.AWS;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true, true)
    .AddEnvironmentVariables();

//AWS Secrets Manager
builder.Configuration
    .AddAmazonSecretsManager("us-east-1", "lambda-auth-credentials");

builder.Services.Configure<TechLanchesCognitoSecrets>(builder.Configuration);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.Configure<WorkerOptions>(builder.Configuration.GetSection("Worker"));
builder.Services.Configure<RabbitOptions>(builder.Configuration.GetSection("RabbitMQ"));

////Criar uma politica de retry (tente 3x, com timeout de 3 segundos)
var retryPolicy = HttpPolicyExtensions.HandleTransientHttpError()
                  .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(retryAttempt));

//Registrar httpclient
builder.Services.AddHttpClient(Constantes.API_PEDIDO, httpClient =>
{
    //var url = Environment.GetEnvironmentVariable("PEDIDO_SERVICE");
    //httpClient.BaseAddress = new Uri("http://" + url + ":5050");

    httpClient.BaseAddress = new Uri($"http://localhost:5298");
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

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.Run();
