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
    public void SharedKernelMarker_LivesInAssembly_IsSharedKernel()
    {
        typeof(SharedKernelMarker).Assembly.GetName().Name
            .ShouldBe("Re.Shop.SharedKernel");
    }

    [Fact]
    public void DomainMarker_LivesInAssembly_IsDomain()
    {
        typeof(DomainMarker).Assembly.GetName().Name
            .ShouldBe("Re.Shop.Domain");
    }

    [Fact]
    public void ContractsMarker_LivesInAssembly_IsContracts()
    {
        typeof(ContractsMarker).Assembly.GetName().Name
            .ShouldBe("Re.Shop.Contracts");
    }

    [Fact]
    public void ApplicationMarker_LivesInAssembly_IsApplication()
    {
        typeof(ApplicationMarker).Assembly.GetName().Name
            .ShouldBe("Re.Shop.Application");
    }

    [Fact]
    public void InfrastructureMarker_LivesInAssembly_IsInfrastructure()
    {
        typeof(InfrastructureMarker).Assembly.GetName().Name
            .ShouldBe("Re.Shop.Infrastructure");
    }

    [Fact]
    public void ApiMarker_LivesInAssembly_IsApi()
    {
        typeof(ApiMarker).Assembly.GetName().Name
            .ShouldBe("Re.Shop.Api");
    }

    [Fact]
    public void AdminMarker_LivesInAssembly_IsAdmin()
    {
        typeof(AdminMarker).Assembly.GetName().Name
            .ShouldBe("Re.Shop.Admin");
    }
}
