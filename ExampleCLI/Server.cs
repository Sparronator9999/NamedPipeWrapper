using System;
using System.Collections.Generic;
using System.Text;
using NamedPipeWrapper;

namespace ExampleCLI
{
    internal class Server
    {
        private readonly List<int> ClientIDs = new List<int>();
        private readonly Dictionary<int, string> ClientNames = new Dictionary<int, string>();

        public Server(string pipeName)
        {
            NamedPipeServer<string> server = new NamedPipeServer<string>(pipeName);
            server.ClientConnected += OnClientConnected;
            server.ClientDisconnected += OnClientDisconnected;
            server.ClientMessage += OnClientMessage;
            server.Error += OnError;
            server.Start();

            Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e)
            {
                // stop server on ^C
                server.Stop();
            };

            Loop(server);
        }

        private void Loop(NamedPipeServer<string> server)
        {
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
                    switch (args[0].ToLower())
                    {
                        case "!exit":
                        {
                            Console.WriteLine("Exiting, please wait...");
                            server.Stop();
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
                                "\nServer commands:\n" +
                                "  !exit\t\t\t\tExit the client.\n" +
                                "  !clear\t\t\tClear the console output.\n" +
                                "  !help\t\t\t\tPrint this help message.\n" +
                                "  !list\t\t\t\tList all connected clients and their IDs\n" +
                                "  !push <client_id> <message>\tPush <message> to the specified client by ID.\n" +
                                "  \t\t\t\tUse !list to get a list of possible client IDs.\n" +
                                "  !broadcast, !bc <message>\tPush <message> to all clients.\n" +
                                "  \t\t\t\tThis is the default command if no command is specified.\n");
                            break;
                        }
                        case "!list":
                        {
                            Console.WriteLine($"There are {ClientIDs.Count} clients connected:");
                            for (int i = 0; i < ClientIDs.Count; i++)
                            {
                                Console.WriteLine($"Name: {ClientNames[ClientIDs[i]]}, ID: {ClientIDs[i]}");
                            }
                            break;
                        }
                        case "!broadcast":
                        case "!bc":
                        {
                            if (args.Length <= 1)
                            {
                                Console.Error.WriteLine("ERROR: please enter a message to broadcast to all clients");
                                break;
                            }

                            StringBuilder message = new StringBuilder();
                            for (int i = 1; i < args.Length; i++)
                                message.Append(args[i] + " ");

                            server.PushMessage(message.ToString().Trim());
                            break;
                        }
                        case "!push":
                        {
                            if (args.Length <= 1)
                            {
                                Console.Error.WriteLine("ERROR: please enter the client ID to push a message to");
                                break;
                            }

                            if (!int.TryParse(args[1], out int clientID))
                            {
                                Console.Error.WriteLine("ERROR: client ID is not a valid number");
                                break;
                            }

                            if (args.Length <= 2)
                            {
                                Console.Error.WriteLine("ERROR: please enter a message to push to the client");
                                break;
                            }

                            StringBuilder message = new StringBuilder();
                            for (int i = 2; i < args.Length; i++)
                                message.Append(args[i] + " ");

                            server.PushMessage(message.ToString().Trim());
                            break;
                        }
                        default:
                        {
                            Console.Error.WriteLine($"ERROR: Unknown command: {args[0]}");
                            break;
                        }
                    }
                }
                else
                {
                    server.PushMessage(input);
                }
            }
        }

        private void OnClientConnected(NamedPipeConnection<string, string> connection)
        {
            ClientIDs.Add(connection.ID);
            ClientNames.Add(connection.ID, connection.Name);
            Console.WriteLine($"Client {connection.ID} is now connected!");
            connection.PushMessage("Welcome! You are now connected to the server.");
        }

        private void OnClientDisconnected(NamedPipeConnection<string, string> connection)
        {
            ClientIDs.Remove(connection.ID);
            ClientNames.Remove(connection.ID);
            Console.WriteLine($"Client {connection.ID} disconnected");
        }

        private void OnClientMessage(NamedPipeConnection<string, string> connection, string message)
        {
            Console.WriteLine($"<Client {connection.ID}> {message}");
        }

        private void OnError(Exception exception)
        {
            Console.Error.WriteLine($"ERROR: {exception}");
        }
    }
}
