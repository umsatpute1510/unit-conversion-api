using UnitConversion.Model.Dtos.Requests;
using UnitConversion.Model.Dtos.Responses;
using UnitConversion.Utilities.Common.Models;

namespace UnitConversion.Service.Conversion.Contract;

public interface IConversionService
{
    ApiResponse<ConversionResponse> Convert(ConversionRequest request);
    ApiResponse<SupportedUnitsResponse> GetSupportedUnits();
}
