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
        public IActionResult Index()
        {
            // Create a CapabilityStatement object with the server's metadata
            CapabilityStatement capabilityStatement = new CapabilityStatement
            {
                // Set basic information
                Title = "Capability Statement",
                Id=  "0565560f-016a-4473-82a7-cb44d3447f3c",
                Url = "https://localhost:7216/metadata",
                FhirVersion = FHIRVersion.N4_0_1,
                Name = "One CDP FHIR Facade Capability Statement",
                Status = PublicationStatus.Active,
                Experimental = true,
                Date = DateTime.Now.ToString(),
                Publisher = "CDC 1CDP FHIR Facade",
                Kind = CapabilityStatementKind.Instance,
                Format = ["json+fhir", "json"],
                PatchFormat = ["json-patch+json"],

                // Add Rest details
                Rest = new List<RestComponent>()
                {
                    new RestComponent
                    {
                        Mode = RestfulCapabilityMode.Client,
                        Security = new SecurityComponent
                        {
                            Service = new List<CodeableConcept>
                            {
                                new CodeableConcept
                                {
                                    Coding = new List<Coding>
                                    {
                                        new Coding
                                        {
                                            System = "",
                                            Code = "",
                                            Display = ""
                                        }
                                    }
                                },
                            },
                            Cors = false,
                            Description = ""
                        },
                        // Add a custom extension to indicate 'Facade'
                        Extension = new List<Extension>
                        {
                            new Extension
                            {
                                // Use an example URL for the extension; replace with your system's URL
                                Url = "https://ocioedefhirhealthtst-ocioedefhirtst.fhir.azurehealthcareapis.com/metadata",
                                Value = new FhirString("Facade")
                            }
                        },
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
                                    },
                                },
                                Versioning = ResourceVersionPolicy.VersionedUpdate,
                                ConditionalRead = ConditionalReadStatus.FullSupport,
                                ConditionalDelete = ConditionalDeleteStatus.Multiple,
                                ReferencePolicy = [],
                                SearchParam = new List<SearchParamComponent>()
                                {
                                    new SearchParamComponent
                                    {
                                        Type = SearchParamType.String
                                    }
                                }
                            },


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
                             },
                        },
                        Operation = new List<OperationComponent>
                        {
                            new OperationComponent
                            {
                                Name = "export",
                                Definition = ""
                            }
                        }
                    },
                },
                Messaging = new List<MessagingComponent>
                {
                    new MessagingComponent
                    {
                        SupportedMessage = new List<SupportedMessageComponent>
                        {
                            new SupportedMessageComponent
                            {
                                Mode = EventCapabilityMode.Receiver
                            },
                            new SupportedMessageComponent
                            {
                                Mode = EventCapabilityMode.Sender
                            }
                        }
                    }
                },
                Document = new List<DocumentComponent>
                {
                    new DocumentComponent
                    {
                        Mode = DocumentMode.Consumer
                    },
                    new DocumentComponent
                    {
                        Mode = DocumentMode.Producer
                    }
                }
            };

            var jsonSerializer = new FhirJsonSerializer();
            return Content(jsonSerializer.SerializeToString(capabilityStatement), "application/fhir+json");
        }
    }
}

