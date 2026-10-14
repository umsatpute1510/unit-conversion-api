using Microsoft.AspNetCore.Mvc;
using UnitConversion.Model.Dtos.Requests;
using UnitConversion.Model.Dtos.Responses;
using UnitConversion.Service.Conversion.Contract;
using UnitConversion.Utilities.Common.Models;

namespace UnitConversion.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class ConversionController(IConversionService conversionService) : ControllerBase
{
    [HttpPost("convert")]
    [ProducesResponseType(typeof(ApiResponse<ConversionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ConversionResponse>), StatusCodes.Status400BadRequest)]
    public ActionResult<ApiResponse<ConversionResponse>> Convert([FromBody] ConversionRequest request)
    {
        ApiResponse<ConversionResponse> response = conversionService.Convert(request);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("units")]
    [ProducesResponseType(typeof(ApiResponse<SupportedUnitsResponse>), StatusCodes.Status200OK)]
    public ActionResult<ApiResponse<SupportedUnitsResponse>> GetSupportedUnits()
    {
        ApiResponse<SupportedUnitsResponse> response = conversionService.GetSupportedUnits();
        return StatusCode(response.StatusCode, response);
    }
}
