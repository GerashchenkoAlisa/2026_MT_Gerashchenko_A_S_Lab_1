using _2026_MT_Gerashchenko_A_S_Lab_2.Data;
using _2026_MT_Gerashchenko_A_S_Lab_2.UnitsOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using System;
using System.IO;
using System.Threading.Tasks;

namespace _2026_MT_Gerashchenko_A_S_Lab_5;

class Program
{
    static async Task Main()
    {
        AnsiConsole.Write(new FigletText("Lab 5 - Analytics").Color(Color.Cyan1));

        var services = new ServiceCollection();
        var baseDir = AppContext.BaseDirectory;
        var dbPath = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "..", "2026_MT_Gerashchenko_A_S_Lab_2", "app.db"));
        services.AddDbContext<BuildSystemDbContext>(opt => opt.UseSqlite($"Data Source={dbPath}"));
        services.AddScoped<IBuildSystemUnitOfWork, BuildSystemUnitOfWork>();
        services.AddScoped<AnalyticsService>();

        using var provider = services.BuildServiceProvider();
        var analytics = provider.GetRequiredService<AnalyticsService>();

        await AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots3)
            .StartAsync("Загрузка данных из базы...", async ctx =>
            {
                await analytics.LoadDataAsync();
            });

        RunInteractiveMenu(analytics);
    }

    private static void RunInteractiveMenu(AnalyticsService analytics)
    {
        while (true)
        {
            AnsiConsole.Clear();
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Выберите отчёт:")
                    .AddChoices(
                        "1. Топ-3 эффективных методов (2000x2000)",
                        "2. Эффект параллелизма",
                        "3. Аномалии параллелизма",
                        "4. Сравнение окружений",
                        "5. Лучшая структура по размеру",
                        "6. Сравнение порядков умножения",
                        "7. Dashboard",
                        "Выход"
                    ));

            if (choice == "Выход") break;

            AnsiConsole.Clear();
            try
            {
                switch (choice[0])
                {
                    case '1': ReportRenderer.RenderTopMethods(analytics.GetTopEfficientMethods()); break;
                    case '2': ReportRenderer.RenderParallelismEffect(analytics.CalculateParallelismEffect()); break;
                    case '3': ReportRenderer.RenderAnomalies(analytics.FindAnomalies()); break;
                    case '4': ReportRenderer.RenderEnvironmentComparison(analytics.CompareEnvironments()); break;
                    case '5': ReportRenderer.RenderBestStructure(analytics.GetBestStructureBySize()); break;
                    case '6': ReportRenderer.RenderOrderComparison(analytics.CompareMultiplicationOrders()); break;
                    case '7': ReportRenderer.RenderDashboard(); break;
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Ошибка: {ex.Message}[/]");
            }

            AnsiConsole.WriteLine("\nНажмите любую клавишу для возврата в меню...");
            Console.ReadKey();
        }
    }
}