using FluentAssertions;
using NamedPipeWrapper;
using NUnit.Framework;
using System;
using System.Collections.Concurrent;
using System.IO.Pipes;
using System.Text;
using System.Threading;

namespace UnitTests
{
    [TestFixture]
    public class StringNamedPipeTests : IDisposable
    {
        private const string PipeName = "test-pipe";
        private const int Timeout = 1000;

        private NamedPipeServer<string> _server;
        private NamedPipeClientStream _client;

        private ConcurrentQueue<string> _serverMessageQueue;

        private ManualResetEvent _serverReceivedMessageEvent;

        private bool _disposed;

        [SetUp]
        public void SetUp()
        {
            _serverMessageQueue = new ConcurrentQueue<string>();

            _serverReceivedMessageEvent = new ManualResetEvent(false);

            StartServer();
            StartClient();
        }


        [Test]
        public void ServerShouldReceiveSameMessageClientSent()
        {
            string message = Guid.NewGuid().ToString();
            ClientSendMessage(message);

            _serverReceivedMessageEvent.WaitOne(Timeout);

            _serverMessageQueue.TryDequeue(out string messageReceived);
            messageReceived.Should().Be(message);
        }


        [Test]
        public void ClientShouldReceiveSameMessageServerSent()
        {
            string message = Guid.NewGuid().ToString();
            _server.PushMessage(message);

            string messageReceived = ClientReadMessage();

            messageReceived.Should().Be(message);
        }

        #region Helpers

        private void StartServer()
        {
            _server = new NamedPipeServer<string>(PipeName);
            _server.ClientMessage += OnClientMessageReceived;
            _server.Start();
        }

        private void StartClient()
        {
            _client = new NamedPipeClientStream(PipeName);
            _client.Connect();

            // Read pipe name
            string pipeName = ClientReadMessage();
            _client.Close();

            // Wait for data pipe connection to be created 
            Thread.Sleep(1000);

            // Connect to data pipe
            _client = new NamedPipeClientStream(pipeName);
            _client.Connect();
        }

        private string ClientReadMessage()
        {
            const int bufferSize = 1024;
            byte[] buffer = new byte[bufferSize];
            _client.Read(buffer, 0, bufferSize);
            return Encoding.Unicode.GetString(buffer).TrimEnd('\0');
        }

        private void ClientSendMessage(string message)
        {
            byte[] messageBytes = Encoding.Unicode.GetBytes(message);
            _client.Write(messageBytes, 0, messageBytes.Length);
            _client.Flush();
        }

        private void OnClientMessageReceived(NamedPipeConnection<string, string> connection, string message)
        {
            _serverMessageQueue.Enqueue(message);
            _serverReceivedMessageEvent.Set();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _serverReceivedMessageEvent.Dispose();
                _client.Dispose();
            }

            _disposed = true;
        }

        #endregion
    }
}
