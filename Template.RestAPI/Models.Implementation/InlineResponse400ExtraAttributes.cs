using System.Collections.Generic;

namespace Template.RestAPI.Models
{
    public partial class InlineResponse400ExtraAttributes
    {
        #region Constructors
        public InlineResponse400ExtraAttributes(){}
        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="descriptionDynamicContents">List of the dynamic contents used for the detail</param>
        /// <param name="module">Module of the service generating the error</param>
        /// <param name="serviceName">Name of service</param>
        /// <param name="serviceLocation">Location of the instance of the service</param>
        public InlineResponse400ExtraAttributes(
#nullable enable
            List<string>? descriptionDynamicContents,
            string? module,
            string? serviceName,
            string? serviceLocation
#nullable disable
            )
        {
            // Set the attributes
            DesciptionDynamicContents = descriptionDynamicContents;
            Module = module;
            ServiceName = serviceName;
            ServiceLocation = serviceLocation;
        }
        #endregion
    }
}
