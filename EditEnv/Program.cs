// See https://aka.ms/new-console-template for more information

using System.Reflection;
using CliFx;
using EditEnv;

await new CliApplicationBuilder()
    .AddCommandsFromThisAssembly()
    .SetTitle("Edit Environment Variable")
    .SetDescription("A commandline tool to edit Edit environment variable.")
    .SetExecutableName("EditEnv")
    .Build()
    .RunAsync();
