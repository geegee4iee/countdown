using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Countdown.Core.Infrastructure;
using System;
using System.Security.Cryptography;

namespace Coundown.Benchmarks
{
    public class Md5VsSha256VsMurmur3
    {
        private const int N = 10000;
        private readonly byte[] data;

        private readonly SHA256 sha256 = SHA256.Create();
        private readonly MD5 md5 = MD5.Create();
        private readonly Murmur3Hash murmur3 = new Murmur3Hash();

        public Md5VsSha256VsMurmur3()
        {
            data = new byte[N];
            new Random(42).NextBytes(data);
        }

        [Benchmark]
        public byte[] Sha256() => sha256.ComputeHash(data);

        [Benchmark]
        public byte[] Md5() => md5.ComputeHash(data);

        [Benchmark]
        public byte[] Murmur3() => murmur3.ComputeHash(data);
    }

    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<Md5VsSha256VsMurmur3>();
        }
    }
}
