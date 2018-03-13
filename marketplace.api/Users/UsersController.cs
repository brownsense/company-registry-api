using marketplace.api.Data;
using marketplace.api.Security;
using marketplace.api.Users;
using Marketplace.Api.Security;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace  MarketPlace.Api.Entities
{
    public class UsersController : Controller
    {
        private MarketplaceContext context;
        private QueryHelper queryHelper;
        private AuthorizationService authorizationService;

        public UsersController(
                MarketplaceContext context,
                QueryHelper queryHelper,
                AuthorizationService authorizationService
            )
        {
            this.context = context;
            this.queryHelper = queryHelper;
            this.authorizationService = authorizationService;
        }

        public ActionResult QueryUsers()
        {
            if(!authorizationService.HasRole(GlobalRole.Admin))
            {
                return Unauthorized();
            }

            return Json(queryHelper.Query(context.Users, 10, 0, a => true));
        }

        public ActionResult GetUser()
        {
            var userId = authorizationService.GetUserId();

            return Json(context.Users.SingleOrDefault(u => u.Id == userId));
        }

        public ActionResult GetUser(int userId)
        {
            if(!authorizationService.HasRole(GlobalRole.Admin))
            {
                return Unauthorized();
            }

            var foundUser = context.Users.SingleOrDefault(u => u.Id == userId);

            if (foundUser == null)
                return NotFound();
            else
                return Json(foundUser);
        }

        public object UpdateUser(User updatedUser)
        {
            var userId = authorizationService.GetUserId();
            
            return UpdateUser(userId, updatedUser);
        }

        public object UpdateUser(int userId, User updatedUser)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(updatedUser);
            }
            if (!authorizationService.HasRole(GlobalRole.Admin) && authorizationService.GetUserId() != userId)
            {
                return Unauthorized();
            }
            
            var foundUser = context.Users.Single(u => u.Id == userId);

            foundUser.Name = updatedUser.Name;
            context.SaveChanges();
            return Ok();
        }

        public object DeleteUser(int userId)
        {
            if(!authorizationService.HasRole(GlobalRole.Admin))
            {
                throw new Exception("Cannot delete user as non-admin");
            }


            var foundUser = context.Users.SingleOrDefault(u => u.Id == userId);

            if(foundUser == null)
            {
                return NotFound();
            }

            foundUser.State = UserState.Deleted;
            context.SaveChanges();
            return Ok();
        }
    }
}
