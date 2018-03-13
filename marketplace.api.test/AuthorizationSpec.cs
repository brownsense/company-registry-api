using marketplace.api.Security;
using Marketplace.Api.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace marketplace.api.test
{
    [TestClass]
    public class AuthorizationSpec
    {
        private AuthorizationService authService;

        [TestInitialize]
        public void Init()
        {
            authService = new TokenAuthorizationService();
        }

        [TestMethod]
        public void GivenARequestWithNoToken_WhenQueryingAuthorization_TheUserIsNotAuthorizedForAnyRoles()
        {
            var fakeClaim = string.Empty;

            var response = authService.HasRole(GlobalRole.Admin);
            var entityResponse = authService.HasRole(1, EntityRole.Owner);

            Assert.IsFalse(response);
        }
    }
}
