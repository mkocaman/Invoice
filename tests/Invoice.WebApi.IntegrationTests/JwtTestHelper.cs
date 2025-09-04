using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Invoice.WebApi.IntegrationTests;

/// <summary>
/// Helper class for generating test JWT tokens for CSMS authentication scenarios
/// </summary>
public static class JwtTestHelper
{
    private const string TestSigningKey = "test-signing-key-for-integration-tests-only-32-chars";
    private const string TestIssuer = "https://test-csms.example.com";
    private const string TestAudience = "https://test-invoice-api.example.com";

    /// <summary>
    /// Creates a test JWT token with CSMS scope for integration tests
    /// </summary>
    /// <param name="userId">User ID for the token</param>
    /// <param name="additionalClaims">Additional claims to include</param>
    /// <returns>JWT token string</returns>
    public static string CreateTestToken(string userId = "test-user-123", Dictionary<string, string>? additionalClaims = null)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestSigningKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Name, "Test User"),
            new("scope", "csms.invoice"),
            new("iss", TestIssuer),
            new("aud", TestAudience),
            new("exp", DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            new("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        // Add additional claims if provided
        if (additionalClaims != null)
        {
            foreach (var claim in additionalClaims)
            {
                claims.Add(new Claim(claim.Key, claim.Value));
            }
        }

        var token = new JwtSecurityToken(
            issuer: TestIssuer,
            audience: TestAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Creates a test JWT token without CSMS scope (for testing unauthorized scenarios)
    /// </summary>
    /// <param name="userId">User ID for the token</param>
    /// <returns>JWT token string</returns>
    public static string CreateTestTokenWithoutScope(string userId = "test-user-123")
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestSigningKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Name, "Test User"),
            new("iss", TestIssuer),
            new("aud", TestAudience),
            new("exp", DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            new("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        var token = new JwtSecurityToken(
            issuer: TestIssuer,
            audience: TestAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Creates an expired test JWT token for testing token expiration scenarios
    /// </summary>
    /// <param name="userId">User ID for the token</param>
    /// <returns>Expired JWT token string</returns>
    public static string CreateExpiredTestToken(string userId = "test-user-123")
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestSigningKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Name, "Test User"),
            new("scope", "csms.invoice"),
            new("iss", TestIssuer),
            new("aud", TestAudience),
            new("exp", DateTimeOffset.UtcNow.AddHours(-1).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            new("iat", DateTimeOffset.UtcNow.AddHours(-2).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        var token = new JwtSecurityToken(
            issuer: TestIssuer,
            audience: TestAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(-1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
