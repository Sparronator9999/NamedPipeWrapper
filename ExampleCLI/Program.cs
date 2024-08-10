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

            if (args.Length >= 1 && args[0].ToLower() == "/server")
            {
                Console.WriteLine("Started in SERVER mode.");
                new Server(DefaultPipeName);
            }
            else
            {
                Console.WriteLine("Started in CLIENT mode.");
                new Client(DefaultPipeName);
            }
        }
    }
}
