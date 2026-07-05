using Entities;
using Models;
using Spectre.Console;
using UnitsOfWork;

namespace Lab5;

public class AnalyticsService(IBuildSystemUnitOfWork uow)
{
    private readonly IBuildSystemUnitOfWork uow = uow;
    private List<PerformanceMetric> metrics =
        [];

    public async Task LoadDataAsync()
    {
        var all = await this.uow.PerformanceMetrics.GetAllWithRelationsAsync().ConfigureAwait(false);
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
                test: m.BenchmarkTest.TestDescription,
                timeMs: m.SingleThreadTimeMs,
                gain: m.PerformanceGain,
                processor: m.ServerConfiguration?.ProcessorModel?.ProcessorName ?? "Unknown",
                algorithm: m.BenchmarkTest.TestDescription,
                isParallel: false))];
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
                test: m.BenchmarkTest.TestDescription,
                singleMs: m.SingleThreadTimeMs,
                multiMs: m.MultiThreadTimeMs,
                overhead: m.MultiThreadTimeMs - m.SingleThreadTimeMs))];
    }

    // 4. Сравнение процессоров
    public ICollection<EnvironmentEntry> CompareEnvironments()
    {
        return
            [.. this.metrics
            .GroupBy(m =>
                m.ServerConfiguration?.ProcessorModel?.ProcessorName ?? "Unknown")
            .Select(g => new EnvironmentEntry(
                processor: g.Key,
                avgSingle: g.Average(x => x.SingleThreadTimeMs),
                avgMulti: g.Average(x => x.MultiThreadTimeMs),
                count: g.Count()))
            .OrderBy(x => x.avgSingle)];
    }

    // 5. Лучшая структура
    public ICollection<BestStructureEntry> GetBestStructureBySize()
    {
        return
            [.. this.metrics
            .GroupBy(m => m.BenchmarkTest.TestDescription)
            .Select(g => new BestStructureEntry(
                sizeGroup: g.Key,
                bestGain: g.Max(x => x.PerformanceGain)))];
    }

    // 6. Сравнение порядков
    public ICollection<OrderComparisonEntry> CompareMultiplicationOrders()
    {
        return
            [.. this.metrics
            .GroupBy(m => m.BenchmarkTest?.TestDescription ?? "Unknown")
            .Select(g => new OrderComparisonEntry(
                test: g.Key,
                avgTime: g.Average(x => x.SingleThreadTimeMs)))
            .OrderBy(x => x.avgTime)];
    }
}