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
           Console.WriteLine(result);
            Console.WriteLine(result.ToString());
         //   var okResult = result as Results;
          //  Console.WriteLine(okResult.Content);
            // TODO Add Assert(s)        
        }

        [TearDown]
        public void TearDown()
        {
            // Dispose the disposable object in TearDown to release the resource
          //  _controller?.Dispose();
        }

    }

}

