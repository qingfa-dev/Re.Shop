using System.Net;

using Microsoft.AspNetCore.Mvc.Testing;

using Shouldly;

namespace IntegrationTests;

/// <summary>
/// Boots the real API composition root in-process through
/// <c>WebApplicationFactory</c> and drives it over HTTP, so failures in DI
/// wiring or middleware show up here rather than only at deployment.
/// </summary>
public sealed class ApiHostSpec
{
    [Fact]
    public async Task ApiHost_BootsAndServesRequests_NoServerError()
    {
        await using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/");

        response.StatusCode.ShouldNotBe(
            HttpStatusCode.InternalServerError,
            "a composition root that cannot build, or middleware that throws, surfaces as 500 here");
    }
}
