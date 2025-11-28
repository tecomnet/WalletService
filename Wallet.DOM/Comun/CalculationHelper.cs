namespace Wallet.DOM.Comun;

/// <summary>
/// Clase de ayuda para realizar cálculos relacionados con valores decimales.
/// </summary>
public static class CalculationHelper
{
    /// <summary>
    /// Verifica si un valor decimal tiene la precisión especificada por el número de lugares decimales.
    /// </summary>
    /// <param name="checkValue">El valor decimal a verificar.</param>
    /// <param name="decimalPlaces">El número de lugares decimales contra los que se verificará. Por defecto es 2.</param>
    /// <returns><c>true</c> si el valor tiene la precisión especificada; de lo contrario, <c>false</c>.</returns>
    public static bool TestPrecision(Decimal checkValue, int decimalPlaces = 2)
    {
        Decimal num = (Decimal) Math.Pow(x: 10.0, y: (double) decimalPlaces);
        return Math.Truncate(d: checkValue * num) == checkValue * num;
    }

    /// <summary>
    /// Trunca un valor decimal al número especificado de lugares decimales sin redondear.
    /// </summary>
    /// <param name="value">El valor decimal a truncar.</param>
    /// <param name="decimalPlaces">El número de lugares decimales a los que se truncará el valor. Por defecto es 4.</param>
    /// <returns>El valor decimal truncado.</returns>
    public static Decimal Truncate(Decimal value, int decimalPlaces = 4)
    {
        Decimal num = (Decimal) Math.Pow(x: 10.0, y: (double) decimalPlaces);
        return Math.Truncate(d: value * num) / num;
    }

    /// <summary>
    /// Calcula la tasa de tramo (tranche rate) basándose en porcentajes iniciales y años de finalización.
    /// </summary>
    /// <param name="previousInitialPercentage">El porcentaje inicial del período anterior.</param>
    /// <param name="previousYearEnd">El año de finalización del período anterior.</param>
    /// <param name="currentInitialPercentage">El porcentaje inicial del período actual.</param>
    /// <param name="currentYearEnd">El año de finalización del período actual.</param>
    /// <param name="decimalPlaces">El número de lugares decimales para truncar el resultado.</param>
    /// <returns>La tasa de tramo calculada, truncada a los lugares decimales especificados.</returns>
    public static Decimal GetTrancheRate(
        Decimal previousInitialPercentage,
        int previousYearEnd,
        Decimal currentInitialPercentage,
        int currentYearEnd,
        int decimalPlaces)
    {
        // Calcula la diferencia de porcentajes y luego la divide por un factor que considera el promedio de los años
        // y la diferencia entre ellos. El resultado se trunca a los lugares decimales especificados.
        return CalculationHelper.Truncate(
            value: (currentInitialPercentage - previousInitialPercentage) /
                   (((Decimal)currentYearEnd + (Decimal)previousYearEnd + 1M) /
                    2M *
                    ((Decimal)currentYearEnd - (Decimal)previousYearEnd)),
            decimalPlaces: decimalPlaces);
    }
}