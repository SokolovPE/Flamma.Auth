using AzisFood.DataEngine.Postgres.Extensions;
using Flamma.Auth.Data.Access;
using Flamma.Auth.Data.Access.Extensions;
using Flamma.Auth.Extensions;
using Flamma.Auth.Mappings;
using Flamma.Auth.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add serilog logging
builder.Host
    .UseSerilog((_, configuration) => configuration.ReadFrom.Configuration(builder.Configuration));

// Additional configuration is required to successfully run gRPC on macOS
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container
builder.Services.AddCoreServices();

// Add data engine
builder.Services
    .AddPostgresSupport(builder.Configuration)
    .AddPostgresContext<AuthDbContext>();

// builder.Services.AddTransient<AuthDbSeeder>();

// Add Grpc
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

// Add mappings
builder.Services.AddAutoMapper(typeof(MappingProfile));

var app = builder.Build();

// Enable reflection for development
if (app.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService();
    //app.Services.AddSeeder(app.Configuration);
}
// Configure the HTTP request pipeline
app.MapGrpcService<AccountManagerService>();
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");


// app.Services.AddSeeder(app.Configuration);

app.Run();