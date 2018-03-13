using marketplace.api.Data;
using marketplace.api.Products;
using marketplace.api.Security;
using Marketplace.Api.Security;
using MarketPlace.Api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace marketplace.api.test
{
    [TestClass]
    public class ProductsControllerSpec
    {
        private ProductsController productsController;

        private Mock<AuthorizationService> authorizationServiceMock;
        private Mock<MarketplaceContext> contextMock;
        private Mock<QueryHelper> queryHelperMock;

        [TestInitialize]
        public void Init()
        {
            authorizationServiceMock = new Mock<AuthorizationService>();
            contextMock = new Mock<MarketplaceContext>();
            queryHelperMock = new Mock<QueryHelper>();

            productsController = new ProductsController(
                authorizationServiceMock.Object,
                contextMock.Object,
                queryHelperMock.Object
            );
        }

        [TestMethod]
        public void GivenAnEntity_WhenSubmittingAProductAsTheEntityOwner_TheProductIsAddedToTheEntityProducts()
        {
            var fakeEntityId = 2;
            var fakeProduct = new Product { };
            var productSetMock = new Mock<DbSet<Product>>();
            authorizationServiceMock.Setup(r => r.HasRole(fakeEntityId, EntityRole.Owner)).Returns(true);
            contextMock.SetupGet(r => r.Products).Returns(productSetMock.Object);

            var result = productsController.AddProduct(fakeEntityId, fakeProduct);

            Assert.IsInstanceOfType(result, typeof(CreatedResult));
            productSetMock.Verify(r => r.Add(fakeProduct), Times.Once);
            contextMock.Verify(r => r.SaveChanges());
        }

        [TestMethod]
        public void GivenAnEntity_WhenSubmittingAProductAsANonOwner_AnErrorIsReturned()
        {
            var fakeEntityId = 2;
            var fakeProduct = new Product { };
            authorizationServiceMock.Setup(r => r.HasRole(fakeEntityId, EntityRole.Owner)).Returns(false);

            var result = productsController.AddProduct(fakeEntityId, fakeProduct);

            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
        }

        [TestMethod]
        public void GivenAnEntity_WhenSubmittingAnInvalidProduct_AnErrorIsReturned()
        {
            var fakeEntityId = 2;
            var invalidProduct = new Product { };
            authorizationServiceMock.Setup(r => r.HasRole(It.IsAny<int>(), EntityRole.Owner)).Returns(true);
            productsController.ModelState.AddModelError("Id", "invalid bla bla");

            var result = productsController.AddProduct(fakeEntityId, invalidProduct);

            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }

        [TestMethod]
        public void GivenAnEntity_WhenQueryingTheAssociatedProducts_APagedListOfTheProductsisReturned()
        {
            var fakeProducts = new[] {
                new Product { Id = 1, EntityId = 1},
                new Product { Id = 2, EntityId = 2},
                new Product { Id = 3, EntityId = 1},
                new Product { Id = 4, EntityId = 2}
            };
            var fakeEntities = new[]
            {
                new Entity { Id = 1 }
            };
            var fakePage = new PagedList<Product> { Items = fakeProducts.Where(p => p.EntityId == 2) };
            contextMock.SetupGet(r => r.Entities).Returns(fakeEntities.ToDbSet());
            queryHelperMock.Setup(q => q.Query(It.IsAny<IQueryable<Product>>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Expression<Func<Product, bool>>>())).Returns(fakePage);

            var result = productsController.Query(1);

            Assert.IsInstanceOfType(result, typeof(JsonResult));
            var jsonResult = result as JsonResult;
            Assert.IsInstanceOfType(jsonResult.Value, typeof(PagedList<Product>));
            var model = jsonResult.Value as PagedList<Product>;
            Assert.AreEqual(fakePage, model);
        }

        [TestMethod]
        public void GivenAnInvalidEntity_WhenQueryingTheAssociatedProducts_AnErrorIsReturned()
        {
            var fakeEntities = new[]
            {
                new Entity { Id = 0 }
            };
            var entitiesMock = new Mock<Entity>();
            contextMock.SetupGet(r => r.Entities).Returns(fakeEntities.ToDbSet());

            var result = productsController.Query(1);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void GivenAProduct_WhenUpdatingAsOwner_TheProductIsUpdated()
        {
            var entityId = 1;
            var update = new Product
            {
                Name = "NewName",
                Description = "NewDescription",
                Price = 7m,
                ImageId = 5
            };
            var fakeProduct = new Product
            {
                Id = 1,
                Name = "OldName",
                Description = "OldDescription",
                Price = null,
                ImageId = 1,
                EntityId = entityId
            };
            var fakeProducts = Enumerable.Range(0, 200)
                .Select(id => id == 1 ? fakeProduct : new Product { Id = id });
            contextMock.SetupGet(r => r.Products).Returns(fakeProducts.ToDbSet());
            authorizationServiceMock.Setup(r => r.HasRole(entityId, EntityRole.Owner)).Returns(true);

            var response = productsController.UpdateProduct(1, update);

            Assert.IsInstanceOfType(response, typeof(OkResult));
            Assert.AreEqual(update.Name, fakeProduct.Name);
            Assert.AreEqual(update.Description, fakeProduct.Description);
            Assert.AreEqual(update.Price, fakeProduct.Price);
            Assert.AreEqual(update.ImageId, fakeProduct.ImageId);
            contextMock.Verify(r => r.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void GivenAProduct_WhenUpdatingAsNonOwner_AnErrorIsReturned()
        {
            var entityId = 1;
            var update = new Product
            {};
            var fakeProduct = new Product
            {
                Id = 1,
                EntityId = entityId
            };
            var fakeProducts = Enumerable.Range(0, 200)
                .Select(id => id == 1 ? fakeProduct : new Product { Id = id });
            contextMock.SetupGet(r => r.Products).Returns(fakeProducts.ToDbSet());
            authorizationServiceMock.Setup(r => r.HasRole(entityId, EntityRole.Owner)).Returns(false);

            var response = productsController.UpdateProduct(1, update);

            Assert.IsInstanceOfType(response, typeof(UnauthorizedResult));
            contextMock.Verify(r => r.SaveChanges(), Times.Never);
        }

        [TestMethod]
        public void GivenANonExistentProduct_WhenUpdating_AnErrorIsReturned()
        {
            var entityId = 1;
            var update = new Product
            { };
            var fakeProduct = new Product
            {
                Id = 1,
                EntityId = entityId
            };
            var fakeProducts = Enumerable.Range(0, 200)
                .Select(id => id == 1 ? fakeProduct : new Product { Id = id });
            contextMock.SetupGet(r => r.Products).Returns(fakeProducts.ToDbSet());

            var response = productsController.UpdateProduct(500, update);

            Assert.IsInstanceOfType(response, typeof(NotFoundResult));
            contextMock.Verify(r => r.SaveChanges(), Times.Never);
        }

        [TestMethod]
        public void GivenAProduct_WhenDeletingAsAnAdmin_TheProductIsMarkedAsDeleted()
        {
            var entityId = 5;
            var fakeProduct = new Product { Id = 1, Status = ProductStatus.Active, EntityId = entityId };
            var fakeProducts = Enumerable.Range(0, 200)
                .Select(id => id == 1 ? fakeProduct : new Product { Id = id });
            contextMock.SetupGet(r => r.Products).Returns(fakeProducts.ToDbSet());
            authorizationServiceMock.Setup(r => r.HasRole(GlobalRole.Admin)).Returns(true);

            var response = productsController.DeleteProduct(1);

            Assert.IsInstanceOfType(response, typeof(OkResult));
            contextMock.Verify(r => r.SaveChanges(), Times.Once);
            Assert.AreEqual(ProductStatus.Deleted, fakeProduct.Status);
        }

        [TestMethod]
        public void GivenAProduct_WhenDeletingAsTheEntityOwner_TheProductIsMarkedAsDeleted()
        {
            var entityId = 5;
            var fakeProduct = new Product { Id = 1, Status = ProductStatus.Active, EntityId = entityId };
            var fakeProducts = Enumerable.Range(0, 200)
                .Select(id => id == 1 ? fakeProduct : new Product { Id = id });
            contextMock.SetupGet(r => r.Products).Returns(fakeProducts.ToDbSet());
            authorizationServiceMock.Setup(r => r.HasRole(entityId, EntityRole.Owner)).Returns(true);

            var response = productsController.DeleteProduct(1);

            Assert.IsInstanceOfType(response, typeof(OkResult));
            contextMock.Verify(r => r.SaveChanges(), Times.Once);
            Assert.AreEqual(ProductStatus.Deleted, fakeProduct.Status);
        }

        [TestMethod]
        public void GivenAProduct_WhenDeletingAsANonAdminNonEntityOwner_AnErrorIsReturned()
        {
            var entityId = 5;
            var fakeProduct = new Product { Id = 1, Status = ProductStatus.Active, EntityId = entityId };
            var fakeProducts = Enumerable.Range(0, 200)
                .Select(id => id == 1 ? fakeProduct : new Product { Id = id });
            contextMock.SetupGet(r => r.Products).Returns(fakeProducts.ToDbSet());
            authorizationServiceMock.Setup(r => r.HasRole(entityId, EntityRole.Owner)).Returns(false);

            var response = productsController.DeleteProduct(1);

            Assert.IsInstanceOfType(response, typeof(UnauthorizedResult));
        }

        [TestMethod]
        public void GivenAListOfProducts_WhenQueryingAcrossEntities_APagedListOfProductsIsReturned()
        {
            var fakeProducts = new[] {
                new Product { Id = 1, EntityId = 1},
                new Product { Id = 2, EntityId = 2},
                new Product { Id = 3, EntityId = 1},
                new Product { Id = 4, EntityId = 2}
            };
            var fakePage = new PagedList<Product> { Items = fakeProducts };
            queryHelperMock.Setup(q => q.Query(It.IsAny<IQueryable<Product>>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Expression<Func<Product, bool>>>())).Returns(fakePage);

            var result = productsController.Query();

            Assert.IsInstanceOfType(result, typeof(JsonResult));
            var jsonResult = result as JsonResult;
            Assert.IsInstanceOfType(jsonResult.Value, typeof(PagedList<Product>));
            var model = jsonResult.Value as PagedList<Product>;
            Assert.AreEqual(fakePage, model);
        }
    }
}
