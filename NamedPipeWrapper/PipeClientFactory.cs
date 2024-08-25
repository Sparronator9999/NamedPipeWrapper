using System;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Threading;
using NamedPipeWrapper.IO;

namespace NamedPipeWrapper
{
    internal static class PipeClientFactory
    {
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool WaitNamedPipe(string name, int timeout);

        public static bool NamedPipeExists(string pipeName)
        {
            try
            {
                bool exists = WaitNamedPipe(pipeName, -1);
                if (!exists)
                {
                    int error = Marshal.GetLastWin32Error();
                    if (error == 0 || error == 2)
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static PipeStreamWrapper<TRead, TWrite> Connect<TRead, TWrite>(string pipeName)
            where TRead : class
            where TWrite : class
        {
            return new PipeStreamWrapper<TRead, TWrite>(CreateAndConnectPipe(pipeName));
        }

        public static NamedPipeClientStream CreateAndConnectPipe(string pipeName, int timeout = 10)
        {
            string normalizedPath = Path.GetFullPath($"\\\\.\\pipe\\{pipeName}");
            while (!NamedPipeExists(normalizedPath))
            {
                Thread.Sleep(timeout);
            }
            NamedPipeClientStream pipe = CreatePipe(pipeName);
            pipe.Connect(1000);
            return pipe;
        }

        private static NamedPipeClientStream CreatePipe(string pipeName)
        {
            return new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.Asynchronous | PipeOptions.WriteThrough);
        }
    }
}
