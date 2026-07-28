using System;
using System.Web.Http.Controllers;

namespace Web.Grind._808nd.com.Areas.RouteDebugger.Models
{
    /// <summary>
    /// Represents the parameters.
    /// </summary>
    public class HttpParameterDescriptorInfo
    {
        public HttpParameterDescriptorInfo(HttpParameterDescriptor descriptor)
        {
            ParameterName = descriptor.ParameterName;
            ParameterType = descriptor.ParameterType;
            ParameterTypeName = descriptor.ParameterType.Name;
        }

        public string ParameterName { get; set; }

        public Type ParameterType { get; set; }

        public string ParameterTypeName { get; set; }
    }
}
