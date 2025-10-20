using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Template.DOM.Comun;


public abstract class PersistentClass
{
    public PersistentClass()
    {
    }

    protected internal PersistentClass(Guid creationUser, string? testCase = null)
    {
        this.Guid = Guid.NewGuid();
        this.CreationTimestamp = this.ModificationTimestamp = DateTime.Now;
        this.CreationUser = this.ModificationUser = creationUser;
        this.TestCaseID = testCase;
    }

    protected internal void Update(Guid modificationUser)
    {
        this.ModificationTimestamp = DateTime.Now;
        this.ModificationUser = modificationUser;
    }

    [Key] public int Id { get; private set; }

    [Timestamp]
    public
#nullable disable
        byte[] ConcurrencyToken { get; private set; }

    public Guid Guid { get; protected internal set; }

    [Required]
    [DataType(DataType.DateTime)]
    public DateTime CreationTimestamp { get; protected internal set; }

    [Required]
    [DataType(DataType.DateTime)]
    public DateTime ModificationTimestamp { get; protected internal set; }

    [Required] public Guid CreationUser { get; protected internal set; }

    [Required] public Guid ModificationUser { get; protected internal set; }

    [MaxLength(100)]
    public
#nullable enable
        string? TestCaseID { get; [param: AllowNull] protected internal set; }
}
