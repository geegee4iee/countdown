using System;
using System.Collections.Generic;
using System.Text;

namespace Countdown.ML.Core
{
    public interface ILearningInputBuilder<ItemType>: IBaseInputBuilder<ItemType>
    {
        double[][] GetX();
        int[] GetY();
        string[] GetFeatureSchema();
    }
}
