using marketplace.api.Data;
using Marketplace.Api.Security;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace  MarketPlace.Api.Entities
{
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

        public ActionResult QueryEntities()
        {
            var theReturn = queryHelper.Query(context.Entities, 10, 0, (a) => true);
            return Json(theReturn);
        }

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

        public ActionResult GetEntity(int targetId)
        {
            var response = context.Entities.SingleOrDefault(r => r.Id == targetId);
            var theReturn = response == null ? NotFound() as ActionResult : Json(response);
            return theReturn;
        }

        public ActionResult UpdateEntity(int id, Entity update)
        {
            var foundEntity = context.Entities.SingleOrDefault(r => r.Id == id);
            if(foundEntity == null)
            {
                return NotFound(id);
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

        public ActionResult DeleteEntity(int id)
        {
            var foundEntity = context.Entities.SingleOrDefault(r => r.Id == id);
            if (foundEntity == null)
            {
                return NotFound(id);
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
