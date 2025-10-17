using Template.DOM.Errors;
using Template.Funcionalidad.Helper;

namespace Template.Funcionalidad.ServiceClient;

internal static class GeneralConfigurationSettingsData
{
    internal const string ServiceName = "GeneralMonitorService"; 
    internal const string Version = "0.1";
    internal const string RemoteServiceNameConfig = "general-configuration";
    internal const string ServiceErrorCode = "EM-INCORRECT-AUTHORIZATION-TYPE";
}

public interface IGeneralConfigurationManagementFacade
{
    string EncryptText(string plainText);
    string DecryptText(string cypher);
}

public class GeneralConfigurationServiceFacade(
    IServiceProvider serviceProvider,
    UrlBuilder urlBuilder)
    : ServiceFacadeBase(urlBuilder: urlBuilder,
        runningServiceName: GeneralConfigurationSettingsData.ServiceName,
        runningModuleName: nameof(GeneralConfigurationServiceFacade),
        remoteServiceNameConfig: GeneralConfigurationSettingsData.RemoteServiceNameConfig,
        version: GeneralConfigurationSettingsData.Version), IGeneralConfigurationManagementFacade
{
    protected override EmGeneralAggregateException? ExtractEMGeneralAggregateException(Exception exception)
    {
        throw new NotImplementedException();
    }

    public string EncryptText(string plainText)
    {
        throw new NotImplementedException();
    }

    public string DecryptText(string cypher)
    {
        throw new NotImplementedException();
    }
}