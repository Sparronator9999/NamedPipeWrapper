using System;

namespace NamedPipeWrapper
{
    /// <summary>
    /// Provides data for the
    /// <see cref="NamedPipeClient{TRead, TWrite}.ServerMessage"/> and
    /// <see cref="NamedPipeServer{TRead, Write}.ClientMessage"/> events.
    /// </summary>
    /// <typeparam name="TRead">
    /// The reference type used when reading from the named pipe.
    /// </typeparam>
    /// <typeparam name="TWrite">
    /// The reference type used when writing to the named pipe.
    /// </typeparam>
    public class PipeMessageEventArgs<TRead, TWrite> : EventArgs
        where TRead : class
        where TWrite : class
    {
        /// <summary>
        /// The connection that sent the message.
        /// </summary>
        public NamedPipeConnection<TRead, TWrite> Connection { get; }

        /// <summary>
        /// The message sent by the other end of the pipe.
        /// </summary>
        public TRead Message { get; }

        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="PipeMessageEventArgs{TRead, TWrite}"/> class.
        /// </summary>
        /// <param name="connection">
        /// The connection that sent the message.
        /// </param>
        /// <param name="message">
        /// The message sent by the other end of the pipe.
        /// </param>
        internal PipeMessageEventArgs(
            NamedPipeConnection<TRead, TWrite> connection,
            TRead message)
        {
            Connection = connection;
            Message = message;
        }
    }
}
