using marketplace.api.Data;
using marketplace.api.Gallery;
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

namespace marketplace.api.test
{
    [TestClass]
    public class GalleySpec
    {
        private GalleyController controller;

        private Mock<MarketplaceContext> contextMock;
        private Mock<ImageService> imageServiceMock;
        private Mock<AuthorizationService> authorizationServiceMock;
        private Mock<QueryHelper> queryHelperMock;

        [TestInitialize]
        public void Init()
        {
            contextMock = new Mock<MarketplaceContext>();
            imageServiceMock = new Mock<ImageService>();
            authorizationServiceMock = new Mock<AuthorizationService>();
            queryHelperMock = new Mock<QueryHelper>();

            controller = new GalleyController(
                contextMock.Object,
                imageServiceMock.Object,
                authorizationServiceMock.Object,
                queryHelperMock.Object
            );
        }

        [TestMethod]
        public void GivenAnEntity_WhenSubmittingAnImage_TheImageIsAddedToTheEntityGallery()
        {
            
        }

        [TestMethod]
        public void GivenAProduct_WhenSubmittingAnImage_TheImageIsAddedToTheProductGalley()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void GivenAnEntity_WhenQueryingTheGallery_APagedListOfImagesIsReturned()
        {
            IEnumerable<Image> fakeImages = Enumerable.Range(0, 10)
                .Select(r => new EntityImage { Id = r, FileName = "H", EntityId = r % 2 });
            var fakePage = new PagedList<Image> { };
            contextMock.SetupGet(r => r.Images).Returns(fakeImages.ToDbSet());
            queryHelperMock.Setup(r => r.Query(
                    It.Is<IQueryable<Image>>(q => q.All(p => new[] { 1, 3, 5, 7, 9 }.Contains(p.Id))),
                    It.IsAny<int>(), It.IsAny<int>(),
                    It.IsAny<Expression<Func<Image, bool>>>()
                )).Returns(fakePage);

            var result = controller.GetGalleryForEntity(1);

            Assert.IsInstanceOfType(result, typeof(JsonResult));
        }

        [TestMethod]
        public void GivenAProduct_WhenQueryingTheGallery_APagedListOfImagesIsReturned()
        {
            IEnumerable<Image> fakeImages = Enumerable.Range(0, 10)
                .Select(r => new ProductImage { Id = r, FileName = "H", ProductId = r % 2 });
            var fakePage = new PagedList<Image> { };
            contextMock.SetupGet(r => r.Images).Returns(fakeImages.ToDbSet());
            queryHelperMock.Setup(r => r.Query(
                    It.Is<IQueryable<Image>>(q => q.All(p => new[] {1,3,5,7,9}.Contains(p.Id))),
                    It.IsAny<int>(), It.IsAny<int>(),
                    It.IsAny<Expression<Func<Image, bool>>>()
                )).Returns(fakePage);

            var result = controller.GetGalleryForProduct(1);

            Assert.IsInstanceOfType(result, typeof(JsonResult));
            var jsonResult = result as JsonResult;
            Assert.IsInstanceOfType(jsonResult.Value, typeof(PagedList<Image>));
            var model = jsonResult.Value as PagedList<Image>;
            Assert.AreEqual(fakePage, model);
        }

        [TestMethod]
        public void GivenAnImageId_WhenGetting_TheImageIsReturned()
        {
            var imageId = 5;
            IEnumerable<Image> fakeImages = Enumerable.Range(0, 100)
                .Select(r => new EntityImage { Id = r });
            contextMock.SetupGet(r => r.Images).Returns(fakeImages.ToDbSet());

            var response = controller.GetImage(imageId);

            Assert.IsInstanceOfType(response, typeof(JsonResult));
            var jsonResult = response as JsonResult;
            var model = jsonResult.Value;
            Assert.IsInstanceOfType(model, typeof(Image));
            Assert.AreEqual(imageId, (model as Image).Id);
        }

        [TestMethod]
        public void GivenAGalleyImage_WhenDeletingByAnAdmin_TheGalleryImageIsMarkedAsDeleted()
        {
            var imageId = 5;
            IEnumerable<Image> fakeImages = Enumerable.Range(0, 100)
                .Select(r => new EntityImage { Id = r, FileName = "H", EntityId = 2 });
            var imagesMock = fakeImages.ToDbSetMock();
            contextMock.SetupGet(r => r.Images).Returns(imagesMock.Object);
            authorizationServiceMock.Setup(r => r.HasRole(GlobalRole.Admin)).Returns(true);
            authorizationServiceMock.Setup(r => r.HasRole(2, EntityRole.Owner)).Returns(true);

            var response = controller.Delete(imageId);

            Assert.IsInstanceOfType(response, typeof(OkResult));
            imageServiceMock.Verify(r => r.DeleteImage(imageId), Times.Once);
            imagesMock.Verify(r => r.Remove(It.Is<Image>(p => p.Id == imageId)));
            contextMock.Verify(r => r.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void GivenAGalleryImage_WhenDeletingByOwner_TheGalleryImageIsMarkedAsDeleted()
        {
            var imageId = 5;
            IEnumerable<Image> fakeImages = Enumerable.Range(0, 100)
                .Select(r => new EntityImage { Id = r, FileName = "H", EntityId = 2 });
            var imagesMock = fakeImages.ToDbSetMock();
            contextMock.SetupGet(r => r.Images).Returns(imagesMock.Object);
            authorizationServiceMock.Setup(r => r.HasRole(GlobalRole.Admin)).Returns(false);
            authorizationServiceMock.Setup(r => r.HasRole(2, EntityRole.Owner)).Returns(true);

            var response = controller.Delete(imageId);

            Assert.IsInstanceOfType(response, typeof(OkResult));
            imageServiceMock.Verify(r => r.DeleteImage(imageId), Times.Once);
            imagesMock.Verify(r => r.Remove(It.Is<EntityImage>(p => p.Id == 5)));
            contextMock.Verify(r => r.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void GivenAGalleryImage_WhenDeletedByNonAdminNonOwner_AnErrorIsReturned()
        {
            var imageId = 5;
            IEnumerable<Image> fakeImages = Enumerable.Range(0, 100)
                .Select(r => new EntityImage { Id = r, FileName = "H", EntityId = 2 });
            var imagesMock = fakeImages.ToDbSetMock();
            contextMock.SetupGet(r => r.Images).Returns(imagesMock.Object);
            authorizationServiceMock.Setup(r => r.HasRole(GlobalRole.Admin)).Returns(false);
            authorizationServiceMock.Setup(r => r.HasRole(2, EntityRole.Owner)).Returns(false);

            var response = controller.Delete(imageId);

            Assert.IsInstanceOfType(response, typeof(UnauthorizedResult));
            imageServiceMock.Verify(r => r.DeleteImage(imageId), Times.Never);
            imagesMock.Verify(r => r.Remove(It.IsAny<EntityImage>()), Times.Never);
            contextMock.Verify(r => r.SaveChanges(), Times.Never);
        }

        [TestMethod]
        public void GivenAnInvalidGalleryImage_WhenDeleting_AnErrorIsReturned()
        {
            var imageId = 500;
            IEnumerable<Image> fakeImages = Enumerable.Range(0, 100)
                .Select(r => new EntityImage { Id = r, FileName = "H", EntityId = 2 });
            var imagesMock = fakeImages.ToDbSetMock();
            contextMock.SetupGet(r => r.Images).Returns(imagesMock.Object);
            authorizationServiceMock.Setup(r => r.HasRole(GlobalRole.Admin)).Returns(true);
            authorizationServiceMock.Setup(r => r.HasRole(2, EntityRole.Owner)).Returns(true);

            var response = controller.Delete(imageId);

            Assert.IsInstanceOfType(response, typeof(NotFoundResult));
            imageServiceMock.Verify(r => r.DeleteImage(imageId), Times.Never);
            imagesMock.Verify(r => r.Remove(It.IsAny<Image>()), Times.Never);
            contextMock.Verify(r => r.SaveChanges(), Times.Never);
        }
    }
}
