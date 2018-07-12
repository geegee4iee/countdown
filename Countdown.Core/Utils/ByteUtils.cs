using System;
using System.Collections.Generic;
using System.Text;

namespace Countdown.Core.Utils
{
    public static class ByteUtils
    {
        public static string ToHex(this byte[] bytes, bool upperCase = false)
        {
            StringBuilder result = new StringBuilder(bytes.Length * 2);

            for (int i = 0; i < bytes.Length; i++)
                result.Append(bytes[i].ToString(upperCase ? "X2" : "x2"));

            return result.ToString();
        }
    }
}
