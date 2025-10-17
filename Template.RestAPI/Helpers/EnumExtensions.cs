using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Template.RestAPI.Helpers;
/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public class CustomStringToEnumConverter<T> : StringEnumConverter where T : struct, Enum
{
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
        if (string.IsNullOrWhiteSpace(value)) return null;
        try
        {
            return EnumExtensions.GetValueFromEnumMember<T>(value);
        }
        catch (ArgumentException)
        {
            string validValues = string.Join(", ", EnumExtensions.GetEnumMemberValues<T>());
            throw new JsonSerializationException($"Valor '{value}' no es válido. Valores permitidos: {validValues}.");
        }
    }
}

/// <summary>
/// 
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Obtiene el valor del atributo [EnumMember] de un enum.
    /// </summary>
    public static string GetEnumMemberValue(Enum enumValue)
    {
        var member = enumValue.GetType().GetMember(enumValue.ToString()).FirstOrDefault();
        return member?.GetCustomAttribute<EnumMemberAttribute>()?.Value ?? enumValue.ToString();
    }

    /// <summary>
    /// Obtiene todos los valores de [EnumMember] de un enum, incluyendo los enteros correspondientes.
    /// </summary>
    public static string[] GetEnumMemberValues<T>() where T : struct, Enum
    {
        return typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static)
                        .Select(f => $"{f.GetCustomAttribute<EnumMemberAttribute>()?.Value ?? f.Name} ({(int)f.GetValue(null)})")
                        .ToArray();
    }

    /// <summary>
    /// Obtiene el valor del enum a partir del valor del [EnumMember], el nombre del enum o su valor numérico.
    /// </summary>
    public static T GetValueFromEnumMember<T>(string value) where T : struct, Enum
    {
        var type = typeof(T);
        if (!type.IsEnum) throw new InvalidOperationException($"{type.Name} no es un enum.");

        // Intentar parsear como número
        if (int.TryParse(value, out int numericValue) && Enum.IsDefined(type, numericValue))
        {
            return (T)Enum.ToObject(type, numericValue);
        }

        // Buscar por EnumMember o nombre del enum
        foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            var attribute = field.GetCustomAttribute<EnumMemberAttribute>();
            if ((attribute != null && attribute.Value.Equals(value, StringComparison.OrdinalIgnoreCase)) ||
                field.Name.Equals(value, StringComparison.OrdinalIgnoreCase))
            {
                return (T)field.GetValue(null);
            }
        }

        // Obtener todos los valores válidos del enum
        var validValues = Enum.GetValues(type)
            .Cast<T>()
            .Select(e => $"{Convert.ToInt32(e)} ({e})")
            .ToList();

        throw new ArgumentException($"Valor '{value}' no es válido. Valores permitidos: {string.Join(", ", validValues)}.");
    }
}
