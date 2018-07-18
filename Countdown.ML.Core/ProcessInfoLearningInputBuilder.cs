using Countdown.Core.MachineLearning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Countdown.ML.Core
{
    public class ProcessInfoLearningInputBuilder : ILearningInputBuilder<ProcessInfoLabeledItem>
    {
        private readonly Func<string, string[]> _filter = TransformTextUtils.Filter;

        private string[] _vocabulary;

        private double[][] _x;
        private int[] _y;

        public void Build(IEnumerable<ProcessInfoLabeledItem> items)
        {
            var words = items.Select(d => d.Title + " " + d.Process).Select(_filter).ToArray();

            if (_vocabulary == null)
            {
                _vocabulary = items.Select(d => d.Title + " " + d.Process).SelectMany(_filter).GroupBy(str => str).Where(grp => grp.Count() > 2).Select(grp => grp.Key).Take(2000).OrderBy(str => str).ToArray();
            }

            _x = TransformTextUtils.TransformTextToX(words, _vocabulary);
            _y = items.Select(d => (int)d.Category).ToArray();
        }

        public string[] GetFeatureSchema()
        {
            return _vocabulary;
        }

        public double[][] GetX()
        {
            return _x;
        }

        public int[] GetY()
        {
            return _y;
        }
    }
}
