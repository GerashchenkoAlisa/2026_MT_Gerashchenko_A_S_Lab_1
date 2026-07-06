using Entities;
using Models;
using Spectre.Console;
using UnitsOfWork;

namespace Programm;

public class AnalyticsService(IBuildSystemUnitOfWork uow)
{
    private readonly IBuildSystemUnitOfWork uow = uow;
    private List<PerformanceMetric> metrics =
        [];

    public async Task LoadDataAsync()
    {
        var all = await this.uow.PerformanceMetrics.GetAllAsync().ConfigureAwait(false);
        this.metrics =
            [.. all];

        AnsiConsole.MarkupLine(
            $"[green]Загружено {this.metrics.Count} записей производительности.[/]");
    }

    // 1. Топ-3 самых быстрых методов
    public ICollection<TopMethodEntry> GetTopEfficientMethods(int size = 2000)
    {
        return
            [.. this.metrics
            .Where(m => m.BenchmarkTest.TestDescription.Contains(
                size.ToString(System.Globalization.CultureInfo.InvariantCulture),
                StringComparison.InvariantCulture))
            .OrderBy(m => m.SingleThreadTimeMs)
            .Take(3)
            .Select(m => new TopMethodEntry(
                Test: m.BenchmarkTest.TestDescription,
                TimeMs: m.SingleThreadTimeMs,
                Gain: m.PerformanceGain,
                Processor: m.ServerConfiguration?.ProcessorModel?.ProcessorName ?? "Unknown",
                Algorithm: m.BenchmarkTest.TestDescription,
                IsParallel: false))];
    }

    // 2. Среднее ускорение
    public double CalculateParallelismEffect()
    {
        var valid = this.metrics.Where(m => m.MultiThreadTimeMs > 0).ToList();

        return valid.Count == 0
            ? 0
            : valid.Average(m =>
                (double)m.SingleThreadTimeMs / m.MultiThreadTimeMs);
    }

    // 3. Аномалии
    public ICollection<AnomalyEntry> FindAnomalies()
    {
        return
            [.. this.metrics
            .Where(m => m.MultiThreadTimeMs > m.SingleThreadTimeMs)
            .OrderByDescending(m => m.MultiThreadTimeMs - m.SingleThreadTimeMs)
            .Select(m => new AnomalyEntry(
                Test: m.BenchmarkTest.TestDescription,
                SingleMs: m.SingleThreadTimeMs,
                MultiMs: m.MultiThreadTimeMs,
                Overhead: m.MultiThreadTimeMs - m.SingleThreadTimeMs))];
    }

    // 4. Сравнение процессоров
    public ICollection<EnvironmentEntry> CompareEnvironments()
    {
        return
            [.. this.metrics
            .GroupBy(m =>
                m.ServerConfiguration?.ProcessorModel?.ProcessorName ?? "Unknown")
            .Select(g => new EnvironmentEntry(
                Processor: g.Key,
                AvgSingle: g.Average(x => x.SingleThreadTimeMs),
                AvgMulti: g.Average(x => x.MultiThreadTimeMs),
                Count: g.Count()))
            .OrderBy(x => x.AvgSingle)];
    }

    // 5. Лучшая структура
    public ICollection<BestStructureEntry> GetBestStructureBySize()
    {
        return
            [.. this.metrics
            .GroupBy(m => m.BenchmarkTest.TestDescription)
            .Select(g => new BestStructureEntry(
                SizeGroup: g.Key,
                BestGain: g.Max(x => x.PerformanceGain)))];
    }

    // 6. Сравнение порядков
    public ICollection<OrderComparisonEntry> CompareMultiplicationOrders()
    {
        return
            [.. this.metrics
            .GroupBy(m => m.BenchmarkTest?.TestDescription ?? "Unknown")
            .Select(g => new OrderComparisonEntry(
                Test: g.Key,
                AvgTime: g.Average(x => x.SingleThreadTimeMs)))
            .OrderBy(x => x.AvgTime)];
    }
}