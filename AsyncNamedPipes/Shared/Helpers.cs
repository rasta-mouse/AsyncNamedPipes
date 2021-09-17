using System;

namespace Shared
{
    public class PipeState
    {
        public byte[] TempBuffer { get; }
        public byte[] FinalBuffer { get; set; }

        public PipeState()
        {
            TempBuffer = new byte[1024];
        }

        public void SetFinal(int bytesRead)
        {
            FinalBuffer = new byte[bytesRead];
            Array.Copy(TempBuffer, FinalBuffer, bytesRead);
        }

        public void ResizeBuffer(int bytesRead)
        {
            var currentData = FinalBuffer;
            var currentSize = FinalBuffer.Length;
                    
            Array.Resize(ref currentData, currentSize + bytesRead);
            Buffer.BlockCopy(TempBuffer, 0, currentData, currentSize, bytesRead);

            FinalBuffer = currentData;
        }
    }
}