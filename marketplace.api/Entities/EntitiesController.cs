using marketplace.api.Data;
using Marketplace.Api.Security;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace  MarketPlace.Api.Entities
{
    [Produces("application/json")]
    [Route("api/entities")]
    public class EntitiesController : Controller
    {
        private readonly MarketplaceContext context;
        private readonly QueryHelper queryHelper;
        private readonly AuthorizationService authorizationService;

        public EntitiesController(MarketplaceContext context,
            AuthorizationService authorizationService,
            QueryHelper queryHelper)
        {
            this.context = context;
            this.authorizationService = authorizationService;
            this.queryHelper = queryHelper;
        }

        [HttpGet]
        public ActionResult QueryEntities()
        {
            var theReturn = queryHelper.Query(context.Entities, 10, 0, (a) => true);
            return Json(theReturn);
        }

        [HttpPost]
        public ActionResult CreateEntity(Entity entity)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(entity);
            }
            context.Entities.Add(entity);
            context.SaveChanges();
            return Created("", entity);
        }

        [HttpGet("{entityId}")]
        public ActionResult GetEntity(int entityId)
        {
            var response = context.Entities.SingleOrDefault(r => r.Id == entityId);
            var theReturn = response == null ? NotFound() as ActionResult : Json(response);
            return theReturn;
        }

        [HttpPut("{entityId}")]
        public ActionResult UpdateEntity(int entityId, Entity update)
        {
            var foundEntity = context.Entities.SingleOrDefault(r => r.Id == entityId);
            if(foundEntity == null)
            {
                return NotFound(entityId);
            }
            else
            {
                foundEntity.Name = update.Name;
                foundEntity.PrimaryEmail = update.PrimaryEmail;
                foundEntity.RegistrationNumber = update.RegistrationNumber;
                context.SaveChanges();
                return Ok();
            }
        }

        [HttpDelete("{entityId}")]
        public ActionResult DeleteEntity(int entityId)
        {
            var foundEntity = context.Entities.SingleOrDefault(r => r.Id == entityId);
            if (foundEntity == null)
            {
                return NotFound(entityId);
            }
            else
            {
                foundEntity.Status = EntityStatus.Deleted;
                context.SaveChanges();
                return Ok();
            }
        }
    }
}
