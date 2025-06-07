using Azure.Communication.Email;
using Azure.Identity;
using Business.Interfaces;
using Business.Services;

var builder = WebApplication.CreateBuilder(args);
var keyVaultUrl = new Uri("https://keyvault-mvp-student.vault.azure.net/");
builder.Configuration.AddAzureKeyVault(keyVaultUrl, new DefaultAzureCredential());
Console.WriteLine("ACS Conn: " + builder.Configuration["ACS:ConnectionString"]);
builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddMemoryCache();
builder.Services.AddSingleton(x => new EmailClient(builder.Configuration["ACS:ConnectionString"]));
builder.Services.AddTransient<IVerificationService, VerificationService>();
builder.Services.AddSwaggerGen();

// KONTROLLERA I POSTMAN

var app = builder.Build();  
app.MapOpenApi();
app.UseCors(x =>  x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.RoutePrefix = string.Empty;
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Verification Service Api");
});
app.MapControllers();

app.Run();
