using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

namespace EditEnv.Commands
{
    [Command(Description = "Set environment variable key value pair")]
    public class EnvCommand : ICommand
    {
        [CommandParameter(0, Description = "Action")]
        public required EnvAction EnvAction { get; init; }

        [CommandParameter(1, Description = "Environment variable key")]
        public required string Key { get; init; }

        [CommandOption("value", 'v', Description = "Environment variable value")]
        public string Value { get; init; } = "";

        [CommandOption("target", Description = "Environment variable target")]
        public EnvironmentVariableTarget Target { get; init; } = EnvironmentVariableTarget.User;

        public async ValueTask ExecuteAsync(IConsole console)
        {
            if (string.IsNullOrWhiteSpace(Key))
            {
                console.WriteLine("Key can not be empty.");
                return;
            }

            if (Key.Equals("PATH", StringComparison.OrdinalIgnoreCase))
            {
                await console.Error.WriteLineAsync($"Please use the PATH command.");
                return;
            }

            try
            {
                switch (EnvAction)
                {
                    case EnvAction.Set:
                        EnvHelper.SetVariable(Key, Value, Target);
                        await Storage.Instance.AddOrUpdateEnv(
                            new EnvModel
                            {
                                Key = Key,
                                Value = Value,
                                Target = Target,
                            }
                        );
                        break;
                    case EnvAction.Get:
                        var value = EnvHelper.GetVariable(Key, Target);
                        await console.Output.WriteLineAsync(value);
                        break;
                    case EnvAction.Remove:
                        EnvHelper.RemoveVariable(Key, Target);
                        await Storage.Instance.RemoveEnv(
                            new EnvModel
                            {
                                Key = Key,
                                Target = Target,
                                Value = "",
                            }
                        );

                        break;
                    case EnvAction.List:
                        await console.Output.WriteLineAsync("not support");

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
