using System.Globalization;

namespace Wallet.Funcionalidad.Helper;

public static class RegionInfoHelper
{
    /// <summary>
    /// Devuelve verdadero si el código de país proporcionado es válido.
    /// </summary>
    /// <param name="countryCode">Código de país de dos letras (ISO 3166-1 alpha-2) a validar.</param>
    /// <returns>Verdadero si el código de país es válido, falso en caso contrario.</returns>
    public static bool IsCountryCodeValid(string countryCode)
    {
        // Obtener todos los nombres de región ISO de dos letras únicos.
        // Se itera a través de todas las culturas específicas, se excluye la cultura invariante (LCID 127),
        // se crea un RegionInfo para cada una y se extrae el código ISO de dos letras.
        // Finalmente, se eliminan duplicados y se ordenan los resultados.
        IEnumerable<string> source = (from culture in CultureInfo.GetCultures(types: CultureTypes.SpecificCultures)
                                      where culture.LCID != 127
                                      select new RegionInfo(name: culture.Name).TwoLetterISORegionName)
                                     .Distinct()
                                     .OrderBy(x => x);

        // Comprobar si existe algún nombre de región ISO en la lista que coincida
        // con el código de país proporcionado (convertido a mayúsculas para una comparación sin distinción de mayúsculas y minúsculas).
        return source.Any(predicate: (string x) => x == countryCode.ToUpper());
    }
}