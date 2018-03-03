using marketplace.api.Data;
using marketplace.api.test;
using Marketplace.Api.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace MarketPlace.Api.Entities.Test
{
    [TestClass]
    public class EntitiesControllerSpec
    {
        private EntitiesController entitiesController;

        private Mock<MarketplaceContext> contextMock;
        private Mock<AuthorizationService> authorizationServiceMock;
        private Mock<QueryHelper> queryHelperMock;

        [TestInitialize]
        public void Init()
        {
            contextMock = new Mock<MarketplaceContext>();
            authorizationServiceMock = new Mock<AuthorizationService>();
            queryHelperMock = new Mock<QueryHelper>();

            entitiesController = new EntitiesController(contextMock.Object, authorizationServiceMock.Object, queryHelperMock.Object);
        }

        [TestMethod]
        public void GivenAListOfPersistedEntities_WhenQuerying_APagedListOfEntitiesIsReturned()
        {
            var fakeResults = Enumerable.Range(0, 100)
                .Select(id => new Entity { Id = id });
            authorizationServiceMock.Setup(r => r.HasRole("entity-admin")).Returns(false);
            contextMock.SetupGet(r => r.Entities).Returns(fakeResults.ToDbSet());
            var fakeReturn = new PagedList<Entity>();
            queryHelperMock.Setup(r => r.Query(It.IsAny<IQueryable<Entity>>(), 10, 0, It.IsAny<Expression<Func<Entity, bool>>>())).Returns(fakeReturn);

            var result = entitiesController.QueryEntities() as JsonResult;

            var entities = result.Value as PagedList<Entity>;
            //queryHelperMock.Verify(r => r.Query(fakeResults.ToDbSet(), 10, 0, It.IsAny<Expression<Func<Entity, bool>>>())).Returns(fakeReturn);
            Assert.IsInstanceOfType(entities, typeof(PagedList<Entity>));
            Assert.AreEqual(fakeReturn, entities);
        }

        [TestMethod]
        public void GivenAValidEntityId_WhenRequestingTheEntityById_TheEntityIsReturned()
        {
            var fakeEntities = Enumerable.Range(0, 100)
                .Select(id => new Entity { Id = id });
            var targetId = 5;
            contextMock.SetupGet(r => r.Entities).Returns(fakeEntities.ToDbSet());

            var result = entitiesController.GetEntity(targetId) as JsonResult;
            
            Assert.IsInstanceOfType(result, typeof(JsonResult));
            var entity = result.Value as Entity;
            Assert.IsInstanceOfType(entity, typeof(Entity));
            Assert.AreEqual(targetId, entity.Id);
        }

        [TestMethod]
        public void GivenAnInvalidEntityId_WhenRequestingTheEntityById_AnErrorIsReturned()
        {
            var fakeEntities = Enumerable.Range(0, 100)
                .Select(id => new Entity { Id = id });
            var targetId = 500;
            contextMock.SetupGet(r => r.Entities).Returns(fakeEntities.ToDbSet());

            var result = entitiesController.GetEntity(targetId) as NotFoundResult;

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void GivenAnEntityRequest_WhenSubmittedByAnAuthenticatedUser_TheEntityIsCreated()
        {
            var fakeEntity = new Entity {
                Name = "John Doe",
                PrimaryEmail = "john.doe@gmail.com",
                RegistrationNumber = "1234567",
                Url = "https://johndoes.com"
            };
            var entitiesDbSetMock = new Mock<DbSet<Entity>>();
            contextMock.SetupGet(r => r.Entities).Returns(entitiesDbSetMock.Object);

            var response = entitiesController.CreateEntity(fakeEntity) as CreatedResult;

            entitiesDbSetMock.Verify(r => r.Add(fakeEntity), Times.Once);
            contextMock.Verify(r => r.SaveChanges(), Times.Once);
            Assert.IsInstanceOfType(response, typeof(CreatedResult));
        }

        [TestMethod]
        public void GivenAnInvalidEntityRequest_WhenSubmitting_AnErrorIsReturned()
        {
            var invalidEntity = new Entity { };
            entitiesController.ModelState.AddModelError("Name", "Name is required");

            var response = entitiesController.CreateEntity(invalidEntity);

            Assert.IsInstanceOfType(response, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public void GivenAnExistingEntity_WhenUpdating_ThePersistedEntityIsUpdated()
        {
            var fakeEntity = new Entity { Id = 2 };
            var fakeUpdate = new Entity { Id = 2, Name = "N1", PrimaryEmail = "p1", RegistrationNumber = "r1", Url = "U1" };
            var entityList = Enumerable.Range(0, 100)
                .Select(r => r == fakeEntity.Id ? fakeEntity : new Entity { Id = r });
            contextMock.SetupGet(r => r.Entities).Returns(entityList.ToDbSet());

            var response = entitiesController.UpdateEntity(fakeUpdate.Id, fakeUpdate);

            contextMock.Verify(r => r.SaveChanges(), Times.Once);
            Assert.AreEqual(fakeUpdate.Name, fakeEntity.Name);
            Assert.AreEqual(fakeUpdate.PrimaryEmail, fakeEntity.PrimaryEmail);
            Assert.AreEqual(fakeUpdate.RegistrationNumber, fakeEntity.RegistrationNumber);
        }

        [TestMethod]
        public void GivenAnEntityThatDoesNotExist_WhenUpdating_AnErrorIsReturned()
        {
            var fakeUpdate = new Entity { Id = 500 };
            var entityList = Enumerable.Range(0, 100)
                .Select(r => new Entity { Id = r });
            contextMock.SetupGet(r => r.Entities).Returns(entityList.ToDbSet());

            var response = entitiesController.UpdateEntity(fakeUpdate.Id, fakeUpdate);

            Assert.IsInstanceOfType(response, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public void GivenAnExistingEntity_WhenDeleting_TheEntityIsDeactivated()
        {
            var fakeEntity = new Entity { Id = 2, Status = EntityStatus.Active };
            var entityList = Enumerable.Range(0, 100)
                .Select(r => r == fakeEntity.Id ? fakeEntity : new Entity { Id = r });
            contextMock.SetupGet(r => r.Entities).Returns(entityList.ToDbSet());

            var response = entitiesController.DeleteEntity(fakeEntity.Id);

            contextMock.Verify(r => r.SaveChanges(), Times.Once);
            Assert.AreEqual(EntityStatus.Deleted, fakeEntity.Status);
        }

        [TestMethod]
        public void GivenAnEntityThatDoesNotExist_WhenDeleting_AnErrorIsReturned()
        {
            var entityList = Enumerable.Range(0, 100)
                .Select(r => new Entity { Id = r });
            contextMock.SetupGet(r => r.Entities).Returns(entityList.ToDbSet());

            var response = entitiesController.DeleteEntity(500);

            Assert.IsInstanceOfType(response, typeof(NotFoundObjectResult));
        }
    }
}