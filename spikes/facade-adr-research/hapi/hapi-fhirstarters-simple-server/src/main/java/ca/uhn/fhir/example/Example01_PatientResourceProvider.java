package ca.uhn.fhir.example;

import java.io.File;
import java.io.FileInputStream;
import java.util.HashMap;
import java.util.Map;
import java.util.Properties;
import java.util.UUID;

import org.hl7.fhir.instance.model.api.IBaseResource;
import org.hl7.fhir.r4.model.IdType;
import org.hl7.fhir.r4.model.OperationOutcome;
import org.hl7.fhir.r4.model.Patient;

import com.amazonaws.auth.AWSStaticCredentialsProvider;
import com.amazonaws.auth.BasicAWSCredentials;
import com.amazonaws.services.s3.AmazonS3;
import com.amazonaws.services.s3.AmazonS3Client;
import com.amazonaws.services.s3.model.ObjectMetadata;
import com.amazonaws.services.s3.model.PutObjectRequest;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.google.gson.Gson;

import ca.uhn.fhir.rest.annotation.Create;
import ca.uhn.fhir.rest.annotation.IdParam;
import ca.uhn.fhir.rest.annotation.Read;
import ca.uhn.fhir.rest.annotation.ResourceParam;
import ca.uhn.fhir.rest.api.MethodOutcome;
import ca.uhn.fhir.rest.server.IResourceProvider;
import ca.uhn.fhir.rest.server.exceptions.ResourceNotFoundException;
import ca.uhn.fhir.rest.server.exceptions.UnprocessableEntityException;

public class Example01_PatientResourceProvider implements IResourceProvider {

	private Map<String, Patient> myPatients = new HashMap<String, Patient>();

	/**
	 * Constructor
	 */
	public Example01_PatientResourceProvider() {
		Patient pat1 = new Patient();
		pat1.setId("1");
		pat1.addIdentifier().setSystem("http://acme.com/MRNs").setValue("7000135");
		pat1.addName().setFamily("Simpson").addGiven("Homer").addGiven("J");
		myPatients.put("1", pat1);
	}

	@Override
	public Class<? extends IBaseResource> getResourceType() {
		return Patient.class;
	}

	/**
	 * Simple implementation of the "read" method
	 * Search Patient by ID
	 */
	@Read()
	public Patient read(@IdParam IdType theId) {
		Patient retVal = myPatients.get(theId.getIdPart());
		if (retVal == null) {
			throw new ResourceNotFoundException(theId);
		}
		return retVal;
	}

	/**
	 * Receive a Patient bundle to process. This method prints the Patient bundle to console
	 * and also sends to S3 bucket.
	 * @param thePatient
	 * @return MethodOutcome
	 */
	@Create
	public MethodOutcome createPatient(@ResourceParam Patient thePatient) {

		/*
		 * First we might want to do business validation. The
		 * UnprocessableEntityException results in an HTTP 422, which is appropriate for
		 * business rule failure
		 */
		if (thePatient.getIdentifierFirstRep().isEmpty()) {
			/*
			 * It is also possible to pass an OperationOutcome resource to the
			 * UnprocessableEntityException if you want to return a custom populated
			 * OperationOutcome. Otherwise, a simple one is created using the string
			 * supplied below.
			 */
			throw new UnprocessableEntityException("No identifier supplied");
		}

		// Save this patient to the database...
		printPatientToConsole(thePatient);
		savePatientToS3(thePatient);

		// This method returns a MethodOutcome object which contains
		// the ID (composed of the type Patient, the logical ID 3746, and the
		// version ID 1)
		MethodOutcome retVal = new MethodOutcome();
		retVal.setId(new IdType("Patient", "3746", "1"));

		// You can also add an OperationOutcome resource to return
		// This part is optional though:
		OperationOutcome outcome = new OperationOutcome();
		outcome.addIssue().setDiagnostics("One minor issue detected");
		retVal.setOperationOutcome(outcome);

		return retVal;
	}

	private void savePatientToS3(Patient thePatient) {
		ObjectMapper objectMapper = new ObjectMapper();
		ObjectMetadata omd = new ObjectMetadata();
		Properties prop = new Properties();				
		String clientRegion ="us-east-1";
		String bucketName = "dexfhirbucket";		
		UUID anID = UUID.randomUUID();
		String filename = anID.toString();
        String aFile = filename + ".json";
		try {
			FileInputStream fin = new FileInputStream("fhir.properties");
			prop.load(fin);
			String arn= prop.getProperty("ARN");
			String accessKey= prop.getProperty("Accesskey");
			String secretKey=  prop.getProperty("SecretToken");
			File objectKey = new File(aFile);
			Gson gson = new Gson();
			String jsonString = gson.toJson(thePatient);
			objectMapper.writeValue(objectKey, jsonString);
			PutObjectRequest request = new PutObjectRequest(bucketName, aFile, objectKey);			
			BasicAWSCredentials creds = new BasicAWSCredentials(accessKey, secretKey);
	        AmazonS3 s3Client = AmazonS3Client.builder()
	                .withRegion(clientRegion)
	                .withCredentials(new AWSStaticCredentialsProvider(creds))
	                .build();	       	       
	        s3Client.putObject(request);
		} catch (Exception exp) {
			exp.printStackTrace();
		}
	}

	private void printPatientToConsole(Patient thePatient) {
		Gson gson = new Gson();
		String jsonString = gson.toJson(thePatient);
		myPatients.put(thePatient.getId(), thePatient);
		System.out.println("Received the bundle");
		System.out.println(jsonString);
	}

}
