using Shared;

using System;
using System.Globalization;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        private static NamedPipeClientStream _pipeClient; 
        private static ManualResetEvent _signal = new(false);
        
        static void Main(string[] args)
        {
            _pipeClient = new NamedPipeClientStream(".", "TestPipe", PipeDirection.InOut, PipeOptions.Asynchronous);
            _pipeClient.Connect(5000);
            
            // read task
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    _signal.Reset();

                    var state = new PipeState();
                    _pipeClient.BeginRead(state.TempBuffer, 0, state.TempBuffer.Length, ReadCallBack, state);

                    _signal.WaitOne();
                }
            });
            
            while (true)
            {
                var message = $"Hello from client: {DateTime.UtcNow}";
                var data = Encoding.UTF8.GetBytes(message);
                
                _pipeClient.BeginWrite(data, 0, data.Length, WriteCallBack, null);

                Thread.Sleep(3000);
            }
        }

        private static void WriteCallBack(IAsyncResult ar)
        {
            _pipeClient.EndWrite(ar);
        }

        private static void ReadCallBack(IAsyncResult ar)
        {
            var state = (PipeState)ar.AsyncState;
            var bytesRead = _pipeClient.EndRead(ar);

            if (bytesRead > 0)
            {
                if (state.FinalBuffer is null) state.SetFinal(bytesRead);
                else state.ResizeBuffer(bytesRead);
            }
            
            if (bytesRead == state.TempBuffer.Length)
                _pipeClient.BeginRead(state.TempBuffer, 0, state.TempBuffer.Length, ReadCallBack, state);

            if (state.FinalBuffer.Length == 0) return;
            
            var message = Encoding.UTF8.GetString(state.FinalBuffer);
            Console.WriteLine(message);

            _signal.Set();
        }
    }
}