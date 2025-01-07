# JWTAuthLib README

## Overview
JWTAuthLib is a library designed for implementing JWT-based authentication using ASP.NET Core Identity. This guide provides steps to integrate the library as a NuGet package and use it to secure your APIs.

## Prerequisites
Ensure you have the following:
- .NET 8.0.
- A project with ASP.NET Core Identity.

## Installation
1. Install the JWTAuthLib NuGet package:

   ```bash
   dotnet add package JwtAuthLib
   ```

2. Ensure your `appsettings.json` file includes the necessary JWT settings:

   ```json
   {
     "JwtSettings": {
       "Issuer": "YourIssuer",
       "Audience": "YourAudience",
       "SigningKey": "YourSecretSigningKey",
       "ExpirationMinutes": 60
     }
   }
   ```

## Configuration

### Step 1: Update `Program.cs`

Add the following lines to configure JWT authentication:

```csharp
using JwtAuthLib;
using JwtAuthLibrary;

var builder = WebApplication.CreateBuilder(args);
var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();

// Configure JWT authentication
builder.Services.AddJwtAuthentication(builder.Configuration, logger);

// Build configuration
var configuration = new ConfigurationBuilder()
    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

// Generate a JWT token (for testing or initial use)
var userId = "19";
var userName = "John David";
var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
var token = JwtTokenGenerator.GenerateJwtToken(
    jwtSettings.Issuer,
    jwtSettings.Audience,
    jwtSettings.SigningKey,
    userId,
    userName,
    jwtSettings.ExpirationMinutes
);

// Add middleware
var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
```

### Step 2: Use Authorization in Controllers

Decorate your API controllers or specific endpoints with the `[Authorize]` attribute to enforce authentication:

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class SecureController : ControllerBase
{
    [HttpGet("secure-data")]
    [Authorize]
    public IActionResult GetSecureData()
    {
        return Ok(new { message = "This is protected data." });
    }
}
```

## Library Features

### 1. Token Generation
The library includes a `JwtTokenGenerator` class to create JWT tokens:

```csharp
using JwtAuthLibrary;

var token = JwtTokenGenerator.GenerateJwtToken(
    issuer: "YourIssuer",
    audience: "YourAudience",
    signingKey: "YourSecretSigningKey",
    userId: "UserID",
    userName: "UserName",
    expirationMinutes: 60
);
```

### 2. Token Decoding
The `JwtExtension` class can decode tokens for validation or debugging purposes.

## Testing
1. Install and configure the library as outlined above.
2. Generate a token using `JwtTokenGenerator`.
3. Include the token in the `Authorization` header of your API requests:

   ```http
   Authorization: Bearer <your-token>
   ```
4. Access protected endpoints to verify authentication.
