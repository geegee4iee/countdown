using Countdown.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Countdown.Core.Infrastructure
{
    public interface IRepository<TEntity> where TEntity : EntityModel
    {
    }
}
