using Countdown.Core.Infrastructure;
using Countdown.Core.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Countdown.Test
{
    
    public class MurmurHash3Tests
    {
        [Fact]
        public void hash_the_same_string_should_return_the_same_result()
        {
            var text = "!@#$@DSFSDAFS@#$@DSFA@#$";
            var hashFunc = new Murmur3Hash();
            var hashBytes1 = hashFunc.ComputeHash(Encoding.Unicode.GetBytes(text));
            var hashBytes2 = hashFunc.ComputeHash(Encoding.Unicode.GetBytes(text));
            var str1 = hashBytes1.ToHex();
            var str2 = hashBytes2.ToHex();

            Assert.Equal(str1, str2);
        }

        [Fact]
        public void hash_different_string_should_return_different_result()
        {
            var text1 = "$1";
            var text2 = "$2";

            var hashFunc = new Murmur3Hash();
            var hashBytes1 = hashFunc.ComputeHash(Encoding.Unicode.GetBytes(text1));
            var hashBytes2 = hashFunc.ComputeHash(Encoding.Unicode.GetBytes(text2));

            Assert.NotEqual(hashBytes1.ToHex(), hashBytes2.ToHex());
        }

        [Fact]
        public void hash_the_same_string_multiple_times_should_return_the_same_result()
        {
            var text = "!@#$@DSFSDAFS@#$@DSFA@#$";
            var hashFunc = new Murmur3Hash();
            var hashBytes1 = hashFunc.ComputeHash(Encoding.Unicode.GetBytes(text));

            for(int i = 0; i < 100000; i++)
            {
                var comparedHashBytes = hashFunc.ComputeHash(Encoding.Unicode.GetBytes(text));

                Assert.Equal(hashBytes1.ToHex(), comparedHashBytes.ToHex());
            }
        }
    }
}
