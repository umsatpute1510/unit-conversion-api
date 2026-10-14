namespace UnitConversion.Model.Dtos.Responses;

public record ConversionResponse(
    string Category,
    string FromUnit,
    string ToUnit,
    double InputValue,
    double ConvertedValue
);
