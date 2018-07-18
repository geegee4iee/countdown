using Countdown.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Countdown.Core.Infrastructure
{
    public abstract class Specification<T> where T: EntityModel
    {
        public abstract Expression<Func<T, bool>> ToExpression();

        public bool IsSatisfiedBy(T entity)
        {
            Func<T, bool> predicate = ToExpression().Compile();

            return predicate(entity);
        }
    }
}
