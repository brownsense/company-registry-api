using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace marketplace.api.Data
{
    public class QueryHelper
    {
        public virtual PagedList<T> Query<T>(IQueryable<T> dataSet, int itemsPerPage, int page, Expression<Func<T, bool>> filter)
        {
            var allResults = dataSet.Where(filter);
            var count = allResults.Count();
            return new PagedList<T> {
                Count = count,
                Items = allResults.Skip(itemsPerPage * page).Take(itemsPerPage),
                Page = page,
                Pages = (int)Math.Ceiling((double)(count / (page + 1)))
            };
        }
    }
}
