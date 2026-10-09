using PizzaApp.Api.Extensions;
using PizzaApp.DataAccess;
using PizzaApp.Services;
using PizzaApp.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddDataAccess(builder.Configuration)
    .AddServices()
    .AddJwtAuthentication()
    .AddOpenApiDocumentation();
// TODO : Create AddApi extension method

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapApiDocs(); // /openapi/v1.json + Scalar UI at /scalar
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
