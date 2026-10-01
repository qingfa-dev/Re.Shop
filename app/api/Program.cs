var builder = WebApplication.CreateBuilder(args: args);

var app = builder.Build();

app.Run();

// Re-exposes the compiler-generated host entry point so
// WebApplicationFactory<Program> can boot this assembly from tests.
public partial class Program { }
