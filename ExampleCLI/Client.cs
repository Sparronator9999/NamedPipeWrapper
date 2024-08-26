using NamedPipeWrapper;
using System;
using System.Globalization;
using System.Text;

namespace ExampleCLI
{
    internal static class Client
    {
        public static void Start(string pipeName)
        {
            NamedPipeClient<string> client = new NamedPipeClient<string>(pipeName);
            client.ServerMessage += OnServerMessage;
            client.Error += OnError;
            client.Start();

            Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e)
            {
                // stop server on ^C
                client.Stop();
            };

            while (true)
            {
                string input = Console.ReadLine();
                string[] args = input.Split(' ');

                if (args.Length == 0 || args[0] == "")
                {
                    // ignore empty commands
                    // (i.e. when user types nothing, then presses ENTER)
                    continue;
                }
                else if (args[0][0] == '!')
                {
                    switch (args[0].ToLower(CultureInfo.InvariantCulture))
                    {
                        case "!exit":
                        {
                            Console.WriteLine("Exiting, please wait...");
                            client.Stop();
                            return;
                        }
                        case "!clear":
                        {
                            Console.Clear();
                            break;
                        }
                        case "!help":
                        {
                            Console.WriteLine(
                                "\nClient commands:\n" +
                                "  !exit\t\t\tExit the client.\n" +
                                "  !clear\t\tClear the console output.\n" +
                                "  !help\t\t\tPrint this help message.\n" +
                                "  !push <message>\tPush <message> to the server.\n" +
                                "  \t\t\tThis is the default command if no command is specified.\n" +
                                "  \t\t\tQuotes are not required around the message to be sent.\n");
                            break;
                        }
                        case "!push":
                        {
                            if (args.Length <= 1)
                            {
                                Console.Error.WriteLine("ERROR: please enter a message to push to the client");
                                break;
                            }

                            StringBuilder message = new StringBuilder();
                            for (int i = 1; i < args.Length; i++)
                                message.Append(args[i] + " ");

                            client.PushMessage(message.ToString().Trim());
                            break;
                        }
                        default:
                        {
                            Console.Error.WriteLine($"ERROR: unknown command: {args[0]}");
                            break;
                        }
                    }
                }
                else
                {
                    client.PushMessage(input.Trim());
                }
            }
        }

        private static void OnServerMessage(object sender, PipeMessageEventArgs<string, string> e)
        {
            Console.WriteLine($"<Server> {e.Message}");
        }

        private static void OnError(object sender, WorkerErrorEventArgs e)
        {
            Console.Error.WriteLine($"ERROR: {e}");
        }
    }
}
