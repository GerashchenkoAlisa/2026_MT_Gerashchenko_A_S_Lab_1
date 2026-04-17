public interface ICommandExecutor
{
    Task<int> ExecuteCommandAsync(string command, string arguments, string workingDirectory);
}