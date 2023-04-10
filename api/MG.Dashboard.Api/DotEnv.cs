using System;
using System.IO;

namespace MG.Dashboard.Env;

public static class DotEnv
{
    public static void Load(string file)
    {
        if (!File.Exists(file))
        {
            return;
        }

        foreach (var line in File.ReadAllLines(file))
        {
            var parts = line.Split('=', count: 2, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2)
            {
                continue;
            }

            Environment.SetEnvironmentVariable(parts[0], parts[1]);
        }
    }
}
