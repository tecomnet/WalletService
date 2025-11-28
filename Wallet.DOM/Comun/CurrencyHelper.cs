using System.Globalization;

namespace Wallet.DOM.Comun;

public static class CurrencyHelper
{
    public static bool ValidateIsoCode(string currencyIsoCode)
    {
        return ((IEnumerable<CultureInfo>) CultureInfo.GetCultures(types: CultureTypes.SpecificCultures)).Where<CultureInfo>(predicate: (Func<CultureInfo, bool>) (culture => culture.LCID != (int) sbyte.MaxValue)).Select<CultureInfo, string>(selector: (Func<CultureInfo, string>) (x => new RegionInfo(name: x.Name).ISOCurrencySymbol)).Distinct<string>().OrderBy<string, string>(keySelector: (Func<string, string>) (x => x)).Any<string>(predicate: (Func<string, bool>) (x => x == currencyIsoCode.ToUpper()));
    }
}