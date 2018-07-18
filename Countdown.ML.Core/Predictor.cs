using Accord.IO;
using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Statistics.Kernels;
using Countdown.Core.MachineLearning;
using Countdown.Core.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Countdown.ML.Core
{
    public class KarmaPredictor
    {
        string[] _featureSchema;
        MulticlassSupportVectorMachine<IKernel> _trainedModel;
        private const string DataFolder = "TrainedModels";
        private const string ModelFileName = "multiclass_svm.bin";
        private const string FeatureSchemaFileName = "feature_schema.bin";

        public void TrainModel(IEnumerable<ProcessInfoLabeledItem> items)
        {
            if (_trainedModel != null) return;

            ILearningInputBuilder<ProcessInfoLabeledItem> learningInputBuilder = new ProcessInfoLearningInputBuilder();

            learningInputBuilder.Build(items);

            var smo = new MulticlassSupportVectorLearning<IKernel>()
            {
                Learner = (param) => new SequentialMinimalOptimization<IKernel>()
                {
                    Complexity = 1,
                    Kernel = new Polynomial(2, 1),
                }
            };

            _featureSchema = learningInputBuilder.GetFeatureSchema();
            _trainedModel = smo.Learn(learningInputBuilder.GetX(), learningInputBuilder.GetY());
        }

        public void SaveModel()
        {
            _trainedModel.Save(Path.Combine(GetModelFolderPath(), ModelFileName));
            _featureSchema.Save(Path.Combine(GetModelFolderPath(), FeatureSchemaFileName));
        }

        public void LoadModel()
        {
            _trainedModel = Serializer.Load<MulticlassSupportVectorMachine<IKernel>>(Path.Combine(GetModelFolderPath(), ModelFileName));
            _featureSchema = Serializer.Load<string[]>(Path.Combine(GetModelFolderPath(), FeatureSchemaFileName));
        }

        public Karma Predict(ProcessInfo processInfo)
        {
            return Predict(new[] { processInfo })[0];
        }

        public Karma[] Predict(ProcessInfo[] processInfoCollection)
        {
            if (_trainedModel == null)
            {
                LoadModel();
            }

            IPredictedInputBuilder<ProcessInfo> inputBuilder = new ProcessInfoPredictedInputBuilder();
            inputBuilder.ImportFeatureSchema(_featureSchema);
            inputBuilder.Build(processInfoCollection);

            int[] results = _trainedModel.Decide(inputBuilder.GetX());

            return results.Select(s => (Karma)s).ToArray();
        }

        public bool IsModelTrained()
        {
            return File.Exists(Path.Combine(GetModelFolderPath(), ModelFileName)) && File.Exists(Path.Combine(GetModelFolderPath(), FeatureSchemaFileName));
        }

        private string GetModelFolderPath()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), DataFolder);
        }
    }
}
