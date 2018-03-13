using marketplace.api.Data;
using marketplace.api.Security;
using marketplace.api.test;
using marketplace.api.Users;
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
    public class UserControllerSpec
    {
        private UsersController usersController;
        
        private Mock<MarketplaceContext> contextMock;
        private Mock<AuthorizationService> authorizationServiceMock;
        private Mock<QueryHelper> queryHelperMock;

        [TestInitialize]
        public void Init()
        {
            contextMock = new Mock<MarketplaceContext>();
            authorizationServiceMock = new Mock<AuthorizationService>();
            queryHelperMock = new Mock<QueryHelper>();

            usersController = new UsersController(contextMock.Object, queryHelperMock.Object, authorizationServiceMock.Object);
        }

        [TestMethod]
        public void GivenAnAdminRole_WhenQuerying_APagedListOfUsersIsReturned()
        {
            var fakeResults = Enumerable.Range(0, 100)
                .Select(id => new User { Id = id });
            authorizationServiceMock.Setup(r => r.HasRole(GlobalRole.Admin)).Returns(true);
            contextMock.SetupGet(r => r.Users).Returns(fakeResults.ToDbSet());
            var fakeReturn = new PagedList<User>();
            queryHelperMock.Setup(r => r.Query(
                It.IsAny<IQueryable<User>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Expression<Func<User, bool>>>())).Returns(fakeReturn);

            var result = usersController.QueryUsers();

            Assert.IsInstanceOfType(result, typeof(JsonResult));
            var jsonResult = result as JsonResult;
            var entities = jsonResult.Value as PagedList<User>;
            Assert.IsInstanceOfType(entities, typeof(PagedList<User>));
            Assert.AreEqual(fakeReturn, entities);
        }

        [TestMethod]
        public void GivenANonAdminRole_WWhenQueryingUsers_AnErrorIsReturned()
        {
            authorizationServiceMock.Setup(r => r.HasRole(It.IsAny<int>(), EntityRole.Owner)).Returns(true);
            
            var result = usersController.QueryUsers();

            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
        }

        [TestMethod]
        public void GivenAValidUserIdAndOwner_WhenGettingById_TheUserIsReturned()
        {
            var fakeUsers = Enumerable.Range(0, 100)
                .Select(r => new User { Id = r });
            authorizationServiceMock.Setup(r => r.GetUserId()).Returns(1);
            contextMock.SetupGet(r => r.Users).Returns(fakeUsers.ToDbSet());

            var result = usersController.GetUser();

            Assert.IsInstanceOfType(result, typeof(JsonResult));
            var jsonResult = result as JsonResult;
            Assert.IsInstanceOfType(jsonResult.Value, typeof(User));
            var userResult = jsonResult.Value as User;
            Assert.AreEqual(1, userResult.Id);
        }

        [TestMethod]
        public void GivenAValidUserIdAndAdminRole_WhenGettingById_TheUserIsReturned()
        {
            var fakeUsers = Enumerable.Range(0, 100)
                .Select(r => new User { Id = r });
            authorizationServiceMock.Setup(r => r.HasRole(GlobalRole.Admin)).Returns(true);
            contextMock.SetupGet(r => r.Users).Returns(fakeUsers.ToDbSet());

            var result = usersController.GetUser(1);

            Assert.IsInstanceOfType(result, typeof(JsonResult));
            var jsonResult = result as JsonResult;
            Assert.IsInstanceOfType(jsonResult.Value, typeof(User));
            var userResult = jsonResult.Value as User;
            Assert.AreEqual(1, userResult.Id);
        }

        [TestMethod]
        public void GivenAValidUserIdAndNonAdminRoleNonOwner_WhenGettingById_TheUserIsReturned()
        {
            authorizationServiceMock.Setup(r => r.HasRole(GlobalRole.Admin)).Returns(false);

            var result = usersController.GetUser(1);

            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
        }

        [TestMethod]
        public void GivenAnInvalidUserId_WhenGettingById_AnErrorIsReturned()
        {
            var fakeUsers = Enumerable.Range(0, 100)
                .Select(r => new User { Id = r });
            authorizationServiceMock.Setup(r => r.HasRole(GlobalRole.Admin)).Returns(true);
            contextMock.SetupGet(r => r.Users).Returns(fakeUsers.ToDbSet());

            var result = usersController.GetUser(500);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void GivenTheOwnerOfTheAccountWithAValidUpdate_WhenUpdatingTheUser_TheUserIsUpdated()
        {
            var fakeUsers = Enumerable.Range(0, 100)
                .Select(r => new User { Id = r })
                .ToList();
            authorizationServiceMock.Setup(r => r.GetUserId()).Returns(1);
            contextMock.SetupGet(r => r.Users).Returns(fakeUsers.ToDbSet());
            var updatedUser = new User {
                Name = "New Name"
            };

            var result = usersController.UpdateUser(updatedUser);

            Assert.IsInstanceOfType(result, typeof(OkResult));
            var updatedDbUser = fakeUsers.ElementAt(1);
            Assert.AreEqual(updatedUser.Name, updatedDbUser.Name);
            contextMock.Verify(r => r.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void GivenAnAdminRoleAndAValidUpdate_WhenUpdatingTheUser_TheUserIsUpdated()
        {
            var fakeUsers = Enumerable.Range(0, 100)
                .Select(r => new User { Id = r }).ToList();
            authorizationServiceMock.Setup(r => r.HasRole(GlobalRole.Admin)).Returns(true);
            contextMock.SetupGet(r => r.Users).Returns(fakeUsers.ToDbSet());
            var updatedUser = new User
            {
                Name = "New Name"
            };

            var result = usersController.UpdateUser(1, updatedUser);

            Assert.IsInstanceOfType(result, typeof(OkResult));
            var updatedDbUser = fakeUsers.ElementAt(1);
            Assert.AreEqual(updatedUser.Name, updatedDbUser.Name);
            contextMock.Verify(r => r.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void GivenANonOwnerNonAdminRoleAndAValidUpdate_WhenUpdatingTheUser_AnErrorIsReturned()
        {
            var fakeUsers = Enumerable.Range(0, 100)
                .Select(r => new User { Id = r }).ToList();
            authorizationServiceMock.Setup(r => r.HasRole(GlobalRole.Admin)).Returns(false);
            contextMock.SetupGet(r => r.Users).Returns(fakeUsers.ToDbSet());
            var updatedUser = new User
            {
                Name = "New Name"
            };

            var result = usersController.UpdateUser(1, updatedUser);

            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
            var updatedDbUser = fakeUsers.ElementAt(1);
            Assert.AreEqual(null, updatedDbUser.Name);
            contextMock.Verify(r => r.SaveChanges(), Times.Never);
        }

        [TestMethod]
        public void GivenAnAuthorisedUserWithAnInvalidUpdate_WhenUpdatingTheUser_AnErrorIsReturned()
        {
            var fakeUsers = Enumerable.Range(0, 100)
                .Select(r => new User { Id = r }).ToList();
            authorizationServiceMock.Setup(r => r.HasRole(GlobalRole.Admin)).Returns(true);
            contextMock.SetupGet(r => r.Users).Returns(fakeUsers.ToDbSet());
            var updatedUser = new User
            {
            };
            usersController.ModelState.AddModelError("Name", "Invalid name");

            var result = usersController.UpdateUser(1, updatedUser);

            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var updatedDbUser = fakeUsers.ElementAt(1);
            contextMock.Verify(r => r.SaveChanges(), Times.Never);
        }

        [TestMethod]
        public void GivenAnAdminRole_WhenDeletingAUser_TheUserAsWellAsAllTheirEntitiesIsDeactivated()
        {
            var fakeUsers = Enumerable.Range(0, 100)
                .Select(r => new User { Id = r }).ToList();
            authorizationServiceMock.Setup(r => r.HasRole(GlobalRole.Admin)).Returns(true);
            contextMock.SetupGet(r => r.Users).Returns(fakeUsers.ToDbSet());

            var result = usersController.DeleteUser(1);

            Assert.IsInstanceOfType(result, typeof(OkResult));
            var updatedDbUser = fakeUsers.ElementAt(1);
            Assert.AreEqual<UserState>(UserState.Deleted, updatedDbUser.State);
            contextMock.Verify(r => r.SaveChanges(), Times.Once);
        }
    }
}