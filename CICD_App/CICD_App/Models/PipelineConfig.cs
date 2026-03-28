using System.Text.Json.Serialization;

namespace CICD_App.Models;

public class PipelineConfig
{
    [JsonPropertyName("pipeline")]
    public List<PipelineStage>? Pipeline { get; set; }
}