var builder = DistributedApplication.CreateBuilder(args: args);

var postgres = builder.AddPostgres(name: "postgres");

var api = builder
    .AddProject<Projects.Re_Shop_Api>(name: "api")
    .WithReference(
        source: postgres,
        connectionName: "Shop",
        optional: false)
    .WaitFor(dependency: postgres);

builder.AddProject<Projects.Re_Shop_Admin>(name: "admin")
    .WithEnvironment(
        name: "Api__BaseUrl",
        value: "http://api");

builder.AddViteApp(
        name: "storefront",
        appDirectory: Path.GetFullPath(
            path: Path.Combine(
                paths: [builder.AppHostDirectory, "..", "..", "app", "storefront"])),
        runScriptName: "dev")
    .WithPnpm();

builder.Build().Run();