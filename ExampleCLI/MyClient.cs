using System;
using NamedPipeWrapper;

namespace ExampleCLI
{
    internal class MyClient
    {
        private bool KeepRunning
        {
            get
            {
                ConsoleKeyInfo key = Console.ReadKey();
                return key.Key != ConsoleKey.Q;
            }
        }

        public MyClient(string pipeName)
        {
            NamedPipeClient<MyMessage> client = new NamedPipeClient<MyMessage>(pipeName);
            client.ServerMessage += OnServerMessage;
            client.Error += OnError;
            client.Start();
            while (KeepRunning)
            {
                // Do nothing - wait for user to press 'q' key
            }
            client.Stop();
        }

        private void OnServerMessage(NamedPipeConnection<MyMessage, MyMessage> connection, MyMessage message)
        {
            Console.WriteLine("Server says: {0}", message);
        }

        private void OnError(Exception exception)
        {
            Console.Error.WriteLine("ERROR: {0}", exception);
        }
    }
}
