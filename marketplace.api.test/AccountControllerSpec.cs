using Marketplace.Api.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace MarketPlace.Api.Entities.Test
{
    [TestClass]
    public class AccountControllerSpec
    {
        private AccountController controller;
        
        private Mock<MarketplaceContext> contextMock;
        private Mock<AuthorizationService> oauthMock;
        
        [TestInitialize]
        public void Init()
        {
            contextMock = new Mock<MarketplaceContext>();
            oauthMock = new Mock<AuthorizationService>();

            controller = new AccountController(contextMock.Object);
        }

        public void GivenAnAccountRequest_WhenSubmitting_ANewAccountIsRegisteredAndTheAssociatedUserCreated()
        {
            Assert.Inconclusive();
        }

        public void GivenAnAuthenticatedSession_WhenRetrievingTheAccount_TheAccountIsReturned()
        {
            Assert.Inconclusive();
        }

        public void GivenAnInvalidSession_WhenRetrievingTheAccount_AnErrorIsReturned()
        {
            Assert.Inconclusive();
        }
    }
}