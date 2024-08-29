using log4net.Appender;
using log4net.Config;
using log4net.Layout;
using NamedPipeWrapper;
using NUnit.Framework;
using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace UnitTests
{
    [TestFixture]
    internal sealed class DataTests : IDisposable
    {
        private static readonly log4net.ILog Logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static DataTests()
        {
            PatternLayout layout = new PatternLayout("%-6timestamp %-5level - %message%newline");
            ConsoleAppender appender = new ConsoleAppender { Layout = layout };
            layout.ActivateOptions();
            appender.ActivateOptions();
            BasicConfigurator.Configure(appender);
        }

        private const string PipeName = "data_test_pipe";

        private NamedPipeServer<byte[]> _server;
        private NamedPipeClient<byte[]> _client;

        private string _expectedHash;
        private string _actualHash;
        private bool _clientDisconnected;

        private DateTime _startTime;

        private readonly ManualResetEvent _barrier = new ManualResetEvent(false);

        private bool _disposed;

        #region Setup and teardown

        [SetUp]
        public void SetUp()
        {
            Logger.Debug("Setting up test...");

            _barrier.Reset();

            _server = new NamedPipeServer<byte[]>(PipeName);
            _client = new NamedPipeClient<byte[]>(PipeName);

            _expectedHash = null;
            _actualHash = null;
            _clientDisconnected = false;

            _server.ClientDisconnected += ServerOnClientDisconnected;
            _server.ClientMessage += ServerOnClientMessage;

            _server.Error += ServerOnError;
            _client.Error += ClientOnError;

            _server.Start();
            _client.Start();

            // Give the client and server a few seconds to connect before sending data
            Thread.Sleep(TimeSpan.FromSeconds(1));

            Logger.Debug("Client and server started");
            Logger.Debug("---");

            _startTime = DateTime.Now;
        }

        private void ServerOnError(object sender, PipeErrorEventArgs<byte[], byte[]> e)
        {
            throw new NotImplementedException();
        }

        private void ClientOnError(object sender, PipeErrorEventArgs<byte[], byte[]> e)
        {
            throw new NotImplementedException();
        }

        [TearDown]
        public void TearDown()
        {
            Logger.Debug("---");
            Logger.Debug("Stopping client and server...");

            _server.Stop();
            _client.Stop();

            _server.ClientDisconnected -= ServerOnClientDisconnected;
            _server.ClientMessage -= ServerOnClientMessage;

            _server.Error -= ServerOnError;
            _client.Error -= ClientOnError;

            Logger.Debug("Client and server stopped");
            Logger.Debug($"Test took {DateTime.Now - _startTime}");
            Logger.Debug("~~~~~~~~~~~~~~~~~~~~~~~~~~");
        }

        #endregion

        #region Events

        private void ServerOnClientDisconnected(object sender, PipeConnectionEventArgs<byte[], byte[]> e)
        {
            Logger.Warn("Client disconnected");
            _clientDisconnected = true;
            _barrier.Set();
        }

        private void ServerOnClientMessage(object sender, PipeMessageEventArgs<byte[], byte[]> e)
        {
            Logger.Debug($"Received {e.Message.Length} bytes from the client");
            _actualHash = Hash(e.Message);
            _barrier.Set();
        }

        #endregion

        #region Tests

        [Test]
        public void TestEmptyMessageDoesNotDisconnectClient()
        {
            SendMessageToServer(0);
            _barrier.WaitOne(TimeSpan.FromSeconds(2));
            Assert.NotNull(_actualHash, "Server should have received a zero-byte message from the client");
            Assert.AreEqual(_expectedHash, _actualHash, "SHA-256 hashes for zero-byte message should match");
            Assert.IsFalse(_clientDisconnected, "Server should not disconnect the client for explicitly sending zero-length data");
        }

        [Test]
        public void TestMessageSize1B()
        {
            const int numBytes = 1;
            SendMessageToServer(numBytes);
            _barrier.WaitOne(TimeSpan.FromSeconds(20));
            Assert.NotNull(_actualHash, $"Server should have received client's {numBytes} byte message");
            Assert.AreEqual(_expectedHash, _actualHash, $"SHA-256 hashes for {numBytes} byte message should match");
            Assert.IsFalse(_clientDisconnected, "Server should still be connected to the client");
        }

        [Test]
        public void TestMessageSize2B()
        {
            const int numBytes = 2;
            SendMessageToServer(numBytes);
            _barrier.WaitOne(TimeSpan.FromSeconds(20));
            Assert.NotNull(_actualHash, $"Server should have received client's {numBytes} byte message");
            Assert.AreEqual(_expectedHash, _actualHash, $"SHA-256 hashes for {numBytes} byte message should match");
            Assert.IsFalse(_clientDisconnected, "Server should still be connected to the client");
        }

        [Test]
        public void TestMessageSize3B()
        {
            const int numBytes = 3;
            SendMessageToServer(numBytes);
            _barrier.WaitOne(TimeSpan.FromSeconds(20));
            Assert.NotNull(_actualHash, $"Server should have received client's {numBytes} byte message");
            Assert.AreEqual(_expectedHash, _actualHash, $"SHA-256 hashes for {numBytes} byte message should match");
            Assert.IsFalse(_clientDisconnected, "Server should still be connected to the client");
        }

        [Test]
        public void TestMessageSize9B()
        {
            const int numBytes = 9;
            SendMessageToServer(numBytes);
            _barrier.WaitOne(TimeSpan.FromSeconds(20));
            Assert.NotNull(_actualHash, $"Server should have received client's {numBytes} byte message");
            Assert.AreEqual(_expectedHash, _actualHash, $"SHA-256 hashes for {numBytes} byte message should match");
            Assert.IsFalse(_clientDisconnected, "Server should still be connected to the client");
        }

        [Test]
        public void TestMessageSize33B()
        {
            const int numBytes = 33;
            SendMessageToServer(numBytes);
            _barrier.WaitOne(TimeSpan.FromSeconds(20));
            Assert.NotNull(_actualHash, $"Server should have received client's {numBytes} byte message");
            Assert.AreEqual(_expectedHash, _actualHash, $"SHA-256 hashes for {numBytes} byte message should match");
            Assert.IsFalse(_clientDisconnected, "Server should still be connected to the client");
        }

        [Test]
        public void TestMessageSize129B()
        {
            const int numBytes = 129;
            SendMessageToServer(numBytes);
            _barrier.WaitOne(TimeSpan.FromSeconds(20));
            Assert.NotNull(_actualHash, $"Server should have received client's {numBytes} byte message");
            Assert.AreEqual(_expectedHash, _actualHash, $"SHA-256 hashes for {numBytes} byte message should match");
            Assert.IsFalse(_clientDisconnected, "Server should still be connected to the client");
        }

        [Test]
        public void TestMessageSize1K()
        {
            const int numBytes = 1025;
            SendMessageToServer(numBytes);
            _barrier.WaitOne(TimeSpan.FromSeconds(20));
            Assert.NotNull(_actualHash, $"Server should have received client's {numBytes} byte message");
            Assert.AreEqual(_expectedHash, _actualHash, $"SHA-256 hashes for {numBytes} byte message should match");
            Assert.IsFalse(_clientDisconnected, "Server should still be connected to the client");
        }

        [Test]
        public void TestMessageSize1M()
        {
            const int numBytes = 1024 * 1024 + 1;
            SendMessageToServer(numBytes);
            _barrier.WaitOne(TimeSpan.FromSeconds(20));
            Assert.NotNull(_actualHash, $"Server should have received client's {numBytes} byte message");
            Assert.AreEqual(_expectedHash, _actualHash, $"SHA-256 hashes for {numBytes} byte message should match");
            Assert.IsFalse(_clientDisconnected, "Server should still be connected to the client");
        }

        [Test]
        public void TestMessageSize100M()
        {
            const int numBytes = 1024 * 1024 * 100 + 1;
            SendMessageToServer(numBytes);
            _barrier.WaitOne(TimeSpan.FromSeconds(20));
            Assert.NotNull(_actualHash, $"Server should have received client's {numBytes} byte message");
            Assert.AreEqual(_expectedHash, _actualHash, $"SHA-256 hashes for {numBytes} byte message should match");
            Assert.IsFalse(_clientDisconnected, "Server should still be connected to the client");
        }

        [Test]
        public void TestMessageSize200M()
        {
            const int numBytes = 1024 * 1024 * 200 + 1;
            SendMessageToServer(numBytes);
            _barrier.WaitOne(TimeSpan.FromSeconds(20));
            Assert.NotNull(_actualHash, $"Server should have received client's {numBytes} byte message");
            Assert.AreEqual(_expectedHash, _actualHash, $"SHA-256 hashes for {numBytes} byte message should match");
            Assert.IsFalse(_clientDisconnected, "Server should still be connected to the client");
        }

        [Test]
        public void TestMessageSize300M()
        {
            const int numBytes = 1024 * 1024 * 300 + 1;
            SendMessageToServer(numBytes);
            _barrier.WaitOne(TimeSpan.FromSeconds(20));
            Assert.NotNull(_actualHash, $"Server should have received client's {numBytes} byte message");
            Assert.AreEqual(_expectedHash, _actualHash, $"SHA-256 hashes for {numBytes} byte message should match");
            Assert.IsFalse(_clientDisconnected, "Server should still be connected to the client");
        }

        [Test]
        public void TestMessageSize100Mx3()
        {
            const int numBytes = 1024 * 1024 * 100 + 1;

            _barrier.Reset();
            SendMessageToServer(numBytes);
            _barrier.WaitOne(TimeSpan.FromSeconds(20));
            Assert.NotNull(_actualHash, $"Server should have received client's {numBytes} byte message");
            Assert.AreEqual(_expectedHash, _actualHash, $"SHA-256 hashes for {numBytes} byte message should match");
            Assert.IsFalse(_clientDisconnected, "Server should still be connected to the client");

            Logger.Debug("...");

            _barrier.Reset();
            SendMessageToServer(numBytes);
            _barrier.WaitOne(TimeSpan.FromSeconds(20));
            Assert.NotNull(_actualHash, $"Server should have received client's {numBytes} byte message");
            Assert.AreEqual(_expectedHash, _actualHash, $"SHA-256 hashes for {numBytes} byte message should match");
            Assert.IsFalse(_clientDisconnected, "Server should still be connected to the client");

            Logger.Debug("...");

            _barrier.Reset();
            SendMessageToServer(numBytes);
            _barrier.WaitOne(TimeSpan.FromSeconds(20));
            Assert.NotNull(_actualHash, $"Server should have received client's {numBytes} byte message");
            Assert.AreEqual(_expectedHash, _actualHash, $"SHA-256 hashes for {numBytes} byte message should match");
            Assert.IsFalse(_clientDisconnected, "Server should still be connected to the client");
        }

        [Test]
        public void TestMessageSize300Mx3()
        {
            const int numBytes = 1024 * 1024 * 300 + 1;

            _barrier.Reset();
            SendMessageToServer(numBytes);
            _barrier.WaitOne(TimeSpan.FromSeconds(20));
            Assert.NotNull(_actualHash, $"Server should have received client's {numBytes} byte message");
            Assert.AreEqual(_expectedHash, _actualHash, $"SHA-256 hashes for {numBytes} byte message should match");
            Assert.IsFalse(_clientDisconnected, "Server should still be connected to the client");

            Logger.Debug("...");

            _barrier.Reset();
            SendMessageToServer(numBytes);
            _barrier.WaitOne(TimeSpan.FromSeconds(20));
            Assert.NotNull(_actualHash, $"Server should have received client's {numBytes} byte message");
            Assert.AreEqual(_expectedHash, _actualHash, $"SHA-256 hashes for {numBytes} byte message should match");
            Assert.IsFalse(_clientDisconnected, "Server should still be connected to the client");

            Logger.Debug("...");

            _barrier.Reset();
            SendMessageToServer(numBytes);
            _barrier.WaitOne(TimeSpan.FromSeconds(20));
            Assert.NotNull(_actualHash, $"Server should have received client's {numBytes} byte message");
            Assert.AreEqual(_expectedHash, _actualHash, $"SHA-256 hashes for {numBytes} byte message should match");
            Assert.IsFalse(_clientDisconnected, "Server should still be connected to the client");
        }

        #endregion

        #region Helper methods

        private void SendMessageToServer(int numBytes)
        {
            Logger.Debug($"Generating {numBytes} bytes of random data...");

            // Generate some random data and compute its SHA-256 hash
            byte[] data = new byte[numBytes];
            new Random().NextBytes(data);

            Logger.Debug($"Computing SHA-256 hash for {numBytes} bytes of data...");

            _expectedHash = Hash(data);

            Logger.Debug($"Sending {numBytes} bytes of data to the client...");

            _client.PushMessage(data);

            Logger.Debug($"Finished sending {numBytes} bytes of data to the client");
        }

        /// <summary>
        /// Computes the SHA-256 hash (lowercase) of the specified byte array.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private static string Hash(byte[] bytes)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(bytes);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("x2", CultureInfo.InvariantCulture));
                }
                return sb.ToString();
            }
        }

        #endregion

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                // Dispose managed resources
                _barrier.Dispose();
            }

            // no unmanaged resources to dispose when `disposing` is false

            _disposed = true;
        }
    }
}
