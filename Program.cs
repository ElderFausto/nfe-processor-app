using Microsoft.EntityFrameworkCore;
using NfeProcessor.Data;
using NfeProcessor.Services;

var builder = WebApplication.CreateBuilder(args);

// Define a string de conexão para usar um arquivo SQLite chamado 'nfe.db'
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=nfe.db";

// Registra o DbContext no sistema de injeção de dependência
builder.Services.AddDbContext<NfeDbContext>(options =>
    options.UseSqlite(connectionString));

// Adiciona serviços padrão
builder.Services.AddScoped<NfeService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configura o pipeline de requisições HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();