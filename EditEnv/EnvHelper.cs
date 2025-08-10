namespace EditEnv
{
    public static class EnvHelper
    {
        public static void SetVariable(string key, string value, EnvironmentVariableTarget target)
        {
            var exist = !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(key, target));

            if (exist)
            {
                Console.Write($"The Variable key [{key}] is exist, will you update it? (y/n)");
                var update = Console.ReadLine();
                if (update?.Equals("y", StringComparison.OrdinalIgnoreCase) == false)
                { //cancel
                    return;
                }
            }

            Environment.SetEnvironmentVariable(key, value, target);
            Console.WriteLine($"Set :{key} = {value}");
        }

        public static string GetVariable(string key, EnvironmentVariableTarget target)
        {
            return Environment.GetEnvironmentVariable(key, target) ?? "";
        }

        public static void RemoveVariable(string key, EnvironmentVariableTarget target)
        {
            var exist = !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(key, target));

            if (exist)
            {
                Environment.SetEnvironmentVariable(key, null, target);
            }
            else
            {
                throw new KeyNotFoundException($"The key {key} is not exist");
            }
        }

        public static void AddToPath(string directoryPath, EnvironmentVariableTarget target)
        {
            var fullPath = Path.GetFullPath(directoryPath);

            if (!Directory.Exists(fullPath))
            {
                throw new DirectoryNotFoundException($"Directory is not exist: {fullPath}");
            }

            // get PATH
            var currentPath = Environment.GetEnvironmentVariable("PATH", target) ?? "";

            if (ContainsPath(currentPath, fullPath))
            {
                Console.WriteLine($"The Directory [{fullPath}] is in PATH , skipped. ");
                return;
            }

            var newPath = string.IsNullOrEmpty(currentPath)
                ? fullPath
                : $"{currentPath};{fullPath}";

            Environment.SetEnvironmentVariable("PATH", newPath, target);
        }

        private static bool ContainsPath(string pathVariable, string pathToCheck)
        {
            var paths = pathVariable.Split(';');

            return paths.Any(path =>
                string.Equals(
                    Path.GetFullPath(path.Trim()),
                    Path.GetFullPath(pathToCheck.Trim()),
                    StringComparison.OrdinalIgnoreCase
                )
            );
        }

        public static void RemoveFromPath(string pathVariable, EnvironmentVariableTarget target)
        {
            var paths = ListPath(target);

            if (!paths.Contains(pathVariable))
            {
                Console.WriteLine($"{pathVariable} is not in PATH ");
                return;
            }

            paths.Remove(pathVariable);

            var newPath = string.Join(";", paths);

            Environment.SetEnvironmentVariable("PATH", newPath, target);
        }

        public static List<string> ListPath(EnvironmentVariableTarget target)
        {
            var currentPath = Environment.GetEnvironmentVariable("PATH", target) ?? "";

            var paths = currentPath.Split(";") ;

            return [.. paths];
        }
    }

    public enum EnvAction
    {
        Set,
        Get,
        Remove,
        List,
    }

    public enum PathAction
    {
        Add,
        Remove,
        List,
    }
}
