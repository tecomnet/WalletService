using System.ComponentModel.DataAnnotations;
using Wallet.DOM.Comun;
using Wallet.DOM.Errors;

namespace Wallet.DOM.Modelos;

/// <summary>
/// Representa una configuración clave-valor en el sistema.
/// Hereda de <see cref="ValidatablePersistentObjectLogicalDelete"/> para proporcionar validación y eliminación lógica.
/// </summary>
public class KeyValueConfig : ValidatablePersistentObjectLogicalDelete
{
    /// <summary>
    /// Define las restricciones de validación para las propiedades.
    /// </summary>
    protected override List<PropertyConstraint> PropertyConstraints =>
    [
        // Restricción para la llave
        PropertyConstraint.StringPropertyConstraint(
            propertyName: nameof(Key),
            isRequired: true,
            minimumLength: 1,
            maximumLength: 100),
        // Restricción para el valor
        PropertyConstraint.StringPropertyConstraint(
            propertyName: nameof(Value),
            isRequired: true,
            minimumLength: 1,
            maximumLength: 2000)
    ];

    /// <summary>
    /// La clave de la configuración.
    /// </summary>
    [Required]
    [MaxLength(length: 100)]
    public string Key { get; private set; }

    /// <summary>
    /// El valor de la configuración.
    /// </summary>
    [Required]
    [MaxLength(length: 2000)]
    public string Value { get; private set; }

    /// <summary>
    /// Constructor por defecto necesario para EF Core.
    /// </summary>
    public KeyValueConfig()
    {
        Key = null!;
        Value = null!;
    }

    /// <summary>
    /// Constructor para crear una nueva configuración.
    /// </summary>
    /// <param name="key">La clave.</param>
    /// <param name="value">El valor.</param>
    /// <param name="creationUser">Usuario que crea el registro.</param>
    /// <param name="testCase">Identificador de caso de prueba (opcional).</param>
    public KeyValueConfig(string key, string value, Guid creationUser, string? testCase = null)
        : base(creationUser: creationUser, testCase: testCase)
    {
        List<EMGeneralException> exceptions = new();

        IsPropertyValid(propertyName: nameof(Key), value: key, exceptions: ref exceptions);
        IsPropertyValid(propertyName: nameof(Value), value: value, exceptions: ref exceptions);

        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);

        this.Key = key;
        this.Value = value;
    }

    /// <summary>
    /// Actualiza el valor de la configuración.
    /// </summary>
    /// <param name="value">El nuevo valor a asignar.</param>
    /// <param name="modificationUser">Usuario que realiza la modificación.</param>
    public void Update(string value, Guid modificationUser)
    {
        List<EMGeneralException> exceptions = new();
        IsPropertyValid(propertyName: nameof(Value), value: value, exceptions: ref exceptions);

        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);

        if (Value == value) return;

        Value = value;
        base.Update(modificationUser: modificationUser);
    }
}
