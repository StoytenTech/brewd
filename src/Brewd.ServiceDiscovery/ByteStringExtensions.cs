using System;

namespace Brewd.ServiceDiscovery
{
    public static class ByteStringExtensions
    {
        /// <summary>
        /// Convert C-style 0-terminated character array to string
        /// </summary>
        public static string ToStringWithoutTerminator(this char[] input)
        {
            var s = new string(input);
            return s.Substring(0, Math.Max(0, s.IndexOf('\0')));
        }
    }
}