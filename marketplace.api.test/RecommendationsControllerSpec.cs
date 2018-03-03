using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace MarketPlace.Api.Entities.Test
{
    [TestClass]
    public class RecommendationsControllerSpec
    {
        private RecommendationsController controller;

        [TestInitialize]
        public void Init()
        {
        }

        [TestMethod]
        public void GivenAValidEntity_WhenRecommending_TheEntityIsRecommendedByTheUser()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void GivenAValidEntityWhichWasAlreadyRecommendedByTheUser_WhenRecommending_AnErrorIsReturned()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void GivenAValidEntityWhichWasAlreadyRecommendedByTheUser_WhenUnrecommending_TheEntityIsUnrecommended()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void GivenAValidEntityWhichWasNotAlreadyRecommendedByTheUser_WhenUnrecommending_AnErrorIsReturned()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void GivenAValidUser_WhenQueryingRecommendations_APagedListOfRecommendedEntitiesIsReturned()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void GivenAValidEntity_WhenQueryingRecommendations_APagedListOfRecommendingUsersIsReturned()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void GivenAValidEntity_WhenGettingTheRecommendationsSummary_TheNumberOfRecommendationsIsReturned()
        {
            Assert.Inconclusive();
        }
    }
}