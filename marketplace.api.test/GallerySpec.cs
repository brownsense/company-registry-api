using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace marketplace.api.test
{
    [TestClass]
    public class GalleySpec
    {
        [TestMethod]
        public void GivenAnEntity_WhenSubmittingAnImage_TheImageIsAddedToTheEntityGallery()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void GivenAnEntity_WhenQueryingTheGallery_APagedListOfImagesIsReturned()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void GivenAGalleyImage_WhenDeletingByAnAdmin_TheGalleryImageIsMarkedAsDeleted()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void GivenAGalleryImage_WhenDeletingByOwner_TheGalleryImageIsMarkedAsDeleted()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void GivenAGalleryImage_WhenDeletedByNonAdminNonOwner_AnErrorIsReturned()
        {
            Assert.Inconclusive();
        }
    }
}
