using Marketplace.Api.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace marketplace.api.Security
{
    public class TokenAuthorizationService : AuthorizationService
    {
        public int GetUserId()
        {
            return 1;
        }

        public bool HasRole(GlobalRole role)
        {
            return true;
        }

        public bool HasRole(int entityId, EntityRole role)
        {
            return true;
        }
    }
}
