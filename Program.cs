using Microsoft.EntityFrameworkCore;
using NfeProcessor.Data;
using NfeProcessor.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Define a string de conexão
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=nfe.db";

// Registra o DbContext
builder.Services.AddDbContext<NfeDbContext>(options =>
    options.UseSqlite(connectionString));

// Registra os serviços
builder.Services.AddScoped<NfeService>();

// O 'AddControllers' configura o JSON
builder.Services.AddControllers().AddJsonOptions(options =>
{
    // Diz ao serializador para preservar as referências em vez de entrar em um ciclo.
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

// fim da atualização

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuração do CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configura o pipeline de requisições HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Habilita o CORS
app.UseCors("AllowVueApp");

app.UseAuthorization();

app.MapControllers();

app.Run();