# Unit Conversion API

An ASP.NET Core Web API that converts numerical values between units of measurement.

## Supported categories

| Category    | Units |
|-------------|-------|
| Length      | meter, kilometer, centimeter, millimeter, mile, yard, foot, inch, nautical mile |
| Weight/Mass | kilogram, gram, milligram, tonne, pound, ounce, stone |
| Temperature | celsius, fahrenheit, kelvin |

Run `GET /api/conversion/units` for the full list at runtime.

## Run locally

**Prerequisites:** [.NET 10 SDK](https://dotnet.microsoft.com/download)

```bash
git clone https://github.com/umsatpute1510/unit-conversion-api
cd unit-conversion-api
dotnet run --project UnitConversion.Api
```

API is available at `http://localhost:5076`.  
OpenAPI spec: `GET /openapi/v1.json`  
Health check: `GET /health`

## Run tests

```bash
dotnet test
```

## API

### `POST /api/conversion/convert`

```json
{
  "category": "length",
  "fromUnit": "meter",
  "toUnit": "kilometer",
  "value": 1000
}
```

Response:

```json
{
  "category": "length",
  "fromUnit": "meter",
  "toUnit": "kilometer",
  "inputValue": 1000,
  "convertedValue": 1
}
```

Returns `400 Bad Request` with a problem detail when the category or unit is unknown.

### `GET /api/conversion/units`

Returns all supported categories and their unit names.

## Project structure

```
UnitConversion.Api/
├── Controllers/            HTTP layer — routes requests, returns responses
└── Middleware/             Global exception handler

UnitConversion.Model/
└── Dtos/
    ├── Requests/           ConversionRequest record
    └── Responses/          ConversionResponse, SupportedUnitsResponse records

UnitConversion.Service/
└── Conversion/
    ├── Contract/           IConversionService interface
    ├── Validators/         ConversionRequestValidator (FluentValidation)
    ├── ConversionService   Business logic — validation, dispatch, conversion
    └── UnitDefinitions     Hardcoded unit factors — isolated from logic (SRP)

UnitConversion.Utilities/
├── Common/Models/          ApiResponse<T> result wrapper
└── Exceptions/Conversion/  InvalidConversionException

UnitConversion.Tests/
├── ConversionServiceTests.cs              Unit tests (logic only)
└── ConversionControllerIntegrationTests.cs  Integration tests (full HTTP stack)
```

## Design decisions

**Unit data separated from business logic (SRP)**  
`UnitDefinitions` holds only the hardcoded factors. `ConversionService` holds only the conversion algorithm. Adding a new category means touching only `UnitDefinitions`.

**Linear factor table + temperature formula**  
All linear units store a factor relative to a base unit. Conversion: `value × fromFactor / toFactor`. Temperature uses explicit formulas since it is non-linear.

**Interface-driven service**  
`IConversionService` keeps the controller decoupled from the implementation and makes unit testing straightforward without any mocking framework.

**Global exception middleware**  
Unhandled exceptions are caught once at the middleware level and returned as RFC 7807 `ProblemDetails`. The controller only handles domain-level `ArgumentException`.
