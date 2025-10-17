using System.Globalization;

namespace Template.Funcionalidad.Helper;

public static class DecimalTrim
{
    public static decimal Trim(decimal value, int decimalPlaces)
    {
        var output = value;
        var places = decimalPlaces + 1;
        var input = value.ToString(CultureInfo.InvariantCulture);
        // Find the index of the decimal point
        var decimalIndex = input.IndexOf('.');
        // If the decimal point exists and there are more than two characters after it
        if (decimalIndex != -1 && input.Length > decimalIndex + places)
        {
            // Trim the string to two characters after the decimal point
            var trimmedString = input.Substring(0, decimalIndex + places);
            output = decimal.Parse(trimmedString);
        }
        
        return output;
    }
}