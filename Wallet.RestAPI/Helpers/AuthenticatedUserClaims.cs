using System;
using System.Linq;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Wallet.DOM;
using Wallet.DOM.Errors;

namespace Wallet.RestAPI.Helpers;

/// <summary>
/// Clase para los GUID de usuarios autenticados.
/// </summary>
public static class AuthenticatedUserClaims
{
    /// <summary>
    /// Genera un GUID a partir de los datos del usuario.
    /// </summary>
    /// <param name="cb">Extensión de ControllerBase.</param>
    /// <returns>Un GUID generado.</returns>
    public static Guid GetAuthenticatedUserGuid(this ControllerBase cb)
    {
        // Busca el claim de tipo "Guid" en las identidades del usuario.
        var guidClaim = cb.User.Identities.ElementAt(index: 0).Claims.SingleOrDefault(predicate: c => c.Type == "Guid");
        // Si el claim del GUID es nulo, lanza una excepción.
        if (guidClaim is null)
        {
            // Lanza una excepción indicando un error en el claim del usuario.
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.EmClaimUserError,
                dynamicContent: [],
                module: "REST-API"));
        }
        // Devuelve el GUID generado a partir del valor del claim.
        return new Guid(g: guidClaim.Value);
    }
    
    /// <summary>
    /// Obtiene el token del encabezado de la solicitud.
    /// </summary>
    /// <param name="cb">Extensión de ControllerBase.</param>
    /// <returns>El token JWS.</returns>
    public static string GetToken(this ControllerBase cb)
    {
        // Extrae la información de autorización del encabezado de la solicitud.
        var authHeader = AuthenticationHeaderValue.Parse(input: cb.Request.Headers[key: "Authorization"]);
        // Obtiene el token de los parámetros de la cabecera de autorización.
        var jwsToken = authHeader.Parameter;
        // Devuelve el token JWS.
        return jwsToken;
    }
}