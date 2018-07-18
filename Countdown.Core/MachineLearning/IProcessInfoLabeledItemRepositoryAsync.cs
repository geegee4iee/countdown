using Countdown.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Countdown.Core.MachineLearning
{
    public interface IProcessInfoLabeledItemRepositoryAsync: IRepository<ProcessInfoLabeledItem>
    {
        Task Add(ProcessInfoLabeledItem item);
    }
}
