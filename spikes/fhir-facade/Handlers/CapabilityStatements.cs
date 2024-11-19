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
                Url = "https://onecdpfhirfacade/metadata",
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
                                            System = "http://hl7.org/fhir/restful-security-service",
                                            Code = "oauth2",
                                            Display = "OAuth2"
                                        }
                                    }
                                },
                            },
                        },
                        // Add a custom extension to indicate 'Facade'
                        Extension = new List<Hl7.Fhir.Model.Extension>
                        {
                            new Hl7.Fhir.Model.Extension
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
                                Type = "Patient",
                                SearchParam = new List<SearchParamComponent>()
                                {
                                    new SearchParamComponent { Name = "family" },
                                    new SearchParamComponent { Name = "given" }
                                },
                                Interaction = new List<ResourceInteractionComponent>
                                {
                                    new ResourceInteractionComponent
                                    {
                                        Code = TypeRestfulInteraction.Read
                                    },
                                    new ResourceInteractionComponent
                                    {
                                        Code = TypeRestfulInteraction.SearchType
                                    },
                                    new ResourceInteractionComponent
                                    {
                                        Code = TypeRestfulInteraction.Create
                                    },
                                    new ResourceInteractionComponent
                                    {
                                        Code = TypeRestfulInteraction.Update
                                    },
                                    new ResourceInteractionComponent
                                    {
                                        Code = TypeRestfulInteraction.Delete
                                    }
                                },
                            },

                        },
                    },
                },
            };
            return capabilityStatement;
        }


    }
}