namespace UnitConversion.Model.Dtos.Requests;

public record ConversionRequest(
    string Category,
    string FromUnit,
    string ToUnit,
    double Value
);
