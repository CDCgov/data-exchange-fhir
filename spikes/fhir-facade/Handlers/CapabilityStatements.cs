using Hl7.Fhir.Model;
using static Hl7.Fhir.Model.CapabilityStatement;

namespace OneCDPFHIRFacade.Handlers
{
    public static class CapabilityStatements
    {
        public static CapabilityStatement CreateCapabilityStatement()
        {
            // Create a new CapabilityStatement object
            CapabilityStatement capabilityStatement = new CapabilityStatement
            {
                // Set basic information
                Title = "Capability Statement",
                Url = "",
                FhirVersion = FHIRVersion.N4_0_1,
                Name = "One CDP FHIR Facade Capability Statement",
                Status = PublicationStatus.Active,
                Experimental = true,
                Date = DateTime.Now.ToString(),
                Publisher = "CDC 1CDP FHIR Facade",
                Kind = CapabilityStatementKind.Instance,

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
            };
            return capabilityStatement;
        }


    }
}