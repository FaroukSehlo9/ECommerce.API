using ECommerce.Domain.Entities.Base;
using ECommerce.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.InfraStructure.Reposatories
{
    public class SpecificationEvaluator<TEntity> where TEntity : Auditable
    {
        public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery, ISpecification<TEntity> spec)
        {
            var query = inputQuery;

            if (spec.Criteria != null)
                query = query.Where(spec.Criteria);

            if (spec.IsPaginationEnabled)
                query = query.Skip(spec.Skip).Take(spec.Take);

            if (spec.OrderBy != null)
                query = query.OrderBy(spec.OrderBy);
            if (spec.OrderByDescending != null)
                query = query.OrderByDescending(spec.OrderByDescending);

            query = spec.Include.Aggregate(query, (currentQuery, IncludeExpression) => currentQuery.Include(IncludeExpression));


            return query;
        }
    }
}
