using System.Text.Json.Serialization;

public class PipelineConfig
{
    [JsonPropertyName("pipeline")]
    public List<PipelineStage>? Pipeline { get; set; }
}