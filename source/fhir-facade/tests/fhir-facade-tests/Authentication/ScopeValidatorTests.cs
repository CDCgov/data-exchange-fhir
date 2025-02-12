using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Moq;
using OneCDPFHIRFacade.Authentication;
using OneCDPFHIRFacade.Controllers;
using OneCDPFHIRFacade.Utilities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace fhir_facade_tests.Tests
{
    [TestFixture]
    public class ScopeValidatorTests
    {
  
        private ScopeValidator _controller;

        [SetUp]
        public void SetUp()
        {

            var serviceProvider = new  Mock<IServiceProvider>();
            
            _controller = new ScopeValidator(serviceProvider.Object)
            {
            };
        }

        [Test]
        public void testValidates()
        {
            string scopeClaim = "HAS";
             string[] requiredScopes = new string[] { "Apple", "Banana", "Cherry" };

            var result = _controller.Validate(scopeClaim, requiredScopes);

        
        }

        [TearDown]
        public void TearDown()
        {
            // Dispose the disposable object in TearDown to release the resource
          //  _controller?.Dispose();
        }

    }

}

