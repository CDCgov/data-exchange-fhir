using OneCDPFHIRFacade.Controllers;

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
        public void TestGet()
        {
            var result = _controller.GetHealth;
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void TestGetAwsServiceHealth()
        {
            var result = _controller.GetAwsServiceHealth;
            Assert.That(result, Is.Not.Null);
        }

        [TearDown]
        public void TearDown()
        {

        }

    }

}

