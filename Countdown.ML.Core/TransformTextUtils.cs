using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Countdown.ML.Core
{
    internal class TransformTextUtils
    {
        public static Func<string, string[]> Filter = w => Regex.Replace(w, @"[^a-zA-Z]", " ").Trim().ToLowerInvariant().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Where(d => d.Count() < 20).ToArray();

        public static double[][] TransformTextToX(string[][] words, string[] vocabulary)
        {
            double[][] x = new double[words.Length][];
            for (int i = 0; i < words.Length; i++)
            {
                double[] features = new double[vocabulary.Length];
                for (int j = 0; j < vocabulary.Length; j++)
                {
                    features[j] = words[i].Count(str => String.Equals(str, vocabulary[j]));
                }

                x[i] = features;
            }

            return x;
        }
    }
}
