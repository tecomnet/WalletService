using System.ComponentModel.DataAnnotations;
using Wallet.DOM.Comun;
using Wallet.DOM.Errors;

namespace Wallet.DOM.Modelos;

/// <summary>
/// Representa una entidad de Empresa dentro del dominio.
/// Hereda de <see cref="ValidatablePersistentObjectLogicalDelete"/> para incluir validaciones y funcionalidad de borrado lógico.
/// </summary>
public class Empresa : ValidatablePersistentObjectLogicalDelete
{
    /// <summary>
    /// Define las restricciones de las propiedades para la validación.
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
    /// Obtiene o establece el nombre de la empresa.
    /// Es un campo requerido con una longitud máxima de 100 caracteres.
    /// </summary>
    [Required]
    [MaxLength(length: 100)]
    public string Nombre { get; internal set; }

    /// <summary>
    /// Obtiene la lista de clientes asociados a esta empresa. TODO: Cambiar relación a usuarios
    /// </summary>
    /// <summary>
    /// Obtiene la lista de clientes asociados a esta empresa. TODO: Cambiar relación a usuarios
    /// </summary>
    public List<Cliente> Clientes { get; private set; }

    /// <summary>
    /// Obtiene la lista de productos asociados a esta empresa (relación muchos a muchos).
    /// </summary>
    public ICollection<Producto> Productos { get; set; } = new List<Producto>();

    /// <summary>
    /// Constructor por defecto para la clase <see cref="Empresa"/>.
    /// </summary>
    public Empresa() : base()
    {
    }

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="Empresa"/> con el nombre y usuario de creación especificados.
    /// </summary>
    /// <param name="nombre">El nombre de la empresa.</param>
    /// <param name="creationUser">El identificador del usuario que crea la empresa.</param>
    /// <param name="testCase">Opcional. Un identificador para casos de prueba.</param>
    /// <exception cref="EMGeneralAggregateException">Se lanza si las validaciones de las propiedades fallan.</exception>
    public Empresa(string nombre, Guid creationUser,
        string? testCase = null) : base(creationUser: creationUser, testCase: testCase)
    {
        // Inicializa la lista de excepciones para recolectar errores de validación.
        List<EMGeneralException> exceptions = new();
        // Valida la propiedad 'Nombre' utilizando las restricciones definidas.
        IsPropertyValid(propertyName: nameof(Nombre), value: nombre, exceptions: ref exceptions);
        // Si hay excepciones, las lanza como una excepción agregada.
        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);
        // Asignación de propiedades después de una validación exitosa.
        this.Nombre = nombre;
    }

    /// <summary>
    /// Actualiza el nombre de la empresa.
    /// </summary>
    /// <param name="nombre">El nuevo nombre de la empresa.</param>
    /// <param name="modificationUser">El identificador del usuario que realiza la modificación.</param>
    /// <exception cref="EMGeneralAggregateException">Se lanza si las validaciones de las propiedades fallan.</exception>
    public void Actualizar(string nombre, Guid modificationUser)
    {
        // Inicializa la lista de excepciones para recolectar errores de validación.
        List<EMGeneralException> exceptions = new();
        // Valida la propiedad 'Nombre' con el nuevo valor.
        IsPropertyValid(propertyName: nameof(Nombre), value: nombre, exceptions: ref exceptions);
        // Si hay excepciones, las lanza como una excepción agregada.
        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);

        if (this.Nombre == nombre) return;

        // Asignación de propiedades después de una validación exitosa.
        this.Nombre = nombre;
        // Llama al método de actualización de la clase base para registrar el usuario de modificación.
        base.Update(modificationUser: modificationUser);
    }
}