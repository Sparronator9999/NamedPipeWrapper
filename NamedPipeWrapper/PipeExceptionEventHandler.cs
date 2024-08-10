using System;

namespace NamedPipeWrapper
{
    /// <summary>
    /// Handles exceptions thrown during a read
    /// or write operation on a named pipe.
    /// </summary>
    /// <param name="exception">
    /// The exception that was thrown.
    /// </param>
    public delegate void PipeExceptionEventHandler(Exception exception);
}
