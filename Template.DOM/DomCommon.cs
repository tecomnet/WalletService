

using Template.DOM.Errors;

namespace Template.DOM
{
    public static class DomCommon
    {
        public const string ServiceName = "TemplateService";
        private const string ModuleName = "DOM";

        private static readonly ServiceErrorsBuilder ServiceError = ServiceErrorsBuilder.Instance();

        private static EMGeneralException BuildEmGeneralException(IServiceError serviceError, List<object> dynamicContent, string module = ModuleName)
        {
            return new EMGeneralException(
                serviceError: serviceError,
                serviceName: ServiceName,
                module: module,
                descriptionDynamicContents: dynamicContent);
        }

        public static EMGeneralException BuildEmGeneralException(string errorCode, List<object> dynamicContent, string module = ModuleName)
        {
            var serviceError = ServiceError.GetError(errorCode);
            var itaGeneralException = BuildEmGeneralException(serviceError, dynamicContent, module);
            return itaGeneralException;
        }
    }
}
