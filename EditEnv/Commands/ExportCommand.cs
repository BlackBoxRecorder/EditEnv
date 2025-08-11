using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

namespace EditEnv.Commands
{
    [Command("export", Description = "Export all env var")]
    public class ExportCommand : ICommand
    {
        private string OutputFile { get; } =
            Path.Combine(Environment.CurrentDirectory, "EditEnv.data.json");

        public async ValueTask ExecuteAsync(IConsole console)
        {
            var json = await Storage.ExportData();
            try
            {
                await File.WriteAllTextAsync(OutputFile, json, Encoding.UTF8);
                await console.Output.WriteLineAsync($"Export success : {OutputFile}");
            }
            catch (Exception ex)
            {
                await console.Error.WriteLineAsync($"Export failed : {ex.Message}");
            }
        }
    }
}
