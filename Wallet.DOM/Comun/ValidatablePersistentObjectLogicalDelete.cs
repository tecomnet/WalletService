using Wallet.DOM.Errors;

namespace Wallet.DOM.Comun;

public class ValidatablePersistentObjectLogicalDelete : PersistentClassLogicalDelete
{
    protected virtual List<PropertyConstraint> PropertyConstraints
    {
        get => new List<PropertyConstraint>();
    }

    protected ValidatablePersistentObjectLogicalDelete()
    {
    }

    public ValidatablePersistentObjectLogicalDelete(Guid creationUser, string? testCase = null)
        : base(creationUser, testCase)
    {
    }

    public bool IsPropertyValid(
        string propertyName,
        object? value,
        ref List<EMGeneralException> exceptions)
    {
        List<EMGeneralException> newExceptions = new List<EMGeneralException>();
        if (this.PropertyConstraints.All<PropertyConstraint>((Func<PropertyConstraint, bool>) (pc => pc.PropertyName != propertyName)))
        {
            IServiceError serviceErrorForCode = new ServiceErrors().GetServiceErrorForCode(ServiceErrorsBuilder.PropertyValidationPropertyNotFound);
            List<object> descriptionDynamicContents = new List<object>()
            {
                (object) propertyName
            };
            exceptions.Add(new EMGeneralException(serviceErrorForCode.Message, serviceErrorForCode.ErrorCode, serviceErrorForCode.Title, serviceErrorForCode.Description(descriptionDynamicContents.ToArray()), "PersistentObject", (string) null, (string) null, "DOM", descriptionDynamicContents));
            return false;
        }
        int num = this.PropertyConstraints.Single<PropertyConstraint>((Func<PropertyConstraint, bool>) (pc => pc.PropertyName == propertyName)).IsPropertyValid(value, out newExceptions) ? 1 : 0;
        exceptions.AddRange((IEnumerable<EMGeneralException>) newExceptions);
        return num != 0;
    }
}