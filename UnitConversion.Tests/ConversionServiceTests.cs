using Microsoft.Extensions.Logging.Abstractions;
using UnitConversion.Model.Dtos.Requests;
using UnitConversion.Model.Dtos.Responses;
using UnitConversion.Service.Conversion;
using UnitConversion.Service.Conversion.Validators;
using UnitConversion.Utilities.Common.Models;

namespace UnitConversion.Tests;

public class ConversionServiceTests
{
    private readonly ConversionService _sut = new(
        new ConversionRequestValidator(),
        NullLogger<ConversionService>.Instance);

    [Theory]
    [InlineData("length", "meter", "kilometer", 1000.0, 1.0)]
    [InlineData("length", "mile", "kilometer", 1.0, 1.609344)]
    [InlineData("length", "foot", "meter", 1.0, 0.3048)]
    [InlineData("length", "inch", "centimeter", 1.0, 2.54)]
    public void Convert_Length_ReturnsCorrectResult(string category, string from, string to, double input, double expected)
    {
        ApiResponse<ConversionResponse> result = _sut.Convert(new ConversionRequest(category, from, to, input));

        Assert.False(result.IsError);
        Assert.Equal(expected, result.Data!.ConvertedValue, precision: 5);
    }

    [Theory]
    [InlineData("weight", "kilogram", "gram", 1.0, 1000.0)]
    [InlineData("weight", "pound", "kilogram", 1.0, 0.45359237)]
    [InlineData("weight", "ounce", "gram", 1.0, 28.349523125)]
    public void Convert_Weight_ReturnsCorrectResult(string category, string from, string to, double input, double expected)
    {
        ApiResponse<ConversionResponse> result = _sut.Convert(new ConversionRequest(category, from, to, input));

        Assert.False(result.IsError);
        Assert.Equal(expected, result.Data!.ConvertedValue, precision: 5);
    }

    [Theory]
    [InlineData("temperature", "celsius", "fahrenheit", 0.0, 32.0)]
    [InlineData("temperature", "celsius", "fahrenheit", 100.0, 212.0)]
    [InlineData("temperature", "fahrenheit", "celsius", 32.0, 0.0)]
    [InlineData("temperature", "celsius", "kelvin", 0.0, 273.15)]
    [InlineData("temperature", "kelvin", "celsius", 273.15, 0.0)]
    public void Convert_Temperature_ReturnsCorrectResult(string category, string from, string to, double input, double expected)
    {
        ApiResponse<ConversionResponse> result = _sut.Convert(new ConversionRequest(category, from, to, input));

        Assert.False(result.IsError);
        Assert.Equal(expected, result.Data!.ConvertedValue, precision: 5);
    }

    [Fact]
    public void Convert_UnknownCategory_Returns400()
    {
        ApiResponse<ConversionResponse> result = _sut.Convert(new ConversionRequest("volume", "liter", "gallon", 1.0));

        Assert.True(result.IsError);
        Assert.Equal(400, result.StatusCode);
    }

    [Fact]
    public void Convert_UnknownUnit_Returns400()
    {
        ApiResponse<ConversionResponse> result = _sut.Convert(new ConversionRequest("length", "parsec", "meter", 1.0));

        Assert.True(result.IsError);
        Assert.Equal(400, result.StatusCode);
    }

    [Fact]
    public void Convert_EmptyCategory_Returns400WithValidationError()
    {
        ApiResponse<ConversionResponse> result = _sut.Convert(new ConversionRequest("", "meter", "kilometer", 1.0));

        Assert.True(result.IsError);
        Assert.Equal(400, result.StatusCode);
        Assert.Contains("Category", result.Error);
    }

    [Fact]
    public void Convert_SameUnit_ReturnsSameValue()
    {
        ApiResponse<ConversionResponse> result = _sut.Convert(new ConversionRequest("length", "meter", "meter", 42.5));

        Assert.False(result.IsError);
        Assert.Equal(42.5, result.Data!.ConvertedValue);
    }

    [Fact]
    public void GetSupportedUnits_ContainsAllCategories()
    {
        ApiResponse<SupportedUnitsResponse> result = _sut.GetSupportedUnits();

        Assert.False(result.IsError);
        Assert.True(result.Data!.Units.ContainsKey("length"));
        Assert.True(result.Data.Units.ContainsKey("weight"));
        Assert.True(result.Data.Units.ContainsKey("temperature"));
    }
}
