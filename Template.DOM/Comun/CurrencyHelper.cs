using System.Globalization;

namespace Template.DOM.Comun;

public static class CurrencyHelper
{
    public static bool ValidateIsoCode(string currencyIsoCode)
    {
        return ((IEnumerable<CultureInfo>) CultureInfo.GetCultures(CultureTypes.SpecificCultures)).Where<CultureInfo>((Func<CultureInfo, bool>) (culture => culture.LCID != (int) sbyte.MaxValue)).Select<CultureInfo, string>((Func<CultureInfo, string>) (x => new RegionInfo(x.Name).ISOCurrencySymbol)).Distinct<string>().OrderBy<string, string>((Func<string, string>) (x => x)).Any<string>((Func<string, bool>) (x => x == currencyIsoCode.ToUpper()));
    }
}