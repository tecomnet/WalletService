using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Wallet.RestAPI.Helpers;

/// <summary>
/// Converter to deserialize string to enum, supporting EnumMemberAttribute.
/// </summary>
/// <typeparam name="T">The enum type.</typeparam>
public class CustomStringToEnumConverter<T> : StringEnumConverter where T : struct, Enum
{
    /// <summary>
    /// Reads the JSON representation of the object.
    /// </summary>
    /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
    /// <param name="objectType">Type of the object.</param>
    /// <param name="existingValue">The existing value of object being read.</param>
    /// <param name="serializer">The calling serializer.</param>
    /// <returns>The object value.</returns>
    public override object ReadJson(
        JsonReader reader,
        Type objectType,
        object existingValue,
        JsonSerializer serializer)
    {
        if (reader.Value == null)
        {
            return null;
        }

        string value = reader.Value.ToString();
        if (string.IsNullOrWhiteSpace(value: value)) return null;
        try
        {
            return EnumExtensions.GetValueFromEnumMember<T>(value: value);
        }
        catch (ArgumentException)
        {
            string validValues = string.Join(separator: ", ", value: EnumExtensions.GetEnumMemberValues<T>());
            throw new JsonSerializationException(
                message: $"Valor '{value}' no es válido. Valores permitidos: {validValues}.");
        }
    }
}

/// <summary>
/// Extension methods for Enum types.
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Obtiene el valor del atributo [EnumMember] de un enum.
    /// </summary>
    public static string GetEnumMemberValue(Enum enumValue)
    {
        var member = enumValue.GetType().GetMember(name: enumValue.ToString()).FirstOrDefault();
        return member?.GetCustomAttribute<EnumMemberAttribute>()?.Value ?? enumValue.ToString();
    }

    /// <summary>
    /// Obtiene todos los valores de [EnumMember] de un enum, incluyendo los enteros correspondientes.
    /// </summary>
    public static string[] GetEnumMemberValues<T>() where T : struct, Enum
    {
        return typeof(T).GetFields(bindingAttr: BindingFlags.Public | BindingFlags.Static)
            .Select(selector: f =>
                $"{f.GetCustomAttribute<EnumMemberAttribute>()?.Value ?? f.Name} ({(int)f.GetValue(obj: null)})")
            .ToArray();
    }

    /// <summary>
    /// Obtiene el valor del enum a partir del valor del [EnumMember], el nombre del enum o su valor numérico.
    /// </summary>
    public static T GetValueFromEnumMember<T>(string value) where T : struct, Enum
    {
        var type = typeof(T);
        if (!type.IsEnum) throw new InvalidOperationException(message: $"{type.Name} no es un enum.");

        // Intentar parsear como número
        if (int.TryParse(s: value, result: out int numericValue) && Enum.IsDefined(enumType: type, value: numericValue))
        {
            return (T)Enum.ToObject(enumType: type, value: numericValue);
        }

        // Buscar por EnumMember o nombre del enum
        foreach (var field in type.GetFields(bindingAttr: BindingFlags.Public | BindingFlags.Static))
        {
            var attribute = field.GetCustomAttribute<EnumMemberAttribute>();
            if ((attribute != null &&
                 attribute.Value.Equals(value: value, comparisonType: StringComparison.OrdinalIgnoreCase)) ||
                field.Name.Equals(value: value, comparisonType: StringComparison.OrdinalIgnoreCase))
            {
                return (T)field.GetValue(obj: null);
            }
        }

        // Obtener todos los valores válidos del enum
        var validValues = Enum.GetValues(enumType: type)
            .Cast<T>()
            .Select(selector: e => $"{Convert.ToInt32(value: e)} ({e})")
            .ToList();

        throw new ArgumentException(
            message:
            $"Valor '{value}' no es válido. Valores permitidos: {string.Join(separator: ", ", values: validValues)}.");
    }
}
