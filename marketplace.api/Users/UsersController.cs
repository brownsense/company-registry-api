using System;

namespace  MarketPlace.Api.Entities
{
    public class UsersController
    {
        private MarketplaceContext @object;

        public UsersController(MarketplaceContext @object)
        {
            this.@object = @object;
        }

        public object QueryUsers()
        {
            throw new NotImplementedException();
        }
    }
}
