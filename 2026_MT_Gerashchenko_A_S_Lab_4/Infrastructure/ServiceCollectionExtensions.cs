using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Persistence;
using Services;
using UnitsOfWork;

namespace Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLab4Services(this IServiceCollection services, string dbPath)
    {
        services.AddHttpClient("ApiClient", client =>
        {
            client.Timeout = TimeSpan.FromSeconds(10);
            client.DefaultRequestHeaders.Add("User-Agent", "HttpScanner/1.0");
        });

        services.AddHttpClient("DownloadClient", client =>
        {
            client.Timeout = TimeSpan.FromSeconds(60);
            client.DefaultRequestHeaders.Add("User-Agent", "HttpScanner/1.0");
        });

        services.AddDbContext<BuildSystemDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}"));

        services.AddScoped<IBuildSystemUnitOfWork, BuildSystemUnitOfWork>();
        services.AddScoped<IHttpProcessor, HttpProcessor>();
        services.AddScoped<BenchmarkRunner>();
        services.AddScoped<MetricsPersistenceService>();
        services.AddSingleton<IConsoleWriter, ConsoleWriter>();

        return services;
    }
}