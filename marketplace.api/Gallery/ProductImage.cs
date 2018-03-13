using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace marketplace.api.Gallery
{
    public class ProductImage : Image
    {
        public int ProductId { get; set; }
    }
}
