using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using marketplace.api.Data;
using marketplace.api.Security;
using Marketplace.Api.Security;
using MarketPlace.Api.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace marketplace.api.Products
{
    [Produces("application/json")]
    [Route("api/Products")]
    public class ProductsController : Controller
    {
        private AuthorizationService authorizationService;
        private MarketplaceContext context;
        private QueryHelper queryHelper;

        public ProductsController(AuthorizationService authorizationService, MarketplaceContext context, QueryHelper queryHelper)
        {
            this.authorizationService = authorizationService;
            this.context = context;
            this.queryHelper = queryHelper;
        }

        [HttpPost("~/api/entities/{entityId}/products")]
        public ActionResult AddProduct(int entityId, Product product)
        {
            if(!authorizationService.HasRole(entityId, EntityRole.Owner))
            {
                return Unauthorized();
            }

            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            context.Products.Add(product);
            context.SaveChanges();
            return Created("", product);
        }

        [HttpGet("~/api/entities/{entityId}/products")]
        public ActionResult Query(int entityId)
        {
            if(!context.Entities.Any(r => r.Id == entityId))
            {
                return NotFound();
            }
            var products = queryHelper.Query(context.Products, 10, 0, r => r.EntityId == entityId);
            return Json(products);
        }

        [HttpGet("~/api/products/")]
        public ActionResult Query()
        {
            var products = queryHelper.Query(context.Products, 10, 0, r => true);
            return Json(products);
        }

        public ActionResult UpdateProduct(int productId, Product update)
        {
            var savedProduct = context.Products.SingleOrDefault(p => p.Id == productId);
            if(savedProduct == null)
            {
                return NotFound();
            }
            if(!authorizationService.HasRole(savedProduct.EntityId, EntityRole.Owner))
            {
                return Unauthorized();
            }

            savedProduct.Name = update.Name;
            savedProduct.Price = update.Price;
            savedProduct.ImageId = update.ImageId;
            savedProduct.Description = update.Description;
            context.SaveChanges();
            return Ok();
        }

        public ActionResult DeleteProduct(int productId)
        {
            var foundProduct = context.Products.SingleOrDefault(r => r.Id == productId);
            if(foundProduct == null)
            {
                throw new Exception("Product not found");
            }

            if (!authorizationService.HasRole(GlobalRole.Admin) && !authorizationService.HasRole(foundProduct.EntityId, EntityRole.Owner))
            {
                return Unauthorized();
            }

            foundProduct.Status = ProductStatus.Deleted;
            context.SaveChanges();
            return Ok();
        }
    }
}