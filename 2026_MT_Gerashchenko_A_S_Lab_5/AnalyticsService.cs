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
        var all = await this._uow.PerformanceMetrics.GetAllWithRelationsAsync();
this._metrics = [.. all];

        AnsiConsole.MarkupLine(
            $"[green]Загружено {this._metrics.Count} записей производительности.[/]");
    }

    // 1. Топ-3 самых быстрых методов
    public List<TopMethodEntry> GetTopEfficientMethods(int size = 2000)
    {
        return [.. this._metrics
            .Where(m => m.BenchmarkTest.TestDescription.Contains(size.ToString()))
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
        var valid = this._metrics.Where(m => m.MultiThreadTimeMs > 0);

        return !valid.Any()
            ? 0
            : valid.Average(m =>
            (double)m.SingleThreadTimeMs / m.MultiThreadTimeMs);
    }

    // 3. Аномалии
    public List<AnomalyEntry> FindAnomalies()
    {
        return [.. this._metrics
            .Where(m => m.MultiThreadTimeMs > m.SingleThreadTimeMs)
            .OrderByDescending(m => m.MultiThreadTimeMs - m.SingleThreadTimeMs)
            .Select(m => new AnomalyEntry(
                test: m.BenchmarkTest.TestDescription,
                singleMs: m.SingleThreadTimeMs,
                multiMs: m.MultiThreadTimeMs,
                overhead: m.MultiThreadTimeMs - m.SingleThreadTimeMs))];
    }

    // 4. Сравнение процессоров
    public List<EnvironmentEntry> CompareEnvironments()
    {
        return [.. this._metrics
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
    public List<BestStructureEntry> GetBestStructureBySize()
    {
        return [.. this._metrics
            .GroupBy(m => m.BenchmarkTest.TestDescription)
            .Select(g => new BestStructureEntry(
                sizeGroup: g.Key,
                bestGain: g.Max(x => x.PerformanceGain)))];
    }

    // 6. Сравнение порядков
    public List<OrderComparisonEntry> CompareMultiplicationOrders()
    {
        return [.. this._metrics
            .GroupBy(m => m.BenchmarkTest?.TestDescription ?? "Unknown")
            .Select(g => new OrderComparisonEntry(
                test: g.Key,
                avgTime: g.Average(x => x.SingleThreadTimeMs)))
            .OrderBy(x => x.avgTime)];
    }
}