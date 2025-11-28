namespace Wallet.DOM.Comun;

public static class CalculationHelper
{
    public static bool TestPrecision(Decimal checkValue, int decimalPlaces = 2)
    {
        Decimal num = (Decimal) Math.Pow(x: 10.0, y: (double) decimalPlaces);
        return Math.Truncate(d: checkValue * num) == checkValue * num;
    }

    public static Decimal Truncate(Decimal value, int decimalPlaces = 4)
    {
        Decimal num = (Decimal) Math.Pow(x: 10.0, y: (double) decimalPlaces);
        return Math.Truncate(d: value * num) / num;
    }

    public static Decimal GetTrancheRate(
        Decimal previousInitialPercentage,
        int previousYearEnd,
        Decimal currentInitialPercentage,
        int currentYearEnd,
        int decimalPlaces)
    {
        return CalculationHelper.Truncate(value: (currentInitialPercentage - previousInitialPercentage) / (((Decimal) currentYearEnd + (Decimal) previousYearEnd + 1M) / 2M * ((Decimal) currentYearEnd - (Decimal) previousYearEnd)), decimalPlaces: decimalPlaces);
    }
}