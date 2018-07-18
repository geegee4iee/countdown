using Countdown.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Countdown.ML.Core
{
    public class ProcessInfoPredictedInputBuilder : IPredictedInputBuilder<ProcessInfo>
    {
        string[] _featureSchema;
        double[][] _x;

        public void Build(IEnumerable<ProcessInfo> inputs)
        {
            var testWordsOrigin = inputs.Select(d => d.MainWindowTitle + " " + d.ProcessName).ToArray();
            _x = TransformTextUtils.TransformTextToX(testWordsOrigin.Select(TransformTextUtils.Filter).ToArray(), _featureSchema);
        }

        public double[][] GetX()
        {
            return _x;
        }

        /// <summary>
        /// Must be called before calling Build
        /// </summary>
        /// <param name="featureSchema"></param>
        public void ImportFeatureSchema(string[] featureSchema)
        {
            _featureSchema = featureSchema;
        }

        
    }
}
