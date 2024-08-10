using System;
using NamedPipeWrapper;

namespace ExampleCLI
{
    internal class MyServer
    {
        private bool KeepRunning
        {
            get
            {
                ConsoleKeyInfo key = Console.ReadKey();
                return key.Key != ConsoleKey.Q;
            }
        }

        public MyServer(string pipeName)
        {
            NamedPipeServer<MyMessage> server = new NamedPipeServer<MyMessage>(pipeName);
            server.ClientConnected += OnClientConnected;
            server.ClientDisconnected += OnClientDisconnected;
            server.ClientMessage += OnClientMessage;
            server.Error += OnError;
            server.Start();
            while (KeepRunning)
            {
                // Do nothing - wait for user to press 'q' key
            }
            server.Stop();
        }

        private void OnClientConnected(NamedPipeConnection<MyMessage, MyMessage> connection)
        {
            Console.WriteLine("Client {0} is now connected!", connection.ID);
            connection.PushMessage(new MyMessage
            {
                Id = new Random().Next(),
                Text = "Welcome!"
            });
        }

        private void OnClientDisconnected(NamedPipeConnection<MyMessage, MyMessage> connection)
        {
            Console.WriteLine("Client {0} disconnected", connection.ID);
        }

        private void OnClientMessage(NamedPipeConnection<MyMessage, MyMessage> connection, MyMessage message)
        {
            Console.WriteLine("Client {0} says: {1}", connection.ID, message);
        }

        private void OnError(Exception exception)
        {
            Console.Error.WriteLine("ERROR: {0}", exception);
        }
    }
}
