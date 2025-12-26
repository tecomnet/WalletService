using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wallet.DOM.Comun;
using Wallet.DOM.Errors;
using Wallet.DOM.Modelos.GestionCliente;

namespace Wallet.DOM.Modelos.GestionWallet;

/// <summary>
/// Representa una cuenta de billetera (Wallet) asociada a un cliente.
/// </summary>
public class CuentaWallet : ValidatablePersistentObjectLogicalDelete
{
    protected override List<PropertyConstraint> PropertyConstraints =>
    [
        PropertyConstraint.StringPropertyConstraint(nameof(Moneda), true, 3, 3),
        PropertyConstraint.StringPropertyConstraint(nameof(CuentaCLABE), true, 18, 18)
    ];

    /// <summary>
    /// Identificador del cliente propietario de la cuenta.
    /// </summary>
    /// <summary>
    /// Identificador del cliente propietario de la cuenta.
    /// </summary>
    [Required]
    public int IdCliente { get; private set; }

    /// <summary>
    /// Moneda de la cuenta (MXN, USD).
    /// </summary>
    [Required]
    [MaxLength(3)]
    public string Moneda { get; private set; }

    /// <summary>
    /// Saldo actual de la cuenta.
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(19,2)")]
    public decimal SaldoActual { get; private set; }

    /// <summary>
    /// Cuenta CLABE virtual (STP).
    /// </summary>
    [Required]
    [MaxLength(18)]
    public string CuentaCLABE { get; private set; }

    /// <summary>
    /// Propiedad de navegación al Cliente.
    /// </summary>
    [ForeignKey(nameof(IdCliente))]
    public virtual Cliente? Cliente { get; set; }

    /// <summary>
    /// Bitácora de transacciones asociadas a esta cuenta.
    /// </summary>
    public virtual ICollection<BitacoraTransaccion> BitacoraTransacciones { get; set; } =
        new List<BitacoraTransaccion>();

    // Constructor requerido por EF Core
    protected CuentaWallet()
    {
    }

    public CuentaWallet(int idCliente, string moneda, string cuentaCLABE, Guid creationUser, string? testCase = null)
        : base(creationUser, testCase)
    {
        List<EMGeneralException> exceptions = new();

        IsPropertyValid(nameof(Moneda), moneda, ref exceptions);
        IsPropertyValid(nameof(CuentaCLABE), cuentaCLABE, ref exceptions);

        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions);

        IdCliente = idCliente;
        Moneda = moneda;
        CuentaCLABE = cuentaCLABE;
        SaldoActual = 0; // Saldo inicial
    }

    public void ActualizarSaldo(decimal nuevoSaldo, Guid modificationUser)
    {
        SaldoActual = nuevoSaldo;
        Update(modificationUser);
    }
}
