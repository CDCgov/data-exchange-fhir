# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

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
