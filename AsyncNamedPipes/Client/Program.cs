using Shared;

using System;
using System.IO.Pipes;
using System.Text;
using System.Threading;

namespace Client
{
    class Program
    {
        public static byte[] Buffer = new byte[1024];

        static void Main(string[] args)
        {
            while (true)
            {
                var pipe = new NamedPipeClientStream(".", "testpipe", PipeDirection.InOut, PipeOptions.Asynchronous);
                pipe.Connect(5000);

                var data = Encoding.UTF8.GetBytes("Ping");

                pipe.BeginWrite(data, 0, data.Length, new AsyncCallback(WriteCallBack), pipe);

                Thread.Sleep(1000);
            }
        }

        private static void WriteCallBack(IAsyncResult ar)
        {
            var pipe = ar.AsyncState as NamedPipeClientStream;
            pipe.EndWrite(ar);
            pipe.BeginRead(Buffer, 0, Buffer.Length, new AsyncCallback(ReadCallBack), pipe);
        }

        private static void ReadCallBack(IAsyncResult ar)
        {
            var pipe = ar.AsyncState as NamedPipeClientStream;
            var bytesRead = pipe.EndRead(ar);

            if (bytesRead > 0)
            {
                Console.WriteLine(Encoding.UTF8.GetString(Buffer.TrimBytes()));
            }

            pipe.Close();
        }
    }
}