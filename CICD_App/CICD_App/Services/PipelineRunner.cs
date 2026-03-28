using CICD_App.Models;

namespace CICD_App.Services;

public class PipelineRunner : IPipelineRunner
{
    private readonly ILoggerService _logger;
    private readonly ICommandExecutor _commandExecutor;

    public PipelineRunner(ILoggerService logger, ICommandExecutor commandExecutor)
    {
        _logger = logger;
        _commandExecutor = commandExecutor;
    }

    public bool RunPipeline(PipelineConfig config, string workingDirectory)
    {
        if (config.Pipeline == null || config.Pipeline.Count == 0)
        {
            _logger.LogWarning("No stages defined in pipeline");
            return true;
        }

        foreach (var stage in config.Pipeline)
        {
            _logger.LogInfo($"Starting stage: {stage.Name}");
            _logger.LogInfo($"Command: {stage.Command} {stage.Arguments}");

            try
            {
                int exitCode = _commandExecutor.ExecuteCommandAsync(
                    stage.Command,
                    stage.Arguments,
                    workingDirectory).GetAwaiter().GetResult();

                if (exitCode == 0)
                {
                    _logger.LogSuccess($"Stage finished with ExitCode {exitCode}");
                }
                else
                {
                    _logger.LogError($"Stage failed with ExitCode {exitCode}");

                    if (stage.StopOnFailure)
                    {
                        _logger.LogError($"Stopping pipeline due to failure in stage: {stage.Name}");
                        return false;
                    }
                    else
                    {
                        _logger.LogWarning("Continuing pipeline despite failure (stopOnFailure = false)");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception executing stage {stage.Name}: {ex.Message}");

                if (stage.StopOnFailure)
                {
                    _logger.LogError($"Stopping pipeline due to exception in stage: {stage.Name}");
                    return false;
                }
            }
        }

        return true;
    }
}