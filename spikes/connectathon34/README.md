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


The **HL7 FHIR Connectathon 34** took place on September 9-10, 2023, in Phoenix, Arizona. This in-person event focused on various healthcare interoperability challenges, bringing together professionals to test and advance the **FHIR (Fast Healthcare Interoperability Resources)** standard.

**Key Tracks from Connectathon 34:**

1. **Clinical Reasoning Track**: Led by the CMS eCQM Standards Team, this track concentrated on testing FHIR-based quality measures, aiming to enhance clinical decision support and quality measurement. citeturn0search0

2. **Da Vinci Tracks**: These tracks, part of the Da Vinci Project, focused on payer-provider data exchange, including the Payer Data Exchange (PDex) and Formulary. Participants worked on improving interoperability between payers and providers. citeturn0search1

3. **Risk Adjustment Track**: This track addressed the use of FHIR in risk adjustment processes, aiming to improve the accuracy and efficiency of risk assessments in healthcare. citeturn0search3

4. **MedMorph Track**: Focused on public health data exchange, this track explored the use of FHIR for reporting and surveillance, including initiatives like RESP-NET and Health Care Surveys. citeturn0search6

5. **PACIO Advance Directive Interoperability**: This track aimed to enhance the interoperability of advance directives, ensuring that patient preferences are accurately represented and accessible across systems. citeturn0search6

These tracks exemplify the collaborative efforts to advance healthcare interoperability through the FHIR standard, addressing various aspects of data exchange, quality measurement, and public health reporting.


# MedMorph - RESP-NET - Health Care Surveys

## What was the track trying to achieve?

The purpose of this track was to advance the **Making Electronic Data More Available for Research and Public Health (MedMorph)** project. This initiative seeks to improve public health and patient-centered outcomes by leveraging emerging health data and exchange standards, such as **Health Level 7 (HL7®)** Fast Healthcare Interoperability Resources (**FHIR®**), to develop and implement an interoperable solution that will enable access to clinical data.

As part of **Connectathon 34** objectives, the plan was to use the **EPIC**, **Cerner**, and **MELD sandboxes** to create Healthcare Survey bundles. These bundles were tested for the following workflows:
- Provisioning
- Notification
- Report Creation
- Report Submission

The goal was to test the creation of the bundles based on the given knowledge artifact and successfully load the bundle in the **EDAV System** after all required validations were completed and passed.


# MedMorph - RESP-NET - Health Care Surveys Tasks

1. Reviewed the MedMorph Reference Architecture, Provisioning Workflow, Notification, Report Creation, and Report Submission Workflow.
2. Reviewed the HCS Knowledge Artifact in depth.
3. Created Bundles from EHR Sandbox (Cerner) and MELD systems.
4. Posted Bundles to DEX server for HCS.
5. Posted Valid and Invalid Bundles and verified that they pass and fail successfully.
6. Set up MELD cloud system to allow participants to use MELD systems.
7. The team was able to debug and fix 21 out of 26 bundle validation errors from the HDEA App for bundles originating either in Cerner or MELD.
8. Discovered and fixed issues with the HCS PlanDefinition Knowledge Artifact.
9. Created a sample RNOF PlanDefinition resource.
10. Processed the RNOF PlanDefinitions (Provisioning Workflow).
11. Created an RNOF notification (Notification Workflow).
12. Created an RNOF bundle (Report Generation Workflow) utilizing a Cerner sandbox as the data source. All of the profiles used were eCR profiles.



