using CICD_App.Models;

namespace CICD_App.Services;

public interface IPipelineRunner
{
    bool RunPipeline(PipelineConfig config, string workingDirectory);
}