using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc;
using OneCDPFHIRFacade.Handlers;

namespace OneCDPOneCDPFHIRFacade.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PatientController : ControllerBase
    {

        [HttpPost]
        public async Task<IResult> Post([FromBody] Patient patient)
        {
            if (patient == null)
            {
                return (IResult)BadRequest(new
                {
                    error = "Invalid payload",
                    message = "Patient is required."
                });
            }
            PatientHandler handler = new PatientHandler();
            return await handler.CreatePatient(patient);

        }
    }
}
