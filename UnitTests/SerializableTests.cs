using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Layout;
using NamedPipeWrapper;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace UnitTests
{
    [TestFixture]
    internal class SerializableTests : IDisposable
    {
        private static readonly ILog Logger =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static SerializableTests()
        {
            PatternLayout layout = new PatternLayout("%-6timestamp %-5level - %message%newline");
            ConsoleAppender appender = new ConsoleAppender { Layout = layout };
            layout.ActivateOptions();
            appender.ActivateOptions();
            BasicConfigurator.Configure(appender);
        }

        private const string PipeName = "data_test_pipe";

        private NamedPipeServer<TestCollection> _server;
        private NamedPipeClient<TestCollection> _client;

        private TestCollection _expectedData;
        private int _expectedHash;
        private TestCollection _actualData;
        private int _actualHash;

        private DateTime _startTime;

        private readonly ManualResetEvent _barrier = new ManualResetEvent(false);

        private readonly List<Exception> _exceptions = new List<Exception>();

        private bool _disposed;

        #region Setup and teardown

        [SetUp]
        public void SetUp()
        {
            Logger.Debug("Setting up test...");

            _barrier.Reset();
            _exceptions.Clear();

            _server = new NamedPipeServer<TestCollection>(PipeName);
            _client = new NamedPipeClient<TestCollection>(PipeName);

            _expectedData = null;
            _expectedHash = 0;
            _actualData = null;
            _actualHash = 0;

            _server.ClientMessage += ServerOnClientMessage;

            _server.Error += OnError;
            _client.Error += OnError;

            _server.Start();
            _client.Start();

            // Give the client and server a few seconds to connect before sending data
            Thread.Sleep(TimeSpan.FromSeconds(1));

            Logger.Debug("Client and server started");
            Logger.Debug("---");

            _startTime = DateTime.Now;
        }

        private void OnError(object sender, WorkerErrorEventArgs e)
        {
            _exceptions.Add(e.Exception);
            _barrier.Set();
        }

        [TearDown]
        public void TearDown()
        {
            Logger.Debug("---");
            Logger.Debug("Stopping client and server...");

            _server.ClientMessage -= ServerOnClientMessage;

            _server.Stop();
            _client.Stop();

            Logger.Debug("Client and server stopped");
            Logger.DebugFormat("Test took {0}", DateTime.Now - _startTime);
            Logger.Debug("~~~~~~~~~~~~~~~~~~~~~~~~~~");
        }

        #endregion

        #region Events

        private void ServerOnClientMessage(object sender, PipeMessageEventArgs<TestCollection, TestCollection> e)
        {
            Logger.Debug($"Received collection with {e.Message.Count} items from the client");
            _actualData = e.Message;
            _actualHash = e.Message.GetHashCode();
            _barrier.Set();
        }

        #endregion

        #region Tests

        [Test]
        public void TestCircularReferences()
        {
            _expectedData = new TestCollection();
            for (int i = 0; i < 10; i++)
            {
                TestItem item = new TestItem(i, _expectedData, RandomEnum());
                _expectedData.Add(item);
            }

            _expectedHash = _expectedData.GetHashCode();

            _client.PushMessage(_expectedData);
            _barrier.WaitOne(TimeSpan.FromSeconds(5));

            if (_exceptions.Count != 0)
                throw new AggregateException(_exceptions);

            Assert.NotNull(_actualHash, $"Server should have received client's {_expectedData.Count} item message");
            Assert.AreEqual(_expectedHash, _actualHash, $"Hash codes for {_expectedData.Count} item message should match");
            Assert.AreEqual(_expectedData.Count, _actualData.Count, "Collection lengths should be equal");

            for (int i = 0; i < _actualData.Count; i++)
            {
                TestItem expectedItem = _expectedData[i];
                TestItem actualItem = _actualData[i];
                Assert.AreEqual(expectedItem, actualItem, $"Items at index {i} should be equal");
                Assert.AreEqual(actualItem.Parent, _actualData, $"Item at index {i}'s Parent property should reference the item's parent collection");
            }
        }

        private static TestEnum RandomEnum()
        {
            switch (new Random().Next(0, 3))
            {
                case 0:
                    return TestEnum.A;
                case 1:
                    return TestEnum.B;
                default:
                    return TestEnum.C;
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

    [Serializable]
    internal class TestCollection : List<TestItem>
    {
        public override int GetHashCode()
        {
            List<string> strs = new List<string>(Count);
            foreach (TestItem item in ToArray())
            {
                strs.Add(item.GetHashCode().ToString(CultureInfo.InvariantCulture));
            }

            string str = string.Join(",", strs);
            return Hash(Encoding.UTF8.GetBytes(str)).GetHashCode();
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
    }

    [Serializable]
    internal class TestItem
    {
        public readonly int ID;
        public readonly TestCollection Parent;
        public readonly TestEnum Enum;

        public TestItem(int id, TestCollection parent, TestEnum @enum)
        {
            ID = id;
            Parent = parent;
            Enum = @enum;
        }

        protected bool Equals(TestItem other)
        {
            return ID == other.ID && Enum == other.Enum;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((TestItem)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (ID * 397) ^ (int)Enum;
            }
        }
    }

    internal enum TestEnum
    {
        A = 1,
        B = 2,
        C = 3,
    }
}
