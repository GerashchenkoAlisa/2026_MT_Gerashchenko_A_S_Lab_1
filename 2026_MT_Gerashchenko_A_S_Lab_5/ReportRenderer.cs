using Spectre.Console;
using _2026_MT_Gerashchenko_A_S_Lab_5.Models;

namespace _2026_MT_Gerashchenko_A_S_Lab_5;

public static class ReportRenderer
{
    public static void RenderTopMethods(List<TopMethodEntry> data)
    {
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
                item.Processor
            );
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

    public static void RenderAnomalies(List<AnomalyEntry> anomalies)
    {
        if (anomalies.Count==0)
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
                item.SingleMs.ToString(),
                item.MultiMs.ToString(),
                $"[red]{item.Overhead}[/]"
            );
        }

        AnsiConsole.Write(table);
    }

    public static void RenderEnvironmentComparison(List<EnvironmentEntry> data)
    {
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
                item.AvgSingle.ToString("F0"),
                item.AvgMulti.ToString("F0"),
                item.Count.ToString()
            );
        }

        AnsiConsole.Write(table);
    }

    public static void RenderBestStructure(List<BestStructureEntry> data)
    {
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

    public static void RenderOrderComparison(List<OrderComparisonEntry> data)
    {
        var table = new Table()
            .Border(TableBorder.Rounded)
            .Title("[bold]Среднее время выполнения[/]");

        table.AddColumn("Тест");
        table.AddColumn(new TableColumn("Среднее время").RightAligned());

        foreach (var item in data)
        {
            table.AddRow(
                item.Test,
                item.AvgTime.ToString("F0")
            );
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