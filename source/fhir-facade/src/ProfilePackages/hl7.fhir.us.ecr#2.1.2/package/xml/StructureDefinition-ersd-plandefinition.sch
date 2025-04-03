<?xml version="1.0" encoding="UTF-8"?>
<sch:schema xmlns:sch="http://purl.oclc.org/dsdl/schematron" queryBinding="xslt2">
  <sch:ns prefix="f" uri="http://hl7.org/fhir"/>
  <sch:ns prefix="h" uri="http://www.w3.org/1999/xhtml"/>
  <!-- 
    This file contains just the constraints for the profile USPublicHealthPlanDefinition
    It includes the base constraints for the resource as well.
    Because of the way that schematrons and containment work, 
    you may need to use this schematron fragment to build a, 
    single schematron that validates contained resources (if you have any) 
  -->
  <sch:pattern>
    <sch:title>f:PlanDefinition</sch:title>
    <sch:rule context="f:PlanDefinition">
      <sch:assert test="count(f:extension[@url = 'http://hl7.org/fhir/us/ecr/StructureDefinition/us-ph-receiver-address-extension']) &lt;= 1">extension with URL = 'http://hl7.org/fhir/us/ecr/StructureDefinition/us-ph-receiver-address-extension': maximum cardinality of 'extension' is 1</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>f:PlanDefinition/f:action/f:trigger</sch:title>
    <sch:rule context="f:PlanDefinition/f:action/f:trigger">
      <sch:assert test="count(f:extension[@url = 'http://hl7.org/fhir/us/ecr/StructureDefinition/us-ph-named-eventtype-extension']) &lt;= 1">extension with URL = 'http://hl7.org/fhir/us/ecr/StructureDefinition/us-ph-named-eventtype-extension': maximum cardinality of 'extension' is 1</sch:assert>
      <sch:assert test="count(f:extension[@url = 'http://hl7.org/fhir/us/ecr/StructureDefinition/us-ph-named-eventtype-extension']) &lt;= 1">extension with URL = 'http://hl7.org/fhir/us/ecr/StructureDefinition/us-ph-named-eventtype-extension': maximum cardinality of 'extension' is 1</sch:assert>
      <sch:assert test="count(f:extension[@url = 'http://hl7.org/fhir/us/ecr/StructureDefinition/us-ph-named-eventtype-extension']) &lt;= 1">extension with URL = 'http://hl7.org/fhir/us/ecr/StructureDefinition/us-ph-named-eventtype-extension': maximum cardinality of 'extension' is 1</sch:assert>
      <sch:assert test="count(f:extension[@url = 'http://hl7.org/fhir/us/ecr/StructureDefinition/us-ph-named-eventtype-extension']) &lt;= 1">extension with URL = 'http://hl7.org/fhir/us/ecr/StructureDefinition/us-ph-named-eventtype-extension': maximum cardinality of 'extension' is 1</sch:assert>
      <sch:assert test="count(f:extension[@url = 'http://hl7.org/fhir/us/ecr/StructureDefinition/us-ph-named-eventtype-extension']) &lt;= 1">extension with URL = 'http://hl7.org/fhir/us/ecr/StructureDefinition/us-ph-named-eventtype-extension': maximum cardinality of 'extension' is 1</sch:assert>
      <sch:assert test="count(f:extension[@url = 'http://hl7.org/fhir/us/ecr/StructureDefinition/us-ph-named-eventtype-extension']) &lt;= 1">extension with URL = 'http://hl7.org/fhir/us/ecr/StructureDefinition/us-ph-named-eventtype-extension': maximum cardinality of 'extension' is 1</sch:assert>
      <sch:assert test="count(f:extension[@url = 'http://hl7.org/fhir/us/ecr/StructureDefinition/us-ph-named-eventtype-extension']) &lt;= 1">extension with URL = 'http://hl7.org/fhir/us/ecr/StructureDefinition/us-ph-named-eventtype-extension': maximum cardinality of 'extension' is 1</sch:assert>
      <sch:assert test="count(f:extension[@url = 'http://hl7.org/fhir/us/ecr/StructureDefinition/us-ph-named-eventtype-extension']) &lt;= 1">extension with URL = 'http://hl7.org/fhir/us/ecr/StructureDefinition/us-ph-named-eventtype-extension': maximum cardinality of 'extension' is 1</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>f:PlanDefinition/f:action/f:input</sch:title>
    <sch:rule context="f:PlanDefinition/f:action/f:input">
      <sch:assert test="count(f:extension[@url = 'http://hl7.org/fhir/us/ecr/StructureDefinition/us-ph-relateddata-extension']) &lt;= 1">extension with URL = 'http://hl7.org/fhir/us/ecr/StructureDefinition/us-ph-relateddata-extension': maximum cardinality of 'extension' is 1</sch:assert>
      <sch:assert test="count(f:extension[@url = 'http://hl7.org/fhir/us/ecr/StructureDefinition/us-ph-fhirquerypattern-extension']) &lt;= 1">extension with URL = 'http://hl7.org/fhir/us/ecr/StructureDefinition/us-ph-fhirquerypattern-extension': maximum cardinality of 'extension' is 1</sch:assert>
      <sch:assert test="count(f:extension[@url = 'http://hl7.org/fhir/us/ecr/StructureDefinition/us-ph-relateddata-extension']) &lt;= 1">extension with URL = 'http://hl7.org/fhir/us/ecr/StructureDefinition/us-ph-relateddata-extension': maximum cardinality of 'extension' is 1</sch:assert>
      <sch:assert test="count(f:extension[@url = 'http://hl7.org/fhir/us/ecr/StructureDefinition/us-ph-relateddata-extension']) &lt;= 1">extension with URL = 'http://hl7.org/fhir/us/ecr/StructureDefinition/us-ph-relateddata-extension': maximum cardinality of 'extension' is 1</sch:assert>
      <sch:assert test="count(f:extension[@url = 'http://hl7.org/fhir/us/ecr/StructureDefinition/us-ph-relateddata-extension']) &lt;= 1">extension with URL = 'http://hl7.org/fhir/us/ecr/StructureDefinition/us-ph-relateddata-extension': maximum cardinality of 'extension' is 1</sch:assert>
      <sch:assert test="count(f:extension[@url = 'http://hl7.org/fhir/us/ecr/StructureDefinition/us-ph-relateddata-extension']) &lt;= 1">extension with URL = 'http://hl7.org/fhir/us/ecr/StructureDefinition/us-ph-relateddata-extension': maximum cardinality of 'extension' is 1</sch:assert>
      <sch:assert test="count(f:extension[@url = 'http://hl7.org/fhir/us/ecr/StructureDefinition/us-ph-relateddata-extension']) &lt;= 1">extension with URL = 'http://hl7.org/fhir/us/ecr/StructureDefinition/us-ph-relateddata-extension': maximum cardinality of 'extension' is 1</sch:assert>
      <sch:assert test="count(f:extension[@url = 'http://hl7.org/fhir/us/ecr/StructureDefinition/us-ph-relateddata-extension']) &lt;= 1">extension with URL = 'http://hl7.org/fhir/us/ecr/StructureDefinition/us-ph-relateddata-extension': maximum cardinality of 'extension' is 1</sch:assert>
      <sch:assert test="count(f:extension[@url = 'http://hl7.org/fhir/us/ecr/StructureDefinition/us-ph-relateddata-extension']) &lt;= 1">extension with URL = 'http://hl7.org/fhir/us/ecr/StructureDefinition/us-ph-relateddata-extension': maximum cardinality of 'extension' is 1</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>f:PlanDefinition/f:action/f:input/f:extension</sch:title>
    <sch:rule context="f:PlanDefinition/f:action/f:input/f:extension">
      <sch:assert test="count(f:id) &lt;= 1">id: maximum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:url) &gt;= 1">url: minimum cardinality of 'url' is 1</sch:assert>
      <sch:assert test="count(f:url) &lt;= 1">url: maximum cardinality of 'url' is 1</sch:assert>
      <sch:assert test="count(f:value[x]) &lt;= 1">value[x]: maximum cardinality of 'value[x]' is 1</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>f:PlanDefinition/f:action</sch:title>
    <sch:rule context="f:PlanDefinition/f:action">
      <sch:assert test="count(f:id) &gt;= 1">id: minimum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:description) &gt;= 1">description: minimum cardinality of 'description' is 1</sch:assert>
      <sch:assert test="count(f:textEquivalent) &gt;= 1">textEquivalent: minimum cardinality of 'textEquivalent' is 1</sch:assert>
      <sch:assert test="count(f:code) &gt;= 1">code: minimum cardinality of 'code' is 1</sch:assert>
      <sch:assert test="count(f:trigger) &gt;= 1">trigger: minimum cardinality of 'trigger' is 1</sch:assert>
      <sch:assert test="count(f:relatedAction) &gt;= 1">relatedAction: minimum cardinality of 'relatedAction' is 1</sch:assert>
      <sch:assert test="count(f:relatedAction) &lt;= 1">relatedAction: maximum cardinality of 'relatedAction' is 1</sch:assert>
      <sch:assert test="count(f:id) &gt;= 1">id: minimum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:description) &gt;= 1">description: minimum cardinality of 'description' is 1</sch:assert>
      <sch:assert test="count(f:code) &gt;= 1">code: minimum cardinality of 'code' is 1</sch:assert>
      <sch:assert test="count(f:id) &gt;= 1">id: minimum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:code) &gt;= 1">code: minimum cardinality of 'code' is 1</sch:assert>
      <sch:assert test="count(f:id) &gt;= 1">id: minimum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:relatedAction) &gt;= 1">relatedAction: minimum cardinality of 'relatedAction' is 1</sch:assert>
      <sch:assert test="count(f:relatedAction) &lt;= 1">relatedAction: maximum cardinality of 'relatedAction' is 1</sch:assert>
      <sch:assert test="count(f:id) &gt;= 1">id: minimum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:relatedAction) &gt;= 1">relatedAction: minimum cardinality of 'relatedAction' is 1</sch:assert>
      <sch:assert test="count(f:relatedAction) &lt;= 1">relatedAction: maximum cardinality of 'relatedAction' is 1</sch:assert>
      <sch:assert test="count(f:id) &gt;= 1">id: minimum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:description) &gt;= 1">description: minimum cardinality of 'description' is 1</sch:assert>
      <sch:assert test="count(f:textEquivalent) &gt;= 1">textEquivalent: minimum cardinality of 'textEquivalent' is 1</sch:assert>
      <sch:assert test="count(f:id) &gt;= 1">id: minimum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:description) &gt;= 1">description: minimum cardinality of 'description' is 1</sch:assert>
      <sch:assert test="count(f:textEquivalent) &gt;= 1">textEquivalent: minimum cardinality of 'textEquivalent' is 1</sch:assert>
      <sch:assert test="count(f:code) &gt;= 1">code: minimum cardinality of 'code' is 1</sch:assert>
      <sch:assert test="count(f:trigger) &gt;= 1">trigger: minimum cardinality of 'trigger' is 1</sch:assert>
      <sch:assert test="count(f:relatedAction) &gt;= 1">relatedAction: minimum cardinality of 'relatedAction' is 1</sch:assert>
      <sch:assert test="count(f:relatedAction) &lt;= 1">relatedAction: maximum cardinality of 'relatedAction' is 1</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>f:PlanDefinition/f:action/f:action</sch:title>
    <sch:rule context="f:PlanDefinition/f:action/f:action">
      <sch:assert test="count(f:id) &gt;= 1">id: minimum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:id) &lt;= 1">id: maximum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:prefix) &lt;= 1">prefix: maximum cardinality of 'prefix' is 1</sch:assert>
      <sch:assert test="count(f:title) &lt;= 1">title: maximum cardinality of 'title' is 1</sch:assert>
      <sch:assert test="count(f:description) &lt;= 1">description: maximum cardinality of 'description' is 1</sch:assert>
      <sch:assert test="count(f:textEquivalent) &lt;= 1">textEquivalent: maximum cardinality of 'textEquivalent' is 1</sch:assert>
      <sch:assert test="count(f:priority) &lt;= 1">priority: maximum cardinality of 'priority' is 1</sch:assert>
      <sch:assert test="count(f:code) &gt;= 1">code: minimum cardinality of 'code' is 1</sch:assert>
      <sch:assert test="count(f:subject[x]) &lt;= 1">subject[x]: maximum cardinality of 'subject[x]' is 1</sch:assert>
      <sch:assert test="count(f:timing[x]) &lt;= 1">timing[x]: maximum cardinality of 'timing[x]' is 1</sch:assert>
      <sch:assert test="count(f:type) &lt;= 1">type: maximum cardinality of 'type' is 1</sch:assert>
      <sch:assert test="count(f:groupingBehavior) &lt;= 1">groupingBehavior: maximum cardinality of 'groupingBehavior' is 1</sch:assert>
      <sch:assert test="count(f:selectionBehavior) &lt;= 1">selectionBehavior: maximum cardinality of 'selectionBehavior' is 1</sch:assert>
      <sch:assert test="count(f:requiredBehavior) &lt;= 1">requiredBehavior: maximum cardinality of 'requiredBehavior' is 1</sch:assert>
      <sch:assert test="count(f:precheckBehavior) &lt;= 1">precheckBehavior: maximum cardinality of 'precheckBehavior' is 1</sch:assert>
      <sch:assert test="count(f:cardinalityBehavior) &lt;= 1">cardinalityBehavior: maximum cardinality of 'cardinalityBehavior' is 1</sch:assert>
      <sch:assert test="count(f:definition[x]) &lt;= 1">definition[x]: maximum cardinality of 'definition[x]' is 1</sch:assert>
      <sch:assert test="count(f:transform) &lt;= 1">transform: maximum cardinality of 'transform' is 1</sch:assert>
      <sch:assert test="count(f:id) &gt;= 1">id: minimum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:id) &lt;= 1">id: maximum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:prefix) &lt;= 1">prefix: maximum cardinality of 'prefix' is 1</sch:assert>
      <sch:assert test="count(f:title) &lt;= 1">title: maximum cardinality of 'title' is 1</sch:assert>
      <sch:assert test="count(f:description) &lt;= 1">description: maximum cardinality of 'description' is 1</sch:assert>
      <sch:assert test="count(f:textEquivalent) &lt;= 1">textEquivalent: maximum cardinality of 'textEquivalent' is 1</sch:assert>
      <sch:assert test="count(f:priority) &lt;= 1">priority: maximum cardinality of 'priority' is 1</sch:assert>
      <sch:assert test="count(f:code) &gt;= 1">code: minimum cardinality of 'code' is 1</sch:assert>
      <sch:assert test="count(f:subject[x]) &lt;= 1">subject[x]: maximum cardinality of 'subject[x]' is 1</sch:assert>
      <sch:assert test="count(f:timing[x]) &lt;= 1">timing[x]: maximum cardinality of 'timing[x]' is 1</sch:assert>
      <sch:assert test="count(f:type) &lt;= 1">type: maximum cardinality of 'type' is 1</sch:assert>
      <sch:assert test="count(f:groupingBehavior) &lt;= 1">groupingBehavior: maximum cardinality of 'groupingBehavior' is 1</sch:assert>
      <sch:assert test="count(f:selectionBehavior) &lt;= 1">selectionBehavior: maximum cardinality of 'selectionBehavior' is 1</sch:assert>
      <sch:assert test="count(f:requiredBehavior) &lt;= 1">requiredBehavior: maximum cardinality of 'requiredBehavior' is 1</sch:assert>
      <sch:assert test="count(f:precheckBehavior) &lt;= 1">precheckBehavior: maximum cardinality of 'precheckBehavior' is 1</sch:assert>
      <sch:assert test="count(f:cardinalityBehavior) &lt;= 1">cardinalityBehavior: maximum cardinality of 'cardinalityBehavior' is 1</sch:assert>
      <sch:assert test="count(f:definition[x]) &lt;= 1">definition[x]: maximum cardinality of 'definition[x]' is 1</sch:assert>
      <sch:assert test="count(f:transform) &lt;= 1">transform: maximum cardinality of 'transform' is 1</sch:assert>
      <sch:assert test="count(f:id) &gt;= 1">id: minimum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:id) &lt;= 1">id: maximum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:prefix) &lt;= 1">prefix: maximum cardinality of 'prefix' is 1</sch:assert>
      <sch:assert test="count(f:title) &lt;= 1">title: maximum cardinality of 'title' is 1</sch:assert>
      <sch:assert test="count(f:description) &lt;= 1">description: maximum cardinality of 'description' is 1</sch:assert>
      <sch:assert test="count(f:textEquivalent) &lt;= 1">textEquivalent: maximum cardinality of 'textEquivalent' is 1</sch:assert>
      <sch:assert test="count(f:priority) &lt;= 1">priority: maximum cardinality of 'priority' is 1</sch:assert>
      <sch:assert test="count(f:code) &gt;= 1">code: minimum cardinality of 'code' is 1</sch:assert>
      <sch:assert test="count(f:subject[x]) &lt;= 1">subject[x]: maximum cardinality of 'subject[x]' is 1</sch:assert>
      <sch:assert test="count(f:timing[x]) &lt;= 1">timing[x]: maximum cardinality of 'timing[x]' is 1</sch:assert>
      <sch:assert test="count(f:type) &lt;= 1">type: maximum cardinality of 'type' is 1</sch:assert>
      <sch:assert test="count(f:groupingBehavior) &lt;= 1">groupingBehavior: maximum cardinality of 'groupingBehavior' is 1</sch:assert>
      <sch:assert test="count(f:selectionBehavior) &lt;= 1">selectionBehavior: maximum cardinality of 'selectionBehavior' is 1</sch:assert>
      <sch:assert test="count(f:requiredBehavior) &lt;= 1">requiredBehavior: maximum cardinality of 'requiredBehavior' is 1</sch:assert>
      <sch:assert test="count(f:precheckBehavior) &lt;= 1">precheckBehavior: maximum cardinality of 'precheckBehavior' is 1</sch:assert>
      <sch:assert test="count(f:cardinalityBehavior) &lt;= 1">cardinalityBehavior: maximum cardinality of 'cardinalityBehavior' is 1</sch:assert>
      <sch:assert test="count(f:definition[x]) &lt;= 1">definition[x]: maximum cardinality of 'definition[x]' is 1</sch:assert>
      <sch:assert test="count(f:transform) &lt;= 1">transform: maximum cardinality of 'transform' is 1</sch:assert>
      <sch:assert test="count(f:id) &gt;= 1">id: minimum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:id) &lt;= 1">id: maximum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:prefix) &lt;= 1">prefix: maximum cardinality of 'prefix' is 1</sch:assert>
      <sch:assert test="count(f:title) &lt;= 1">title: maximum cardinality of 'title' is 1</sch:assert>
      <sch:assert test="count(f:description) &lt;= 1">description: maximum cardinality of 'description' is 1</sch:assert>
      <sch:assert test="count(f:textEquivalent) &lt;= 1">textEquivalent: maximum cardinality of 'textEquivalent' is 1</sch:assert>
      <sch:assert test="count(f:priority) &lt;= 1">priority: maximum cardinality of 'priority' is 1</sch:assert>
      <sch:assert test="count(f:code) &gt;= 1">code: minimum cardinality of 'code' is 1</sch:assert>
      <sch:assert test="count(f:subject[x]) &lt;= 1">subject[x]: maximum cardinality of 'subject[x]' is 1</sch:assert>
      <sch:assert test="count(f:timing[x]) &lt;= 1">timing[x]: maximum cardinality of 'timing[x]' is 1</sch:assert>
      <sch:assert test="count(f:type) &lt;= 1">type: maximum cardinality of 'type' is 1</sch:assert>
      <sch:assert test="count(f:groupingBehavior) &lt;= 1">groupingBehavior: maximum cardinality of 'groupingBehavior' is 1</sch:assert>
      <sch:assert test="count(f:selectionBehavior) &lt;= 1">selectionBehavior: maximum cardinality of 'selectionBehavior' is 1</sch:assert>
      <sch:assert test="count(f:requiredBehavior) &lt;= 1">requiredBehavior: maximum cardinality of 'requiredBehavior' is 1</sch:assert>
      <sch:assert test="count(f:precheckBehavior) &lt;= 1">precheckBehavior: maximum cardinality of 'precheckBehavior' is 1</sch:assert>
      <sch:assert test="count(f:cardinalityBehavior) &lt;= 1">cardinalityBehavior: maximum cardinality of 'cardinalityBehavior' is 1</sch:assert>
      <sch:assert test="count(f:definition[x]) &lt;= 1">definition[x]: maximum cardinality of 'definition[x]' is 1</sch:assert>
      <sch:assert test="count(f:transform) &lt;= 1">transform: maximum cardinality of 'transform' is 1</sch:assert>
      <sch:assert test="count(f:id) &gt;= 1">id: minimum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:id) &lt;= 1">id: maximum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:prefix) &lt;= 1">prefix: maximum cardinality of 'prefix' is 1</sch:assert>
      <sch:assert test="count(f:title) &lt;= 1">title: maximum cardinality of 'title' is 1</sch:assert>
      <sch:assert test="count(f:description) &lt;= 1">description: maximum cardinality of 'description' is 1</sch:assert>
      <sch:assert test="count(f:textEquivalent) &lt;= 1">textEquivalent: maximum cardinality of 'textEquivalent' is 1</sch:assert>
      <sch:assert test="count(f:priority) &lt;= 1">priority: maximum cardinality of 'priority' is 1</sch:assert>
      <sch:assert test="count(f:code) &gt;= 1">code: minimum cardinality of 'code' is 1</sch:assert>
      <sch:assert test="count(f:subject[x]) &lt;= 1">subject[x]: maximum cardinality of 'subject[x]' is 1</sch:assert>
      <sch:assert test="count(f:timing[x]) &lt;= 1">timing[x]: maximum cardinality of 'timing[x]' is 1</sch:assert>
      <sch:assert test="count(f:type) &lt;= 1">type: maximum cardinality of 'type' is 1</sch:assert>
      <sch:assert test="count(f:groupingBehavior) &lt;= 1">groupingBehavior: maximum cardinality of 'groupingBehavior' is 1</sch:assert>
      <sch:assert test="count(f:selectionBehavior) &lt;= 1">selectionBehavior: maximum cardinality of 'selectionBehavior' is 1</sch:assert>
      <sch:assert test="count(f:requiredBehavior) &lt;= 1">requiredBehavior: maximum cardinality of 'requiredBehavior' is 1</sch:assert>
      <sch:assert test="count(f:precheckBehavior) &lt;= 1">precheckBehavior: maximum cardinality of 'precheckBehavior' is 1</sch:assert>
      <sch:assert test="count(f:cardinalityBehavior) &lt;= 1">cardinalityBehavior: maximum cardinality of 'cardinalityBehavior' is 1</sch:assert>
      <sch:assert test="count(f:definition[x]) &lt;= 1">definition[x]: maximum cardinality of 'definition[x]' is 1</sch:assert>
      <sch:assert test="count(f:transform) &lt;= 1">transform: maximum cardinality of 'transform' is 1</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>f:PlanDefinition/f:action/f:action/f:condition</sch:title>
    <sch:rule context="f:PlanDefinition/f:action/f:action/f:condition">
      <sch:assert test="count(f:id) &lt;= 1">id: maximum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:kind) &gt;= 1">kind: minimum cardinality of 'kind' is 1</sch:assert>
      <sch:assert test="count(f:kind) &lt;= 1">kind: maximum cardinality of 'kind' is 1</sch:assert>
      <sch:assert test="count(f:expression) &gt;= 1">expression: minimum cardinality of 'expression' is 1</sch:assert>
      <sch:assert test="count(f:expression) &lt;= 1">expression: maximum cardinality of 'expression' is 1</sch:assert>
      <sch:assert test="count(f:id) &lt;= 1">id: maximum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:kind) &gt;= 1">kind: minimum cardinality of 'kind' is 1</sch:assert>
      <sch:assert test="count(f:kind) &lt;= 1">kind: maximum cardinality of 'kind' is 1</sch:assert>
      <sch:assert test="count(f:expression) &gt;= 1">expression: minimum cardinality of 'expression' is 1</sch:assert>
      <sch:assert test="count(f:expression) &lt;= 1">expression: maximum cardinality of 'expression' is 1</sch:assert>
      <sch:assert test="count(f:id) &lt;= 1">id: maximum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:kind) &gt;= 1">kind: minimum cardinality of 'kind' is 1</sch:assert>
      <sch:assert test="count(f:kind) &lt;= 1">kind: maximum cardinality of 'kind' is 1</sch:assert>
      <sch:assert test="count(f:expression) &gt;= 1">expression: minimum cardinality of 'expression' is 1</sch:assert>
      <sch:assert test="count(f:expression) &lt;= 1">expression: maximum cardinality of 'expression' is 1</sch:assert>
      <sch:assert test="count(f:id) &lt;= 1">id: maximum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:kind) &gt;= 1">kind: minimum cardinality of 'kind' is 1</sch:assert>
      <sch:assert test="count(f:kind) &lt;= 1">kind: maximum cardinality of 'kind' is 1</sch:assert>
      <sch:assert test="count(f:expression) &gt;= 1">expression: minimum cardinality of 'expression' is 1</sch:assert>
      <sch:assert test="count(f:expression) &lt;= 1">expression: maximum cardinality of 'expression' is 1</sch:assert>
      <sch:assert test="count(f:id) &lt;= 1">id: maximum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:kind) &gt;= 1">kind: minimum cardinality of 'kind' is 1</sch:assert>
      <sch:assert test="count(f:kind) &lt;= 1">kind: maximum cardinality of 'kind' is 1</sch:assert>
      <sch:assert test="count(f:expression) &gt;= 1">expression: minimum cardinality of 'expression' is 1</sch:assert>
      <sch:assert test="count(f:expression) &lt;= 1">expression: maximum cardinality of 'expression' is 1</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>f:PlanDefinition/f:action/f:action/f:condition/f:expression</sch:title>
    <sch:rule context="f:PlanDefinition/f:action/f:action/f:condition/f:expression">
      <sch:assert test="count(f:id) &lt;= 1">id: maximum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:description) &lt;= 1">description: maximum cardinality of 'description' is 1</sch:assert>
      <sch:assert test="count(f:name) &lt;= 1">name: maximum cardinality of 'name' is 1</sch:assert>
      <sch:assert test="count(f:language) &gt;= 1">language: minimum cardinality of 'language' is 1</sch:assert>
      <sch:assert test="count(f:language) &lt;= 1">language: maximum cardinality of 'language' is 1</sch:assert>
      <sch:assert test="count(f:expression) &lt;= 1">expression: maximum cardinality of 'expression' is 1</sch:assert>
      <sch:assert test="count(f:reference) &lt;= 1">reference: maximum cardinality of 'reference' is 1</sch:assert>
      <sch:assert test="count(f:id) &lt;= 1">id: maximum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:description) &lt;= 1">description: maximum cardinality of 'description' is 1</sch:assert>
      <sch:assert test="count(f:name) &lt;= 1">name: maximum cardinality of 'name' is 1</sch:assert>
      <sch:assert test="count(f:language) &gt;= 1">language: minimum cardinality of 'language' is 1</sch:assert>
      <sch:assert test="count(f:language) &lt;= 1">language: maximum cardinality of 'language' is 1</sch:assert>
      <sch:assert test="count(f:expression) &lt;= 1">expression: maximum cardinality of 'expression' is 1</sch:assert>
      <sch:assert test="count(f:reference) &lt;= 1">reference: maximum cardinality of 'reference' is 1</sch:assert>
      <sch:assert test="count(f:id) &lt;= 1">id: maximum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:description) &lt;= 1">description: maximum cardinality of 'description' is 1</sch:assert>
      <sch:assert test="count(f:name) &lt;= 1">name: maximum cardinality of 'name' is 1</sch:assert>
      <sch:assert test="count(f:language) &gt;= 1">language: minimum cardinality of 'language' is 1</sch:assert>
      <sch:assert test="count(f:language) &lt;= 1">language: maximum cardinality of 'language' is 1</sch:assert>
      <sch:assert test="count(f:expression) &lt;= 1">expression: maximum cardinality of 'expression' is 1</sch:assert>
      <sch:assert test="count(f:reference) &lt;= 1">reference: maximum cardinality of 'reference' is 1</sch:assert>
      <sch:assert test="count(f:id) &lt;= 1">id: maximum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:description) &lt;= 1">description: maximum cardinality of 'description' is 1</sch:assert>
      <sch:assert test="count(f:name) &lt;= 1">name: maximum cardinality of 'name' is 1</sch:assert>
      <sch:assert test="count(f:language) &gt;= 1">language: minimum cardinality of 'language' is 1</sch:assert>
      <sch:assert test="count(f:language) &lt;= 1">language: maximum cardinality of 'language' is 1</sch:assert>
      <sch:assert test="count(f:expression) &lt;= 1">expression: maximum cardinality of 'expression' is 1</sch:assert>
      <sch:assert test="count(f:reference) &lt;= 1">reference: maximum cardinality of 'reference' is 1</sch:assert>
      <sch:assert test="count(f:id) &lt;= 1">id: maximum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:description) &lt;= 1">description: maximum cardinality of 'description' is 1</sch:assert>
      <sch:assert test="count(f:name) &lt;= 1">name: maximum cardinality of 'name' is 1</sch:assert>
      <sch:assert test="count(f:language) &gt;= 1">language: minimum cardinality of 'language' is 1</sch:assert>
      <sch:assert test="count(f:language) &lt;= 1">language: maximum cardinality of 'language' is 1</sch:assert>
      <sch:assert test="count(f:expression) &lt;= 1">expression: maximum cardinality of 'expression' is 1</sch:assert>
      <sch:assert test="count(f:reference) &lt;= 1">reference: maximum cardinality of 'reference' is 1</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>f:PlanDefinition/f:action/f:action/f:input</sch:title>
    <sch:rule context="f:PlanDefinition/f:action/f:action/f:input">
      <sch:assert test="count(f:id) &gt;= 1">id: minimum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:id) &lt;= 1">id: maximum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:type) &gt;= 1">type: minimum cardinality of 'type' is 1</sch:assert>
      <sch:assert test="count(f:type) &lt;= 1">type: maximum cardinality of 'type' is 1</sch:assert>
      <sch:assert test="count(f:subject[x]) &lt;= 1">subject[x]: maximum cardinality of 'subject[x]' is 1</sch:assert>
      <sch:assert test="count(f:limit) &lt;= 1">limit: maximum cardinality of 'limit' is 1</sch:assert>
      <sch:assert test="count(f:id) &gt;= 1">id: minimum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:id) &lt;= 1">id: maximum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:type) &gt;= 1">type: minimum cardinality of 'type' is 1</sch:assert>
      <sch:assert test="count(f:type) &lt;= 1">type: maximum cardinality of 'type' is 1</sch:assert>
      <sch:assert test="count(f:subject[x]) &lt;= 1">subject[x]: maximum cardinality of 'subject[x]' is 1</sch:assert>
      <sch:assert test="count(f:limit) &lt;= 1">limit: maximum cardinality of 'limit' is 1</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>f:PlanDefinition/f:action/f:action/f:input/f:codeFilter</sch:title>
    <sch:rule context="f:PlanDefinition/f:action/f:action/f:input/f:codeFilter">
      <sch:assert test="count(f:id) &lt;= 1">id: maximum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:path) &lt;= 1">path: maximum cardinality of 'path' is 1</sch:assert>
      <sch:assert test="count(f:searchParam) &lt;= 1">searchParam: maximum cardinality of 'searchParam' is 1</sch:assert>
      <sch:assert test="count(f:valueSet) &lt;= 1">valueSet: maximum cardinality of 'valueSet' is 1</sch:assert>
      <sch:assert test="count(f:id) &lt;= 1">id: maximum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:path) &lt;= 1">path: maximum cardinality of 'path' is 1</sch:assert>
      <sch:assert test="count(f:searchParam) &lt;= 1">searchParam: maximum cardinality of 'searchParam' is 1</sch:assert>
      <sch:assert test="count(f:valueSet) &lt;= 1">valueSet: maximum cardinality of 'valueSet' is 1</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>f:PlanDefinition/f:action/f:action/f:input/f:dateFilter</sch:title>
    <sch:rule context="f:PlanDefinition/f:action/f:action/f:input/f:dateFilter">
      <sch:assert test="count(f:id) &lt;= 1">id: maximum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:path) &lt;= 1">path: maximum cardinality of 'path' is 1</sch:assert>
      <sch:assert test="count(f:searchParam) &lt;= 1">searchParam: maximum cardinality of 'searchParam' is 1</sch:assert>
      <sch:assert test="count(f:value[x]) &lt;= 1">value[x]: maximum cardinality of 'value[x]' is 1</sch:assert>
      <sch:assert test="count(f:id) &lt;= 1">id: maximum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:path) &lt;= 1">path: maximum cardinality of 'path' is 1</sch:assert>
      <sch:assert test="count(f:searchParam) &lt;= 1">searchParam: maximum cardinality of 'searchParam' is 1</sch:assert>
      <sch:assert test="count(f:value[x]) &lt;= 1">value[x]: maximum cardinality of 'value[x]' is 1</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>f:PlanDefinition/f:action/f:action/f:input/f:sort</sch:title>
    <sch:rule context="f:PlanDefinition/f:action/f:action/f:input/f:sort">
      <sch:assert test="count(f:id) &lt;= 1">id: maximum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:path) &gt;= 1">path: minimum cardinality of 'path' is 1</sch:assert>
      <sch:assert test="count(f:path) &lt;= 1">path: maximum cardinality of 'path' is 1</sch:assert>
      <sch:assert test="count(f:direction) &gt;= 1">direction: minimum cardinality of 'direction' is 1</sch:assert>
      <sch:assert test="count(f:direction) &lt;= 1">direction: maximum cardinality of 'direction' is 1</sch:assert>
      <sch:assert test="count(f:id) &lt;= 1">id: maximum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:path) &gt;= 1">path: minimum cardinality of 'path' is 1</sch:assert>
      <sch:assert test="count(f:path) &lt;= 1">path: maximum cardinality of 'path' is 1</sch:assert>
      <sch:assert test="count(f:direction) &gt;= 1">direction: minimum cardinality of 'direction' is 1</sch:assert>
      <sch:assert test="count(f:direction) &lt;= 1">direction: maximum cardinality of 'direction' is 1</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>f:PlanDefinition/f:action/f:action/f:relatedAction</sch:title>
    <sch:rule context="f:PlanDefinition/f:action/f:action/f:relatedAction">
      <sch:assert test="count(f:id) &lt;= 1">id: maximum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:actionId) &gt;= 1">actionId: minimum cardinality of 'actionId' is 1</sch:assert>
      <sch:assert test="count(f:actionId) &lt;= 1">actionId: maximum cardinality of 'actionId' is 1</sch:assert>
      <sch:assert test="count(f:relationship) &gt;= 1">relationship: minimum cardinality of 'relationship' is 1</sch:assert>
      <sch:assert test="count(f:relationship) &lt;= 1">relationship: maximum cardinality of 'relationship' is 1</sch:assert>
      <sch:assert test="count(f:offset[x]) &lt;= 1">offset[x]: maximum cardinality of 'offset[x]' is 1</sch:assert>
      <sch:assert test="count(f:id) &lt;= 1">id: maximum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:actionId) &gt;= 1">actionId: minimum cardinality of 'actionId' is 1</sch:assert>
      <sch:assert test="count(f:actionId) &lt;= 1">actionId: maximum cardinality of 'actionId' is 1</sch:assert>
      <sch:assert test="count(f:relationship) &gt;= 1">relationship: minimum cardinality of 'relationship' is 1</sch:assert>
      <sch:assert test="count(f:relationship) &lt;= 1">relationship: maximum cardinality of 'relationship' is 1</sch:assert>
      <sch:assert test="count(f:id) &lt;= 1">id: maximum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:actionId) &gt;= 1">actionId: minimum cardinality of 'actionId' is 1</sch:assert>
      <sch:assert test="count(f:actionId) &lt;= 1">actionId: maximum cardinality of 'actionId' is 1</sch:assert>
      <sch:assert test="count(f:relationship) &gt;= 1">relationship: minimum cardinality of 'relationship' is 1</sch:assert>
      <sch:assert test="count(f:relationship) &lt;= 1">relationship: maximum cardinality of 'relationship' is 1</sch:assert>
      <sch:assert test="count(f:offset[x]) &lt;= 1">offset[x]: maximum cardinality of 'offset[x]' is 1</sch:assert>
      <sch:assert test="count(f:id) &lt;= 1">id: maximum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:actionId) &gt;= 1">actionId: minimum cardinality of 'actionId' is 1</sch:assert>
      <sch:assert test="count(f:actionId) &lt;= 1">actionId: maximum cardinality of 'actionId' is 1</sch:assert>
      <sch:assert test="count(f:relationship) &gt;= 1">relationship: minimum cardinality of 'relationship' is 1</sch:assert>
      <sch:assert test="count(f:relationship) &lt;= 1">relationship: maximum cardinality of 'relationship' is 1</sch:assert>
      <sch:assert test="count(f:offset[x]) &lt;= 1">offset[x]: maximum cardinality of 'offset[x]' is 1</sch:assert>
      <sch:assert test="count(f:id) &lt;= 1">id: maximum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:actionId) &gt;= 1">actionId: minimum cardinality of 'actionId' is 1</sch:assert>
      <sch:assert test="count(f:actionId) &lt;= 1">actionId: maximum cardinality of 'actionId' is 1</sch:assert>
      <sch:assert test="count(f:relationship) &gt;= 1">relationship: minimum cardinality of 'relationship' is 1</sch:assert>
      <sch:assert test="count(f:relationship) &lt;= 1">relationship: maximum cardinality of 'relationship' is 1</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>f:PlanDefinition/f:action/f:action/f:participant</sch:title>
    <sch:rule context="f:PlanDefinition/f:action/f:action/f:participant">
      <sch:assert test="count(f:id) &lt;= 1">id: maximum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:type) &gt;= 1">type: minimum cardinality of 'type' is 1</sch:assert>
      <sch:assert test="count(f:type) &lt;= 1">type: maximum cardinality of 'type' is 1</sch:assert>
      <sch:assert test="count(f:role) &lt;= 1">role: maximum cardinality of 'role' is 1</sch:assert>
      <sch:assert test="count(f:id) &lt;= 1">id: maximum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:type) &gt;= 1">type: minimum cardinality of 'type' is 1</sch:assert>
      <sch:assert test="count(f:type) &lt;= 1">type: maximum cardinality of 'type' is 1</sch:assert>
      <sch:assert test="count(f:role) &lt;= 1">role: maximum cardinality of 'role' is 1</sch:assert>
      <sch:assert test="count(f:id) &lt;= 1">id: maximum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:type) &gt;= 1">type: minimum cardinality of 'type' is 1</sch:assert>
      <sch:assert test="count(f:type) &lt;= 1">type: maximum cardinality of 'type' is 1</sch:assert>
      <sch:assert test="count(f:role) &lt;= 1">role: maximum cardinality of 'role' is 1</sch:assert>
      <sch:assert test="count(f:id) &lt;= 1">id: maximum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:type) &gt;= 1">type: minimum cardinality of 'type' is 1</sch:assert>
      <sch:assert test="count(f:type) &lt;= 1">type: maximum cardinality of 'type' is 1</sch:assert>
      <sch:assert test="count(f:role) &lt;= 1">role: maximum cardinality of 'role' is 1</sch:assert>
      <sch:assert test="count(f:id) &lt;= 1">id: maximum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:type) &gt;= 1">type: minimum cardinality of 'type' is 1</sch:assert>
      <sch:assert test="count(f:type) &lt;= 1">type: maximum cardinality of 'type' is 1</sch:assert>
      <sch:assert test="count(f:role) &lt;= 1">role: maximum cardinality of 'role' is 1</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>f:PlanDefinition/f:action/f:action/f:dynamicValue</sch:title>
    <sch:rule context="f:PlanDefinition/f:action/f:action/f:dynamicValue">
      <sch:assert test="count(f:id) &lt;= 1">id: maximum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:path) &lt;= 1">path: maximum cardinality of 'path' is 1</sch:assert>
      <sch:assert test="count(f:expression) &lt;= 1">expression: maximum cardinality of 'expression' is 1</sch:assert>
      <sch:assert test="count(f:id) &lt;= 1">id: maximum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:path) &lt;= 1">path: maximum cardinality of 'path' is 1</sch:assert>
      <sch:assert test="count(f:expression) &lt;= 1">expression: maximum cardinality of 'expression' is 1</sch:assert>
      <sch:assert test="count(f:id) &lt;= 1">id: maximum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:path) &lt;= 1">path: maximum cardinality of 'path' is 1</sch:assert>
      <sch:assert test="count(f:expression) &lt;= 1">expression: maximum cardinality of 'expression' is 1</sch:assert>
      <sch:assert test="count(f:id) &lt;= 1">id: maximum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:path) &lt;= 1">path: maximum cardinality of 'path' is 1</sch:assert>
      <sch:assert test="count(f:expression) &lt;= 1">expression: maximum cardinality of 'expression' is 1</sch:assert>
      <sch:assert test="count(f:id) &lt;= 1">id: maximum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:path) &lt;= 1">path: maximum cardinality of 'path' is 1</sch:assert>
      <sch:assert test="count(f:expression) &lt;= 1">expression: maximum cardinality of 'expression' is 1</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>f:PlanDefinition/f:action/f:condition</sch:title>
    <sch:rule context="f:PlanDefinition/f:action/f:condition">
      <sch:assert test="count(f:expression) &gt;= 1">expression: minimum cardinality of 'expression' is 1</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>f:PlanDefinition/f:action/f:condition/f:expression</sch:title>
    <sch:rule context="f:PlanDefinition/f:action/f:condition/f:expression">
      <sch:assert test="count(f:id) &lt;= 1">id: maximum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:description) &lt;= 1">description: maximum cardinality of 'description' is 1</sch:assert>
      <sch:assert test="count(f:name) &lt;= 1">name: maximum cardinality of 'name' is 1</sch:assert>
      <sch:assert test="count(f:language) &gt;= 1">language: minimum cardinality of 'language' is 1</sch:assert>
      <sch:assert test="count(f:language) &lt;= 1">language: maximum cardinality of 'language' is 1</sch:assert>
      <sch:assert test="count(f:expression) &lt;= 1">expression: maximum cardinality of 'expression' is 1</sch:assert>
      <sch:assert test="count(f:reference) &lt;= 1">reference: maximum cardinality of 'reference' is 1</sch:assert>
    </sch:rule>
  </sch:pattern>
</sch:schema>
