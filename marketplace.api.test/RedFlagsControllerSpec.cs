using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace MarketPlace.Api.Entities.Test
{
    [TestClass]
    public class RedFlagsControllerSpec
    {
        private RedFlagsController controller;

        [TestInitialize]
        public void Init()
        {

        }

        [TestMethod]
        public void GivenAValidCompany_WhenSubmittingAValidRedFlag_TheFlagIsAccepted()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void GivenAValidCompany_WhenSubmittingAnInvalidRedFlag_AnErrorIsReturned()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void GivenAnAdminUser_WhenQueryingRedFlagsForACompany_APagedListOfTheProcessedAndPendingFlagsAreReturned()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void GivenANonAdminUser_WhenQueryingRedFlagsForAnEntity_APagedListOfOnlyProcessedFlagsAndPendingFlagsByTheUserAreReturned()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void GivenAnEntityOwner_WhenQueryingRedFlagsForThatEntity_APagedListOfTheProcessedAndPendingFlagsAreReturned()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void GivenAUser_WhenGettingAProcessedFlag_TheFlagIsReturned()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void GivenAnAdminUser_WhenGettingAnUnprocessedFlag_TheFlagIsReturned()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void GivenANonAdminUserWhoSubmittedTheFlag_WhenGettingTheUnprocessedFlag_TheFlagIsReturned()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void GivenAUserWhoOwnsTheEntity_WhenGettingAnUnprocessedFlagForThatEntity_TheFlagIsReturned()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void GivenANonAdminNonSubmitterNonEntityOwnerUser_WhenGettingAnUnprocessedFlag_AnErrorIsReturned()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void GivenAnAdminUser_WhenProcessingAFlag_TheFlagIsProcessed()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void GivenANonAdminUser_WhenProcessingAFlag_AnErrorIsReturned()
        {
            Assert.Inconclusive();
        }
    }
}
