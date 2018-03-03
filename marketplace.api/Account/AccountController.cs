namespace  MarketPlace.Api.Entities
{

    public class AccountController
    {
        private MarketplaceContext @object;

        public AccountController(MarketplaceContext @object)
        {
            this.@object = @object;
        }
    }
}
