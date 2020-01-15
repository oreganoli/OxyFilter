using System;

namespace OxyFilter
{
    public static class Util
    {
        // Gets an environment variable and quits if it isn't set.
        public static string ForceGetEnv(string key)
        {
            var output = Environment.GetEnvironmentVariable(key);
            if (output != null) return output;
            Console.Error.WriteLine($"The environment variable {key} must be set.");
            Environment.Exit(1);
            return output;
        }
    }
}