using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coundown.Spa.ReactJs.Dtos
{
    public class AppUsageRecordDto
    {
        public DateTime Date { get; set; }
        public IEnumerable<ProcessInfoDto> ActiveApps { get; set; }
    }
}
