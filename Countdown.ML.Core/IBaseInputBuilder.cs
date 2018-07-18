using System;
using System.Collections.Generic;
using System.Text;

namespace Countdown.ML.Core
{
    public interface IBaseInputBuilder<InputType>
    {
        void Build(IEnumerable<InputType> inputs);
    }
}
