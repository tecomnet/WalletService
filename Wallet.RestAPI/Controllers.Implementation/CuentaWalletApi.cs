using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Wallet.DOM.Enums;
using Wallet.Funcionalidad.Functionality.BitacoraTransaccionFacade;
using Wallet.Funcionalidad.Functionality.CuentaWalletFacade;
using Wallet.Funcionalidad.Functionality.GestionWallet;
using Wallet.RestAPI.Models;
using Wallet.RestAPI.Helpers;

namespace Wallet.RestAPI.Controllers.Implementation;

/// <summary>
/// Implementation of the CuentaWallet API controller.
/// </summary>
public class CuentaWalletApiController(
    ICuentaWalletFacade cuentaWalletFacade,
    ITarjetaVinculadaFacade tarjetaVinculadaFacade,
    ITarjetaEmitidaFacade tarjetaEmitidaFacade,
    IBitacoraTransaccionFacade bitacoraTransaccionFacade,
    IMapper mapper)
    : CuentaWalletApiControllerBase
{
    private async Task<int> GetClienteIdFromWalletId(string idWallet)
    {
        if (!int.TryParse(idWallet, out var id))
        {
            throw new ArgumentException("Invalid Wallet ID format");
        }

        var wallet = await cuentaWalletFacade.ObtenerPorIdAsync(id);
        return wallet.IdCliente;
    }

    /// <inheritdoc />
    public override async Task<IActionResult> GetCuentaWalletPorClienteAsync(string version, int? idCliente)
    {
        if (idCliente == null) throw new ArgumentNullException(nameof(idCliente));

        // Call facade method
        var wallet = await cuentaWalletFacade.ObtenerPorClienteAsync(idCliente: idCliente.Value);

        // Map to response model
        var response = mapper.Map<CuentaWalletResult>(source: wallet);

        // Return OK response
        return Ok(value: response);
    }

    /// <inheritdoc />
    public override async Task<IActionResult> GetTarjetasEmitidasPorWalletAsync(string version, string idWallet)
    {
        var idCliente = await GetClienteIdFromWalletId(idWallet);
        var tarjetas = await tarjetaEmitidaFacade.ObtenerTarjetasPorClienteAsync(idCliente);
        var response = mapper.Map<List<TarjetaEmitidaResult>>(tarjetas);
        return Ok(response);
    }

    /// <inheritdoc />
    public override async Task<IActionResult> GetTarjetasVinculadasPorClienteAsync(string version, string idWallet)
    {
        var idCliente = await GetClienteIdFromWalletId(idWallet);
        var tarjetas = await tarjetaVinculadaFacade.ObtenerTarjetasPorClienteAsync(idCliente);
        var response = mapper.Map<List<TarjetaVinculadaResult>>(tarjetas);
        return Ok(response);
    }

    /// <inheritdoc />
    public override async Task<IActionResult> GetTransaccionesPorWalletAsync(string version, string idWallet)
    {
        var idCliente = await GetClienteIdFromWalletId(idWallet);
        // Note: Facade currently gets by client. Ideally should get by wallet directly if possible, 
        // but matching Client logic for now as per facade availability.
        var transacciones = await bitacoraTransaccionFacade.ObtenerPorClienteAsync(idCliente);
        var response = mapper.Map<List<TransaccionResult>>(transacciones);
        return Ok(response);
    }

    /// <inheritdoc />
    public override async Task<IActionResult> PostTransaccionesPorWalletAsync(string version, string idWallet,
        TransaccionRequest body)
    {
        if (!int.TryParse(idWallet, out var idBilletera))
        {
            throw new ArgumentException("Invalid Wallet ID format");
        }

        // Enum conversion handling
        // Assuming body.TipoTransaccion is matching the string enum or needs parsing 
        // Logic depends on Facade expecting string or Enum. Facade expects string 'tipo'.
        // Request body enum is mapped as string in facade.

        var transaccion = await bitacoraTransaccionFacade.GuardarTransaccionAsync(
            idBilletera: idBilletera,
            monto: (decimal)body.Monto,
            tipo: body.TipoTransaccion.ToString(),
            direccion: "N/A", // Not provided in request? Check Spec. Spec says direccion is required in Result, but not in Request? Request has 'concepto'.
            estatus: "Completada", // Default?
            creationUser: this.GetAuthenticatedUserGuid(),
            refExternaId: null
        );

        var response = mapper.Map<TransaccionResult>(transaccion);
        return Created($"/{version}/cuentaWallet/{idWallet}/transaccion", response);
    }

    /// <inheritdoc />
    public override async Task<IActionResult> SolicitarTarjetaFisicaAsync(SolicitarTarjetaFisicaRequest body,
        string version,
        string idWallet)
    {
        var idCliente = await GetClienteIdFromWalletId(idWallet);
        // Verify body.IdCliente matches wallet's client? Or just use authorized user?
        // Using resolved client ID for safety.

        var tarjeta =
            await tarjetaEmitidaFacade.SolicitarTarjetaFisicaAsync(idCliente, body.NombreImpreso,
                this.GetAuthenticatedUserGuid());
        var response = mapper.Map<TarjetaEmitidaResult>(tarjeta);
        return Created($"/{version}/cuentaWallet/{idWallet}/tarjetasemitidas/fisica", response);
    }

    /// <inheritdoc />
    public override async Task<IActionResult> SolicitarTarjetaVirtualAsync(SolicitarTarjetaVirtualRequest body,
        string version,
        string idWallet)
    {
        var idCliente = await GetClienteIdFromWalletId(idWallet);
        var tarjeta =
            await tarjetaEmitidaFacade.SolicitarTarjetaVirtualAdicionalAsync(idCliente,
                this.GetAuthenticatedUserGuid());
        var response = mapper.Map<TarjetaEmitidaResult>(tarjeta);
        return Created($"/{version}/cuentaWallet/{idWallet}/tarjetasemitidas/virtual", response);
    }
}
