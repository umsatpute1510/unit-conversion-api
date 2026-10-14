namespace UnitConversion.Service.Conversion;

// Factors are relative to a base unit: Length → meter, Weight → kilogram. Temperature is formula-based.
internal static class UnitDefinitions
{
    internal static readonly IReadOnlyDictionary<string, double> Length =
        new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
        {
            ["meter"]         = 1.0,
            ["kilometer"]     = 1000.0,
            ["centimeter"]    = 0.01,
            ["millimeter"]    = 0.001,
            ["mile"]          = 1609.344,
            ["yard"]          = 0.9144,
            ["foot"]          = 0.3048,
            ["inch"]          = 0.0254,
            ["nautical mile"] = 1852.0,
        };

    internal static readonly IReadOnlyDictionary<string, double> Weight =
        new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
        {
            ["kilogram"]  = 1.0,
            ["gram"]      = 0.001,
            ["milligram"] = 0.000001,
            ["tonne"]     = 1000.0,
            ["pound"]     = 0.45359237,
            ["ounce"]     = 0.028349523125,
            ["stone"]     = 6.35029318,
        };

    internal static readonly IReadOnlyList<string> Temperature =
        ["celsius", "fahrenheit", "kelvin"];

    internal static readonly IReadOnlyDictionary<string, IReadOnlyDictionary<string, double>> LinearCategories =
        new Dictionary<string, IReadOnlyDictionary<string, double>>(StringComparer.OrdinalIgnoreCase)
        {
            ["length"] = Length,
            ["weight"] = Weight,
        };
}
