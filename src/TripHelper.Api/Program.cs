using AdminCenter.Application.Common.Secrets;
using TripHelper.Api.Common.ConnectionStringOptions;
using TripHelper.Application;
using TripHelper.Infrastructure;
using TripHelper.Infrastructure.Common.Middleware;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;
ConnectionStringOptions settings = config.GetRequiredSection("ConnectionStrings").Get<ConnectionStringOptions>() ?? throw new("ConnectionStrings");

var keyVaultUrl = settings.KeyVaultUrl;
if (string.IsNullOrWhiteSpace(keyVaultUrl))
    throw new("KeyVaultUrl is not set in the configuration");

var secretManager = new SecretsManager(keyVaultUrl);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();
builder.Services.AddHttpContextAccessor();

builder.Services
    .AddApplication()
    .AddInfrastructure(secretManager.GetSecret(Environment.GetEnvironmentVariable("DB_CONNECTION_STRING_KEY")!));

var app = builder.Build();

app.UseExceptionHandler();
app.AddInfrastructureMiddleware();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
