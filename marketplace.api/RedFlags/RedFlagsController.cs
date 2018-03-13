using System;
using System.Linq;
using marketplace.api.Data;
using marketplace.api.RedFlags;
using marketplace.api.Security;
using Marketplace.Api.Security;
using Microsoft.AspNetCore.Mvc;

namespace  MarketPlace.Api.Entities
{
    public class RedFlagsController : Controller
    {
        private MarketplaceContext db;
        private AuthorizationService authorizationService;
        private QueryHelper queryHelper;
        
        private static readonly RedFlagStatus[] resolvedStatusses = new[] { RedFlagStatus.ResolutionFailed, RedFlagStatus.Resolved, RedFlagStatus.ResolvedInvalid };
        private static readonly RedFlagStatus[] unresolvedStatusses = new[] { RedFlagStatus.Pending, RedFlagStatus.AwaitingEntityInput, RedFlagStatus.AwaitingEvaluation };

        public RedFlagsController(MarketplaceContext object1,
            AuthorizationService object2,
            QueryHelper queryHelper)
        {
            this.db = object1;
            this.authorizationService = object2;
            this.queryHelper = queryHelper;
        }

        public ActionResult SubmitFlag(int entityId, RedFlag redFlag)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(redFlag);
            }

            redFlag.EntityId = entityId;
            redFlag.Submitter = authorizationService.GetUserId();
            db.RedFlags.Add(redFlag);
            db.SaveChanges();
            return Created("", redFlag);
        }

        public object QueryFlags(int entityId)
        {
            var isGlobalAdmin = authorizationService.HasRole(GlobalRole.Admin);
            var isOwner = authorizationService.HasRole(entityId, EntityRole.Owner);
            var userId = authorizationService.GetUserId();

            if (isGlobalAdmin || isOwner)
            {
                var result = queryHelper.Query(db.RedFlags.Where(r => r.EntityId == entityId), 10, 0, a => true);
                return Json(result);
            }
            else
            {
                var entities = db.RedFlags.Where(r => r.EntityId == entityId && (resolvedStatusses.Contains(r.Status) || r.Submitter == userId));
                var result = queryHelper.Query(entities, 10, 0, a => true);
                return Json(result);
            }
        }

        public ActionResult GetFlag(int id)
        {
            var foundFlag = db.RedFlags.SingleOrDefault(f => f.Id == id);
            if(foundFlag == null)
            {
                throw new Exception("Flag not found");
            }

            if(unresolvedStatusses.Contains(foundFlag.Status))
            {
                if(!authorizationService.HasRole(GlobalRole.Admin)
                    && authorizationService.GetUserId() != foundFlag.Submitter
                    && !authorizationService.HasRole(foundFlag.EntityId, EntityRole.Owner))
                {
                    return Unauthorized();
                }
            }
            
            return Json(foundFlag);
        }

        public object UpdateStatus(int id, RedFlagStatus newStatus)
        {
            var foundFlag = db.RedFlags.Single(r => r.Id == id);

            if(!authorizationService.HasRole(GlobalRole.Admin))
            {
                return Unauthorized();
            }

            foundFlag.Status = newStatus;
            db.SaveChanges();
            return Ok();
        }
    }
}
