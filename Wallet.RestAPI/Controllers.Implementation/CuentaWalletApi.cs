using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Wallet.DOM.Enums;
using Wallet.DOM.Modelos.GestionWallet;
using Wallet.Funcionalidad.Functionality.BitacoraTransaccionFacade;
using Wallet.Funcionalidad.Functionality.CuentaWalletFacade;
using Wallet.Funcionalidad.Functionality.DetallesPagoServicioFacade;
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
    IDetallesPagoServicioFacade detallesPagoServicioFacade,
    IMapper mapper)
    : CuentaWalletApiControllerBase
{
    private async Task<int> GetClienteIdFromWalletId(int idWallet)
    {
        var wallet = await cuentaWalletFacade.ObtenerPorIdAsync(idWallet: idWallet);
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
    public override async Task<IActionResult> GetTarjetasEmitidasPorWalletAsync(string version, int? idWallet)
    {
        var idCliente = await GetClienteIdFromWalletId(idWallet:idWallet.Value);
        var tarjetas = await tarjetaEmitidaFacade.ObtenerTarjetasPorClienteAsync(idCliente);
        var response = mapper.Map<List<TarjetaEmitidaResult>>(tarjetas);
        return Ok(response);
    }

    /// <inheritdoc />
    public override async Task<IActionResult> GetTarjetasVinculadasPorWalletAsync(string version, int? idWallet)
    {
        var idCliente = await GetClienteIdFromWalletId(idWallet:idWallet.Value);
        var tarjetas = await tarjetaVinculadaFacade.ObtenerTarjetasPorClienteAsync(idCliente);
        var response = mapper.Map<List<TarjetaVinculadaResult>>(tarjetas);
        return Ok(response);
    }

    /// <inheritdoc />
    public override async Task<IActionResult> GetTransaccionesPorWalletAsync(string version, int? idWallet)
    {
        var idCliente = await GetClienteIdFromWalletId(idWallet:idWallet.Value);
        // Note: Facade currently gets by client. Ideally should get by wallet directly if possible, 
        // but matching Client logic for now as per facade availability.
        var transacciones = await bitacoraTransaccionFacade.ObtenerPorClienteAsync(idCliente);
        var response = mapper.Map<List<TransaccionResult>>(transacciones);
        return Ok(response);
    }

    /// <inheritdoc />
    public override async Task<IActionResult> PostTransaccionesPorWalletAsync(string version, int? idWallet,
        TransaccionServicioRequest body)
    {
        // Enum conversion handling
        // Assuming body.TipoTransaccion is matching the string enum or needs parsing 
        // Logic depends on Facade expecting string or Enum. Facade expects string 'tipo'.
        // Request body enum is mapped as string in facade.

        /*var transaccion = await bitacoraTransaccionFacade.GuardarTransaccionAsync(
            cuentaWalletId: cuentaWalletId,
            monto: (decimal)body.Monto,
            tipo: EnumExtensions.GetEnumMemberValue(body.TipoTransaccion),
            direccion: "CARGO", // Not provided in request? Check Spec. Spec says direccion is required in Result, but not in Request? Request has 'concepto'.
            estatus: "Completada", // Default?
            creationUser: this.GetAuthenticatedUserGuid(),
            refExternaId: null
        );*/
        BitacoraTransaccion transaccion;
        if (body.TipoTransaccion == TipoTransaccionEnum.RecargaTelefonicaEnum)
        {
            transaccion = await detallesPagoServicioFacade.RegistrarPagoServicioAsync(
                idBilletera: idWallet.Value,
                monto: body.Monto.Value,
                tipo: EnumExtensions.GetEnumMemberValue(body.TipoTransaccion),
                direccion: "CARGO",
                estatus: "Completada",
                idProducto: body.IdProducto.Value,
                numeroReferencia: body.NumeroReferencia,
                creationUser: this.GetAuthenticatedUserGuid()
            );
        }
        else
        {
            throw new ArgumentException("Transaccion aun no disponible");
        }
        
        var response = mapper.Map<TransaccionResult>(transaccion);
        return Created($"/{version}/cuentaWallet/{idWallet}/transaccion", response);
    }

    /// <inheritdoc />
    public override async Task<IActionResult> SolicitarTarjetaFisicaAsync(SolicitarTarjetaFisicaRequest body,
        string version,
        int? idWallet)
    {
        var idCliente = await GetClienteIdFromWalletId(idWallet:idWallet.Value);
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
        int? idWallet)
    {
        var idCliente = await GetClienteIdFromWalletId(idWallet:idWallet.Value);
        var tarjeta =
            await tarjetaEmitidaFacade.SolicitarTarjetaVirtualAdicionalAsync(idCliente,
                this.GetAuthenticatedUserGuid());
        var response = mapper.Map<TarjetaEmitidaResult>(tarjeta);
        return Created($"/{version}/cuentaWallet/{idWallet}/tarjetasemitidas/virtual", response);
    }
}
