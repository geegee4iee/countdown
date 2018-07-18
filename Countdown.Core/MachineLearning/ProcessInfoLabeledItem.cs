using Countdown.Core.Infrastructure;
using Countdown.Core.Models;
using Countdown.Core.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Countdown.Core.MachineLearning
{
    public class ProcessInfoLabeledItem: EntityModel
    {
        public string Title { get; set; }
        public string Process { get; set; }
        public Karma Category { get; set; }

        public string GenerateId(IHashAlgorithm hashAlgorithm)
        {
            Debug.Assert(hashAlgorithm != null);

            this.Id = hashAlgorithm.ComputeHash(Encoding.Unicode.GetBytes(this.Process + this.Title)).ToHex();

            return this.Id;
        }
    }
}
