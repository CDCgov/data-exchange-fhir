using Microsoft.AspNetCore.Mvc;
using OneCDPFHIRFacade.Handlers;

namespace OneCDPOneCDPFHIRFacade.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PatientController : ControllerBase
    {

        [HttpPost]
        public async Task<IResult> Post([FromBody] string json)
        {
            if (json == null)
            {
                return (IResult)BadRequest(new
                {
                    error = "Invalid payload",
                    message = "Patient is required."
                });
            }
            PatientHandler handler = new PatientHandler();
            return await handler.CreatePatient(json);

        }
    }
}
