using DataManager.WebApi.Data;
using DataManager.WebApi.Authorization;
using DataManager.WebApi.OpenAPI;
using DataManager.WebApi.Middleware;
using DataManager.WebApi.Endpoints;
using DataManager.Core;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRepositories(builder.Configuration);

builder.Services.AddAuthentication().AddJwtBearer();
builder.Services.AddWebAuthorization();
builder.Services.AddHttpLogging(option => { });
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new(1.0);
    options.AssumeDefaultVersionWhenUnspecified = true;
})
.AddApiExplorer(options => options.GroupNameFormat = "'v'VVV");

var connString = builder.Configuration.GetConnectionString("ConnectionString");

builder.Services.AddNpgsql<DataManagerDbContext>(connString);

builder.Services.AddSwaggerGen()
                .AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>()
                .AddEndpointsApiExplorer();

var app = builder.Build();

app.UseMiddleware<RequestTimingMiddleware>();

await app.Services.InitializeDbAsync();

app.UseHttpLogging();
app.MapModelOnesEndpoints();
app.MapModelTwosEndpoints();

app.UseDataManagerSwagger();

app.Run();