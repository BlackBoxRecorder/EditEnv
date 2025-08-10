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

        [CommandOption("target", Description = "Environment variable target")]
        public EnvironmentVariableTarget Target { get; init; } = EnvironmentVariableTarget.User;

        public ValueTask ExecuteAsync(IConsole console)
        {
            if (
                (PathAction == PathAction.Add || PathAction == PathAction.Remove)
                && string.IsNullOrWhiteSpace(Value)
            )
            {
                console.WriteLine("Value can not be empty.");
                return default;
            }

            if (PathAction == PathAction.Add || PathAction == PathAction.Remove)
            {
                var full = Path.GetFullPath(Value);
                if (!Directory.Exists(full))
                {
                    console.WriteLine($"Directory [{full}] is not exist.");
                    return default;
                }
            }

            try
            {
                switch (PathAction)
                {
                    case PathAction.Add:
                        EnvHelper.AddToPath(Value, Target);
                        break;

                    case PathAction.Remove:
                        EnvHelper.RemoveFromPath(Value, Target);
                        break;

                    case PathAction.List:
                        var allPaths = EnvHelper.ListPath(Target);

                        foreach (var item in allPaths)
                        {
                            console.WriteLine($"{item}");
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                console.Error.WriteLine(ex.Message);
            }

            return default;
        }
    }
}
