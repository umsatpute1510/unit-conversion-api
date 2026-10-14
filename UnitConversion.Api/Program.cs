using FluentValidation;
using UnitConversion.Api.Middleware;
using UnitConversion.Model.Dtos.Requests;
using UnitConversion.Service.Conversion;
using UnitConversion.Service.Conversion.Contract;
using UnitConversion.Service.Conversion.Validators;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHealthChecks();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IConversionService, ConversionService>();
builder.Services.AddScoped<IValidator<ConversionRequest>, ConversionRequestValidator>();

WebApplication app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
