using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using _2026_MT_Gerashchenko_A_S_Lab_4.Persistence;
using _2026_MT_Gerashchenko_A_S_Lab_4.Services;

namespace _2026_MT_Gerashchenko_A_S_Lab_4.Infrastructure;
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

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}"));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IHttpProcessor, HttpProcessor>();
        services.AddScoped<BenchmarkRunner>();
        services.AddScoped<MetricsPersistenceService>();
        services.AddSingleton<IConsoleWriter, ConsoleWriter>();

        return services;
    }
}