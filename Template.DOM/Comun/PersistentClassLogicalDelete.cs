using System.ComponentModel.DataAnnotations;

namespace Template.DOM.Comun;

public abstract class PersistentClassLogicalDelete : PersistentClass
{
    public PersistentClassLogicalDelete()
    {
    }

    protected internal PersistentClassLogicalDelete(Guid creationUser, string? testCase = null)
        : base(creationUser, testCase)
    {
        this.IsActive = true;
    }

    [Required]
    public bool IsActive { get; protected internal set; }

    public virtual void Deactivate(Guid modificationUser)
    {
        this.IsActive = false;
        this.Update(modificationUser);
    }

    public virtual void Activate(Guid modificationUser)
    {
        this.IsActive = true;
        this.Update(modificationUser);
    }
}