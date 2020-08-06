using Shared;

using System;
using System.IO.Pipes;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;

namespace Server
{
    class Program
    {
        public static byte[] Buffer = new byte[1024];

        static void Main(string[] args)
        {
            var ps = new PipeSecurity();
            var everyone = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
            ps.AddAccessRule(new PipeAccessRule(everyone, PipeAccessRights.FullControl, AccessControlType.Allow));

            while (true)
            {
                var pipe = new NamedPipeServerStream("testpipe", PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Message, PipeOptions.Asynchronous, 1024, 1024, ps);
                pipe.BeginWaitForConnection(new AsyncCallback(ConnectCallBack), pipe);

                Thread.Sleep(1000);
            }
        }

        private static void ConnectCallBack(IAsyncResult ar)
        {
            var pipe = ar.AsyncState as NamedPipeServerStream;
            pipe.EndWaitForConnection(ar);
            pipe.BeginRead(Buffer, 0, Buffer.Length, new AsyncCallback(ReadCallBack), pipe);
        }

        private static void ReadCallBack(IAsyncResult ar)
        {
            var pipe = ar.AsyncState as NamedPipeServerStream;
            var bytesRead = pipe.EndRead(ar);

            if (bytesRead > 0)
            {
                Console.WriteLine(Encoding.UTF8.GetString(Buffer.TrimBytes()));

                var data = Encoding.UTF8.GetBytes("Pong");
                pipe.BeginWrite(data, 0, data.Length, new AsyncCallback(WriteCallBack), pipe);
            }
        }

        private static void WriteCallBack(IAsyncResult ar)
        {
            var pipe = ar.AsyncState as NamedPipeServerStream;

            pipe.EndWrite(ar);
            pipe.Close();
        }
    }
}