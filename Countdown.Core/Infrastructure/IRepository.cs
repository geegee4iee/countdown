using Countdown.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Countdown.Core.Infrastructure
{
    public interface IRepository<TEntity> where TEntity : EntityModel
    {
        void Add(TEntity entity);
        TEntity Get(object id);
        void Update(TEntity entity, object id);
    }
}
