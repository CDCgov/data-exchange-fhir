# CDC FHIR Façade (FHIR Lighweight Ingest)

## Overview
Code Repository for the CDC FHIR Façade solution. The purpose of this FHIR Facade is ingestion and storeage of FHIR data with additional features for authentication, authorization, and observability. Proof of concept built of the FHIR Facade is using AWS cloud infrastructure.

## Content
   /source/Fhir-Façade  – The one CDC – OneCDPFHIRFacade project

### OneCDPFHIRFacade project: 

src/Config – Helper functions for setting up the services
src/Controller – API Endpoints configuration
src/Docs – Contains a Postman Collection test local file
src/Service – These .cs functions help with writing to S3 or Local file, and Logging to cloud watch.
src/Utilities – Contains a Logging class that logs messages to both CloudWatch and S3.
tests/fhir-façade-tests - This folder contains unit test for the OneCDPFHIRFacade project.

### OneCDP FHIR Facade - Bundle Controller:
The BundleController is part of the OneCDPFHIRFacade project, responsible for handling FHIR bundle requests. This controller parses incoming FHIR bundles, validates their content, and saves them either locally or to AWS S3, based on the application configuration.

### This implementation leverages:
•	HL7 FHIR library for parsing and handling FHIR-compliant data.
•	Microsoft ASP .NET Core for API development.
•	Custom utilities and services for logging and file storage.

####	FHIR Bundle Parsing:
•	Accepts incoming JSON payloads.
•	Parses the payload into FHIR Bundle objects using the FhirJsonParser.

####	Validation:
•	Ensures the incoming JSON is HL7 FHIR-compliant (currently Hl7.Fhir.R4 is checked) payload and checks the presence of a Bundle Id in the payload.
•	Logs and returns appropriate error messages for invalid payloads.

####	Configurable File Storage:
•	Saves the parsed FHIR bundle either locally or in AWS S3 based on configuration.
•	Utilizes LocalFileService for local storage and S3FileService for AWS S3 storage.

####	Robust Logging:
•	Logs request activity and errors to a cloud-based logging utility.
•	Optionally stores log files in AWS S3 for persistence.

POST /Bundle
Purpose: Handles incoming FHIR bundle requests.
Request:
•	Body: A valid FHIR Bundle in JSON format.
Response:
•	200 OK: Successfully processed the bundle.
•	400 Bad Request: Invalid JSON payload or missing Bundle Id.
•	401 Unauthorized: User not authorized
•	500 Internal Server Error: Issues with AWS S3 configuration or unexpected errors.

## Scope-Based Authorization Configuration
The application can be customized to check specific FHIR scopes for access.

## Logging Configuration

1.	LoggerService
Handles logging to CloudWatch Logs or the console based on the environment:
•	CloudWatch Logs: Appends logs to AWS CloudWatch with timestamped log groups and streams.
•	Console: Writes logs to the local console during development.

2.	LogToS3FileService
Manages logging to Amazon S3:
•	Aggregates log messages.
•	Saves logs in JSON format to a specified S3 bucket.

3.	LoggingUtility
A utility class that orchestrates:
•	Logging messages to both CloudWatch and S3.
•	Collecting log messages for batch processing.
How it works:
Use the LoggingUtility.Logging() method to log messages as JSON.
•	Logs are written to CloudWatch Logs or the console.
•	Messages are stored in a list for saving to S3.
Save Logs to S3:
Use the LoggingUtility.SaveLogS3() method to save all aggregated logs to Amazon S3.

## Technologies
•	C#/.NET Core: API development.
•	HL7 FHIR Library: Parsing and handling FHIR resources.
•	AWS S3 SDK: Cloud storage.
•	ASP .NET Core MVC: Web API framework.



