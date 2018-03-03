namespace Marketplace.Api.Security
{
    public interface AuthorizationService
    {
        bool HasRole(string v);
    }
}
