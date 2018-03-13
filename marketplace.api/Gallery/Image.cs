using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace marketplace.api.Gallery
{
    public abstract class Image
    {
        public int Id { get; set; }
        public string FileName { get; set; }
    }
}
