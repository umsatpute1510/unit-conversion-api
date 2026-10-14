using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using UnitConversion.Api;
using UnitConversion.Model.Dtos.Requests;
using UnitConversion.Model.Dtos.Responses;
using UnitConversion.Utilities.Common.Models;

namespace UnitConversion.Tests;

public class ConversionControllerIntegrationTests(WebApplicationFactory<IApiMarker> factory)
    : IClassFixture<WebApplicationFactory<IApiMarker>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Convert_ValidRequest_Returns200()
    {
        ApiResponse<ConversionResponse>? result = await PostConvertAsync(
            new ConversionRequest("length", "meter", "kilometer", 1000));

        Assert.NotNull(result);
        Assert.False(result.IsError);
        Assert.Equal(1.0, result.Data!.ConvertedValue, precision: 5);
    }

    [Fact]
    public async Task Convert_InvalidCategory_Returns400()
    {
        HttpResponseMessage response = await _client.PostAsJsonAsync(
            "/api/v1/conversion/convert",
            new ConversionRequest("volume", "liter", "gallon", 1));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        ApiResponse<ConversionResponse>? result = await response.Content
            .ReadFromJsonAsync<ApiResponse<ConversionResponse>>();

        Assert.NotNull(result);
        Assert.True(result.IsError);
    }

    [Fact]
    public async Task GetSupportedUnits_ReturnsAllCategories()
    {
        HttpResponseMessage response = await _client.GetAsync("/api/v1/conversion/units");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        ApiResponse<SupportedUnitsResponse>? result = await response.Content
            .ReadFromJsonAsync<ApiResponse<SupportedUnitsResponse>>();

        Assert.NotNull(result?.Data?.Units);
        Assert.True(result.Data.Units.ContainsKey("length"));
        Assert.True(result.Data.Units.ContainsKey("weight"));
        Assert.True(result.Data.Units.ContainsKey("temperature"));
    }

    [Fact]
    public async Task Convert_BoilingPoint_Returns212Fahrenheit()
    {
        ApiResponse<ConversionResponse>? result = await PostConvertAsync(
            new ConversionRequest("temperature", "celsius", "fahrenheit", 100));

        Assert.NotNull(result);
        Assert.Equal(212.0, result.Data!.ConvertedValue, precision: 5);
    }

    [Fact]
    public async Task HealthCheck_Returns200()
    {
        HttpResponseMessage response = await _client.GetAsync("/health");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private async Task<ApiResponse<ConversionResponse>?> PostConvertAsync(ConversionRequest request)
    {
        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/v1/conversion/convert", request);
        return await response.Content.ReadFromJsonAsync<ApiResponse<ConversionResponse>>();
    }
}
