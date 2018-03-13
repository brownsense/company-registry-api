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

namespace marketplace.api.Gallery
{
    [Produces("application/json")]
    [Route("api/Galley")]
    public class GalleyController : Controller
    {
        private MarketplaceContext marketplaceContext;
        private ImageService imageService;
        private AuthorizationService authorizationService;
        private QueryHelper queryHelper;

        public GalleyController(
            MarketplaceContext marketplaceContext,
            ImageService imageService,
            AuthorizationService authorizationService,
            QueryHelper queryHelper)
        {
            this.marketplaceContext = marketplaceContext;
            this.imageService = imageService;
            this.authorizationService = authorizationService;
            this.queryHelper = queryHelper;
        }

        public ActionResult Delete(int imageId)
        {
            var foundImage = marketplaceContext.Images.SingleOrDefault(r => r.Id == imageId);

            if(foundImage == null)
            {
                return NotFound();
            }

            switch(foundImage)
            {
                case EntityImage img:
                    {
                        if (!authorizationService.HasRole(img.EntityId, EntityRole.Owner) && !authorizationService.HasRole(GlobalRole.Admin))
                            return Unauthorized();
                        break;
                    }
                case ProductImage img:
                    {
                        var productForImage = marketplaceContext.Products.Single(r => r.Id == img.ProductId);
                        if (!authorizationService.HasRole(productForImage.EntityId, EntityRole.Owner) && !authorizationService.HasRole(GlobalRole.Admin))
                            return Unauthorized();
                        break;
                    }
                default: throw new Exception("Yikes");
            }

            marketplaceContext.Images.Remove(foundImage);
            marketplaceContext.SaveChanges();
            imageService.DeleteImage(imageId);
            return Ok();
        }

        public ActionResult GetGalleryForEntity(int entityId)
        {
            var imagesForProduct = marketplaceContext.Images.Where(r => (r as EntityImage).EntityId == entityId);
            var pagedList = queryHelper.Query(imagesForProduct, 10, 1, i => true);
            return Json(pagedList);
        }

        public ActionResult GetGalleryForProduct(int productId)
        {
            var imagesForProduct = marketplaceContext.Images.Where(r => (r as ProductImage).ProductId == productId);
            var pagedList = queryHelper.Query(imagesForProduct, 10, 1, i => true);
            return Json(pagedList);
        }

        public ActionResult GetImage(int imageId)
        {
            var foundImage = marketplaceContext.Images.Single(r => r.Id == imageId);

            return Json(foundImage);
        }
    }
}