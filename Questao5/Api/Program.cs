using MediatR;
using Questao5.Application.Commands;
using Questao5.Application.Queries;
using Questao5.Domain.Interfaces;
using Questao5.Infrastructure.Sqlite.Repositories;
using Questao5.Infrastructure.Sqlite;
using FluentValidation;
using Questao5.Application.Validators;
using Questao5.Application.Exceptions;
using Questao5.Infrastructure.Idempotency;
using Dapper;
using System.Data; // Certifique-se de que esta linha está presente

var builder = WebApplication.CreateBuilder(args);

Console.WriteLine("Iniciando a configuração da aplicação...");

// Adiciona serviços ao contêiner.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Banco API", Version = "v1" });
});

// Configura a Conexão SQLite
var connectionString = builder.Configuration.GetConnectionString("SqliteConnection");
if (string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine("ERRO: String de conexão 'SqliteConnection' não encontrada em appsettings.json.");
    // Você pode definir um valor padrão ou lançar uma exceção aqui
    connectionString = "Data Source=banco.db"; // Valor padrão para continuar, mas é bom investigar o appsettings.json
}
Console.WriteLine($"String de conexão SQLite: {connectionString}");

builder.Services.AddSingleton(new ConnectionFactory(connectionString));

// Inicialização do Banco de Dados
Console.WriteLine("Tentando inicializar o banco de dados...");
try
{
    using (var scope = builder.Services.BuildServiceProvider().CreateScope())
    {
        var connectionFactory = scope.ServiceProvider.GetRequiredService<ConnectionFactory>();
        using (IDbConnection connection = connectionFactory.GetOpenConnection()) // Garante que a conexão seja fechada
        {
            Console.WriteLine("Conexão com o banco de dados aberta. Chamando DatabaseBootstrap.Setup...");
            DatabaseBootstrap.Setup(connection);
            Console.WriteLine("DatabaseBootstrap.Setup concluído.");
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"ERRO ao inicializar o banco de dados: {ex.Message}");
    Console.WriteLine($"Detalhes do erro: {ex.StackTrace}");
}


// Registra Dapper
DefaultTypeMap.MatchNamesWithUnderscores = true; // Para mapeamento automático

// Registra Repositórios
builder.Services.AddScoped<IContaCorrenteRepository, ContaCorrenteRepository>();
builder.Services.AddScoped<IMovimentoRepository, MovimentoRepository>();
builder.Services.AddScoped<IIdempotenciaRepository, IdempotenciaRepository>();

// Registra MediatR
builder.Services.AddMediatR(typeof(MovimentoContaCorrenteCommand).Assembly);

// Registra FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<MovimentoContaCorrenteCommandValidator>();

// Registra filtro de exceção global
builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(BusinessExceptionFilter));
});

// Adiciona configuração explícita de URLs para Kestrel
builder.WebHost.UseUrls("http://localhost:5000", "https://localhost:5001");


var app = builder.Build();

// Configura o pipeline de requisições HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Banco API v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

Console.WriteLine("Aplicação pronta para iniciar. Chamando app.Run()...");
try
{
    app.Run();
    Console.WriteLine("Aplicação encerrada com sucesso.");
}
catch (Exception ex)
{
    Console.WriteLine($"ERRO FATAL: A aplicação falhou ao iniciar ou durante a execução: {ex.Message}");
    Console.WriteLine($"Detalhes do erro: {ex.StackTrace}");
}



//using MediatR;
//using Questao5.Application.Commands;
//using Questao5.Application.Queries;
//using Questao5.Domain.Interfaces;
//using Questao5.Infrastructure.Sqlite.Repositories;
//using Questao5.Infrastructure.Sqlite;
//using FluentValidation;
//using Questao5.Application.Validators;
//using Questao5.Application.Exceptions;
//using Questao5.Infrastructure.Idempotency;
//using Dapper;
//using System.Data; // Certifique-se de que esta linha está presente

//var builder = WebApplication.CreateBuilder(args);

//Console.WriteLine("Iniciando a configuração da aplicação...");

//// Adiciona serviços ao contêiner.
//builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen(c =>
//{
//    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Banco API", Version = "v1" });
//});

//// Configura a Conexão SQLite
//var connectionString = builder.Configuration.GetConnectionString("SqliteConnection");
//if (string.IsNullOrEmpty(connectionString))
//{
//    Console.WriteLine("ERRO: String de conexão 'SqliteConnection' não encontrada em appsettings.json.");
//    // Você pode definir um valor padrão ou lançar uma exceção aqui
//    connectionString = "Data Source=banco.db"; // Valor padrão para continuar, mas é bom investigar o appsettings.json
//}
//Console.WriteLine($"String de conexão SQLite: {connectionString}");

//builder.Services.AddSingleton(new ConnectionFactory(connectionString));

//// Inicialização do Banco de Dados
//Console.WriteLine("Tentando inicializar o banco de dados...");
//try
//{
//    using (var scope = builder.Services.BuildServiceProvider().CreateScope())
//    {
//        var connectionFactory = scope.ServiceProvider.GetRequiredService<ConnectionFactory>();
//        using (IDbConnection connection = connectionFactory.GetOpenConnection()) // Garante que a conexão seja fechada
//        {
//            Console.WriteLine("Conexão com o banco de dados aberta. Chamando DatabaseBootstrap.Setup...");
//            DatabaseBootstrap.Setup(connection);
//            Console.WriteLine("DatabaseBootstrap.Setup concluído.");
//        }
//    }
//}
//catch (Exception ex)
//{
//    Console.WriteLine($"ERRO ao inicializar o banco de dados: {ex.Message}");
//    Console.WriteLine($"Detalhes do erro: {ex.StackTrace}");
//}


//// Registra Dapper
//DefaultTypeMap.MatchNamesWithUnderscores = true; // Para mapeamento automático

//// Registra Repositórios
//builder.Services.AddScoped<IContaCorrenteRepository, ContaCorrenteRepository>();
//builder.Services.AddScoped<IMovimentoRepository, MovimentoRepository>();
//builder.Services.AddScoped<IIdempotenciaRepository, IdempotenciaRepository>();

//// Registra MediatR

////builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(MovimentoContaCorrenteCommand).Assembly));

//// Antes:
// //builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(MovimentoContaCorrenteCommand).Assembly));

//// Depois:
//builder.Services.AddMediatR(typeof(MovimentoContaCorrenteCommand).Assembly);

//// Registra FluentValidation
//builder.Services.AddValidatorsFromAssemblyContaining<MovimentoContaCorrenteCommandValidator>();

//// Registra filtro de exceção global
//builder.Services.AddControllers(options =>
//{
//    options.Filters.Add(typeof(BusinessExceptionFilter));
//});


//var app = builder.Build();

//// Configura o pipeline de requisições HTTP.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI(c =>
//    {
//        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Banco API v1");
//    });
//}

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

//Console.WriteLine("Aplicação pronta para iniciar. Executando...");
//app.Run();
//Console.WriteLine("Aplicação encerrada.");


//using MediatR;
//using Questao5.Application.Commands;
//using Questao5.Application.Queries;
//using Questao5.Domain.Interfaces;
//using Questao5.Infrastructure.Sqlite.Repositories;
//using Questao5.Infrastructure.Sqlite;
//using FluentValidation;
//using Questao5.Application.Validators;
//using Questao5.Application.Exceptions;
//using Questao5.Infrastructure.Idempotency;
//using Dapper;
//using Microsoft.AspNetCore.Connections;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.Extensions.DependencyInjection;
//using System.Reflection;


//var builder = WebApplication.CreateBuilder(args);


//builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen(c =>
//{
//    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Banco API", Version = "v1" });
//});

//var connectionString = builder.Configuration.GetConnectionString("SqliteConnection");
//builder.Services.AddSingleton(new ConnectionFactory(connectionString));


//using (var scope = builder.Services.BuildServiceProvider().CreateScope())
//{
//    var connectionFactory = scope.ServiceProvider.GetRequiredService<ConnectionFactory>();
//    DatabaseBootstrap.Setup(connectionFactory.GetOpenConnection());
//}

//DefaultTypeMap.MatchNamesWithUnderscores = true; 


//builder.Services.AddScoped<IContaCorrenteRepository, ContaCorrenteRepository>();
//builder.Services.AddScoped<IMovimentoRepository, MovimentoRepository>();
//builder.Services.AddScoped<IIdempotenciaRepository, IdempotenciaRepository>();

////builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(MovimentoContaCorrenteCommand).Assembly));
//builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

//builder.Services.AddValidatorsFromAssemblyContaining<MovimentoContaCorrenteCommandValidator>();


//builder.Services.AddControllers(options =>
//{
//    options.Filters.Add(typeof(BusinessExceptionFilter));
//});


//var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI(c =>
//    {
//        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Banco API v1");
//    });
//}

//app.UseHttpsRedirection();
//app.UseAuthorization();
//app.MapControllers();

//app.Run();

