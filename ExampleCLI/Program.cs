using System;

namespace ExampleCLI
{
    internal static class Program
    {
        private const string DefaultPipeName = "named_pipe_test_server";

        private static void Main(string[] args)
        {
            Console.WriteLine("Named Pipe Client - Example CLI");
            Console.WriteLine("For help with commands, type \"!help\".");
            Console.WriteLine("Type \"!exit\" (or press ^C) to exit.");
            Console.WriteLine();

            if (args.Length >= 1 && string.Equals(args[0], "/server", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Started in SERVER mode.");
                Server.Start(DefaultPipeName);
            }
            else
            {
                Console.WriteLine("Started in CLIENT mode.");
                Client.Start(DefaultPipeName);
            }
        }
    }
}
