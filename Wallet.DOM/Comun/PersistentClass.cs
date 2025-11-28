using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Wallet.DOM.Comun;

/// <summary>
/// Clase base abstracta para todas las entidades persistentes en el dominio.
/// Proporciona propiedades comunes como identificadores, marcas de tiempo y usuarios de creación/modificación.
/// </summary>
public abstract class PersistentClass
{
    /// <summary>
    /// Constructor predeterminado de la clase <see cref="PersistentClass"/>.
    /// </summary>
    public PersistentClass()
    {
    }

    /// <summary>
    /// Constructor protegido interno para inicializar una nueva instancia de <see cref="PersistentClass"/>.
    /// Establece el GUID, las marcas de tiempo y el usuario de creación/modificación.
    /// </summary>
    /// <param name="creationUser">El GUID del usuario que crea la entidad.</param>
    /// <param name="testCase">Opcional. Un identificador de caso de prueba para fines de prueba.</param>
    protected internal PersistentClass(Guid creationUser, string? testCase = null)
    {
        this.Guid = Guid.NewGuid();
        this.CreationTimestamp = this.ModificationTimestamp = DateTime.Now;
        this.CreationUser = this.ModificationUser = creationUser;
        this.TestCaseID = testCase;
    }

    /// <summary>
    /// Actualiza la marca de tiempo de modificación y el usuario de modificación de la entidad.
    /// </summary>
    /// <param name="modificationUser">El GUID del usuario que modifica la entidad.</param>
    protected internal void Update(Guid modificationUser)
    {
        this.ModificationTimestamp = DateTime.Now;
        this.ModificationUser = modificationUser;
    }

    /// <summary>
    /// Obtiene o establece el identificador único de la entidad en la base de datos.
    /// </summary>
    [Key]
    public virtual int Id { get; protected internal set; }

    /// <summary>
    /// Obtiene el token de concurrencia utilizado para control de concurrencia optimista.
    /// </summary>
    [Timestamp]
    public
#nullable disable
        byte[] ConcurrencyToken { get; private set; }

    /// <summary>
    /// Obtiene o establece el identificador único global (GUID) de la entidad.
    /// </summary>
    public Guid Guid { get; protected internal set; }

    /// <summary>
    /// Obtiene o establece la marca de tiempo de creación de la entidad.
    /// </summary>
    [Required]
    [DataType(dataType: DataType.DateTime)]
    public DateTime CreationTimestamp { get; protected internal set; }

    /// <summary>
    /// Obtiene o establece la última marca de tiempo de modificación de la entidad.
    /// </summary>
    [Required]
    [DataType(dataType: DataType.DateTime)]
    public DateTime ModificationTimestamp { get; protected internal set; }

    /// <summary>
    /// Obtiene o establece el GUID del usuario que creó la entidad.
    /// </summary>
    [Required]
    public Guid CreationUser { get; protected internal set; }

    /// <summary>
    /// Obtiene o establece el GUID del último usuario que modificó la entidad.
    /// </summary>
    [Required]
    public Guid ModificationUser { get; protected internal set; }

    /// <summary>
    /// Obtiene o establece un identificador de caso de prueba opcional.
    /// Utilizado principalmente para fines de prueba y depuración.
    /// </summary>
    [MaxLength(length: 100)]
    public
#nullable enable
        string? TestCaseID { get; [param: AllowNull] protected internal set; }
}
