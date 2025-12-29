using System.Globalization;

namespace Wallet.Funcionalidad.Helper;

/// <summary>
/// Proporciona métodos de utilidad para recortar valores decimales.
/// </summary>
public static class DecimalTrim
{
    /// <summary>
    /// Recorta un valor decimal a un número específico de posiciones decimales.
    /// </summary>
    /// <param name="value">El valor decimal a recortar.</param>
    /// <param name="decimalPlaces">El número de posiciones decimales a mantener.</param>
    /// <returns>El valor decimal recortado.</returns>
    public static decimal Trim(decimal value, int decimalPlaces)
    {
        var output = value; // Inicializa la salida con el valor original.
        // Calcula el número de caracteres a considerar después del punto decimal para el recorte.
        // Se suma 1 para incluir el punto decimal en la longitud total.
        var places = decimalPlaces + 1;
        // Convierte el valor decimal a una cadena usando la cultura invariante para asegurar el punto decimal.
        var input = value.ToString(provider: CultureInfo.InvariantCulture);
        // Encuentra el índice del punto decimal en la cadena.
        var decimalIndex = input.IndexOf(value: '.');
        // Si existe un punto decimal y hay más caracteres de los deseados después de él,
        // procede a recortar.
        if (decimalIndex != -1 && input.Length > decimalIndex + places)
        {
            // Recorta la cadena para incluir solo el número deseado de caracteres después del punto decimal.
            var trimmedString = input.Substring(startIndex: 0, length: decimalIndex + places);
            // Convierte la cadena recortada de nuevo a un decimal.
            output = decimal.Parse(s: trimmedString);
        }
        
        return output; // Retorna el valor decimal recortado.
    }
}