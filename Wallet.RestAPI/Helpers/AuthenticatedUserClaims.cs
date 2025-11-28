using System;
using System.Linq;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Wallet.DOM;
using Wallet.DOM.Errors;

namespace Wallet.RestAPI.Helpers;

/// <summary>
/// Class for authenticated users guid
/// </summary>
public static class AuthenticatedUserClaims
{
    /// <summary>
    /// Generate a Guid from user data
    /// </summary>
    /// <param name="cb"></param>Extension of ControllerBase
    /// <returns>A generated Guid</returns>
    public static Guid GetAuthenticatedUserGuid(this ControllerBase cb)
    {
        // Returns the generated guid
        var guidClaim = cb.User.Identities.ElementAt(index: 0).Claims.SingleOrDefault(predicate: c => c.Type == "Guid");
        // If guidClaim is null
        if (guidClaim is null)
        {
            // Lanza exception
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.EmClaimUserError,
                dynamicContent: [],
                module: "REST-API"));
        }
        return new Guid(g: guidClaim.Value);
    }
    
    /// <summary>
    ///  Get the token from the request header
    /// </summary>
    /// <param name="cb"></param>
    /// <returns></returns>
    public static string GetToken(this ControllerBase cb)
    {
        // Extract the authorization information from the header
        var authHeader = AuthenticationHeaderValue.Parse(input: cb.Request.Headers[key: "Authorization"]);
        // Get the token from the parameters
        var jwsToken = authHeader.Parameter;
        // Returns the generated guid
        return jwsToken;
    }
}