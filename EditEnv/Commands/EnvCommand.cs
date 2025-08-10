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

        public ValueTask ExecuteAsync(IConsole console)
        {
            if (string.IsNullOrWhiteSpace(Key))
            {
                console.WriteLine("Key can not be empty.");
                return default;
            }

            if (Key.Equals("PATH", StringComparison.OrdinalIgnoreCase))
            {
                console.Error.WriteLine($"Please use the PATH command.");
                return default;
            }

            try
            {
                switch (EnvAction)
                {
                    case EnvAction.Set:
                        EnvHelper.SetVariable(Key, Value, Target);
                        break;
                    case EnvAction.Get:
                        var value = EnvHelper.GetVariable(Key, Target);
                        console.Output.WriteLine(value);
                        break;
                    case EnvAction.Remove:
                        EnvHelper.RemoveVariable(Key, Target);
                        break;
                    case EnvAction.List:
                        console.Output.WriteLine("not support");

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
