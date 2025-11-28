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
        : base(creationUser: creationUser, testCase: testCase)
    {
    }

    public bool IsPropertyValid(
        string propertyName,
        object? value,
        ref List<EMGeneralException> exceptions)
    {
        List<EMGeneralException> newExceptions = new List<EMGeneralException>();
        if (this.PropertyConstraints.All<PropertyConstraint>(predicate: (Func<PropertyConstraint, bool>) (pc => pc.PropertyName != propertyName)))
        {
            IServiceError serviceErrorForCode = new ServiceErrors().GetServiceErrorForCode(errorCode: ServiceErrorsBuilder.PropertyValidationPropertyNotFound);
            List<object> descriptionDynamicContents = new List<object>()
            {
                (object) propertyName
            };
            exceptions.Add(item: new EMGeneralException(message: serviceErrorForCode.Message, code: serviceErrorForCode.ErrorCode, title: serviceErrorForCode.Title, description: serviceErrorForCode.Description(args: descriptionDynamicContents.ToArray()), serviceName: "PersistentObject", serviceInstance: (string) null, serviceLocation: (string) null, module: "DOM", descriptionDynamicContents: descriptionDynamicContents));
            return false;
        }
        int num = this.PropertyConstraints.Single<PropertyConstraint>(predicate: (Func<PropertyConstraint, bool>) (pc => pc.PropertyName == propertyName)).IsPropertyValid(value: value, exceptions: out newExceptions) ? 1 : 0;
        exceptions.AddRange(collection: (IEnumerable<EMGeneralException>) newExceptions);
        return num != 0;
    }
}