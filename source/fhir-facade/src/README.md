## Overview
Code Repository for the CDC FHIR Façade solution. The purpose of this FHIR Facade is ingestion and storeage of FHIR data with additional features for authentication, 
authorization, and observability. Proof of concept built of the FHIR Facade is using AWS cloud infrastructure.

### Application Code Instructions
## Run Locally with AWS
- When running the application local with AWS setting, bundles, logs, and Open Telemetry will be saved to the assigned AWS account. This can be set in appsetting. 
# Steps
- Step 1: Update appsettings.Development.json file. Make sure to include AccessKey and SecretKey. "RunEnvironment": Needs to be set to "AWS". 
- Steo 2: Run application by pressing Start Application (F5).
- Step 3: Get an Authentication token and insert Bearer Token into Authorization.
- Step 4: In postman under body, either past a raw Bundle or upload file under form-data. Key needs to be "file" of type File.
- Step 5: Sent a Post request to http://localhost:5215/Bundle

*Note: File size can not exceed 300mb*

## Run Locally without AWS
- When running the application local without AWS bundles will be saved on our computer and logs will be writen to the console. to the assigned AWS account. This can be set in appsetting. 
# Steps
- Step 1: Update appsettings.Development.json file. "RunEnvironment": Needs to be set to "Local"."LocalDevFolder" needs to be set to the location you want the Bundle file to be saved.
- Step 2: OneCDPFHIRFacade.csproj under <DefineConstants>runLocal</DefineConstants> needs to be set to "RunLocal".
- Steo 3: Run application by pressing Start Application (F5).
- Step 4: In postman under body, either past a raw Bundle or upload file under form-data. Key needs to be "file" of type File.
- Step 5: Sent a Post request to http://localhost:5215/Bundle

### Docker Instructions
## Docker Build Instructions
docker build -t one-cdp-fhir-facade .

## Docker Run Instructions
docker run -p 8080:8080 -p 80:80 one-cdp-fhir-facade
