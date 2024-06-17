using AdminCenter.Application.Common.Secrets;
using TripHelper.Api;
using TripHelper.Api.Common.ConnectionStringOptions;
using TripHelper.Application;
using TripHelper.Infrastructure;
using TripHelper.Infrastructure.Authentication.TokenGenerator;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;
ConnectionStringOptions settings = config.GetRequiredSection("ConnectionStrings").Get<ConnectionStringOptions>() ?? throw new("ConnectionStrings");

var keyVaultUrl = settings.KeyVaultUrl;
if (string.IsNullOrWhiteSpace(keyVaultUrl))
    throw new("KeyVaultUrl is not set in the configuration");

var secretManager = new SecretsManager(keyVaultUrl);
var jwtSettings = new JwtSettings
{
    Issuer = secretManager.GetSecret("Issuer"),
    Secret = secretManager.GetSecret("Secret"),
    Audience = secretManager.GetSecret("Audience"),
    TokenExpirationInMinutes = int.Parse(secretManager.GetSecret("TokenExpirationInMinutes"))
};

builder.Services
    .AddPresentation()
    .AddApplication()
    .AddInfrastructure(
        secretManager.GetSecret(Environment.GetEnvironmentVariable("DB_CONNECTION_STRING_KEY")!),
        jwtSettings);

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
