using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.CosmosDb.Autoscale 
{
    public class CommonAlertParser
    {
        public static RequestUnitAlert Parse(dynamic commonAlert) 
        {
            var allOf =  commonAlert?.alertContext?.allOf?.FirstOrDefault();
            var dimensions = allOf?.dimensions as List<dynamic>;

            return new RequestUnitAlert {
                Name = allOf?.metricName,
                AccountName = dimensions.First(item => item.name == "GlobalDatabaseAccountName").value,
                AccountName = dimensions.First(item => item.name == "DatabaseName").value,
            };
        }
    }
}