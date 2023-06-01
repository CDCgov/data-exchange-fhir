# Comparison Summary

Bundles were submitted to FHIR Server $Bundle for responses.

---

## Operation Outcome, **Error**

Bundles were rejected.

|         Bundle                       |    OperationOutcome                                               |
| ---------------------------------    | ---------------------------------                                 |
| 39bcc20c-85c5-41ba-b080-560aef1190ae | Asked to deserialize unknown type 'Leslyn'                        |
| 782e4762-f95c-46de-be4e-aea6e7b0198a | Encountered unknown element 'resourcexyz'                         |
| 815f0ee8-3d9a-4805-acf7-a3ce406587fa | Literal '2020-03-2415:00:00+00:00' cannot be parsed as a dateTime |
| 815f0ee8-3d9a-4805-acf7-a3ce406587fa | Asked to deserialize unknown type 'Medrequest'                    |

---

## Operation Outcome, **No Error**

Bundles were accepted.

|         Bundle                       |    Change     Detail                              |
| ---------------------------------    | ---------------------------------                 |
| 5b507b02-ef88-4a15-9a70-131950b41202 | system/code display change                        |
| 8cde74ac-229d-4f7c-8067-da770a53fa95 | resourceType Encounter deleted                    |
| 66d6ad4a-9bbd-4b1b-b3bf-4b050ea27796 | system code changed from numeric to name 'Leslyn' |
| 782e4762-f95c-46de-be4e-aea6e7b0198a | resourceType Patient deleted                      |
| 68082e1f-64c8-4962-a2a8-d0f2b84337d9 | encounter reference deleted                       |
| 87584a2d-1741-4e08-b93d-3bf39fb02585 | system codes and practitioner reference changes   |
| 3814394c-86ec-4944-9411-4b7e946f0f4a | two resourceType Immunization deleted             |
| 37214310-4dd6-47b9-b427-cacb5f4ee7e8 | code, vaccineCode (multiple), reasonCode delete   |

## Notes

- Full details in bundle files: before, after, before_response, after_response, and some saved pics of the comparison.
- Bundle text difference was compared with VS Code, select 2 files, right click, Compare Selected.