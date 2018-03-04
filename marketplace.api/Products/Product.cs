using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace marketplace.api.Products
{
    public class Product
    {
        public int EntityId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal? Price { get; set; }
        public int ImageId { get; set; }
        public ProductStatus Status { get; set; }
        public ProductType ProductType { get; set; }
    }

    public enum ProductType
    {
        None = 0,
        PhysicalProduct,
        DigitalProduct,
        Service
    }

    public enum ProductStatus
    {
        None = 0,
        Active,
        Deleted
    }
}
