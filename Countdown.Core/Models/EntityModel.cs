using System;
using System.Collections.Generic;
using System.Text;

namespace Countdown.Core.Models
{
    [Serializable]
    public class EntityModel
    {
        private string _id;

        public string Id
        {
            get => _id;
            protected set => _id = value;
        }
    }
}
