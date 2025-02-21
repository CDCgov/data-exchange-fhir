# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## 2.0.2.alpha Pre-release 2024-02-20
	- Discover FHIR Facade bottlenecks and benchmarks 
	- JWT check between API Gateway and FHIR Facade code
	- CI/CD process for TST environment 
	- Validate scope in token 
	- Check logging when running locally 
	- Implement SAMS integration with Cognito 
	- Inferno test suite troubleshooting 
	- Validate request ID during authentication 
	- Terraform API Gateway and Cognito in DEV environment 
	- Apply Terraform changes to TST environment 
	- CD pipeline in DEV environment 
	- Resolve bundle ID requirement 
	- Investigate FHIR Facade activity dashboard 
	- Review app, infrastructure, CI/CD code 
	- Unit test updates
## What's Changed
	- Updates to serialization by @swmuirdhie in #321
	- Updates for logging and docker by @swmuirdhie in #322
	- Skip validation when running locally No Aws settings when running locally by @csalmi56 in #323
	- Request id in start up and opentelemetry cleanup by @csalmi56 in #320
	- Create big bundle sizes with Synthea by @csalmi56 in #319
	- Capability statement redone according to inferno test by @csalmi56 in #324
	- Fixed logging format by @csalmi56 in #326
	- deploy to dev by @rohitpanwar in #327
	- deploy to dev by @rohitpanwar in #328
	- deploy to tst by @rohitpanwar in #329
	- pull vales from secrets by @rohitpanwar in #331
	- Support large bundle file up to 300mb by @csalmi56 in #330
	- Review code updates by @csalmi56 in #332
	- Fhir 1087 unit test updates initial fix by @swmuirdhie in #334

## 2.0.1.alpha Pre-release 2024-02-06
	- Updates to logging feature
	- Updates to authentication and scopes
	- Updates to general structure for unit tests
## What's Changed
	- Initial version of unit test and project solution structure by @swmuirdhie in #302
	- Added missing gitignore file by @swmuirdhie in #306
	- Logsto s3 by @csalmi56 in #316
	- FHIR-1057: Added README file by @svalluripalli in #315
	- Implementing Authentication into Fhir Facade by @csalmi56 in #313




## [0.3.5] 2024-04-10
	- Pre-release before STG/PRD
	- Data Export: 
		- Replace strings with interpolated strings.
		- Add a try/catch blog around config/keyVault to catch issues with connecting with the keyvault.
		- Remove double date from Data Export logs.


## [0.3.4] 2024-04-10
	- Pre-release before STG/PRD
	- Process_Message: 
		- Replace strings with interpolated strings
		- Add a try/catch blog around config/keyVault to catch issues with connecting with the keyvault.
		- Add "Process Message: " in front of process message logs.
		- Make process message log messages shorter.

## 2024-09-24
	- Pre-release before STG/PRD
	- Process_Message and Data Export: Separate App configurations 
	- Remove the combined App configuration.

## [0.3.3] - 2024-09-18
	- Data Export function: Update change log so that PII information is not logged.

## [0.3.2] - 2024-07-18
	- Pre-release before STG/PRD
	- Lining up Main branch from Dev branch
	- Purge function changes purge setting from Days to Hours

## [0.3.1] - 2024-07-03
	- Pre-release before STG/PRD
	- Process message function: changes to error checking for header, for empty or bad payload, and improved logs
	- IG Utility initial commit
	- Deleted terraform folder and files since these is moved to Github/CDCEnt
	- Actual release for the previous changelog entry 0.3.0


## [0.3.0] - 2023-10-27
	- Added DataPurge function as a timed function to remove records from the fire after a period of minimum retention
	- Added manual bulk uploading utility to repo
	- Updated $process-message http response codes
	- Includes various fixes for compliance

## [0.2.0] - 2023-10-02
	- Functional version used at September 2023 Connectathon
	- Moved functionality of event-triggered processing merged into single function app with data exporting
	- Removed now deprecated event function app
	- Updated to $process-message implementation to ingest entire bundle instead content bundle only
	- Added configuration option to export to multiple datalakes based on FHIR contents


## [0.1.0] - 2023-05-06
	- Functional version used at May 2023 Connectathon
	- Added initial functionality for $process-message
	- Added initial functionality for event-triggered processing of newly ingested FHIR Bundles
	- Added initial functionality for processing and exporting FHIR data to datalake
	- Added initial configuration for APIM 
	- Added initial implementation of Azure Logic App Bulk Export utility
