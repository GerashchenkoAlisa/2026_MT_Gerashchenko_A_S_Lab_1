public interface IPipelineRunner
{
    bool RunPipeline(PipelineConfig config, string workingDirectory);
}