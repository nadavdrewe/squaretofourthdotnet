using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revel._808nd.com.OperationsReport.Mongo
{
    public static class OpsMongoDbStrings
    {
        public static string DbName { get; } = "grindRevelOps";
        public static string OpsReportCollectionName { get; } = "hourlyOpsReports";
    }
}
