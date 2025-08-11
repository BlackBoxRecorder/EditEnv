using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Newtonsoft.Json;

namespace EditEnv.Commands
{
    [Command("import", Description = "Restore env from file.")]
    public class ImportCommand : ICommand
    {
        [CommandOption("input", 'i', Description = "The data json file")]
        public required string InputFile { get; init; } =
            Path.Combine(Environment.CurrentDirectory, "EditEnv.data.json");

        public async ValueTask ExecuteAsync(IConsole console)
        {
            var input = Path.GetFullPath(InputFile);

            if (!File.Exists(input))
            {
                await console.Output.WriteLineAsync($"File not exist : {input}");
                return;
            }

            try
            {
                var json = await File.ReadAllTextAsync(input);

                var importModel =
                    JsonConvert.DeserializeObject<ImportDataModel>(json)
                    ?? throw new JsonSerializationException();

                foreach (var item in importModel.EnvModel)
                {
                    EnvHelper.SetVariable(item.Key, item.Value, item.Target);
                    await console.Output.WriteLineAsync($"Set: {item.Key}={item.Value}");
                }

                foreach (var item in importModel.PathModel)
                {
                    EnvHelper.AddToPath(item.Value, item.Target);
                    await console.Output.WriteLineAsync($"Added to PATH: {item.Value}");
                }

                await Storage.ImportData(json);
            }
            catch (Exception ex)
            {
                await console.Error.WriteLineAsync($"Import failed : {ex.Message}");
            }
        }
    }
}
