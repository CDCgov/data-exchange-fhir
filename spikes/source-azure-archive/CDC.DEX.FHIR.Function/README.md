# Data Exchange (DEX) FHIR 


DEX FHIR solution as part of the DEX platform/ingest solution.

This was build in the Azure cloud infrastructute and it has been decomissioned.
​
DEX FHIR first use case was NCHS Health Care Surveys which uses $process-message ingest as detailed by respective FHIR Implementation Guide (IG).

DEX FHIR, Azure build, consists of three .NET Function Applications: Processs Message, Data Export, and Data Purge and other used Azure resources: Azure Health Data Services (FHIR Service), Azure Message Bus, and Microsoft open sourced .NET function [Azure ONC (g)(10) SMART on FHIR Sample](https://github.com/Azure-Samples/azure-health-data-and-ai-samples/tree/9e8204dd58a6d4415f93dca1f3ab53d18dfd954e/samples/Patient%20and%20Population%20Services%20G10).

## ProcessMessage

The ProcessMessage application is an Azure Function that processes FHIR (Fast Healthcare Interoperability Resources) FHIR messages. It includes the main Run method that handles HTTP requests to process FHIR bundles, validating them against a FHIR server, and creating FHIR resources on the FHIR Server for further downstream processing. 

$process-message https://www.hl7.org/fhir/messageheader-operation-process-message.html is FHIR standard operation that support the ability to expose message paradigm posts operations

This operation accepts a message, processes it according to the definition of the event in the message header, and returns one or more response messages.

In addition to processing the message event, a **server may choose to retain all or some the resources** and make them available on a RESTful interface, but is **not required to do so**.

The resources is retained for a configurable period of time but it is not available via a RESTful interface.


This function is designed for handling FHIR resource bundles in healthcare systems, ensuring validation and secure forwarding to a FHIR-compliant server. It provides robust error handling and detailed logging for debugging and monitoring. 

The ProcessMessage function ensures the Authorization header exists, checks that the request body is not empty and attempts to deserialize the body into JSON. 

If validation is not skipped (based on configuration), the function validates the JSON payload using the FHIR Server's $validate endpoint. Then, the function logs validation results If the bundle is valid, it is posted to a FHIR server's Bundle endpoint.  

The function app will respond with http status of 201 - Created on success on success, or with http status of 422 - Unprocessable Entity if validation fails. If the header is missing, then the function app will respond with http status of 401 - Unauthorized. 

In addition, this function logs processing information that will help with debugging errors. The logs include received request, validation outcomes, and duration of key operations. 

## DataExport

The DataExport application is an Azure Function, that listens to a Service Bus queue for recently created FHIR resource messages and then exports these FHIR resources to an EDAV Data Lake Storage account.  

The Data Export function reads and processes the FHIR resource from Service Bus message using the FhirEventProcessor function. It then converts the resource into a JObject for further manipulation. 

The configuration determines of the JSON should be flattened or unbundled. Based on the configuration and resource metadata, determines the destination storage account and blob container. 

If the Bundle needs to be unbundled, it extracts individual resources from the FHIR Bundle and recursively processes nested bundles. Of it needs to get flattened, it converts nested JSON into a flat key-value structure for easier export and analysis. 

The Data Export function uses Azure ClientSecretCredential to authenticate with the EDAV storage account. It then logs this for debugging purposes.  

Lastly, the function writes the processed FHIR resources as files to EDAV Data Lake Storage Account.  

This function handles complex data transformation tasks like unbundling nested FHIR bundles, flattening JSON for simplified storage, and dynamically determining storage destinations based on resource metadata. 


## DataPurge

The DataPurge application is a crohn based service designed to remove client data from fhir repository at scheduled intervals

##  Shared Code
The shared code that is common functionality between functions.


# Source Code

## CDC.DEX.FHIR.Function.DataExport

Applications Data Export and Data Purge are located in this project in respective C Sharp components

FhirEventProcessor is logic to listen in on the FHIR bundle create queue and pull the bundle from the FHIR endpoint

StartupConfiguration is the logic to read configurations from various azure components

## CDC.DEX.FHIR.Function.ProcessMessage

Process Message logic is contained in the respective C Sharp components
Process message listens in on rest services and can validate FHIR bundle if so configured then push the Bundle to the AZURE FHIR services

StartupConfiguration is the logic to read configurations from various azure components
