using System.ComponentModel.DataAnnotations;

namespace Wallet.DOM.Comun;

/// <summary>
/// Clase base abstracta para entidades persistentes que soportan borrado lógico.
/// Permite activar y desactivar registros en lugar de eliminarlos físicamente.
/// </summary>
public abstract class PersistentClassLogicalDelete : PersistentClass
{
    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="PersistentClassLogicalDelete"/>.
    /// </summary>
    public PersistentClassLogicalDelete()
    {
    }

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="PersistentClassLogicalDelete"/> con el usuario de creación y un caso de prueba opcional.
    /// Establece el estado inicial de <see cref="IsActive"/> a <c>true</c>.
    /// </summary>
    /// <param name="creationUser">El GUID del usuario que crea la entidad.</param>
    /// <param name="testCase">Un identificador opcional para casos de prueba.</param>
    protected internal PersistentClassLogicalDelete(Guid creationUser, string? testCase = null)
        : base(creationUser: creationUser, testCase: testCase)
    {
        this.IsActive = true; // Por defecto, una nueva entidad está activa.
    }

    /// <summary>
    /// Obtiene o establece un valor que indica si la entidad está activa (no borrada lógicamente).
    /// Este campo es requerido.
    /// </summary>
    [Required]
    public bool IsActive { get; protected internal set; }

    /// <summary>
    /// Desactiva lógicamente la entidad, marcándola como inactiva.
    /// Actualiza la información de modificación de la entidad.
    /// </summary>
    /// <param name="modificationUser">El GUID del usuario que realiza la desactivación.</param>
    public virtual void Deactivate(Guid modificationUser)
    {
        this.IsActive = false; // Marca la entidad como inactiva.
        this.Update(modificationUser: modificationUser); // Actualiza los metadatos de modificación.
    }

    /// <summary>
    /// Activa lógicamente la entidad, marcándola como activa.
    /// Actualiza la información de modificación de la entidad.
    /// </summary>
    /// <param name="modificationUser">El GUID del usuario que realiza la activación.</param>
    public virtual void Activate(Guid modificationUser)
    {
        this.IsActive = true; // Marca la entidad como activa.
        this.Update(modificationUser: modificationUser); // Actualiza los metadatos de modificación.
    }
}