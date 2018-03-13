using marketplace.api.Security;

namespace Marketplace.Api.Security
{
    public interface AuthorizationService
    {
        bool HasRole(GlobalRole role);
        bool HasRole(int entityId, EntityRole role);
        int GetUserId();
    }
}
