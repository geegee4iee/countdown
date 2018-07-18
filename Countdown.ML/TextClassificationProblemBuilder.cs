using libsvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Countdown.ML
{
    class TextClassificationProblemBuilder
    {
        public Func<string, string[]> RefineText { get; set; }
        public svm_problem CreateProblem(IEnumerable<string> x, double[] y, IReadOnlyList<string> vocabulary)
        {
            return new svm_problem
            {
                y = y,
                x = x.Select(xVector => CreateNode(xVector, vocabulary, RefineText)).ToArray(),
                l = y.Length
            };
        }

        public static svm_node[] CreateNode(string x, IReadOnlyList<string> vocabulary, Func<string, string[]> textFilter = null)
        {
            var node = new List<svm_node>(vocabulary.Count);
            string[] words = null;
            if (textFilter != null)
            {
                words = textFilter(x);
            } else
            {
                words = x.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            }

            for (int i = 0; i < vocabulary.Count; i++)
            {
                int occurenceCount = words.Count(s => String.Equals(s, vocabulary[i], StringComparison.OrdinalIgnoreCase));
                if (occurenceCount == 0)
                    continue;

                node.Add(new svm_node
                {
                    index = i + 1,
                    value = occurenceCount
                });
            }

            return node.ToArray();
        }
    }
}
