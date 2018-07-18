using System;
using System.Collections.Generic;
using System.Text;

namespace Countdown.Core.Infrastructure
{
    public interface IHashAlgorithm
    {
        byte[] ComputeHash(byte[] input);
    }
}
