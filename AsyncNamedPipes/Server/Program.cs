using Shared;

using System;
using System.IO.Pipes;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    internal class Program
    {
        private static NamedPipeServerStream _pipeServer;
        private static ManualResetEvent _signal = new(false);

        static void Main(string[] args)
        {
            var ps = new PipeSecurity();
            var everyone = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
            ps.AddAccessRule(new PipeAccessRule(everyone, PipeAccessRights.FullControl, AccessControlType.Allow));
            
            _pipeServer = new NamedPipeServerStream("TestPipe", PipeDirection.InOut,
                1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous,
                1024, 1024, ps);
            
            _pipeServer.BeginWaitForConnection(ConnectCallBack, null);
            Thread.Sleep(int.MaxValue);
        }

        private static void ConnectCallBack(IAsyncResult ar)
        {
            _pipeServer.EndWaitForConnection(ar);

            // read task
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    _signal.Reset();

                    var state = new PipeState();
                    _pipeServer.BeginRead(state.TempBuffer, 0, state.TempBuffer.Length, ReadCallBack, state);

                    _signal.WaitOne();
                }
            });

            while (true)
            {
                var message = $"Hello from server: {DateTime.UtcNow}";
                var data = Encoding.UTF8.GetBytes(message);

                _pipeServer.BeginWrite(data, 0, data.Length, WriteCallBack, null);
                Thread.Sleep(6000);
            }
        }

        private static void ReadCallBack(IAsyncResult ar)
        {
            var state = (PipeState)ar.AsyncState;
            var bytesRead = _pipeServer.EndRead(ar);

            if (bytesRead > 0)
            {
                if (state.FinalBuffer is null) state.SetFinal(bytesRead);
                else state.ResizeBuffer(bytesRead);
            }
            
            if (bytesRead == state.TempBuffer.Length)
                _pipeServer.BeginRead(state.TempBuffer, 0, state.TempBuffer.Length, ReadCallBack, state);
            
            if (state.FinalBuffer.Length == 0) return;
            
            var message = Encoding.UTF8.GetString(state.FinalBuffer);
            Console.WriteLine(message);

            _signal.Set();
        }

        private static void WriteCallBack(IAsyncResult ar)
        {
            _pipeServer.EndWrite(ar);
        }
    }
}