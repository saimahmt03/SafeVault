using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SafeVaultAPI.DTO.Result;
using SafeVaultAPI.Service;
using Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;

namespace SafeVaultAPI.TEST
{
    public class JwtTokenGeneratorTests
    {
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public JwtTokenGeneratorTests()
        {
            // Arrange: Mock configuration
            var inMemorySettings = new Dictionary<string, string>
            {
                {"JwtSettings:Clients:0:Client", "SafeVaultApp"},
                {"JwtSettings:Clients:0:Key", "ThisIsASecretKeyForTesting123456!"},
                {"JwtSettings:Clients:0:Issuer", "SafeVaultAPI"},
                {"JwtSettings:Clients:0:Audience", "SafeVaultUsers"},
                {"JwtSettings:Clients:0:DurationInMinutes", "60"}
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var loggerMock = new Mock<ILogger<JwtTokenGenerator>>();

            //_jwtTokenGenerator = new JwtTokenGenerator(configuration, loggerMock.Object);
        }

        [Fact]
        public void GenerateToken_Should_Create_Valid_Jwt_With_Expected_Claims()
        {
            // Act
            TokenResult tokenResult = _jwtTokenGenerator.GenerateToken("saimahMT03", 1, "InternalClient");

            // Assert
            Assert.False(string.IsNullOrEmpty(tokenResult.Token));
            Assert.Equal("SafeVaultAPI", tokenResult.IssueBy);
            Assert.Equal("SafeVaultApp", tokenResult.Role);
            Assert.True(tokenResult.Duration > 0);

            // Parse token
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(tokenResult.Token);

            Assert.Equal("testuser", token.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value);
            Assert.Equal("1", token.Claims.First(c => c.Type == ClaimTypes.Role).Value);

            // Validate token expiry
            var expires = token.ValidTo;
            Assert.True(expires > DateTime.UtcNow);
        }
    }
}