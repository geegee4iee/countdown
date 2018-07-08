using Countdown.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Countdown.Core.Infrastructure
{
    public interface IRepository<TEntity> where TEntity : EntityModel
    {
        void Add(TEntity entity);
        Task AddAsync(TEntity entity);

        TEntity Get(object id);
        Task<TEntity> GetAsync(object id);

        void Update(TEntity entity, object id);
        Task UpdateAsync(TEntity entity, object id);
    }
}
