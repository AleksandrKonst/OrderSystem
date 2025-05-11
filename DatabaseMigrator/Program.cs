using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

IConfiguration configuration = builder.Build();

var connectionString = configuration.GetConnectionString("OrdersDb") 
    ?? throw new InvalidOperationException("Connection string 'OrdersDb' not found.");

Console.WriteLine("Starting database migration...");
Console.WriteLine($"Using connection string: {connectionString}");

var services = new ServiceCollection()
    .AddFluentMigratorCore()
    .ConfigureRunner(runner => runner
        .AddPostgres()
        .WithGlobalConnectionString(connectionString)
        .ScanIn(Assembly.GetExecutingAssembly()).For.Migrations())
    .AddLogging(lb => lb.AddFluentMigratorConsole());

using var serviceProvider = services.BuildServiceProvider(false);

using var scope = serviceProvider.CreateScope();
var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

Console.WriteLine("Executing migrations...");
runner.MigrateUp();

Console.WriteLine("Migration completed successfully!"); 