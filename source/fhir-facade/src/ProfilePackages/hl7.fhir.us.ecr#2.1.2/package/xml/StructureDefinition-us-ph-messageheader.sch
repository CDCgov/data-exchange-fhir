<?xml version="1.0" encoding="UTF-8"?>
<sch:schema xmlns:sch="http://purl.oclc.org/dsdl/schematron" queryBinding="xslt2">
  <sch:ns prefix="f" uri="http://hl7.org/fhir"/>
  <sch:ns prefix="h" uri="http://www.w3.org/1999/xhtml"/>
  <!-- 
    This file contains just the constraints for the profile MessageHeader
    It includes the base constraints for the resource as well.
    Because of the way that schematrons and containment work, 
    you may need to use this schematron fragment to build a, 
    single schematron that validates contained resources (if you have any) 
  -->
  <sch:pattern>
    <sch:title>f:MessageHeader</sch:title>
    <sch:rule context="f:MessageHeader">
      <sch:assert test="count(f:extension[@url = 'http://hl7.org/fhir/us/ecr/StructureDefinition/us-ph-data-encrypted-extension']) &lt;= 1">extension with URL = 'http://hl7.org/fhir/us/ecr/StructureDefinition/us-ph-data-encrypted-extension': maximum cardinality of 'extension' is 1</sch:assert>
      <sch:assert test="count(f:extension[@url = 'http://hl7.org/fhir/us/ecr/StructureDefinition/us-ph-message-processing-category-extension']) &gt;= 1">extension with URL = 'http://hl7.org/fhir/us/ecr/StructureDefinition/us-ph-message-processing-category-extension': minimum cardinality of 'extension' is 1</sch:assert>
      <sch:assert test="count(f:extension[@url = 'http://hl7.org/fhir/us/ecr/StructureDefinition/us-ph-message-processing-category-extension']) &lt;= 1">extension with URL = 'http://hl7.org/fhir/us/ecr/StructureDefinition/us-ph-message-processing-category-extension': maximum cardinality of 'extension' is 1</sch:assert>
      <sch:assert test="count(f:destination) &gt;= 1">destination: minimum cardinality of 'destination' is 1</sch:assert>
      <sch:assert test="count(f:sender) &gt;= 1">sender: minimum cardinality of 'sender' is 1</sch:assert>
      <sch:assert test="count(f:reason) &gt;= 1">reason: minimum cardinality of 'reason' is 1</sch:assert>
    </sch:rule>
  </sch:pattern>
</sch:schema>
