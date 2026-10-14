using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using UnitConversion.Model.Dtos.Requests;
using UnitConversion.Model.Dtos.Responses;
using UnitConversion.Service.Conversion.Contract;
using UnitConversion.Utilities.Common.Models;
using UnitConversion.Utilities.Exceptions.Conversion;

namespace UnitConversion.Service.Conversion;

public partial class ConversionService(
    IValidator<ConversionRequest> validator,
    ILogger<ConversionService> logger) : IConversionService
{
    public ApiResponse<ConversionResponse> Convert(ConversionRequest request)
    {
        ValidationResult validation = validator.Validate(request);

        if (!validation.IsValid)
        {
            string errors = string.Join(", ", validation.Errors.Select(e => e.ErrorMessage));
            return ApiResponse.Fail<ConversionResponse>(400, errors);
        }

        string category = request.Category.Trim().ToLowerInvariant();
        string fromUnit = request.FromUnit.Trim().ToLowerInvariant();
        string toUnit   = request.ToUnit.Trim().ToLowerInvariant();

        LogConversionRequest(request.Value, fromUnit, toUnit, category);

        try
        {
            double result = category switch
            {
                UnitDefinitions.TemperatureCategory => ConvertTemperature(fromUnit, toUnit, request.Value),
                _                                   => ConvertLinear(category, fromUnit, toUnit, request.Value),
            };

            ConversionResponse response = new(request.Category, request.FromUnit, request.ToUnit, request.Value, result);
            return ApiResponse.Success(200, response);
        }
        catch (InvalidConversionException ex)
        {
            return ApiResponse.Fail<ConversionResponse>(400, ex.Message);
        }
    }

    public ApiResponse<SupportedUnitsResponse> GetSupportedUnits()
    {
        Dictionary<string, IReadOnlyList<string>> units = new(StringComparer.OrdinalIgnoreCase);

        foreach ((string category, IReadOnlyDictionary<string, double> categoryUnits) in UnitDefinitions.LinearCategories)
            units[category] = [.. categoryUnits.Keys];

        units[UnitDefinitions.TemperatureCategory] = UnitDefinitions.Temperature;

        return ApiResponse.Success(200, new SupportedUnitsResponse(units));
    }

    private static double ConvertLinear(string category, string fromUnit, string toUnit, double value)
    {
        if (!UnitDefinitions.LinearCategories.TryGetValue(category, out IReadOnlyDictionary<string, double>? units))
            throw new InvalidConversionException($"Unknown category '{category}'.");

        if (!units.TryGetValue(fromUnit, out double fromFactor))
            throw new InvalidConversionException($"Unknown unit '{fromUnit}' in category '{category}'.");

        if (!units.TryGetValue(toUnit, out double toFactor))
            throw new InvalidConversionException($"Unknown unit '{toUnit}' in category '{category}'.");

        return value * (fromFactor / toFactor);
    }

    private static double ConvertTemperature(string fromUnit, string toUnit, double value)
    {
        double celsius = fromUnit switch
        {
            "celsius"    => value,
            "fahrenheit" => (value - 32.0) * 5.0 / 9.0,
            "kelvin"     => value - 273.15,
            _ => throw new InvalidConversionException($"Unknown temperature unit '{fromUnit}'."),
        };

        return toUnit switch
        {
            "celsius"    => celsius,
            "fahrenheit" => (celsius * 9.0 / 5.0) + 32.0,
            "kelvin"     => celsius + 273.15,
            _ => throw new InvalidConversionException($"Unknown temperature unit '{toUnit}'."),
        };
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Converting {Value} {FromUnit} -> {ToUnit} (category: {Category})")]
    private partial void LogConversionRequest(double value, string fromUnit, string toUnit, string category);
}
