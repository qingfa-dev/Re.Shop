using Application;

using Contracts;

using Domain;

using Infrastructure;

using Re.Shop.Admin;
using Re.Shop.Api;

using SharedKernel;

using Shouldly;

namespace ArchitectureTests;

/// <summary>
/// Anchors. Every rule in <see cref="DependencySpec"/> locates its subject
/// assembly through one of these markers, so a marker deleted or moved into
/// the wrong project would otherwise make those rules inspect the wrong
/// assembly without saying so. One fact per marked project.
/// </summary>
public sealed class MarkerSpec
{
    [Fact]
    public void SharedKernel_marker_lives_in_the_SharedKernel_assembly()
    {
        typeof(SharedKernelMarker).Assembly.GetName().Name
            .ShouldBe("Re.Shop.SharedKernel");
    }

    [Fact]
    public void Domain_marker_lives_in_the_Domain_assembly()
    {
        typeof(DomainMarker).Assembly.GetName().Name
            .ShouldBe("Re.Shop.Domain");
    }

    [Fact]
    public void Contracts_marker_lives_in_the_Contracts_assembly()
    {
        typeof(ContractsMarker).Assembly.GetName().Name
            .ShouldBe("Re.Shop.Contracts");
    }

    [Fact]
    public void Application_marker_lives_in_the_Application_assembly()
    {
        typeof(ApplicationMarker).Assembly.GetName().Name
            .ShouldBe("Re.Shop.Application");
    }

    [Fact]
    public void Infrastructure_marker_lives_in_the_Infrastructure_assembly()
    {
        typeof(InfrastructureMarker).Assembly.GetName().Name
            .ShouldBe("Re.Shop.Infrastructure");
    }

    [Fact]
    public void Api_marker_lives_in_the_Api_assembly()
    {
        typeof(ApiMarker).Assembly.GetName().Name
            .ShouldBe("Re.Shop.Api");
    }

    [Fact]
    public void Admin_marker_lives_in_the_Admin_assembly()
    {
        typeof(AdminMarker).Assembly.GetName().Name
            .ShouldBe("Re.Shop.Admin");
    }
}
