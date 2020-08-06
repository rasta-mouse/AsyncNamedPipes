using System;

namespace Shared
{
    public static class Helpers
    {
        public static byte[] TrimBytes(this byte[] bytes)
        {
            var index = bytes.Length - 1;
            while (bytes[index] == 0) { index--; }
            byte[] copy = new byte[index + 1];
            Array.Copy(bytes, copy, index + 1);
            return copy;
        }
    }
}