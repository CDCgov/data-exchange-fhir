using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Moq;
using OneCDPFHIRFacade.Controllers;
using OneCDPFHIRFacade.Utilities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace fhir_facade_tests.ControllerTests
{
    [TestFixture]
    public class HealthControllerTests
    {
  
        private HealthController _controller;

        [SetUp]
        public void SetUp()
        {
            _controller = new HealthController()
            {
            };
        }

        [Test]
        public void testGet()
        {
            var result = _controller.Get();
            Assert.That(result, Is.Not.Null);          
        }

        [TearDown]
        public void TearDown()
        {
         
        }

    }

}

