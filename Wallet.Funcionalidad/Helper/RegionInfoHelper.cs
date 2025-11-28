using System.Globalization;

namespace Wallet.Funcionalidad.Helper;

public static class RegionInfoHelper
{
    /// <summary>
    /// Return true if code of country is valid
    /// </summary>
    /// <param name="countryCode">code of country to validate</param>
    /// <returns>bool</returns>
    public static bool IsCountryCodeValid(string countryCode)
    {
        // Get all iso region names
        IEnumerable<string> source = from x in (from culture in CultureInfo.GetCultures(types: CultureTypes.SpecificCultures)
                                                where culture.LCID != 127
                                                select culture
                into x
                                                select new RegionInfo(name: x.Name).TwoLetterISORegionName).Distinct()
                                     orderby x
                                     select x;
        // Exists any iso region name with the country code
        return source.Any(predicate: (string x) => x == countryCode.ToUpper());
    }
}