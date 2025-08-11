using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

namespace EditEnv.Commands
{
    [Command("PATH", Description = "PATH Command")]
    public class PathCommand : ICommand
    {
        [CommandParameter(0, Description = "Action")]
        public required PathAction PathAction { get; init; }

        [CommandOption("value", 'v', Description = "Directory you want add to PATH")]
        public string Value { get; init; } = "";

        [CommandOption("target", 't', Description = "Environment variable target")]
        public EnvironmentVariableTarget Target { get; init; } = EnvironmentVariableTarget.User;

        public async ValueTask ExecuteAsync(IConsole console)
        {
            if (
                (PathAction == PathAction.Add || PathAction == PathAction.Remove)
                && string.IsNullOrWhiteSpace(Value)
            )
            {
                await console.Output.WriteLineAsync("Value can not be empty.");
                return;
            }

            if (PathAction == PathAction.Add || PathAction == PathAction.Remove)
            {
                var full = Path.GetFullPath(Value);
                if (!Directory.Exists(full))
                {
                    await console.Output.WriteLineAsync($"Directory [{full}] is not exist.");
                    return;
                }
            }

            try
            {
                switch (PathAction)
                {
                    case PathAction.Add:
                        EnvHelper.AddToPath(Value, Target);
                        await Storage.Instance.AddPath(
                            new PathModel { Target = Target, Value = Value }
                        );
                        await console.Output.WriteLineAsync($"Added to PATH:{Value}");

                        break;

                    case PathAction.Remove:
                        EnvHelper.RemoveFromPath(Value, Target);
                        await Storage.Instance.RemovePath(
                            new PathModel { Target = Target, Value = Value }
                        );
                        await console.Output.WriteLineAsync($"Removed from PATH: {Value}");

                        break;

                    case PathAction.List:
                        var allPaths = Storage.Instance.GetAllPath(Target);

                        foreach (var item in allPaths)
                        {
                            await console.Output.WriteLineAsync($"{item.Value}");
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                await console.Error.WriteLineAsync(ex.Message);
            }
        }
    }
}
