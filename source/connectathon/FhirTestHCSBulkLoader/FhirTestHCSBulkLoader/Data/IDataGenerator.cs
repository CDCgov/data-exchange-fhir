using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FhirTestHCSBulkLoader.Data
{
    internal interface IDataGenerator
    {

       string GenerateData(bool isValid);

    }
}
