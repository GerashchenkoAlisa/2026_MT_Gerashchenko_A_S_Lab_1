using CICD_App.Models;

namespace CICD_App.Services;

public interface IConfigParser
{
    PipelineConfig LoadConfig(string configPath);
}