using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Moq;
using OneCDPFHIRFacade.Controllers;
using OneCDPFHIRFacade.Utilities;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace fhir_facade_tests.Tests
{
    [TestFixture]
    public class CompareTest
    {
        interface IScopeElement
        {
            String Name { get; }

            bool IsEqual(IScopeElement scopeElement);

        }


        class ClientScope : IScopeElement
        {
            public string Name { get; set; }
            

            public ClientScope(string name, int age)
            {
                Name = name;
             
            }

           public bool IsEqual(IScopeElement scopeElement)
            {
                return false;
            }
        }

        class FhirScope : IScopeElement
        {
            public string Name { get; set; }


            public FhirScope(string name, int age)
            {
                Name = name;

            }

            public bool IsEqual(IScopeElement scopeElement)
            {
                Console.WriteLine("FhirScope comparing " + scopeElement.Name + scopeElement.GetType().Name);
                if (this.Name.StartsWith(scopeElement.Name))
                {
                    return true;
                }
                return false;
            }
        }

        class OrganizationScope : IScopeElement
        {
            public string Name { get; set; }


            public OrganizationScope(string name, int age)
            {
                Name = name;

            }

            public bool IsEqual(IScopeElement scopeElement)
            {
                Console.WriteLine("OrganizationScope comparing" + scopeElement.Name);

                if (this.Name.Contains(scopeElement.Name))
                {
                    return true;
                }

                return false;
            }
        }

        class StreamScope : IScopeElement
        {
            public string Name { get; set; }


            public StreamScope(string name, int age)
            {
                Name = name;

            }

            public bool IsEqual(IScopeElement scopeElement)
            {
                Console.WriteLine("StreamScope comparing" + scopeElement.Name);

                if (this.Name.EndsWith(scopeElement.Name))
                {
                    return true;
                }

                return false;
            }
        }

        class PersonComparer : IEqualityComparer<IScopeElement>
        {
            public bool Equals(IScopeElement? x, IScopeElement? y)
            {
                // Custom comparison: Compare based on Name and Age
                if (x == null || y == null)
                    return false;
                Console.WriteLine("comparing comparing");
                return x.IsEqual(y);
            }

            public int GetHashCode(IScopeElement obj)
            {
                if (obj == null)
                    return 0;
                Console.WriteLine("asdfasdf comparing comparing");
                // Combine hash codes of Name and Age
                return obj.Name.GetHashCode() ;
            }
        }

        /*
         * 
         * ind of. 
We receive this
"scope": "system/bundle.c org/org-name1 stream/eicr-document-bundle"
Then we save it as an array by splitting the string
Then we need to check that the 
System = bundle.c
Save org as AwsConfig.UserId
Stream = bundle.Meta.Profile(after the "/")
         */


        [Test]
        public void CompareTestTest()
        {
            Console.WriteLine("Start Tests");
            var comparer = new PersonComparer();

            var clientScope = new List<IScopeElement>()
        {
            new ClientScope("bundle", 25),
            new ClientScope("OrganizationName", 25),
            new ClientScope("eicr-document-bundle", 30)
        };

            var requestScope = new List<IScopeElement>()
        {
            new FhirScope("bundle.c", 25),
            new OrganizationScope("OrganizationName", 25),
            new StreamScope("eicr-document-bundle", 30),
             new StreamScope("eicr-document-bundle2", 30),
           new StreamScope("eicr-document-bundle3", 30)

        };

       

            var comparer2 = new PersonComparer();

            // Check if list2 contains all elements from list1 using the custom comparer
            bool containsAll = clientScope.All(person => requestScope.Contains(person, comparer));

            Console.WriteLine($"group1 is a subset of group2: {containsAll}");

        }

    }

}

