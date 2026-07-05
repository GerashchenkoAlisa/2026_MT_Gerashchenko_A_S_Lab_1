using Models;
using Spectre.Console;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Program;

public static class ReportRenderer
{
    public static void RenderTopMethods(ICollection<TopMethodEntry> data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var table = new Table()
            .Border(TableBorder.Rounded)
            .Title("[bold yellow]Топ-3 самых быстрых методов (2000×2000)[/]");

        table.AddColumn("Тест");
        table.AddColumn("Алгоритм");
        table.AddColumn(new TableColumn("Время (мс)").RightAligned());
        table.AddColumn(new TableColumn("Ускорение").RightAligned());
        table.AddColumn("Параллельно");
        table.AddColumn("Процессор");

        foreach (var item in data)
        {
            var color = item.TimeMs < 1000 ? "green" : "yellow";

            table.AddRow(
                item.Test,
                item.Algorithm,
                $"[{color}]{item.TimeMs}[/]",
                $"{item.Gain:F2}",
                item.IsParallel ? "[cyan]Да[/]" : "[grey]Нет[/]",
                item.Processor);
        }

        AnsiConsole.Write(table);
    }

    public static void RenderParallelismEffect(double avgSpeedup)
    {
        AnsiConsole.WriteLine();

        AnsiConsole.MarkupLine(
            $"[bold]Средний коэффициент ускорения:[/] [cyan]{avgSpeedup:F2}x[/]");

        AnsiConsole.WriteLine();

        var chart = new BarChart()
            .Width(60)
            .Label("[bold]Ускорение[/]");

        chart.AddItem(
            "Parallel",
            avgSpeedup,
            Color.Blue);

        AnsiConsole.Write(chart);
    }

    public static void RenderAnomalies(ICollection<AnomalyEntry> anomalies)
    {
        ArgumentNullException.ThrowIfNull(anomalies);
        if (anomalies.Count == 0)
        {
            AnsiConsole.MarkupLine("[green]Аномалий не найдено.[/]");
            return;
        }

        var table = new Table()
            .Border(TableBorder.Rounded)
            .Title("[bold red]Parallel медленнее Sequential[/]");

        table.AddColumn("Тест");
        table.AddColumn(new TableColumn("Single").RightAligned());
        table.AddColumn(new TableColumn("Multi").RightAligned());
        table.AddColumn(new TableColumn("Overhead").RightAligned());

        foreach (var item in anomalies.Take(10))
        {
            table.AddRow(
                item.Test,
                item.SingleMs.ToString(System.Globalization.CultureInfo.InvariantCulture),
                item.MultiMs.ToString(System.Globalization.CultureInfo.InvariantCulture),
                $"[red]{item.Overhead}[/]");
        }

        AnsiConsole.Write(table);
    }

    public static void RenderEnvironmentComparison(ICollection<EnvironmentEntry> data)
    {
        ArgumentNullException.ThrowIfNull(data);
        var table = new Table()
            .Border(TableBorder.Rounded)
            .Title("[bold]Сравнение процессоров[/]");

        table.AddColumn("Процессор");
        table.AddColumn(new TableColumn("Single").RightAligned());
        table.AddColumn(new TableColumn("Multi").RightAligned());
        table.AddColumn(new TableColumn("Кол-во").RightAligned());

        foreach (var item in data)
        {
            table.AddRow(
                item.Processor,
                item.AvgSingle.ToString("F0", System.Globalization.CultureInfo.InvariantCulture),
                item.AvgMulti.ToString("F0", System.Globalization.CultureInfo.InvariantCulture),
                item.Count.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }

        AnsiConsole.Write(table);
    }

    public static void RenderBestStructure(ICollection<BestStructureEntry> data)
    {
        ArgumentNullException.ThrowIfNull(data);
        var chart = new BarChart()
            .Width(60)
            .Label("[bold]Лучшие результаты[/]");

        foreach (var item in data)
        {
            chart.AddItem(
                item.SizeGroup,
                (double)item.BestGain,
                Color.Cyan1);
        }

        AnsiConsole.Write(chart);
    }

    public static void RenderOrderComparison(ICollection<OrderComparisonEntry> data)
    {
        ArgumentNullException.ThrowIfNull(data);
        var table = new Table()
            .Border(TableBorder.Rounded)
            .Title("[bold]Среднее время выполнения[/]");

        table.AddColumn("Тест");
        table.AddColumn(new TableColumn("Среднее время").RightAligned());

        foreach (var item in data)
        {
            table.AddRow(
                item.Test,
                item.AvgTime.ToString("F0", System.Globalization.CultureInfo.InvariantCulture));
        }

        AnsiConsole.Write(table);
    }

    public static void RenderDashboard()
    {
        var layout = new Layout("Root");

        layout.SplitRows(
            new Layout("Top"),
            new Layout("Bottom"));

        layout["Top"].SplitColumns(
            new Layout("Left"),
            new Layout("Right"));

        layout["Bottom"].SplitColumns(
            new Layout("LeftBottom"),
            new Layout("RightBottom"));

        layout["Left"].Update(
            new Panel("[green]Матрицы[/]")
                .Header("Аналитика"));

        layout["Right"].Update(
            new Panel("[cyan]Процессоры[/]")
                .Header("Hardware"));

        layout["LeftBottom"].Update(
            new Panel("[yellow]Параллелизм[/]"));

        layout["RightBottom"].Update(
            new Panel("[red]Аномалии[/]"));

        AnsiConsole.Write(layout);
    }
}