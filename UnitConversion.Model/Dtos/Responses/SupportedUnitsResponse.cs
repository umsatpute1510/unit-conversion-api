namespace UnitConversion.Model.Dtos.Responses;

public record SupportedUnitsResponse(
    IReadOnlyDictionary<string, IReadOnlyList<string>> Units
);
