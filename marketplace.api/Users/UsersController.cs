using System;

namespace  MarketPlace.Api.Entities
{
    public class UsersController
    {
        private MarketplaceContext context;

        public UsersController(MarketplaceContext @object)
        {
            this.context = @object;
        }

        public object QueryUsers()
        {
            throw new NotImplementedException();
        }
    }
}
