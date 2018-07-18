using System;
using System.Collections.Generic;
using System.Text;

namespace Countdown.ML.Core
{
    public interface IPredictedInputBuilder<InputType>: IBaseInputBuilder<InputType>
    {
        void ImportFeatureSchema(string[] featureSchema);
        double[][] GetX();
    }
}
