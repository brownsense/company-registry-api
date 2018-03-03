using marketplace.api.Data;
using marketplace.api.test;
using marketplace.api.Users;
using Marketplace.Api.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;

namespace MarketPlace.Api.Entities.Test
{
    [TestClass]
    public class UserControllerSpec
    {
        private UsersController usersController;
        
        private Mock<MarketplaceContext> contextMock;
        private Mock<AuthorizationService> authorizationServiceMock;

        [TestInitialize]
        public void Init()
        {
            contextMock = new Mock<MarketplaceContext>();
            authorizationServiceMock = new Mock<AuthorizationService>();

            usersController = new UsersController(contextMock.Object);
        }

        [TestMethod]
        public void GivenAnAdminRole_WhenQuerying_APagedListOfUsersIsReturned()
        {
            var fakeResults = Enumerable.Range(0, 100)
                .Select(id => new User { Id = id });
            authorizationServiceMock.Setup(r => r.HasRole("entity-admin")).Returns(false);
            contextMock.SetupGet(r => r.Users).Returns(fakeResults.ToDbSet());

            var entities = usersController.QueryUsers() as PagedList<User>;

            Assert.IsInstanceOfType(entities, typeof(PagedList<User>));
            Enumerable.Range(0, 10)
                .ToList()
                .ForEach(i => Assert.AreEqual(i, entities.Items.ElementAt(i).Id));
        }

        [TestMethod]
        public void GivenANonAdminRole_WWhenQueryingUsers_AnErrorIsReturned()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void GivenAValidUserIdAndOwner_WhenGettingById_TheUserIsReturned()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void GivenAValidUserIdAndAdminRole_WhenGettingById_TheUserIsReturned()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void GivenAValidUserIdAndNonAdminRoleNonOwner_WhenGettingById_TheUserIsReturned()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void GivenAnInvalidUserId_WhenGettingById_AnErrorIsReturned()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void GivenTheOwnerOfTheAccountWithAValidUpdate_WhenUpdatingTheUser_TheUserIsUpdated()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void GivenAnAdminRoleAndAValidUpdate_WhenUpdatingTheUser_TheUserIsUpdated()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void GivenAnNonOwnerNonAdminRoleAndAValidUpdate_WhenUpdatingTheUser_AnErrorIsReturned()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void GivenAnAuthorisedUserWithAnInvalidUpdate_WhenUpdatingTheUser_AnErrorIsReturned()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void GivenAnAdminRole_WhenDeletingAUser_TheUserAsWellAsAllTheirEntitiesIsDeactivated()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void GivenAnOwner_WhenRequestingTheUserEntities_APagedListOfTheUserEntitiesIsReturned()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void GivenAnAdmin_WhenRequestingTheUserEntities_APagedListOfTheUserEntitiesIsReturned()
        {
            Assert.Inconclusive();
        }
    }
}