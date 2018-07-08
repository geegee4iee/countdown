using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coundown.Spa.ReactJs.Dtos
{
    public class ProcessInfoDto
    {
        public int Seconds { get; set; }
        public string WindowTitle { get; set; }
        public string ProcessName { get; set; }
    }
}
