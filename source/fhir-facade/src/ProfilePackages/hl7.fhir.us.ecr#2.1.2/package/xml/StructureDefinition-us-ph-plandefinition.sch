<?xml version="1.0" encoding="UTF-8"?>
<sch:schema xmlns:sch="http://purl.oclc.org/dsdl/schematron" queryBinding="xslt2">
  <sch:ns prefix="f" uri="http://hl7.org/fhir"/>
  <sch:ns prefix="h" uri="http://www.w3.org/1999/xhtml"/>
  <!-- 
    This file contains just the constraints for the profile Shareable PlanDefinition
    It includes the base constraints for the resource as well.
    Because of the way that schematrons and containment work, 
    you may need to use this schematron fragment to build a, 
    single schematron that validates contained resources (if you have any) 
  -->
  <sch:pattern>
    <sch:title>f:PlanDefinition</sch:title>
    <sch:rule context="f:PlanDefinition">
      <sch:assert test="count(f:extension[@url = 'http://hl7.org/fhir/us/ecr/StructureDefinition/us-ph-receiver-address-extension']) &lt;= 1">extension with URL = 'http://hl7.org/fhir/us/ecr/StructureDefinition/us-ph-receiver-address-extension': maximum cardinality of 'extension' is 1</sch:assert>
      <sch:assert test="count(f:title) &gt;= 1">title: minimum cardinality of 'title' is 1</sch:assert>
      <sch:assert test="count(f:type) &gt;= 1">type: minimum cardinality of 'type' is 1</sch:assert>
      <sch:assert test="count(f:date) &gt;= 1">date: minimum cardinality of 'date' is 1</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>f:PlanDefinition/f:effectivePeriod</sch:title>
    <sch:rule context="f:PlanDefinition/f:effectivePeriod">
      <sch:assert test="count(f:id) &lt;= 1">id: maximum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:start) &gt;= 1">start: minimum cardinality of 'start' is 1</sch:assert>
      <sch:assert test="count(f:start) &lt;= 1">start: maximum cardinality of 'start' is 1</sch:assert>
      <sch:assert test="count(f:end) &lt;= 1">end: maximum cardinality of 'end' is 1</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>f:PlanDefinition/f:action</sch:title>
    <sch:rule context="f:PlanDefinition/f:action">
      <sch:assert test="count(f:code) &lt;= 1">code: maximum cardinality of 'code' is 1</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>f:PlanDefinition/f:action/f:trigger</sch:title>
    <sch:rule context="f:PlanDefinition/f:action/f:trigger">
      <sch:assert test="count(f:id) &lt;= 1">id: maximum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:extension[@url = 'http://hl7.org/fhir/us/ecr/StructureDefinition/us-ph-named-eventtype-extension']) &lt;= 1">extension with URL = 'http://hl7.org/fhir/us/ecr/StructureDefinition/us-ph-named-eventtype-extension': maximum cardinality of 'extension' is 1</sch:assert>
      <sch:assert test="count(f:type) &gt;= 1">type: minimum cardinality of 'type' is 1</sch:assert>
      <sch:assert test="count(f:type) &lt;= 1">type: maximum cardinality of 'type' is 1</sch:assert>
      <sch:assert test="count(f:name) &lt;= 1">name: maximum cardinality of 'name' is 1</sch:assert>
      <sch:assert test="count(f:timing[x]) &lt;= 1">timing[x]: maximum cardinality of 'timing[x]' is 1</sch:assert>
      <sch:assert test="count(f:condition) &lt;= 1">condition: maximum cardinality of 'condition' is 1</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>f:PlanDefinition/f:action/f:input</sch:title>
    <sch:rule context="f:PlanDefinition/f:action/f:input">
      <sch:assert test="count(f:id) &lt;= 1">id: maximum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:extension[@url = 'http://hl7.org/fhir/us/ecr/StructureDefinition/us-ph-relateddata-extension']) &lt;= 1">extension with URL = 'http://hl7.org/fhir/us/ecr/StructureDefinition/us-ph-relateddata-extension': maximum cardinality of 'extension' is 1</sch:assert>
      <sch:assert test="count(f:type) &gt;= 1">type: minimum cardinality of 'type' is 1</sch:assert>
      <sch:assert test="count(f:type) &lt;= 1">type: maximum cardinality of 'type' is 1</sch:assert>
      <sch:assert test="count(f:subject[x]) &lt;= 1">subject[x]: maximum cardinality of 'subject[x]' is 1</sch:assert>
      <sch:assert test="count(f:limit) &lt;= 1">limit: maximum cardinality of 'limit' is 1</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>f:PlanDefinition/f:action/f:input/f:codeFilter</sch:title>
    <sch:rule context="f:PlanDefinition/f:action/f:input/f:codeFilter">
      <sch:assert test="count(f:id) &lt;= 1">id: maximum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:path) &lt;= 1">path: maximum cardinality of 'path' is 1</sch:assert>
      <sch:assert test="count(f:searchParam) &lt;= 1">searchParam: maximum cardinality of 'searchParam' is 1</sch:assert>
      <sch:assert test="count(f:valueSet) &lt;= 1">valueSet: maximum cardinality of 'valueSet' is 1</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>f:PlanDefinition/f:action/f:input/f:dateFilter</sch:title>
    <sch:rule context="f:PlanDefinition/f:action/f:input/f:dateFilter">
      <sch:assert test="count(f:id) &lt;= 1">id: maximum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:path) &lt;= 1">path: maximum cardinality of 'path' is 1</sch:assert>
      <sch:assert test="count(f:searchParam) &lt;= 1">searchParam: maximum cardinality of 'searchParam' is 1</sch:assert>
      <sch:assert test="count(f:value[x]) &lt;= 1">value[x]: maximum cardinality of 'value[x]' is 1</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>f:PlanDefinition/f:action/f:input/f:sort</sch:title>
    <sch:rule context="f:PlanDefinition/f:action/f:input/f:sort">
      <sch:assert test="count(f:id) &lt;= 1">id: maximum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:path) &gt;= 1">path: minimum cardinality of 'path' is 1</sch:assert>
      <sch:assert test="count(f:path) &lt;= 1">path: maximum cardinality of 'path' is 1</sch:assert>
      <sch:assert test="count(f:direction) &gt;= 1">direction: minimum cardinality of 'direction' is 1</sch:assert>
      <sch:assert test="count(f:direction) &lt;= 1">direction: maximum cardinality of 'direction' is 1</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>f:PlanDefinition/f:action/f:output</sch:title>
    <sch:rule context="f:PlanDefinition/f:action/f:output">
      <sch:assert test="count(f:id) &lt;= 1">id: maximum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:type) &gt;= 1">type: minimum cardinality of 'type' is 1</sch:assert>
      <sch:assert test="count(f:type) &lt;= 1">type: maximum cardinality of 'type' is 1</sch:assert>
      <sch:assert test="count(f:subject[x]) &lt;= 1">subject[x]: maximum cardinality of 'subject[x]' is 1</sch:assert>
      <sch:assert test="count(f:limit) &lt;= 1">limit: maximum cardinality of 'limit' is 1</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>f:PlanDefinition/f:action/f:output/f:codeFilter</sch:title>
    <sch:rule context="f:PlanDefinition/f:action/f:output/f:codeFilter">
      <sch:assert test="count(f:id) &lt;= 1">id: maximum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:path) &lt;= 1">path: maximum cardinality of 'path' is 1</sch:assert>
      <sch:assert test="count(f:searchParam) &lt;= 1">searchParam: maximum cardinality of 'searchParam' is 1</sch:assert>
      <sch:assert test="count(f:valueSet) &lt;= 1">valueSet: maximum cardinality of 'valueSet' is 1</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>f:PlanDefinition/f:action/f:output/f:dateFilter</sch:title>
    <sch:rule context="f:PlanDefinition/f:action/f:output/f:dateFilter">
      <sch:assert test="count(f:id) &lt;= 1">id: maximum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:path) &lt;= 1">path: maximum cardinality of 'path' is 1</sch:assert>
      <sch:assert test="count(f:searchParam) &lt;= 1">searchParam: maximum cardinality of 'searchParam' is 1</sch:assert>
      <sch:assert test="count(f:value[x]) &lt;= 1">value[x]: maximum cardinality of 'value[x]' is 1</sch:assert>
    </sch:rule>
  </sch:pattern>
  <sch:pattern>
    <sch:title>f:PlanDefinition/f:action/f:output/f:sort</sch:title>
    <sch:rule context="f:PlanDefinition/f:action/f:output/f:sort">
      <sch:assert test="count(f:id) &lt;= 1">id: maximum cardinality of 'id' is 1</sch:assert>
      <sch:assert test="count(f:path) &gt;= 1">path: minimum cardinality of 'path' is 1</sch:assert>
      <sch:assert test="count(f:path) &lt;= 1">path: maximum cardinality of 'path' is 1</sch:assert>
      <sch:assert test="count(f:direction) &gt;= 1">direction: minimum cardinality of 'direction' is 1</sch:assert>
      <sch:assert test="count(f:direction) &lt;= 1">direction: maximum cardinality of 'direction' is 1</sch:assert>
    </sch:rule>
  </sch:pattern>
</sch:schema>
