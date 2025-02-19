using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Mvc;
using static Hl7.Fhir.Model.CapabilityStatement;

namespace OneCDPFHIRFacade.Controllers
{
    [ApiController]
    [Route("metadata")]
    public class MetadataController : Controller
    {
        [HttpGet]
        public IResult Index()
        {
            // Create a CapabilityStatement object with the server's metadata
            CapabilityStatement capabilityStatement = new CapabilityStatement
            {
                // Set basic information
                Title = "Capability Statement",
                Id = "0565560f-016a-4473-82a7-cb44d3447f3c",
                Url = "https://localhost:7216/metadata",
                FhirVersion = FHIRVersion.N4_0_1,
                Name = "OneCDPFHIRFacadeCapabilityStatement",
                Status = PublicationStatus.Active,
                Experimental = true,
                Date = "2015-02", //Year and month it was last updated
                Publisher = "CDC 1CDP FHIR Facade",
                Kind = CapabilityStatementKind.Instance,
                Format = ["json", "xml"],
                PatchFormat = ["application/json-patch+json", "application/xml-patch+xml"],
                Description = "One CDP FHIR Facade",
                Implementation = new ImplementationComponent
                {
                    Description = "One CDP Implementation"
                },
                Instantiates =
                [
                    "https://hl7.org/fhir/us/core/CapabilityStatement/us-core-server"
                ],

                // Add Rest details
                Rest = new List<RestComponent>()
                {
                    new RestComponent
                    {
                        Mode = RestfulCapabilityMode.Client,
                        Resource = new List<ResourceComponent>()
                        {
                            // Add information for supported resources
                            new ResourceComponent
                            {
                                Type = "Bundle",
                                Interaction = new List<ResourceInteractionComponent>
                                {
                                    new ResourceInteractionComponent
                                    {
                                        Code = TypeRestfulInteraction.Read
                                    },
                                    new ResourceInteractionComponent
                                    {
                                        Code = TypeRestfulInteraction.Create
                                    }
                                },
                                Versioning = ResourceVersionPolicy.VersionedUpdate,
                                ConditionalRead = ConditionalReadStatus.FullSupport,
                                ConditionalDelete = ConditionalDeleteStatus.Multiple,
                                ReferencePolicy = [],
                            }
                        },
                        Interaction = new List<SystemInteractionComponent>
                        {
                             new SystemInteractionComponent
                             {
                                 Code = SystemRestfulInteraction.HistorySystem
                             },
                             new SystemInteractionComponent
                             {
                                 Code = SystemRestfulInteraction.Batch
                             },
                             new SystemInteractionComponent
                             {
                                 Code = SystemRestfulInteraction.Transaction
                             }
                        }
                    }
                }
            };

            var jsonSerializer = new FhirJsonSerializer();
            return Results.Content(jsonSerializer.SerializeToString(capabilityStatement), "application/fhir+json");
        }
    }
}

