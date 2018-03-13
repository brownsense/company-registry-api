using marketplace.api.Data;
using marketplace.api.RedFlags;
using marketplace.api.Security;
using marketplace.api.test;
using Marketplace.Api.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MarketPlace.Api.Entities.Test
{
    [TestClass]
    public class RedFlagsControllerSpec
    {
        private RedFlagsController controller;

        private Mock<MarketplaceContext> contextMock;
        private Mock<AuthorizationService> authorizationServiceMock;
        private Mock<QueryHelper> queryHelperMock;

        [TestInitialize]
        public void Init()
        {
            contextMock = new Mock<MarketplaceContext>();
            authorizationServiceMock = new Mock<AuthorizationService>();
            queryHelperMock = new Mock<QueryHelper>();

            controller = new RedFlagsController(
                contextMock.Object,
                authorizationServiceMock.Object,
                queryHelperMock.Object
            );
        }

        [TestMethod]
        public void GivenAValidCompany_WhenSubmittingAValidRedFlag_TheFlagIsAccepted()
        {
            var entityId = 2;
            var redFlag = new RedFlag
            {
                Comment = "The guy did not deliver the product"
            };
            var redFlagsDbSetMock = new Mock<DbSet<RedFlag>>();
            contextMock.SetupGet(r => r.RedFlags).Returns(redFlagsDbSetMock.Object);

            var response = controller.SubmitFlag(entityId, redFlag);

            Assert.IsInstanceOfType(response, typeof(CreatedResult));
            redFlagsDbSetMock.Verify(r => r.Add(redFlag), Times.Once);
            contextMock.Verify(r => r.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void GivenAValidCompany_WhenSubmittingAnInvalidRedFlag_AnErrorIsReturned()
        {
            var entityId = 2;
            var redFlag = new RedFlag { };
            var redFlagsDbSetMock = new Mock<DbSet<RedFlag>>();
            contextMock.SetupGet(r => r.RedFlags).Returns(redFlagsDbSetMock.Object);
            controller.ModelState.AddModelError("Bla", "Test error");

            var response = controller.SubmitFlag(entityId, redFlag);

            Assert.IsInstanceOfType(response, typeof(BadRequestObjectResult));
            redFlagsDbSetMock.Verify(r => r.Add(redFlag), Times.Never);
            contextMock.Verify(r => r.SaveChanges(), Times.Never);
        }

        [TestMethod]
        public void GivenAnAdminUser_WhenQueryingRedFlagsForACompany_APagedListOfTheProcessedAndPendingFlagsAreReturned()
        {
            var entityId = 2;
            var fakeFlags = Enumerable.Range(0, 100)
                .Select(id => new RedFlag {
                    Id = id,
                    EntityId = entityId,
                    Status = id % 2 == 0 ? RedFlagStatus.Pending : RedFlagStatus.ResolutionFailed });
            contextMock.SetupGet(r => r.RedFlags).Returns(fakeFlags.ToDbSet());
            var fakePage = new PagedList<RedFlag>();
            queryHelperMock.Setup(q => q.Query(It.IsAny<IQueryable<RedFlag>>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Expression<Func<RedFlag, bool>>>())).Returns(fakePage);
            authorizationServiceMock.Setup(r => r.HasRole(GlobalRole.Admin)).Returns(true);
            authorizationServiceMock.Setup(r => r.HasRole(entityId, EntityRole.Owner)).Returns(false);

            var response = controller.QueryFlags(entityId);

            Assert.IsInstanceOfType(response, typeof(JsonResult));
            var jsonResult = response as JsonResult;
            Assert.IsInstanceOfType(jsonResult.Value, typeof(PagedList<RedFlag>));
            var model = jsonResult.Value;
            Assert.AreEqual(fakePage, model);
        }

        [TestMethod]
        public void GivenANonAdminUser_WhenQueryingRedFlagsForAnEntity_APagedListOfOnlyProcessedFlagsAndPendingFlagsByTheUserAreReturned()
        {
            var entityId = 2;
            var fakeFlags = Enumerable.Range(0, 100)
                .Select(id => new RedFlag
                {
                    Id = id,
                    EntityId = entityId,
                    Status = id % 2 == 0 ? RedFlagStatus.Pending : RedFlagStatus.ResolutionFailed
                });
            contextMock.SetupGet(r => r.RedFlags).Returns(fakeFlags.ToDbSet());
            var fakePage = new PagedList<RedFlag>();
            queryHelperMock.Setup(q => q.Query(It.IsAny<IQueryable<RedFlag>>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Expression<Func<RedFlag, bool>>>())).Returns(fakePage);
            authorizationServiceMock.Setup(r => r.HasRole(GlobalRole.Admin)).Returns(false);
            authorizationServiceMock.Setup(r => r.HasRole(entityId, EntityRole.Owner)).Returns(false);

            var response = controller.QueryFlags(entityId);

            Assert.IsInstanceOfType(response, typeof(JsonResult));
            var jsonResult = response as JsonResult;
            Assert.IsInstanceOfType(jsonResult.Value, typeof(PagedList<RedFlag>));
            var model = jsonResult.Value;
            Assert.AreEqual(fakePage, model);
        }

        [TestMethod]
        public void GivenAnEntityOwner_WhenQueryingRedFlagsForThatEntity_APagedListOfTheProcessedAndPendingFlagsAreReturned()
        {
            var entityId = 2;
            var fakeFlags = Enumerable.Range(0, 100)
                .Select(id => new RedFlag
                {
                    Id = id,
                    EntityId = entityId,
                    Status = id % 2 == 0 ? RedFlagStatus.Pending : RedFlagStatus.ResolutionFailed
                });
            contextMock.SetupGet(r => r.RedFlags).Returns(fakeFlags.ToDbSet());
            var fakePage = new PagedList<RedFlag>();
            queryHelperMock.Setup(q => q.Query(It.IsAny<IQueryable<RedFlag>>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Expression<Func<RedFlag, bool>>>())).Returns(fakePage);
            authorizationServiceMock.Setup(r => r.HasRole(GlobalRole.Admin)).Returns(false);
            authorizationServiceMock.Setup(r => r.HasRole(entityId, EntityRole.Owner)).Returns(true);

            var response = controller.QueryFlags(entityId);

            Assert.IsInstanceOfType(response, typeof(JsonResult));
            var jsonResult = response as JsonResult;
            Assert.IsInstanceOfType(jsonResult.Value, typeof(PagedList<RedFlag>));
            var model = jsonResult.Value;
            Assert.AreEqual(fakePage, model);
        }

        [TestMethod]
        public void GivenAUser_WhenGettingAProcessedFlag_TheFlagIsReturned()
        {
            var id = 7;
            var fakeFlag = new RedFlag { Id = id, Status = RedFlagStatus.Resolved };
            var flags = Enumerable.Range(0, 100)
                .Select(r => r == id ? fakeFlag : new RedFlag { Id = r });
            contextMock.SetupGet(r => r.RedFlags).Returns(flags.ToDbSet());

            var response = controller.GetFlag(id);

            Assert.IsInstanceOfType(response, typeof(JsonResult));
            var jsonResult = response as JsonResult;
            Assert.IsInstanceOfType(jsonResult.Value, typeof(RedFlag));
            var model = jsonResult.Value as RedFlag;
            Assert.AreEqual(fakeFlag, model);
        }

        [TestMethod]
        public void GivenAnAdminUser_WhenGettingAnUnprocessedFlag_TheFlagIsReturned()
        {
            var id = 7;
            var fakeFlag = new RedFlag { Id = id, Status = RedFlagStatus.Resolved };
            var flags = Enumerable.Range(0, 100)
                .Select(r => r == id ? fakeFlag : new RedFlag { Id = r });
            contextMock.SetupGet(r => r.RedFlags).Returns(flags.ToDbSet());
            authorizationServiceMock.Setup(r => r.HasRole(GlobalRole.Admin)).Returns(true);

            var response = controller.GetFlag(id);

            Assert.IsInstanceOfType(response, typeof(JsonResult));
            var jsonResult = response as JsonResult;
            Assert.IsInstanceOfType(jsonResult.Value, typeof(RedFlag));
            var model = jsonResult.Value as RedFlag;
            Assert.AreEqual(fakeFlag, model);
        }

        [TestMethod]
        public void GivenANonAdminUserWhoSubmittedTheFlag_WhenGettingTheUnprocessedFlag_TheFlagIsReturned()
        {
            var id = 7;
            var submitter = 6;
            var fakeFlag = new RedFlag { Id = id, Status = RedFlagStatus.AwaitingEntityInput, Submitter = submitter };
            var flags = Enumerable.Range(0, 100)
                .Select(r => r == id ? fakeFlag : new RedFlag { Id = r });
            contextMock.SetupGet(r => r.RedFlags).Returns(flags.ToDbSet());
            authorizationServiceMock.Setup(r => r.HasRole(GlobalRole.Admin)).Returns(true);
            authorizationServiceMock.Setup(r => r.GetUserId()).Returns(submitter);

            var response = controller.GetFlag(id);

            Assert.IsInstanceOfType(response, typeof(JsonResult));
            var jsonResult = response as JsonResult;
            Assert.IsInstanceOfType(jsonResult.Value, typeof(RedFlag));
            var model = jsonResult.Value as RedFlag;
            Assert.AreEqual(fakeFlag, model);
        }

        [TestMethod]
        public void GivenAUserWhoOwnsTheEntity_WhenGettingAnUnprocessedFlagForThatEntity_TheFlagIsReturned()
        {
            var id = 7;
            var submitter = 6;
            var entity = 5;
            var fakeFlag = new RedFlag { Id = id, Status = RedFlagStatus.AwaitingEntityInput, Submitter = submitter, EntityId = entity };
            var flags = Enumerable.Range(0, 100)
                .Select(r => r == id ? fakeFlag : new RedFlag { Id = r });
            contextMock.SetupGet(r => r.RedFlags).Returns(flags.ToDbSet());
            authorizationServiceMock.Setup(r => r.HasRole(GlobalRole.Admin)).Returns(true);
            authorizationServiceMock.Setup(r => r.GetUserId()).Returns(4);
            authorizationServiceMock.Setup(r => r.HasRole(entity, EntityRole.Owner)).Returns(true);

            var response = controller.GetFlag(id);

            Assert.IsInstanceOfType(response, typeof(JsonResult));
            var jsonResult = response as JsonResult;
            Assert.IsInstanceOfType(jsonResult.Value, typeof(RedFlag));
            var model = jsonResult.Value as RedFlag;
            Assert.AreEqual(fakeFlag, model);
        }

        [TestMethod]
        public void GivenANonAdminNonSubmitterNonEntityOwnerUser_WhenGettingAnUnprocessedFlag_AnErrorIsReturned()
        {
            var id = 7;
            var submitter = 6;
            var entity = 5;
            var fakeFlag = new RedFlag { Id = id, Status = RedFlagStatus.AwaitingEntityInput, Submitter = submitter, EntityId = entity };
            var flags = Enumerable.Range(0, 100)
                .Select(r => r == id ? fakeFlag : new RedFlag { Id = r });
            contextMock.SetupGet(r => r.RedFlags).Returns(flags.ToDbSet());
            authorizationServiceMock.Setup(r => r.HasRole(GlobalRole.Admin)).Returns(false);
            authorizationServiceMock.Setup(r => r.GetUserId()).Returns(4);
            authorizationServiceMock.Setup(r => r.HasRole(entity, EntityRole.Owner)).Returns(false);

            var response = controller.GetFlag(id);

            Assert.IsInstanceOfType(response, typeof(UnauthorizedResult));
        }

        [TestMethod]
        public void GivenAnAdminUser_WhenProcessingAFlag_TheFlagIsProcessed()
        {
            var id = 8;
            var flag = new RedFlag { Id = id, Status = RedFlagStatus.Pending };
            var flags = Enumerable.Range(0, 100)
                .Select(i => i == id ? flag : new RedFlag { Id = i });
            contextMock.SetupGet(c => c.RedFlags).Returns(flags.ToDbSet());
            var newStatus = RedFlagStatus.Resolved;
            authorizationServiceMock.Setup(r => r.HasRole(GlobalRole.Admin)).Returns(true);

            var response = controller.UpdateStatus(id, newStatus);

            Assert.IsInstanceOfType(response, typeof(OkResult));
            Assert.AreEqual(newStatus, flag.Status);
            contextMock.Verify(r => r.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void GivenANonAdminUser_WhenProcessingAFlag_AnErrorIsReturned()
        {
            var id = 8;
            var flag = new RedFlag { Id = id, Status = RedFlagStatus.Pending };
            var flags = Enumerable.Range(0, 100)
                .Select(i => i == id ? flag : new RedFlag { Id = i });
            contextMock.SetupGet(c => c.RedFlags).Returns(flags.ToDbSet());
            var newStatus = RedFlagStatus.Resolved;
            authorizationServiceMock.Setup(r => r.HasRole(GlobalRole.Admin)).Returns(false);

            var response = controller.UpdateStatus(id, newStatus);

            Assert.IsInstanceOfType(response, typeof(UnauthorizedResult));
            Assert.AreEqual(RedFlagStatus.Pending, flag.Status);
            contextMock.Verify(r => r.SaveChanges(), Times.Never);
        }
    }
}
