using System.ComponentModel.DataAnnotations;
using Wallet.DOM.Comun;
using Wallet.DOM.Errors;

namespace Wallet.DOM.Modelos;

/// <summary>
/// Representa el estado de una entidad, como un cliente.
/// Este debe ser un catálogo gestionado a través de una interfaz de administración web.
/// </summary>
public class Estado : ValidatablePersistentObjectLogicalDelete
{
    /// <summary>
    /// Define las restricciones de las propiedades de esta clase para la validación.
    /// </summary>
    protected override List<PropertyConstraint> PropertyConstraints =>
    [
        PropertyConstraint.StringPropertyConstraint(
            propertyName: nameof(Nombre),
            isRequired: true,
            minimumLength: 1,
            maximumLength: 100),
    ];
    
    /// <summary>
    /// Obtiene o establece el nombre del estado.
    /// Es un campo requerido con una longitud máxima de 100 caracteres.
    /// </summary>
    [Required]
    [MaxLength(length: 100)]
    public string Nombre { get; internal set; }

    /// <summary>
    /// Obtiene la lista de clientes asociados a este estado.
    /// </summary>
    public List<Cliente> Clientes { get; private set; }
    
    /// <summary>
    /// Constructor predeterminado de la clase <see cref="Estado"/>.
    /// Inicializa una nueva instancia de la clase base.
    /// </summary>
    public Estado() : base() { }

    /// <summary>
    /// Constructor parametrizado de la clase <see cref="Estado"/>.
    /// Inicializa una nueva instancia del estado con el nombre especificado y el usuario de creación.
    /// </summary>
    /// <param name="nombre">El nombre del estado.</param>
    /// <param name="creationUser">El GUID del usuario que crea el estado.</param>
    /// <param name="testCase">Parámetro opcional para casos de prueba.</param>
    /// <exception cref="EMGeneralAggregateException">Se lanza si hay errores de validación de propiedades.</exception>
    public Estado(string nombre, Guid creationUser,
        string? testCase = null) : base(creationUser: creationUser, testCase: testCase)
    {
        // Inicializa la lista de excepciones para la validación
        List<EMGeneralException> exceptions = new();
        // Valida la propiedad 'Nombre'
        IsPropertyValid(propertyName: nameof(Nombre), value: nombre, exceptions: ref exceptions);
        // Si hay excepciones, las lanza
        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);
        // Asigna el valor a la propiedad
        this.Nombre = nombre;
    }

    /// <summary>
    /// Actualiza el nombre del estado y registra al usuario que realiza la modificación.
    /// </summary>
    /// <param name="nombre">El nuevo nombre del estado.</param>
    /// <param name="modificationUser">El GUID del usuario que modifica el estado.</param>
    /// <exception cref="EMGeneralAggregateException">Se lanza si hay errores de validación de propiedades.</exception>
    public void Actualizar(string nombre, Guid modificationUser)
    {
        // Inicializa la lista de excepciones para la validación
        List<EMGeneralException> exceptions = new();
        // Valida la propiedad 'Nombre'
        IsPropertyValid(propertyName: nameof(Nombre), value: nombre, exceptions: ref exceptions);
        // Si hay excepciones, las lanza
        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);
        // Asigna el nuevo valor a la propiedad
        this.Nombre = nombre;
        // Actualiza los campos de auditoría de la clase base
        base.Update(modificationUser: modificationUser);
    }
 
}