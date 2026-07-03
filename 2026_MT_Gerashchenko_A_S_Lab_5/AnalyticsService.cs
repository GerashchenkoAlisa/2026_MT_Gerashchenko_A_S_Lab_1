using _2026_MT_Gerashchenko_A_S_Lab_2.Entities;
using _2026_MT_Gerashchenko_A_S_Lab_2.UnitsOfWork;
using _2026_MT_Gerashchenko_A_S_Lab_5.Models;
using Spectre.Console;

namespace _2026_MT_Gerashchenko_A_S_Lab_5;

public class AnalyticsService(IBuildSystemUnitOfWork uow)
{
    private readonly IBuildSystemUnitOfWork _uow = uow;
    private List<PerformanceMetric> _metrics = [];

    public async Task LoadDataAsync()
    {
        var all = await _uow.PerformanceMetrics.GetAllAsync();
        _metrics = [.. all];

        AnsiConsole.MarkupLine(
            $"[green]Загружено {_metrics.Count} записей производительности.[/]");
    }

    // 1. Топ-3 самых быстрых методов

    public List<TopMethodEntry> GetTopEfficientMethods(int size = 2000)
    {
        return [.. _metrics
            .Where(m => m.BenchmarkTest.TestDescription.Contains(size.ToString()))
            .OrderBy(m => m.SingleThreadTimeMs)
            .Take(3)
            .Select(m => new TopMethodEntry(
                Test: m.BenchmarkTest.TestDescription,
                TimeMs: m.SingleThreadTimeMs,
                Gain: m.PerformanceGain,
                Processor: m.ServerConfiguration?.ProcessorModel?.ProcessorName ?? "Unknown",
                Algorithm: m.BenchmarkTest.TestDescription,
                IsParallel: false
            ))];
    }

    // 2. Среднее ускорение
    public double CalculateParallelismEffect()
    {
        var valid = _metrics.Where(m => m.MultiThreadTimeMs > 0);

        if (!valid.Any())
            return 0;

        return valid.Average(m =>
            (double)m.SingleThreadTimeMs / m.MultiThreadTimeMs);
    }

    // 3. Аномалии

    public List<AnomalyEntry> FindAnomalies()
    {
        return [.. _metrics
            .Where(m => m.MultiThreadTimeMs > m.SingleThreadTimeMs)
            .OrderByDescending(m => m.MultiThreadTimeMs - m.SingleThreadTimeMs)
            .Select(m => new AnomalyEntry(
                Test: m.BenchmarkTest.TestDescription,
                SingleMs: m.SingleThreadTimeMs,
                MultiMs: m.MultiThreadTimeMs,
                Overhead: m.MultiThreadTimeMs - m.SingleThreadTimeMs
            ))];
    }

    // 4. Сравнение процессоров

    public List<EnvironmentEntry> CompareEnvironments()
    {
        return [.. _metrics
            .GroupBy(m =>
                m.ServerConfiguration?.ProcessorModel?.ProcessorName ?? "Unknown")
            .Select(g => new EnvironmentEntry(
                Processor: g.Key,
                AvgSingle: g.Average(x => x.SingleThreadTimeMs),
                AvgMulti: g.Average(x => x.MultiThreadTimeMs),
                Count: g.Count()
            ))
            .OrderBy(x => x.AvgSingle)];
    }

    // 5. Лучшая структура

    public List<BestStructureEntry> GetBestStructureBySize()
    {
        return [.. _metrics
            .GroupBy(m => m.BenchmarkTest.TestDescription)
            .Select(g => new BestStructureEntry(
                SizeGroup: g.Key,
                BestGain: g.Max(x => x.PerformanceGain)
            ))];
    }

    // 6. Сравнение порядков

    public List<OrderComparisonEntry> CompareMultiplicationOrders()
    {
        return [.. _metrics
            .GroupBy(m => m.BenchmarkTest.TestDescription)
            .Select(g => new OrderComparisonEntry(
                Test: g.Key,
                AvgTime: g.Average(x => x.SingleThreadTimeMs)
            ))
            .OrderBy(x => x.AvgTime)];
    }
}