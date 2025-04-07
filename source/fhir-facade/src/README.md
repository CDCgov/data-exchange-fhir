## Overview
Code Repository for the CDC FHIR Façade solution. The purpose of this FHIR Facade is ingestion and storage of FHIR data with additional features for authentication, 
authorization, and observability. Proof of concept built of the FHIR Facade is using AWS cloud infrastructure.

### Application Code Instructions
## Run Locally with AWS
# PreRequisites:
- An AWS account needs to be setup in before running the application locally. The account will need the following:
    - S3 Bunket with write access for Budles, Logs, and OpenTelemetry to be saved to it.
    - Log Group so that Logs can be written to it.
    - IAM User with access to read and write to S3 Bunket and Log Group. 
    - AWS Instruction - https://docs.aws.amazon.com/AmazonS3/latest/userguide/walkthrough1.html

- When running the application locally with AWS setting, bundles, logs, and Open Telemetry will be saved to the assigned AWS account. 
- Make sure to change appsettings.Local.json according to YOUR AWS settings.
- In Properties -> launchSettings.json -> Set 
     "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Local"
      }
    This should be changed in three different sections.

# Steps
- Step 1: Update appsettings.Local.json file. 
  - AccessKey and SecretKey need to be set from the AWS IAM user that was created above. 
  - "RunEnvironment": Needs to be set to "AWS". 
  - "VerifyAuthURL": Needs to be set according to the Token Signing Key URL
    - Can be found here: Amazon Cognito -> User Pool -> "pool_Name" -> Token Signing Key URL
- Step 3: If using Visual Studio, Run application by pressing Start Application button or press F5.
    - Command Line: cd "ProjectLocation" -> dotnet run
- Step 4: You will need an Authorize your POST Bundle request before sending the request.
    - a) In Swagger under POST Auth you will need to change the client_id and client_secret according to your AWS settings. 
     - This can be setup/found under AWS Cognito -> User Pool -> YourUserPoolName -> App Client (https://docs.aws.amazon.com/cognito/latest/developerguide/cognito-user-pools.html)
   - b) Send POST request and copy access_token from response body.
   - Under POST Bundle OK -> Authorization -> Auth Type Pick "Bearer Token" -> Paste Token in the provided token box
- Step 5: There are two different formats that a bundle can be sent.
    - a) A raw json fromated Bundle can be paste into the Body -> raw field of the POST Bundle OK request.
    - b) A json formated Bundle file can be uploaded to Post a bundle. Body -> form-data -> Key = File -> Format Type = File -> Value = Select file from local machine
- Step 6: Sent a Post request to http://localhost:5215/Bundle

*Note: File size can not exceed 300mb*

## Run Locally without AWS
- When running the application local without AWS settings the Bundle will get saved on your computer and logs will be written to the console. This can be set in appsetting. 

# Steps
- Step 1: Update appsettings.Local.json file. 
    - a) "RunEnvironment": "Local",
    - b) "FileSettings": {"LocalDevFolder": "BundleToBeSaved_FolderPath"}, 
        - Set this to the folder you want the bundle to be saved to. Ex."FileSettings": {"LocalDevFolder": "C:\Users\Desktop\Dev\Bundles"},
- Step 2: OneCDPFHIRFacade.csproj (The Project file) under <PropertyGroup> -> <DefineConstants>AWS</DefineConstants> needs to be set to "runLocal" <DefineConstants>runLocal</DefineConstants>.
- Step 3: Run Applocation:
  - Visual Studio: Run application by pressing the green Start Application button or press F5.
  - Command Line: cd "ProjectLocation" -> dotnet run
- Step 4: There are two different formats that a bundle can be sent.
    - a) A raw json fromated Bundle can be paste into the Body -> raw field of the POST Bundle OK request.
    - b) A json formated Bundle file can be uploaded to Post a bundle. Body -> form-data -> Key = File -> Format Type = File -> Value = Select file from local machine
- Step 5: Sent a Post request to http://localhost:5215/Bundle

- *Note: File size can not exceed 300mb*


### Docker Instructions
## Docker Build Instructions
docker build -t one-cdp-fhir-facade .

## Docker Run Instructions
docker run -p 8080:8080 -p 80:80 one-cdp-fhir-facade
