using Countdown.Core.MachineLearning;
using Countdown.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coundown.Spa.ReactJs.Services
{
    public interface IPredictKarmaService
    {
        Karma Predict(ProcessInfo item);
        Karma[] Predict(ProcessInfo[] items);
        void TrainModel();
    }
}
