using Template.DOM.Errors;

namespace Template.DOM.Comun;

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
    return new PropertyConstraint(propertyName, "string", isRequired, minimumLength, maximumLength, regex, false, false, false, 0);
  }

  public static PropertyConstraint GuidPropertyConstraint(string propertyName, bool isRequired)
  {
    return new PropertyConstraint(propertyName, "Guid", isRequired, 0, 0, (string) null, false, false, false, 0);
  }

  public static PropertyConstraint ObjectPropertyConstraint(string propertyName, bool isRequired)
  {
    return new PropertyConstraint(propertyName, "object", isRequired, 0, 0, (string) null, false, false, false, 0);
  }

  public static PropertyConstraint CurrencyPropertyConstraint(string propertyName, bool isRequired)
  {
    return new PropertyConstraint(propertyName, "currency", isRequired, 3, 3, "^[A-Z]{3}$", false, false, false, 0);
  }

  public static PropertyConstraint IntPropertyConstraint(
    string propertyName,
    bool isRequired,
    bool allowNegative,
    bool allowZero,
    bool allowPositive)
  {
    return new PropertyConstraint(propertyName, "int", isRequired, 0, 0, (string) null, allowNegative, allowZero, allowPositive, 0);
  }

  public static PropertyConstraint DateTimePropertyConstraint(string propertyName, bool isRequired)
  {
    return new PropertyConstraint(propertyName, "DateTime", isRequired, 0, 0, (string) null, false, false, false, 0);
  }

  public static PropertyConstraint DecimalPropertyConstraint(
    string propertyName,
    bool isRequired,
    bool allowNegative,
    bool allowZero,
    bool allowPositive,
    int allowedDecimals)
  {
    return new PropertyConstraint(propertyName, "decimal", isRequired, 0, 0, (string) null, allowNegative, allowZero, allowPositive, allowedDecimals);
  }

  [Obsolete("Use DateTimePropertyConstraint instead")]
  public static PropertyConstraint DateOnlyPropertyContraint(string propertyName, bool isRequired)
  {
    return new PropertyConstraint(propertyName, "DateOnly", isRequired, 0, 0, (string) null, false, false, false, 0);
  }

  public static PropertyConstraint DateOnlyPropertyConstraint(string propertyName, bool isRequired)
  {
    return new PropertyConstraint(propertyName, "DateOnly", isRequired, 0, 0, (string) null, false, false, false, 0);
  }

  public static PropertyConstraint TimeOnlyPropertyConstraint(string propertyName, bool isRequired)
  {
    return new PropertyConstraint(propertyName, "TimeOnly", isRequired, 0, 0, (string) null, false, false, false, 0);
  }

  private bool IsLengthValid(string? value)
  {
    if (string.IsNullOrEmpty(value))
      return true;
    return value.Length >= this.MinimumLength && value.Length <= this.MaximumLength;
  }

  private bool IsRegexValid(string? value)
  {
    return value == null || this.Regex == null || System.Text.RegularExpressions.Regex.IsMatch(value, this.Regex);
  }

  private bool IsRequiredValid(string? value) => !this.IsRequired || !string.IsNullOrEmpty(value);

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
    return !value.HasValue || CalculationHelper.TestPrecision(value.Value, this.AllowedDecimals);
  }

  protected static bool IsCurrencyValid(string? value)
  {
    return string.IsNullOrEmpty(value) || CurrencyHelper.ValidateIsoCode(value.ToUpper());
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
      flag1 = this.IsRequiredValid((string) value);
      flag2 = this.IsLengthValid((string) value);
      flag3 = this.IsRegexValid((string) value);
    }
    else if (this.PropertyType == "Guid")
      flag1 = this.IsRequiredValid((Guid?) value);
    else if (this.PropertyType == "object")
      flag1 = this.IsRequiredValid(value);
    else if (this.PropertyType == "currency")
    {
      flag1 = this.IsRequiredValid((string) value);
      flag2 = this.IsLengthValid((string) value);
      flag8 = PropertyConstraint.IsCurrencyValid((string) value);
    }
    else if (this.PropertyType == "int")
    {
      flag1 = this.IsRequiredValid((int?) value);
      flag4 = this.IsNegativeValid((int?) value);
      flag5 = this.IsZeroValid((int?) value);
      flag6 = this.IsPositiveValid((int?) value);
    }
    else if (this.PropertyType == "decimal")
    {
      flag1 = this.IsRequiredValid((Decimal?) value);
      flag4 = this.IsNegativeValid((Decimal?) value);
      flag5 = this.IsZeroValid((Decimal?) value);
      flag6 = this.IsPositiveValid((Decimal?) value);
      flag7 = this.IsDecimalsValid((Decimal?) value);
    }
    else if (this.PropertyType == "DateTime")
      flag1 = this.IsRequiredValid((DateTime?) value);
    else if (this.PropertyType == "DateOnly")
      flag1 = this.IsRequiredValid((DateOnly?) value);
    else if (this.PropertyType == "TimeOnly")
      flag1 = this.IsRequiredValid((TimeOnly?) value);
    if (!flag1)
    {
      IServiceError serviceErrorForCode = new ServiceErrors().GetServiceErrorForCode("PROPERTY-VALIDATION-REQUIRED-ERROR");
      List<object> descriptionDynamicContents = new List<object>()
      {
        (object) this.PropertyName,
        value ?? (object) ""
      };
      generalExceptionList.Add(new EMGeneralException(serviceErrorForCode.Message, serviceErrorForCode.ErrorCode, serviceErrorForCode.Title, serviceErrorForCode.Description(descriptionDynamicContents.ToArray()), "PersistentObject", (string) null, (string) null, "DOM", descriptionDynamicContents));
    }
    if (!flag8)
    {
      IServiceError serviceErrorForCode = new ServiceErrors().GetServiceErrorForCode("PROPERTY-VALIDATION-CURRENCY-INVALID");
      List<object> descriptionDynamicContents = new List<object>()
      {
        (object) this.PropertyName,
        value ?? (object) string.Empty
      };
      generalExceptionList.Add(new EMGeneralException(serviceErrorForCode.Message, serviceErrorForCode.ErrorCode, serviceErrorForCode.Title, serviceErrorForCode.Description(descriptionDynamicContents.ToArray()), "PersistentObject", (string) null, (string) null, "DOM", descriptionDynamicContents));
    }
    if (!flag2)
    {
      IServiceError serviceErrorForCode = new ServiceErrors().GetServiceErrorForCode("PROPERTY-VALIDATION-LENGTH-INVALID");
      List<object> descriptionDynamicContents = new List<object>()
      {
        (object) this.PropertyName,
        value ?? (object) string.Empty,
        (object) this.MinimumLength,
        (object) this.MaximumLength
      };
      generalExceptionList.Add(new EMGeneralException(serviceErrorForCode.Message, serviceErrorForCode.ErrorCode, serviceErrorForCode.Title, serviceErrorForCode.Description(descriptionDynamicContents.ToArray()), "PersistentObject", (string) null, (string) null, "DOM", descriptionDynamicContents));
    }
    if (!flag3)
    {
      IServiceError serviceErrorForCode = new ServiceErrors().GetServiceErrorForCode("PROPERTY-VALIDATION-REGEX-INVALID");
      List<object> descriptionDynamicContents = new List<object>()
      {
        (object) this.PropertyName,
        value ?? (object) string.Empty,
        (object) (this.Regex ?? string.Empty)
      };
      generalExceptionList.Add(new EMGeneralException(serviceErrorForCode.Message, serviceErrorForCode.ErrorCode, serviceErrorForCode.Title, serviceErrorForCode.Description(descriptionDynamicContents.ToArray()), "PersistentObject", (string) null, (string) null, "DOM", descriptionDynamicContents));
    }
    if (!flag4)
    {
      IServiceError serviceErrorForCode = new ServiceErrors().GetServiceErrorForCode("PROPERTY-VALIDATION-NEGATIVE-INVALID");
      List<object> descriptionDynamicContents = new List<object>()
      {
        (object) this.PropertyName,
        value ?? (object) string.Empty
      };
      generalExceptionList.Add(new EMGeneralException(serviceErrorForCode.Message, serviceErrorForCode.ErrorCode, serviceErrorForCode.Title, serviceErrorForCode.Description(descriptionDynamicContents.ToArray()), "PersistentObject", (string) null, (string) null, "DOM", descriptionDynamicContents));
    }
    if (!flag5)
    {
      IServiceError serviceErrorForCode = new ServiceErrors().GetServiceErrorForCode("PROPERTY-VALIDATION-ZERO-INVALID");
      List<object> descriptionDynamicContents = new List<object>()
      {
        (object) this.PropertyName,
        value ?? (object) string.Empty
      };
      generalExceptionList.Add(new EMGeneralException(serviceErrorForCode.Message, serviceErrorForCode.ErrorCode, serviceErrorForCode.Title, serviceErrorForCode.Description(descriptionDynamicContents.ToArray()), "PersistentObject", (string) null, (string) null, "DOM", descriptionDynamicContents));
    }
    if (!flag6)
    {
      IServiceError serviceErrorForCode = new ServiceErrors().GetServiceErrorForCode("PROPERTY-VALIDATION-POSITIVE-INVALID");
      List<object> descriptionDynamicContents = new List<object>()
      {
        (object) this.PropertyName,
        value ?? (object) string.Empty
      };
      generalExceptionList.Add(new EMGeneralException(serviceErrorForCode.Message, serviceErrorForCode.ErrorCode, serviceErrorForCode.Title, serviceErrorForCode.Description(descriptionDynamicContents.ToArray()), "PersistentObject", (string) null, (string) null, "DOM", descriptionDynamicContents));
    }
    if (!flag7)
    {
      IServiceError serviceErrorForCode = new ServiceErrors().GetServiceErrorForCode("PROPERTY-VALIDATION-DECIMALS-INVALID");
      List<object> descriptionDynamicContents = new List<object>()
      {
        (object) this.PropertyName,
        value ?? (object) string.Empty,
        (object) this.AllowedDecimals
      };
      generalExceptionList.Add(new EMGeneralException(serviceErrorForCode.Message, serviceErrorForCode.ErrorCode, serviceErrorForCode.Title, serviceErrorForCode.Description(descriptionDynamicContents.ToArray()), "PersistentObject", (string) null, (string) null, "DOM", descriptionDynamicContents));
    }
    exceptions = generalExceptionList;
    return flag1 & flag2 & flag3 & flag4 & flag5 & flag6 & flag7 & flag8;
  }
}