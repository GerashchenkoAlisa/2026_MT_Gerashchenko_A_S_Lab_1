class Program
{
    static void Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Usage: 2026_MT_Gerashchenko_A_S_Lab_1.exe <config_path> <target_dir>");
            Console.WriteLine("Example: 2026_MT_Gerashchenko_A_S_Lab_1.exe config.json C:\\temp\\test_project");
            Environment.Exit(1);
        }

        string configPath = args[0];
        string targetDir = args[1];

        try
        {
            var logger = new LoggerService(targetDir);
            var configParser = new ConfigParser();
            var commandExecutor = new CommandExecutor();
            var pipelineRunner = new PipelineRunner(logger, commandExecutor);

            logger.LogInfo($"Loading configuration from: {configPath}");
            var config = configParser.LoadConfig(configPath);
            logger.LogInfo($"Loaded {config.Pipeline?.Count ?? 0} stages");

            bool success = pipelineRunner.RunPipeline(config, targetDir);

            logger.LogInfo($"Pipeline completed. Status: {(success ? "SUCCESS" : "FAILED")}");
            Environment.Exit(success ? 0 : 1);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fatal error: {ex.Message}");
            Environment.Exit(1);
        }
    }
}