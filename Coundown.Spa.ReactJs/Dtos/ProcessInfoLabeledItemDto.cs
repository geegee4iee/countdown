using Countdown.Core.MachineLearning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coundown.Spa.ReactJs.Dtos
{
    public class ProcessInfoLabeledItemDto
    {
        public string Title { get; set; }
        public string Process { get; set; }
        public Karma Category { get; set; }
        public int Seconds { get; set; }
    }
}
