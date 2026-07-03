using System.Text.Json;

public class ConfigParser : IConfigParser
{
    public PipelineConfig LoadConfig(string configPath)
    {
        if (!File.Exists(configPath))
        {
            throw new FileNotFoundException($"Configuration file not found: {configPath}");
        }

        string jsonContent = File.ReadAllText(configPath);

        PipelineConfig? config = JsonSerializer.Deserialize<PipelineConfig>(jsonContent);

        if (config?.Pipeline == null)
        {
            throw new InvalidOperationException("Invalid configuration: missing 'pipeline' array");
        }

        return config;
    }
}