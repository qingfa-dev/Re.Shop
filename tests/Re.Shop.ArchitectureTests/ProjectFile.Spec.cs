using System.Xml.Linq;

using Shouldly;

namespace ArchitectureTests;

/// <summary>
/// Spec §5.3 rules 6-8. These read the project files rather than the
/// assemblies: the AppHost compiles its references out
/// (<c>ReferenceOutputAssembly=false</c>), and whether a project is an
/// executable host is a property of the <c>.csproj</c>, not of the assembly.
/// </summary>
public sealed class ProjectFileSpec
{
    private static readonly string RepoRoot = FindRepoRoot();

    private static string FindRepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);

        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "Re.Shop.slnx")))
        {
            dir = dir.Parent!;
        }

        return dir is null
            ? throw new InvalidOperationException(
                $"Re.Shop.slnx not found above {AppContext.BaseDirectory}")
            : dir.FullName;
    }

    private static IReadOnlyList<string> ProjectReferences(string relativePath)
    {
        var document = XDocument.Load(Path.Combine(RepoRoot, relativePath));

        return document.Descendants()
            .Where(element => element.Name.LocalName == "ProjectReference")
            .Select(element => (string?)element.Attribute("Include") ?? string.Empty)
            .Select(path => Path.GetFileName(path) ?? path)
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToList();
    }

    private static string? OutputType(string relativePath)
    {
        var document = XDocument.Load(Path.Combine(RepoRoot, relativePath));

        return document.Descendants()
            .Where(element => element.Name.LocalName == "OutputType")
            .Select(element => element.Value.Trim())
            .FirstOrDefault();
    }

    [Fact]
    public void AppHost_references_only_the_two_hosts()
    {
        var references = ProjectReferences("aspire/Re.AppHost/Re.AppHost.csproj");

        references.ShouldBe(
            new[] { "Re.Shop.Admin.csproj", "Re.Shop.Api.csproj" });
    }

    [Fact]
    public void ServiceDefaults_references_no_shop_project()
    {
        var references = ProjectReferences("aspire/Re.ServiceDefaults/Re.ServiceDefaults.csproj");

        references.ShouldBeEmpty();
    }

    [Fact]
    public void No_src_project_is_an_executable_host()
    {
        var srcDirectory = Path.Combine(RepoRoot, "src");
        var offenders = new List<string>();

        foreach (var project in Directory.EnumerateFiles(
                     srcDirectory, "*.csproj", SearchOption.AllDirectories))
        {
            var relativePath = Path.GetRelativePath(RepoRoot, project);
            var outputType = OutputType(relativePath);

            if (outputType is "Exe" or "WinExe")
            {
                offenders.Add($"{relativePath}: OutputType={outputType}");
            }
        }

        offenders.ShouldBeEmpty(
            "spec §5.3 rule 6 — nothing under src/ may declare OutputType=Exe");
    }
}
