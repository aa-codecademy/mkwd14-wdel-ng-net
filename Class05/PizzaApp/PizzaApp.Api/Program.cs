using PizzaApp.Api.Extensions;
using PizzaApp.DataAccess;
using PizzaApp.Mappers;
using PizzaApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddDataAccess(builder.Configuration)
    .AddMappers()
    .AddServices()
    .AddApi();

var app = builder.Build();

// => Errors: an exception thrown anywhere after this line (controllers, services, repositories) ends up here, and GlobalExceptionHandler (registered in AddApi()) turns it into an ErrorResponse.
// => The empty lambda is a fallback that never runs: GlobalExceptionHandler handles every exception.
// => It still has to be there: a plain UseExceptionHandler() stops the app at startup unless the app also uses ProblemDetails (services.AddProblemDetails()), .NET's own error format, which ErrorResponse replaces.
app.UseExceptionHandler(_ => { });

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
