using marketplace.api.Data;
using marketplace.api.Recommendations;
using marketplace.api.test;
using Marketplace.Api.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace MarketPlace.Api.Entities.Test
{
    [TestClass]
    public class RecommendationsControllerSpec
    {
        private RecommendationsController controller;

        private Mock<MarketplaceContext> contextMock;
        private Mock<AuthorizationService> authorizationServiceMock;
        private Mock<QueryHelper> queryHelperMock;

        [TestInitialize]
        public void Init()
        {
            contextMock = new Mock<MarketplaceContext>();
            authorizationServiceMock = new Mock<AuthorizationService>();
            queryHelperMock = new Mock<QueryHelper>();

            controller = new RecommendationsController(
                contextMock.Object,
                authorizationServiceMock.Object,
                queryHelperMock.Object
            );
        }

        [TestMethod]
        public void GivenAValidEntity_WhenRecommending_TheEntityIsRecommendedByTheUser()
        {
            var rating = new Rating
            {
                RatingValue = Stars.Five,
                Comment = "Service of the highest quality"
            };
            var entityId = 5;
            var userId = 9;
            var fakeRatings = new Rating[0];
            var mock = fakeRatings.ToDbSetMock();
            contextMock.SetupGet(r => r.Ratings).Returns(mock.Object);
            authorizationServiceMock.Setup(r => r.GetUserId()).Returns(userId);

            var response = controller.Rate(entityId, rating);

            Assert.IsInstanceOfType(response, typeof(CreatedResult));
            mock.Verify(r => r.Add(rating), Times.Once);
            contextMock.Verify(r => r.SaveChanges());
            Assert.AreEqual(entityId, rating.Entity);
            Assert.AreEqual(userId, rating.RatedBy);
        }

        [TestMethod]
        public void GivenAValidEntityWhichWasAlreadyRecommendedByTheUser_WhenRecommending_TheRatingIsUpdated()
        {
            var rating = new Rating
            {
                RatingValue = Stars.Five,
                Comment = "Service of the highest quality"
            };
            var entityId = 5;
            var userId = 9;
            var existingRating = new Rating
            {
                Id = 3,
                RatingValue = Stars.Four,
                Comment = "Previous",
                Entity = entityId,
                RatedBy = userId
            };
            var fakeRatings = Enumerable.Range(0, 100)
                .Select(i => i == existingRating.Id ? existingRating : new Rating { Id = i });
            var mock = fakeRatings.ToDbSetMock();
            contextMock.SetupGet(r => r.Ratings).Returns(mock.Object);
            authorizationServiceMock.Setup(r => r.GetUserId()).Returns(userId);

            var response = controller.Rate(entityId, rating);

            Assert.IsInstanceOfType(response, typeof(OkResult));
            mock.Verify(r => r.Add(rating), Times.Never);
            contextMock.Verify(r => r.SaveChanges(), Times.Once);
            Assert.AreEqual(rating.RatingValue, existingRating.RatingValue);
            Assert.AreEqual(rating.Comment, existingRating.Comment);
        }

        [TestMethod]
        public void GivenAValidUser_WhenQueryingRecommendations_APagedListOfRecommendedEntitiesIsReturned()
        {
            var userId = 5;
            var fakePage = new PagedList<Rating> { };
            var fakeRatings = Enumerable.Range(0, 100).Select(i => new Rating { Id = i });
            queryHelperMock.Setup(r => r.Query(
                It.IsAny<IQueryable<Rating>>(), 10, 0,
                It.IsAny<Expression<Func<Rating, bool>>>()
            )).Returns(fakePage);
            contextMock.SetupGet(r => r.Ratings).Returns(fakeRatings.ToDbSet());

            var response = controller.QueryUserRatings(userId);

            Assert.IsInstanceOfType(response, typeof(JsonResult));
            var jsonResult = response as JsonResult;
            Assert.AreEqual(fakePage, jsonResult.Value);
        }

        [TestMethod]
        public void GivenAValidEntity_WhenQueryingRecommendations_APagedListOfRecommendingUsersIsReturned()
        {
            var entityId = 10;
            var fakePage = new PagedList<Rating> { };
            var fakeRatings = Enumerable.Range(0, 100).Select(i => new Rating { Id = i });
            queryHelperMock.Setup(r => r.Query(
                It.IsAny<IQueryable<Rating>>(), 10, 0,
                It.IsAny<Expression<Func<Rating, bool>>>()
            )).Returns(fakePage);
            contextMock.SetupGet(r => r.Ratings).Returns(fakeRatings.ToDbSet());

            var response = controller.QueryEntityRatings(entityId);

            Assert.IsInstanceOfType(response, typeof(JsonResult));
            var jsonResult = response as JsonResult;
            Assert.AreEqual(fakePage, jsonResult.Value);
        }

        [TestMethod]
        public void GivenAValidEntity_WhenGettingTheRecommendationsSummary_TheNumberOfRecommendationsIsReturned()
        {
            var entityId = 2;
            var fakeRatings = Enumerable.Range(0, 100)
                .Select(id => new Rating { RatingValue = GetFakeStars(id), Entity = entityId })
                .Union(new[] { new Rating { Entity = 3, RatingValue = Stars.Five } });
            contextMock.SetupGet(r => r.Ratings).Returns(fakeRatings.ToDbSet());

            var response = controller.GetEntityRatingsSummary(entityId);

            Assert.IsInstanceOfType(response, typeof(JsonResult));
            var jsonResult = response as JsonResult;
            Assert.IsInstanceOfType(jsonResult.Value, typeof(RatingSummary));
            var model = jsonResult.Value as RatingSummary;
            Assert.AreEqual(5, model.OneStar);
            Assert.AreEqual(10, model.TwoStars);
            Assert.AreEqual(15, model.ThreeStars);
            Assert.AreEqual(30, model.FourStars);
            Assert.AreEqual(40, model.FiveStars);
        }

        private Stars GetFakeStars(int seed)
        {
            if(seed < 5)
            {
                return Stars.One;
            }
            else if(seed < 15)
            {
                return Stars.Two;
            }
            else if(seed < 30)
            {
                return Stars.Three;
            }
            else if(seed < 60)
            {
                return Stars.Four;
            }
            else
            {
                return Stars.Five;
            }
        }
    }
}