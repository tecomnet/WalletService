using System.Globalization;

namespace Wallet.DOM.Comun;

public static class CurrencyHelper
{
    /// <summary>
    /// Valida si un código ISO de moneda es válido.
    /// </summary>
    /// <param name="currencyIsoCode">El código ISO de la moneda a validar.</param>
    /// <returns><c>true</c> si el código ISO es válido; de lo contrario, <c>false</c>.</returns>
    public static bool ValidateIsoCode(string currencyIsoCode)
    {
        return ((IEnumerable<CultureInfo>)CultureInfo.GetCultures(types: CultureTypes.SpecificCultures))
            .Where<CultureInfo>(predicate: (Func<CultureInfo, bool>)(culture => culture.LCID != (int)sbyte.MaxValue))
            .Select<CultureInfo, string>(
                selector: (Func<CultureInfo, string>)(x => new RegionInfo(name: x.Name).ISOCurrencySymbol))
            .Distinct<string>()
            .OrderBy<string, string>(keySelector: (Func<string, string>)(x => x))
            .Any<string>(predicate: (Func<string, bool>)(x => x == currencyIsoCode.ToUpper()));
    }
}