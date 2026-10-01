using Application;

using Contracts;

using Domain;

using Infrastructure;

using NetArchTest.Rules;

using Re.Shop.Admin;

using SharedKernel;

using Shouldly;

namespace ArchitectureTests;

/// <summary>
/// Spec §5.3 rules 1-5, asserted against compiled assemblies through their
/// markers. If a rule fails the message names the offending dependency: fix
/// the <c>ProjectReference</c>, never weaken the rule.
/// </summary>
public sealed class DependencySpec
{
    [Fact]
    public void SharedKernel_depends_on_nothing()
    {
        var result = Types.InAssembly(typeof(SharedKernelMarker).Assembly)
            .Should().NotHaveDependencyOn("Domain")
            .And().NotHaveDependencyOn("Application")
            .And().NotHaveDependencyOn("Contracts")
            .And().NotHaveDependencyOn("Infrastructure")
            .And().NotHaveDependencyOn("Re.Shop.Api")
            .And().NotHaveDependencyOn("Re.Shop.Admin")
            .GetResult();

        result.IsSuccessful.ShouldBeTrue($"failing types: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }

    [Fact]
    public void Domain_depends_only_on_SharedKernel()
    {
        var result = Types.InAssembly(typeof(DomainMarker).Assembly)
            .Should().NotHaveDependencyOn("Application")
            .And().NotHaveDependencyOn("Contracts")
            .And().NotHaveDependencyOn("Infrastructure")
            .And().NotHaveDependencyOn("Re.Shop.Api")
            .And().NotHaveDependencyOn("Re.Shop.Admin")
            .GetResult();

        result.IsSuccessful.ShouldBeTrue($"failing types: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }

    [Fact]
    public void Application_depends_inward_only()
    {
        var result = Types.InAssembly(typeof(ApplicationMarker).Assembly)
            .Should().NotHaveDependencyOn("Contracts")
            .And().NotHaveDependencyOn("Infrastructure")
            .And().NotHaveDependencyOn("Re.Shop.Api")
            .And().NotHaveDependencyOn("Re.Shop.Admin")
            .GetResult();

        result.IsSuccessful.ShouldBeTrue($"failing types: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }

    [Fact]
    public void Contracts_depends_on_nothing()
    {
        var result = Types.InAssembly(typeof(ContractsMarker).Assembly)
            .Should().NotHaveDependencyOn("SharedKernel")
            .And().NotHaveDependencyOn("Domain")
            .And().NotHaveDependencyOn("Application")
            .And().NotHaveDependencyOn("Infrastructure")
            .And().NotHaveDependencyOn("Re.Shop.Api")
            .And().NotHaveDependencyOn("Re.Shop.Admin")
            .GetResult();

        result.IsSuccessful.ShouldBeTrue($"failing types: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }

    [Fact]
    public void Infrastructure_depends_inward_only()
    {
        var result = Types.InAssembly(typeof(InfrastructureMarker).Assembly)
            .Should().NotHaveDependencyOn("Re.Shop.Api")
            .And().NotHaveDependencyOn("Re.Shop.Admin")
            .GetResult();

        result.IsSuccessful.ShouldBeTrue($"failing types: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }

    [Fact]
    public void Admin_depends_only_on_contracts_and_shared_kernel()
    {
        var result = Types.InAssembly(typeof(AdminMarker).Assembly)
            .Should().NotHaveDependencyOn("Domain")
            .And().NotHaveDependencyOn("Application")
            .And().NotHaveDependencyOn("Infrastructure")
            .And().NotHaveDependencyOn("Re.Shop.Api")
            .GetResult();

        result.IsSuccessful.ShouldBeTrue($"failing types: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }

    [Fact]
    public void Only_Infrastructure_and_Api_may_reference_entity_framework()
    {
        var subjectAssemblies = new[]
        {
            typeof(SharedKernelMarker).Assembly,
            typeof(DomainMarker).Assembly,
            typeof(ApplicationMarker).Assembly,
            typeof(ContractsMarker).Assembly,
        };

        foreach (var assembly in subjectAssemblies)
        {
            var result = Types.InAssembly(assembly)
                .Should().NotHaveDependencyOn("Microsoft.EntityFrameworkCore")
                .And().NotHaveDependencyOn("Npgsql")
                .GetResult();

            result.IsSuccessful.ShouldBeTrue(
                $"{assembly.GetName().Name}: failing types {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
        }
    }
}
