using OneCDPFHIRFacade.Config;

namespace OneCDPFHIRFacade.Utilities
{
    public class UserIdFromScopeUtility
    {
        public void GetUserIdFromScope()
        {
            var scope = AwsConfig.ScopeClaim;
            foreach (var item in scope!)
            {
                if (item.Contains("org"))
                {
                    string[] orgArray = item.Split("/");
                    AwsConfig.ClientId = orgArray[1];
                }
            }
        }
    }
}
