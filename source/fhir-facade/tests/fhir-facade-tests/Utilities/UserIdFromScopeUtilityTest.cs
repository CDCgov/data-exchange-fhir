using OneCDPFHIRFacade.Config;
using OneCDPFHIRFacade.Utilities;

namespace fhir_facade_tests.Utilities
{
    internal class UserIdFromScopeUtilityTest
    {
        public UserIdFromScopeUtility mockUserId;

        [SetUp]
        public void SetUp()
        {
            mockUserId = new UserIdFromScopeUtility();
        }
        [Test]
        public void GetUserIdFromUserScopeClaim()
        {
            // Arrange: Create two instances of LoggingUtility
            AwsConfig.ScopeClaim = new string[]
            {
                "system/system.r",
                "org/org.myUser",
                "stream/myBundle"
            };

            // Act: Get request IDs
            mockUserId.GetUserIdFromScope();

            if (AwsConfig.ClientId != null)
            {
                string clientId = AwsConfig.ClientId;
            }
            else
            {
                Assert.Fail();
            }


            // Assert: Ensure they are different
            Assert.That(AwsConfig.ClientId, Is.EqualTo("org.myUser"));
        }

    }

}


