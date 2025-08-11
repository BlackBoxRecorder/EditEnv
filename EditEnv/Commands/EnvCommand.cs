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

        [CommandOption("key", 'k', Description = "Environment variable key")]
        public string Key { get; init; } = "";

        [CommandOption("value", 'v', Description = "Environment variable value")]
        public string Value { get; init; } = "";

        [CommandOption("target", 't', Description = "Environment variable target")]
        public EnvironmentVariableTarget Target { get; init; } = EnvironmentVariableTarget.User;

        public async ValueTask ExecuteAsync(IConsole console)
        {
            if (EnvAction != EnvAction.List && string.IsNullOrWhiteSpace(Key))
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
                        await console.Output.WriteLineAsync($"Set: {Key} = {Value}");

                        break;
                    case EnvAction.Get:
                        var value = EnvHelper.GetVariable(Key, Target);
                        await console.Output.WriteLineAsync($"{Key} = {value}");
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
                        await console.Output.WriteLineAsync($"Removed: {Key}");

                        break;
                    case EnvAction.List:
                        var items = Storage.Instance.GetAllEnv(Target);

                        if (items != null)
                        {
                            foreach (var item in items)
                            {
                                await console.Output.WriteLineAsync($"{item.Key} : {item.Value}");
                            }
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
