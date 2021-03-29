using System;
using System.Security.Claims;
using Xunit;

namespace ToDoApp.Components.Extensions
{
    public class ClaimsPrincipalExtensionsTests
    {
        [Fact]
        public void Id_NoClaim_Zero()
        {
            Int64 expected = 0;
            Int64 actual = new ClaimsPrincipal().Id();

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("", 0)]
        [InlineData("1", 1)]
        public void Id_ReturnsNameIdentifierClaim(String identifier, Int64 id)
        {
            ClaimsIdentity identity = new();
            ClaimsPrincipal principal = new(identity);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, identifier));

            Int64 actual = principal.Id();
            Int64 expected = id;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void UpdateClaim_New()
        {
            ClaimsIdentity identity = new();
            ClaimsPrincipal principal = new(identity);

            principal.UpdateClaim(ClaimTypes.Name, "Test");

            String? actual = principal.FindFirstValue(ClaimTypes.Name);
            String? expected = "Test";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void UpdateClaim_Existing()
        {
            ClaimsIdentity identity = new();
            ClaimsPrincipal principal = new(identity);
            identity.AddClaim(new Claim(ClaimTypes.Name, "ClaimTypeName"));

            principal.UpdateClaim(ClaimTypes.Name, "Test");

            String? actual = principal.FindFirstValue(ClaimTypes.Name);
            String? expected = "Test";

            Assert.Equal(expected, actual);
        }
    }
}
