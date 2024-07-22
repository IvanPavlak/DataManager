using System.Diagnostics;
using DataManager.Core.DBModels;
using DataManager.WebApi.Authorization;
using DataManager.WebApi.DTOs;
using DataManager.WebApi.Repositories;

namespace DataManager.WebApi.Endpoints;

public static class DataModelOnesEndpoints
{
    const string GetDataModelOneV1EndpointName = "GetDataModelOne";

    public static RouteGroupBuilder MapDataModelOnesEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.NewVersionedApi()
                          .MapGroup("/dataModelOnes")
                          .HasApiVersion(1.0);

        group.MapGet("/", async (
            IDataModelOnesRepository repository,
            ILoggerFactory loggerFactory,
            [AsParameters] GetDataModelOneDto request,
            HttpContext http) =>
            {
                var totalCount = await repository.CountDataModelOneAsync(request.Filter);
                http.Response.AddPaginationHeader(totalCount, request.PageSize);

                var logger = loggerFactory.CreateLogger("DataModelOnes Endpoints");

                try
                {
                    var dataModelOnes = await repository.GetAllDataModelOnesAsync(request.PageNumber, request.PageSize, request.Filter);
                    logger.LogInformation("Retrieved all dataModelOnes successfully.");
                    return Results.Ok(dataModelOnes.Select(dataModelOne => dataModelOne.AsDtoV1()));
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Could not process a request on machine {Machine}. TraceId: {TraceId}",
                        Environment.MachineName,
                        Activity.Current?.TraceId);

                    return Results.Problem(
                        title: "Working on it!",
                        statusCode: StatusCodes.Status500InternalServerError,
                        extensions: new Dictionary<string, object>
                        {
                            {"traceId", Activity.Current?.TraceId.ToString()}
                        }
                    );
                }
            })
            .RequireAuthorization(Policies.ReadAccess)
            .MapToApiVersion(1.0);

        group.MapGet("/{id}", async (IDataModelOnesRepository repository, ILoggerFactory loggerFactory, int id) =>
        {
            var logger = loggerFactory.CreateLogger("DataModelOnes Endpoints");

            try
            {
                var dataModelOne = await repository.GetDataModelOneAsync(id);
                if (dataModelOne is not null)
                {
                    logger.LogInformation("Retrieved DataModelOne with ID {Id} successfully.", id);
                    return Results.Ok(dataModelOne.AsDtoV1());
                }
                else
                {
                    logger.LogWarning("DataModelOne with ID {Id} not found.", id);
                    return Results.NotFound();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Could not process a request on machine {Machine}. TraceId: {TraceId}",
                    Environment.MachineName,
                    Activity.Current?.TraceId);

                return Results.Problem(
                    title: "Working on it!",
                    statusCode: StatusCodes.Status500InternalServerError,
                    extensions: new Dictionary<string, object>
                    {
                        {"traceId", Activity.Current?.TraceId.ToString()}
                    }
                );
            }
        })
        .WithName(GetDataModelOneV1EndpointName)
        .RequireAuthorization(Policies.ReadAccess)
        .MapToApiVersion(1.0); ;

        group.MapPost("/", async (IDataModelOnesRepository repository, ILoggerFactory loggerFactory, CreateDataModelOneDto dataModelOneDto) =>
        {
            var logger = loggerFactory.CreateLogger("DataModelOnes Endpoints");

            try
            {
                var dataModelOne = new DataModelOne
                {
                    ExitId = dataModelOneDto.ExitId,
                    Port = dataModelOneDto.Port,
                    UserGroup = dataModelOneDto.UserGroup,
                    Country = dataModelOneDto.Country,
                    MemberId = dataModelOneDto.MemberId,
                    Date = dataModelOneDto.Date,
                    GainAmountOne = dataModelOneDto.GainAmountOne,
                    GainAmountTwo = dataModelOneDto.GainAmountTwo,
                    Loss = dataModelOneDto.Loss,
                    Total = dataModelOneDto.Total
                };

                await repository.CreateDataModelOneAsync(dataModelOne);
                logger.LogInformation("Created a new dataModelOne with ID {Id} successfully.", dataModelOne.Id);
                return Results.CreatedAtRoute(GetDataModelOneV1EndpointName, new { id = dataModelOne.Id }, dataModelOne);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Could not process a request on machine {Machine}. TraceId: {TraceId}",
                    Environment.MachineName,
                    Activity.Current?.TraceId);

                return Results.Problem(
                    title: "Working on it!",
                    statusCode: StatusCodes.Status500InternalServerError,
                    extensions: new Dictionary<string, object>
                    {
                        {"traceId", Activity.Current?.TraceId.ToString()}
                    }
                );
            }
        })
        .RequireAuthorization(Policies.WriteAccess)
        .MapToApiVersion(1.0); ;

        group.MapPut("/{id}", async (IDataModelOnesRepository repository, ILoggerFactory loggerFactory, int id, UpdateDataModelOneDto updatedDataModelOneDto) =>
        {
            var logger = loggerFactory.CreateLogger("DataModelOnes Endpoints");

            try
            {
                var existingDataModelOne = await repository.GetDataModelOneAsync(id);

                if (existingDataModelOne is null)
                {
                    logger.LogWarning("DataModelOne with ID {Id} not found.", id);
                    return Results.NotFound();
                }

                existingDataModelOne.Port = updatedDataModelOneDto.Port;
                existingDataModelOne.UserGroup = updatedDataModelOneDto.UserGroup;
                existingDataModelOne.Country = updatedDataModelOneDto.Country;
                existingDataModelOne.MemberId = updatedDataModelOneDto.MemberId;
                existingDataModelOne.Date = updatedDataModelOneDto.Date;
                existingDataModelOne.GainAmountOne = updatedDataModelOneDto.GainAmountOne;
                existingDataModelOne.GainAmountTwo = updatedDataModelOneDto.GainAmountTwo;
                existingDataModelOne.Loss = updatedDataModelOneDto.Loss;
                existingDataModelOne.Total = updatedDataModelOneDto.Total;

                await repository.UpdateDataModelOneAsync(existingDataModelOne);
                logger.LogInformation("Updated DataModelOne with ID {Id} successfully.", id);

                return Results.NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Could not process a request on machine {Machine}. TraceId: {TraceId}",
                    Environment.MachineName,
                    Activity.Current?.TraceId);

                return Results.Problem(
                    title: "Working on it!",
                    statusCode: StatusCodes.Status500InternalServerError,
                    extensions: new Dictionary<string, object>
                    {
                        {"traceId", Activity.Current?.TraceId.ToString()}
                    }
                );
            }
        })
        .RequireAuthorization(Policies.WriteAccess)
        .MapToApiVersion(1.0); ;

        group.MapDelete("/{id}", async (IDataModelOnesRepository repository, ILoggerFactory loggerFactory, int id) =>
        {
            var logger = loggerFactory.CreateLogger("DataModelOnes Endpoints");

            try
            {
                var dataModelOne = await repository.GetDataModelOneAsync(id);

                if (dataModelOne is not null)
                {
                    await repository.DeleteDataModelOneAsync(id);
                    logger.LogInformation("Deleted DataModelOne with ID {Id} successfully.", id);
                }
                else
                {
                    logger.LogWarning("DataModelOne with ID {Id} not found.", id);
                }

                return Results.NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Could not process a request on machine {Machine}. TraceId: {TraceId}",
                    Environment.MachineName,
                    Activity.Current?.TraceId);

                return Results.Problem(
                    title: "Working on it!",
                    statusCode: StatusCodes.Status500InternalServerError,
                    extensions: new Dictionary<string, object>
                    {
                        {"traceId", Activity.Current?.TraceId.ToString()}
                    }
                );
            }
        })
        .RequireAuthorization(Policies.WriteAccess)
        .MapToApiVersion(1.0); ;

        return group;
    }
}