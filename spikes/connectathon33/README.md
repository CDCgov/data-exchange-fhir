An **HL7 FHIR Connectathon** is an event where healthcare professionals, developers, and technology vendors come together to work on the implementation of the **FHIR (Fast Healthcare Interoperability Resources)** standard, which is used for exchanging healthcare data electronically. The main purpose of a Connectathon is to facilitate collaboration, innovation, and testing among different stakeholders in the healthcare technology ecosystem. 

### Key features of an HL7 FHIR Connectathon:

1. **Collaboration**: 
   Participants—including developers, vendors, and clinicians—work together to implement and test FHIR-based applications, services, and solutions. This collaborative environment fosters the sharing of knowledge and ideas to solve real-world healthcare interoperability challenges.

2. **Hands-on Testing**:
   The event is designed to encourage practical testing, allowing teams to test their solutions with other systems and partners to ensure they can work together. This is important because healthcare data often comes from a variety of different sources, such as hospitals, clinics, and insurance providers.

3. **Real-time Problem Solving**: 
   Attendees typically work through specific use cases related to healthcare interoperability, such as electronic health records (EHR), patient data sharing, clinical decision support, and more. They can troubleshoot and resolve issues in real time.

4. **Education and Training**:
   Newcomers to FHIR can attend training sessions, tutorials, and demonstrations that help them understand how the standard works and how to implement it effectively. 

5. **Standard Adoption**:
   These events are also a way for the HL7 community to encourage widespread adoption of the FHIR standard, providing an opportunity to demonstrate the real-world applicability and benefits of using FHIR for data exchange in healthcare.

6. **Diverse Participation**:
   Participants range from small startups to large health IT vendors, government agencies, and academic institutions. This diversity ensures that the resulting innovations and solutions can address a wide range of healthcare needs.

### Typical Event Structure:
- **Introduction & Overview**: Briefings about the event goals, FHIR standards, and technical architecture.
- **Team Formation**: Participants may join existing teams or form new ones based on specific healthcare use cases or interests.
- **Testing & Collaboration**: Teams begin coding, testing, and exchanging data, often guided by specific scenarios or challenges.
- **Presentation & Review**: After the event, teams usually present their solutions and results. They share the lessons learned and any issues encountered.
- **Networking & Future Plans**: The event ends with discussions about next steps, collaboration opportunities, and other ways to continue developing solutions using FHIR.

In summary, a **FHIR Connectathon** is an event that drives innovation, promotes education, and ensures the practical testing and adoption of the FHIR standard for improving healthcare interoperability across systems and providers.


The **HL7 FHIR Connectathon 33** is a specific iteration of the FHIR Connectathon event, which took place in January 2025. It included various tracks, each focusing on different use cases, technologies, or healthcare domains. Participants in these tracks worked on implementing FHIR solutions to meet specific interoperability challenges.

Here are some examples of the **tracks** from HL7 FHIR Connectathon 33:

### 1. **FHIR to CDA (Clinical Document Architecture) Interoperability**  
   - Focuses on the interoperability between FHIR and CDA documents. Participants explore methods to exchange clinical documents using FHIR resources and integrate them with the CDA standard.

### 2. **FHIR Patient Experience and Identity Management**  
   - Focuses on patient identity management and improving the patient experience by leveraging FHIR to manage and exchange patient data effectively. This track explores solutions to ensure accurate patient identification and seamless data sharing.

### 3. **FHIR for Public Health**  
   - This track explores FHIR's role in public health data exchange, including the management of disease surveillance, vaccination data, and other public health reporting systems.

### 4. **FHIR for Interoperable EHR Systems**  
   - Focuses on using FHIR to enable different electronic health record (EHR) systems to exchange data, ensuring that patient data can move seamlessly across various platforms and settings.

### 5. **FHIR and Smart on FHIR**  
   - Involves developing applications using FHIR and Smart on FHIR technologies. This track explores building healthcare apps that can work with FHIR-based data and be integrated with EHRs, offering clinical decision support or other functionalities.

### 6. **FHIR for Mobile Health (mHealth)**  
   - This track focuses on using FHIR in mobile health applications, where participants build solutions that allow mobile apps to interact with healthcare data for improved patient engagement and care delivery.

### 7. **FHIR for Laboratory Data Exchange**  
   - A track dedicated to using FHIR for managing and exchanging laboratory test results, promoting the interoperability of lab systems with EHRs and other healthcare applications.

### 8. **FHIR for Medication Management**  
   - Focuses on using FHIR to manage medication orders, prescriptions, and medication administration records. This track looks at how FHIR can support accurate medication reconciliation and tracking.

### 9. **FHIR for Imaging and Radiology**  
   - Aimed at integrating FHIR with imaging systems and radiology workflows, exploring how imaging data (such as DICOM) can be exchanged through FHIR and tied to other clinical information.

### 10. **FHIR for Telemedicine and Remote Monitoring**  
   - Focuses on the use of FHIR in telemedicine applications, especially for remote patient monitoring, ensuring that patient data can be shared across platforms for virtual care.

### 11. **FHIR for Payer and Claims Data**  
   - This track looks at how FHIR can be applied to payer systems for managing claims, prior authorization, and other aspects of health insurance data management.

### 12. **FHIR for Research Data**  
   - Participants in this track work on using FHIR to support research data, enabling the exchange of clinical trial information and supporting research workflows with standardized data.

### 13. **FHIR for Digital Health Apps**  
   - Explores the use of FHIR in digital health applications such as wearable devices, health trackers, and fitness apps, integrating them with clinical data and workflows.

### 14. **FHIR Implementation Guides and Tools**  
   - This track focuses on creating and testing FHIR Implementation Guides, which provide detailed instructions for implementing FHIR resources in specific use cases.

### 15. **FHIR for Clinical Decision Support (CDS)**  
   - Participants develop solutions that use FHIR to integrate clinical decision support systems into the healthcare workflow, leveraging FHIR-based data to offer decision support to clinicians.

### 16. **FHIR for Data Security and Privacy**  
   - Focuses on ensuring that FHIR implementations adhere to security and privacy standards such as HIPAA, and investigates solutions for securing data exchange and patient privacy.

### 17. **FHIR for Blockchain and Distributed Ledger Technologies**  
   - This track explores integrating FHIR with blockchain technology, which can be used to ensure secure, verifiable, and auditable exchanges of healthcare data.

### 18. **FHIR for Social Determinants of Health (SDOH)**  
   - This track focuses on integrating FHIR with data related to social determinants of health, such as socioeconomic factors, housing, and education, to improve healthcare outcomes.

These are just some examples of the tracks from the **HL7 FHIR Connectathon 33**. The tracks are typically diverse, focusing on various technical, clinical, and operational challenges in healthcare, and they evolve based on emerging needs and innovations in the field.


Medmorph Track Report

# MedMorph - Flu-Surv-NET - Health Care Surveys

## What was the track trying to achieve?

The MedMorph IG and the two related Content IGs (Flu-Serv-NET and HCS) have been the subject of multiple Connectathons in the past. The key focus of this track could be summarized as follows:

### Infrastructure Testing

A major accomplishment of this track is the fact that all of the Public Health Authority (PHA)-side processes were performed in a tenant of the CDC’s Azure Subscription (i.e., DEX FHIR server). Prior Connectathon events were performed using non-CDC environments.

- Demonstrate the ability of the infrastructure to support many submissions from many submitters at the same time.
    - To support the objective of enabling many submissions from many simulated EMR FHIR servers and one EMR vendor’s sandbox (performed by Dragon against a Cerner sandbox with support from Isaac). In preparation for the Track, we established ten fictitious EMR environments that represented “the Hospital for a fictitious County." These environments were essentially R4 FHIR servers hosted by IOI and hydrated with fictitious data using PatientGen. These ten “Counties” were grouped together to form the fictitious state of “Anywhere, USA” (which is related to the bonus track results – more below).
    - We demonstrated the ability of the infrastructure to support submissions from many endpoints simultaneously to the same PHA target.

To enable participants to do this, we leveraged the extant HDEA Reference Implementation and enhanced it to support submitting data by Content IG type from “their county’s” FHIR (See Illustrations below). Using this tool, participants were asked to submit many bundles of both types as a group.

- The outcome of this happy path testing was verification that the infrastructure performed as desired, including:
    - Not only the receipt of the bundle and its insertion into the PHA FHIR server (i.e., the CDC FHIR server in the CDC Azure Subscription) but also the subsequent loading of data to the data-lake (which was not the CDC’s EDAV, but a proxy stood up to represent it for the purpose of the track).
    - The infrastructure test was successful, with multiple submissions from multiple senders performed in unison, with all submitted data being received and subsequently promulgated to the CDC-hosted FHIR server and the related data lake. Bundle Validation was turned off for this verification step as the track objectives included both:
        - **Positive Testing** (when a well-formed bundle conformant to the Content IG is submitted)
        - **Negative Testing** (where the bundles are not conformant to the Content IGs [Flu-Serv-NET and HCS]).

### Positive Testing of Bundle Validation

In this segment of the Connectathon, we turned on Bundle Validation on both the HDEA side and the PHA side of the exchange. During this session, users submitted well-formed bundles (generated using the enhanced HDEA App) associated with the two content IGs. The participating engineers (from Newwave and Microsoft) evaluated logs and confirmed successful submissions and correlating records. This meant that the bundles created by the HDEA (against the “County” FHIR server of a given user) were loaded to both the FHIR server and the data lake as expected.

- With Validation turned on, we learned that some of the value sets used to generate the contents of our fictitious County’s FHIR server were inconsistent with the value sets as published. The fix for this is to correct the value sets in the FHIR server that are used by PatientGen to generate the relevant resources.
  
- Our participating subject matter experts (SMEs) initially flagged the data being generated by HCS as being different than expected, but upon further evaluation (with the help of Dragon), we verified that the HCS data were being generated in accordance with the published Content IG for HCS (which is intended to be further constrained by HCS report-specific Content IGs). Hence, while the data in the bundle and their size were a surprise to the SME, once Dragon clarified that the published IG was a broad-spectrum IG, and once the next set of HCS-related updates are made (given existing Jira tickets and the subsequent process), we will have updated Content IGs that would align with her expectations (e.g., one encounter per HCS bundle).

### Negative Testing of Bundle Validation

We performed Negative testing, where each participant was given a pair of resources (generated by the HDEA) and asked to “perturb” them before submitting them to the PHA endpoint (i.e., the CDC FHIR Service). The bundles failed as expected.

- We are currently doing the analysis to confirm that the reason why things failed and how those failures in the backend are logged align with the “perturbations” made by the participants. Microsoft will provide an update once they’ve investigated further.
