using System.ComponentModel.DataAnnotations;
using Wallet.DOM.Comun;
using Wallet.DOM.Enums;
using Wallet.DOM.Errors;

namespace Wallet.DOM.Modelos.GestionUsuario;

/// <summary>
/// Representa una verificación de doble factor (2FA) en el sistema.
/// Contiene información sobre el SID de Twilio, el código de verificación,
/// la fecha de vencimiento, el tipo de verificación y su estado.
/// </summary>
public class Verificacion2FA : ValidatablePersistentObjectLogicalDelete
{
    /// <summary>
    /// Define las restricciones de validación para las propiedades de la entidad Verificacion2FA.
    /// </summary>
    protected override List<PropertyConstraint> PropertyConstraints =>
    [
        PropertyConstraint.StringPropertyConstraint(
            propertyName: nameof(TwilioSid),
            isRequired: true,
            minimumLength: 1,
            maximumLength: 100),
        PropertyConstraint.StringPropertyConstraint(
            propertyName: nameof(Codigo),
            isRequired: true,
            minimumLength: 4,
            maximumLength: 4),
        PropertyConstraint.DateTimePropertyConstraint(
            propertyName: nameof(FechaVencimiento),
            isRequired: true),
        PropertyConstraint.ObjectPropertyConstraint(
            propertyName: nameof(Tipo),
            isRequired: true),
        PropertyConstraint.ObjectPropertyConstraint(
            propertyName: nameof(Verificado),
            isRequired: true)
    ];

    /// <summary>
    /// Obtiene el SID (Service Identifier) de Twilio asociado a esta verificación 2FA.
    /// </summary>
    [Required]
    [MaxLength(length: 100)]
    public string TwilioSid { get; private set; }

    /// <summary>
    /// Obtiene el código de verificación de 2FA. Puede ser nulo si aún no se ha asignado o verificado.
    /// </summary>
    [MaxLength(length: 4)]
    public string? Codigo { get; private set; }

    /// <summary>
    /// Obtiene la fecha y hora de vencimiento del código de verificación.
    /// </summary>
    [Required]
    public DateTime FechaVencimiento { get; private set; }

    /// <summary>
    /// Obtiene el tipo de verificación 2FA (por ejemplo, SMS, Email).
    /// </summary>
    [Required]
    public Tipo2FA Tipo { get; private set; }

    /// <summary>
    /// Obtiene un valor que indica si la verificación 2FA ha sido completada y validada.
    /// </summary>
    [Required]
    public bool Verificado { get; private set; }

    // Relaciones

    /// <summary>
    /// Obtiene el identificador único del usuario asociado a esta verificación 2FA.
    /// </summary>
    public int UsuarioId { get; private set; }

    /// <summary>
    /// Obtiene el objeto Usuario asociado a esta verificación 2FA.
    /// </summary>
    public Usuario Usuario { get; private set; }


    /// <summary>
    /// Constructor por defecto de la clase <see cref="Verificacion2FA"/>.
    /// Inicializa una nueva instancia de la clase.
    /// </summary>
    public Verificacion2FA() : base()
    {
    }

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="Verificacion2FA"/> con los parámetros especificados.
    /// </summary>
    /// <param name="twilioSid">El SID de Twilio para la verificación.</param>
    /// <param name="fechaVencimiento">La fecha y hora de vencimiento del código.</param>
    /// <param name="tipo">El tipo de verificación 2FA.</param>
    /// <param name="creationUser">El GUID del usuario que crea esta verificación.</param>
    /// <param name="testCase">Opcional. Un identificador para casos de prueba.</param>
    /// <exception cref="EMGeneralAggregateException">Se lanza si alguna de las propiedades iniciales no es válida.</exception>
    public Verificacion2FA(string twilioSid, DateTime fechaVencimiento, Tipo2FA tipo, Guid creationUser,
        string? testCase = null) : base(creationUser: creationUser, testCase: testCase)
    {
        // Inicializa la lista de excepciones para la validación
        List<EMGeneralException> exceptions = new();
        // Valida las propiedades iniciales
        IsPropertyValid(propertyName: nameof(TwilioSid), value: twilioSid, exceptions: ref exceptions);
        IsPropertyValid(propertyName: nameof(FechaVencimiento), value: fechaVencimiento, exceptions: ref exceptions);
        IsPropertyValid(propertyName: nameof(Tipo), value: tipo, exceptions: ref exceptions);
        // Si hay excepciones, las lanza en un agregado
        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);
        // Asigna las propiedades
        TwilioSid = twilioSid;
        FechaVencimiento = fechaVencimiento;
        Tipo = tipo;
        Verificado = false; // Por defecto, una verificación recién creada no está verificada
    }

    /// <summary>
    /// Marca la verificación 2FA como verificada y asigna el código proporcionado.
    /// </summary>
    /// <param name="codigo">El código de verificación ingresado por el usuario.</param>
    /// <param name="modificationUser">El GUID del usuario que realiza la modificación.</param>
    /// <exception cref="EMGeneralAggregateException">Se lanza si el código proporcionado no es válido.</exception>
    public void MarcarComoVerificado(string codigo, Guid modificationUser)
    {
        if (this.Verificado && this.Codigo == codigo) return;

        // Inicializa la lista de excepciones para la validación
        List<EMGeneralException> exceptions = new();
        // Valida el código proporcionado
        IsPropertyValid(propertyName: nameof(Codigo), value: codigo, exceptions: ref exceptions);
        // Si hay excepciones, las lanza en un agregado
        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);

        bool hasChanges = false;
        if (!this.Verificado)
        {
            this.Verificado = true;
            hasChanges = true;
        }

        if (this.Codigo != codigo)
        {
            this.Codigo = codigo;
            hasChanges = true;
        }

        if (hasChanges)
        {
            // Actualiza los metadatos de la entidad base (fecha de modificación, usuario de modificación)
            base.Update(modificationUser: modificationUser);
        }
    }
}