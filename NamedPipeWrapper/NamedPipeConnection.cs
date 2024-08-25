using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Threading;
using NamedPipeWrapper.IO;
using NamedPipeWrapper.Threading;

namespace NamedPipeWrapper
{
    /// <summary>
    /// Represents a connection between a named pipe client and server.
    /// </summary>
    /// <typeparam name="TRead">
    /// The reference type to read from the named pipe.
    /// </typeparam>
    /// <typeparam name="TWrite">
    /// The reference type to write to the named pipe.
    /// </typeparam>
    public class NamedPipeConnection<TRead, TWrite>
        where TRead : class
        where TWrite : class
    {
        /// <summary>
        /// Gets the connection's unique identifier.
        /// </summary>
        public readonly int ID;

        /// <summary>
        /// Gets the connection's name.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// Gets the connection's handle.
        /// </summary>
        public readonly SafeHandle Handle;

        /// <summary>
        /// Gets a value indicating whether the pipe is connected or not.
        /// </summary>
        public bool IsConnected => _streamWrapper.IsConnected;

        /// <summary>
        /// Invoked when the named pipe connection terminates.
        /// </summary>
        public event ConnectionEventHandler<TRead, TWrite> Disconnected;

        /// <summary>
        /// Invoked whenever a message is received from the other end of the pipe.
        /// </summary>
        public event ConnectionMessageEventHandler<TRead, TWrite> ReceiveMessage;

        /// <summary>
        /// Invoked when an exception is thrown during any read/write operation over the named pipe.
        /// </summary>
        public event ConnectionExceptionEventHandler<TRead, TWrite> Error;

        private readonly PipeStreamWrapper<TRead, TWrite> _streamWrapper;

        private readonly AutoResetEvent _writeSignal = new AutoResetEvent(false);
        private readonly Queue<TWrite> _writeQueue = new Queue<TWrite>();

        private bool _notifiedSucceeded;

        internal NamedPipeConnection(int id, string name, PipeStream serverStream)
        {
            ID = id;
            Name = name;
            Handle = serverStream.SafePipeHandle;
            _streamWrapper = new PipeStreamWrapper<TRead, TWrite>(serverStream);
        }

        /// <summary>
        /// Begins reading from and writing to the
        /// named pipe on a background thread.
        /// </summary>
        /// <remarks>
        /// This method returns immediately.
        /// </remarks>
        internal void Open()
        {
            Worker readWorker = new Worker();
            readWorker.Succeeded += OnSucceeded;
            readWorker.Error += OnError;
            readWorker.DoWork(ReadPipe);

            Worker writeWorker = new Worker();
            writeWorker.Succeeded += OnSucceeded;
            writeWorker.Error += OnError;
            writeWorker.DoWork(WritePipe);
        }

        /// <summary>
        /// Adds the specified message to the write queue.
        /// </summary>
        /// <remarks>
        /// The message will be written to the named pipe by the
        /// background thread at the next available opportunity.
        /// </remarks>
        /// <param name="message">
        /// The message to write to the named pipe.
        /// </param>
        public void PushMessage(TWrite message)
        {
            _writeQueue.Enqueue(message);
            _writeSignal.Set();
        }

        /// <summary>
        /// Closes the named pipe connection and
        /// underlying <see cref="PipeStream"/>.
        /// </summary>
        /// <remarks>
        /// Invoked on the background thread.
        /// </remarks>
        internal void Close()
        {
            _streamWrapper.Close();
            _writeSignal.Set();
        }

        /// <summary>
        /// Invoked on the UI thread.
        /// </summary>
        private void OnSucceeded()
        {
            // Only notify observers once
            if (_notifiedSucceeded)
                return;

            _notifiedSucceeded = true;

            Disconnected?.Invoke(this);
        }

        /// <summary>
        /// Invoked on the UI thread.
        /// </summary>
        /// <param name="exception"></param>
        private void OnError(Exception exception)
        {
            Error?.Invoke(this, exception);
        }

        /// <summary>
        /// Invoked on the background thread.
        /// </summary>
        /// <exception cref="SerializationException"/>
        private void ReadPipe()
        {
            while (IsConnected && _streamWrapper.CanRead)
            {
                TRead obj = _streamWrapper.ReadObject();
                if (obj == null)
                {
                    Close();
                    return;
                }
                ReceiveMessage?.Invoke(this, obj);
            }
        }

        /// <summary>
        /// Invoked on the background thread.
        /// </summary>
        /// <exception cref="SerializationException"/>
        private void WritePipe()
        {
            while (IsConnected && _streamWrapper.CanWrite)
            {
                _writeSignal.WaitOne();
                while (_writeQueue.Count > 0)
                {
                    _streamWrapper.WriteObject(_writeQueue.Dequeue());
                    _streamWrapper.WaitForPipeDrain();
                }
            }
        }
    }

    internal static class ConnectionFactory
    {
        private static int _lastId;

        public static NamedPipeConnection<TRead, TWrite> CreateConnection<TRead, TWrite>(PipeStream pipeStream)
            where TRead : class
            where TWrite : class
        {
            return new NamedPipeConnection<TRead, TWrite>(++_lastId, $"Client {_lastId}", pipeStream);
        }
    }

    /// <summary>
    /// Handles new connections.
    /// </summary>
    /// <param name="connection">
    /// The newly established connection.
    /// </param>
    /// <typeparam name="TRead">
    /// The reference type used when reading from the named pipe.
    /// </typeparam>
    /// <typeparam name="TWrite">
    /// The reference type used when writing to the named pipe.
    /// </typeparam>
    public delegate void ConnectionEventHandler<TRead, TWrite>(NamedPipeConnection<TRead, TWrite> connection)
        where TRead : class
        where TWrite : class;

    /// <summary>
    /// Handles messages received from a named pipe.
    /// </summary>
    /// <typeparam name="TRead">
    /// The reference type used when reading from the named pipe.
    /// </typeparam>
    /// <typeparam name="TWrite">
    /// The reference type used when writing to the named pipe.
    /// </typeparam>
    /// <param name="connection">
    /// The connection that received the message.
    /// </param>
    /// <param name="message">
    /// The message sent by the other end of the pipe.
    /// </param>
    public delegate void ConnectionMessageEventHandler<TRead, TWrite>(NamedPipeConnection<TRead, TWrite> connection, TRead message)
        where TRead : class
        where TWrite : class;

    /// <summary>
    /// Handles exceptions thrown during read/write operations.
    /// </summary>
    /// <typeparam name="TRead">
    /// The reference type used when reading from the named pipe.
    /// </typeparam>
    /// <typeparam name="TWrite">
    /// The reference type used when writing to the named pipe.
    /// </typeparam>
    /// <param name="connection">
    /// The connection that threw the exception.
    /// </param>
    /// <param name="exception">
    /// The exception that was thrown.
    /// </param>
    public delegate void ConnectionExceptionEventHandler<TRead, TWrite>(NamedPipeConnection<TRead, TWrite> connection, Exception exception)
        where TRead : class
        where TWrite : class;
}
