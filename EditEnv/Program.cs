// See https://aka.ms/new-console-template for more information

using CliFx;

await new CliApplicationBuilder()
    .AddCommandsFromThisAssembly()
    .SetTitle("Edit Environment Variable")
    .SetDescription("A commandline tool to edit Edit environment variable.")
    .Build()
    .RunAsync();
