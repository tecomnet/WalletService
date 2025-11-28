using Wallet.DOM.Errors;

namespace Wallet.DOM.Comun;

public class PropertyConstraint
{
  private PropertyConstraint(
    string propertyName,
    string propertyType,
    bool isRequired,
    int minimumLength,
    int maximumLength,
    string? regex,
    bool allowNegative,
    bool allowZero,
    bool allowPositive,
    int allowedDecimals)
  {
    this.PropertyName = propertyName;
    this.PropertyType = propertyType;
    this.IsRequired = isRequired;
    this.MinimumLength = minimumLength;
    this.MaximumLength = maximumLength;
    this.Regex = regex;
    this.AllowNegative = allowNegative;
    this.AllowZero = allowZero;
    this.AllowPositive = allowPositive;
    this.AllowedDecimals = allowedDecimals;
  }

  public string PropertyName { get; private set; }

  public string PropertyType { get; private set; }

  public bool IsRequired { get; private set; }

  public int MinimumLength { get; private set; }

  public int MaximumLength { get; private set; }

  public string? Regex { get; private set; }

  public bool AllowNegative { get; private set; }

  public bool AllowZero { get; private set; }

  public bool AllowPositive { get; private set; }

  public int AllowedDecimals { get; private set; }

  public static PropertyConstraint StringPropertyConstraint(
    string propertyName,
    bool isRequired,
    int minimumLength,
    int maximumLength,
    string? regex = null)
  {
    return new PropertyConstraint(propertyName: propertyName, propertyType: "string", isRequired: isRequired, minimumLength: minimumLength, maximumLength: maximumLength, regex: regex, allowNegative: false, allowZero: false, allowPositive: false, allowedDecimals: 0);
  }

  public static PropertyConstraint GuidPropertyConstraint(string propertyName, bool isRequired)
  {
    return new PropertyConstraint(propertyName: propertyName, propertyType: "Guid", isRequired: isRequired, minimumLength: 0, maximumLength: 0, regex: null, allowNegative: false, allowZero: false, allowPositive: false, allowedDecimals: 0);
  }

  public static PropertyConstraint ObjectPropertyConstraint(string propertyName, bool isRequired)
  {
    return new PropertyConstraint(propertyName: propertyName, propertyType: "object", isRequired: isRequired, minimumLength: 0, maximumLength: 0, regex: null, allowNegative: false, allowZero: false, allowPositive: false, allowedDecimals: 0);
  }

  public static PropertyConstraint CurrencyPropertyConstraint(string propertyName, bool isRequired)
  {
    return new PropertyConstraint(propertyName: propertyName, propertyType: "currency", isRequired: isRequired, minimumLength: 3, maximumLength: 3, regex: "^[A-Z]{3}$", allowNegative: false, allowZero: false, allowPositive: false, allowedDecimals: 0);
  }

  public static PropertyConstraint IntPropertyConstraint(
    string propertyName,
    bool isRequired,
    bool allowNegative,
    bool allowZero,
    bool allowPositive)
  {
    return new PropertyConstraint(propertyName: propertyName, propertyType: "int", isRequired: isRequired, minimumLength: 0, maximumLength: 0, regex: null, allowNegative: allowNegative, allowZero: allowZero, allowPositive: allowPositive, allowedDecimals: 0);
  }

  public static PropertyConstraint DateTimePropertyConstraint(string propertyName, bool isRequired)
  {
    return new PropertyConstraint(propertyName: propertyName, propertyType: "DateTime", isRequired: isRequired, minimumLength: 0, maximumLength: 0, regex: null, allowNegative: false, allowZero: false, allowPositive: false, allowedDecimals: 0);
  }

  public static PropertyConstraint DecimalPropertyConstraint(
    string propertyName,
    bool isRequired,
    bool allowNegative,
    bool allowZero,
    bool allowPositive,
    int allowedDecimals)
  {
    return new PropertyConstraint(propertyName: propertyName, propertyType: "decimal", isRequired: isRequired, minimumLength: 0, maximumLength: 0, regex: null, allowNegative: allowNegative, allowZero: allowZero, allowPositive: allowPositive, allowedDecimals: allowedDecimals);
  }

  [Obsolete(message: "Use DateTimePropertyConstraint instead")]
  public static PropertyConstraint DateOnlyPropertyContraint(string propertyName, bool isRequired)
  {
    return new PropertyConstraint(propertyName: propertyName, propertyType: "DateOnly", isRequired: isRequired, minimumLength: 0, maximumLength: 0, regex: null, allowNegative: false, allowZero: false, allowPositive: false, allowedDecimals: 0);
  }

  public static PropertyConstraint DateOnlyPropertyConstraint(string propertyName, bool isRequired)
  {
    return new PropertyConstraint(propertyName: propertyName, propertyType: "DateOnly", isRequired: isRequired, minimumLength: 0, maximumLength: 0, regex: null, allowNegative: false, allowZero: false, allowPositive: false, allowedDecimals: 0);
  }

  public static PropertyConstraint TimeOnlyPropertyConstraint(string propertyName, bool isRequired)
  {
    return new PropertyConstraint(propertyName: propertyName, propertyType: "TimeOnly", isRequired: isRequired, minimumLength: 0, maximumLength: 0, regex: null, allowNegative: false, allowZero: false, allowPositive: false, allowedDecimals: 0);
  }

  private bool IsLengthValid(string? value)
  {
    if (string.IsNullOrEmpty(value: value))
      return true;
    return value.Length >= this.MinimumLength && value.Length <= this.MaximumLength;
  }

  private bool IsRegexValid(string? value)
  {
    return value == null || this.Regex == null || System.Text.RegularExpressions.Regex.IsMatch(input: value, pattern: this.Regex);
  }

  private bool IsRequiredValid(string? value) => !this.IsRequired || !string.IsNullOrEmpty(value: value);

  private bool IsRequiredValid(Guid? value) => !this.IsRequired || value.HasValue;

  private bool IsRequiredValid(object? value) => !this.IsRequired || value != null;

  private bool IsRequiredValid(int? value) => !this.IsRequired || value.HasValue;

  private bool IsRequiredValid(DateTime? value)
  {
    if (!this.IsRequired)
      return true;
    return value.HasValue && value.HasValue;
  }

  private bool IsRequiredValid(DateOnly? value)
  {
    if (!this.IsRequired)
      return true;
    return value.HasValue && value.HasValue;
  }

  private bool IsRequiredValid(TimeOnly? value)
  {
    if (!this.IsRequired)
      return true;
    return value.HasValue && value.HasValue;
  }

  private bool IsRequiredValid(Decimal? value) => !this.IsRequired || value.HasValue;

  private bool IsNegativeValid(int? value)
  {
    if (!value.HasValue || this.AllowNegative)
      return true;
    int? nullable = value;
    int num = 0;
    return nullable.GetValueOrDefault() >= num & nullable.HasValue;
  }

  private bool IsNegativeValid(Decimal? value)
  {
    if (!value.HasValue || this.AllowNegative)
      return true;
    Decimal? nullable = value;
    Decimal num = 0M;
    return nullable.GetValueOrDefault() >= num & nullable.HasValue;
  }

  private bool IsZeroValid(int? value)
  {
    if (!value.HasValue || this.AllowZero)
      return true;
    int? nullable1 = value;
    int num1 = 0;
    if (nullable1.GetValueOrDefault() < num1 & nullable1.HasValue)
      return true;
    int? nullable2 = value;
    int num2 = 0;
    return nullable2.GetValueOrDefault() > num2 & nullable2.HasValue;
  }

  private bool IsZeroValid(Decimal? value)
  {
    if (!value.HasValue || this.AllowZero)
      return true;
    Decimal? nullable1 = value;
    Decimal num1 = 0M;
    if (nullable1.GetValueOrDefault() < num1 & nullable1.HasValue)
      return true;
    Decimal? nullable2 = value;
    Decimal num2 = 0M;
    return nullable2.GetValueOrDefault() > num2 & nullable2.HasValue;
  }

  private bool IsPositiveValid(int? value)
  {
    if (!value.HasValue || this.AllowPositive)
      return true;
    int? nullable = value;
    int num = 0;
    return nullable.GetValueOrDefault() <= num & nullable.HasValue;
  }

  private bool IsPositiveValid(Decimal? value)
  {
    if (!value.HasValue || this.AllowPositive)
      return true;
    Decimal? nullable = value;
    Decimal num = 0M;
    return nullable.GetValueOrDefault() <= num & nullable.HasValue;
  }

  private bool IsDecimalsValid(Decimal? value)
  {
    return !value.HasValue || CalculationHelper.TestPrecision(checkValue: value.Value, decimalPlaces: this.AllowedDecimals);
  }

  protected static bool IsCurrencyValid(string? value)
  {
    return string.IsNullOrEmpty(value: value) || CurrencyHelper.ValidateIsoCode(currencyIsoCode: value.ToUpper());
  }

  public bool IsPropertyValid(object? value, out List<EMGeneralException> exceptions)
  {
    bool flag1 = true;
    bool flag2 = true;
    bool flag3 = true;
    bool flag4 = true;
    bool flag5 = true;
    bool flag6 = true;
    bool flag7 = true;
    bool flag8 = true;
    List<EMGeneralException> generalExceptionList = new List<EMGeneralException>();
    if (this.PropertyType == "string")
    {
      flag1 = this.IsRequiredValid(value: (string) value);
      flag2 = this.IsLengthValid(value: (string) value);
      flag3 = this.IsRegexValid(value: (string) value);
    }
    else if (this.PropertyType == "Guid")
      flag1 = this.IsRequiredValid(value: (Guid?) value);
    else if (this.PropertyType == "object")
      flag1 = this.IsRequiredValid(value: value);
    else if (this.PropertyType == "currency")
    {
      flag1 = this.IsRequiredValid(value: (string) value);
      flag2 = this.IsLengthValid(value: (string) value);
      flag8 = PropertyConstraint.IsCurrencyValid(value: (string) value);
    }
    else if (this.PropertyType == "int")
    {
      flag1 = this.IsRequiredValid(value: (int?) value);
      flag4 = this.IsNegativeValid(value: (int?) value);
      flag5 = this.IsZeroValid(value: (int?) value);
      flag6 = this.IsPositiveValid(value: (int?) value);
    }
    else if (this.PropertyType == "decimal")
    {
      flag1 = this.IsRequiredValid(value: (Decimal?) value);
      flag4 = this.IsNegativeValid(value: (Decimal?) value);
      flag5 = this.IsZeroValid(value: (Decimal?) value);
      flag6 = this.IsPositiveValid(value: (Decimal?) value);
      flag7 = this.IsDecimalsValid(value: (Decimal?) value);
    }
    else if (this.PropertyType == "DateTime")
      flag1 = this.IsRequiredValid(value: (DateTime?) value);
    else if (this.PropertyType == "DateOnly")
      flag1 = this.IsRequiredValid(value: (DateOnly?) value);
    else if (this.PropertyType == "TimeOnly")
      flag1 = this.IsRequiredValid(value: (TimeOnly?) value);
    if (!flag1)
    {
      IServiceError serviceErrorForCode = new ServiceErrors().GetServiceErrorForCode(errorCode: ServiceErrorsBuilder.PropertyValidationRequiredError);
      List<object> descriptionDynamicContents = new List<object>()
      {
        this.PropertyName,
        value ?? ""
      };
      generalExceptionList.Add(item: new EMGeneralException(message: serviceErrorForCode.Message, code: serviceErrorForCode.ErrorCode, title: serviceErrorForCode.Title, description: serviceErrorForCode.Description(args: descriptionDynamicContents.ToArray()), serviceName: "PersistentObject", serviceInstance: null, serviceLocation: null, module: "DOM", descriptionDynamicContents: descriptionDynamicContents));
    }
    if (!flag8)
    {
      IServiceError serviceErrorForCode = new ServiceErrors().GetServiceErrorForCode(errorCode: ServiceErrorsBuilder.PropertyValidationCurrencyInvalid);
      List<object> descriptionDynamicContents = new List<object>()
      {
        this.PropertyName,
        value ?? string.Empty
      };
      generalExceptionList.Add(item: new EMGeneralException(message: serviceErrorForCode.Message, code: serviceErrorForCode.ErrorCode, title: serviceErrorForCode.Title, description: serviceErrorForCode.Description(args: descriptionDynamicContents.ToArray()), serviceName: "PersistentObject", serviceInstance: null, serviceLocation: null, module: "DOM", descriptionDynamicContents: descriptionDynamicContents));
    }
    if (!flag2)
    {
      IServiceError serviceErrorForCode = new ServiceErrors().GetServiceErrorForCode(errorCode: ServiceErrorsBuilder.PropertyValidationLengthInvalid);
      List<object> descriptionDynamicContents = new List<object>()
      {
        this.PropertyName,
        value ?? string.Empty,
        this.MinimumLength,
        this.MaximumLength
      };
      generalExceptionList.Add(item: new EMGeneralException(message: serviceErrorForCode.Message, code: serviceErrorForCode.ErrorCode, title: serviceErrorForCode.Title, description: serviceErrorForCode.Description(args: descriptionDynamicContents.ToArray()), serviceName: "PersistentObject", serviceInstance: null, serviceLocation: null, module: "DOM", descriptionDynamicContents: descriptionDynamicContents));
    }
    if (!flag3)
    {
      IServiceError serviceErrorForCode = new ServiceErrors().GetServiceErrorForCode(errorCode: ServiceErrorsBuilder.PropertyValidationRegexInvalid);
      List<object> descriptionDynamicContents = new List<object>()
      {
        this.PropertyName,
        value ?? string.Empty,
        this.Regex ?? string.Empty
      };
      generalExceptionList.Add(item: new EMGeneralException(message: serviceErrorForCode.Message, code: serviceErrorForCode.ErrorCode, title: serviceErrorForCode.Title, description: serviceErrorForCode.Description(args: descriptionDynamicContents.ToArray()), serviceName: "PersistentObject", serviceInstance: null, serviceLocation: null, module: "DOM", descriptionDynamicContents: descriptionDynamicContents));
    }
    if (!flag4)
    {
      IServiceError serviceErrorForCode = new ServiceErrors().GetServiceErrorForCode(errorCode: ServiceErrorsBuilder.PropertyValidationNegativeInvalid);
      List<object> descriptionDynamicContents = new List<object>()
      {
        this.PropertyName,
        value ?? string.Empty
      };
      generalExceptionList.Add(item: new EMGeneralException(message: serviceErrorForCode.Message, code: serviceErrorForCode.ErrorCode, title: serviceErrorForCode.Title, description: serviceErrorForCode.Description(args: descriptionDynamicContents.ToArray()), serviceName: "PersistentObject", serviceInstance: null, serviceLocation: null, module: "DOM", descriptionDynamicContents: descriptionDynamicContents));
    }
    if (!flag5)
    {
      IServiceError serviceErrorForCode = new ServiceErrors().GetServiceErrorForCode(errorCode: ServiceErrorsBuilder.PropertyValidationZeroInvalid);
      List<object> descriptionDynamicContents = new List<object>()
      {
        this.PropertyName,
        value ?? string.Empty
      };
      generalExceptionList.Add(item: new EMGeneralException(message: serviceErrorForCode.Message, code: serviceErrorForCode.ErrorCode, title: serviceErrorForCode.Title, description: serviceErrorForCode.Description(args: descriptionDynamicContents.ToArray()), serviceName: "PersistentObject", serviceInstance: null, serviceLocation: null, module: "DOM", descriptionDynamicContents: descriptionDynamicContents));
    }
    if (!flag6)
    {
      IServiceError serviceErrorForCode = new ServiceErrors().GetServiceErrorForCode(errorCode: ServiceErrorsBuilder.PropertyValidationPositiveInvalid);
      List<object> descriptionDynamicContents = new List<object>()
      {
        this.PropertyName,
        value ?? string.Empty
      };
      generalExceptionList.Add(item: new EMGeneralException(message: serviceErrorForCode.Message, code: serviceErrorForCode.ErrorCode, title: serviceErrorForCode.Title, description: serviceErrorForCode.Description(args: descriptionDynamicContents.ToArray()), serviceName: "PersistentObject", serviceInstance: null, serviceLocation: null, module: "DOM", descriptionDynamicContents: descriptionDynamicContents));
    }
    if (!flag7)
    {
      IServiceError serviceErrorForCode = new ServiceErrors().GetServiceErrorForCode(errorCode: ServiceErrorsBuilder.PropertyValidationDecimalsInvalid);
      List<object> descriptionDynamicContents = new List<object>()
      {
        this.PropertyName,
        value ?? string.Empty,
        this.AllowedDecimals
      };
      generalExceptionList.Add(item: new EMGeneralException(message: serviceErrorForCode.Message, code: serviceErrorForCode.ErrorCode, title: serviceErrorForCode.Title, description: serviceErrorForCode.Description(args: descriptionDynamicContents.ToArray()), serviceName: "PersistentObject", serviceInstance: null, serviceLocation: null, module: "DOM", descriptionDynamicContents: descriptionDynamicContents));
    }
    exceptions = generalExceptionList;
    return flag1 & flag2 & flag3 & flag4 & flag5 & flag6 & flag7 & flag8;
  }
}