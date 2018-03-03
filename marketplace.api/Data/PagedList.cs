using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace marketplace.api.Data
{
    public class PagedList<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int Page { get; set; }
        public int Pages { get; set; }
        public int Count { get; set; }
    }
}
