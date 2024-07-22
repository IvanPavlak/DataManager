using System.Diagnostics;
using DataManager.Core.DBModels;
using DataManager.WebApi.Authorization;
using DataManager.WebApi.DTOs;
using DataManager.WebApi.Repositories;

namespace DataManager.WebApi.Endpoints;

public static class DataModelTwosEndpoints
{
    const string GetDataModelTwoV1EndpointName = "GetDataModelTwo";

    public static RouteGroupBuilder MapDataModelTwosEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.NewVersionedApi()
                          .MapGroup("/dataModelTwos")
                          .HasApiVersion(1.0);

        group.MapGet("/", async (
            IDataModelTwosRepository repository,
            ILoggerFactory loggerFactory,
            [AsParameters] GetDataModelTwoDto request,
            HttpContext http) =>
            {
                var totalCount = await repository.CountDataModelTwoAsync();
                http.Response.AddPaginationHeader(totalCount, request.PageSize);
                var logger = loggerFactory.CreateLogger("DataModelTwos Endpoints");

                try
                {
                    var dataModelTwos = await repository.GetAllDataModelTwosAsync(request.PageNumber, request.PageSize);
                    logger.LogInformation("Retrieved all DataModelTwos successfully.");
                    return Results.Ok(dataModelTwos.Select(dataModelTwo => dataModelTwo.AsDtoV1()));
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

        group.MapGet("/{id}", async (IDataModelTwosRepository repository, ILoggerFactory loggerFactory, int id) =>
        {
            var logger = loggerFactory.CreateLogger("DataModelTwos Endpoints");

            try
            {
                var dataModelTwo = await repository.GetDataModelTwoAsync(id);
                if (dataModelTwo is not null)
                {
                    logger.LogInformation("Retrieved DataModelTwo with ID {Id} successfully.", id);
                    return Results.Ok(dataModelTwo.AsDtoV1());
                }
                else
                {
                    logger.LogWarning("DataModelTwo with ID {Id} not found.", id);
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
        .WithName(GetDataModelTwoV1EndpointName)
        .RequireAuthorization(Policies.ReadAccess)
        .MapToApiVersion(1.0);

        group.MapPost("/", async (IDataModelTwosRepository repository, ILoggerFactory loggerFactory, CreateDataModelTwoDto dataModelTwoDto) =>
        {
            var logger = loggerFactory.CreateLogger("DataModelTwos Endpoints");

            try
            {
                var dataModelTwo = new DataModelTwo
                {
                    ExitId = dataModelTwoDto.ExitId,
                    PeriodStartDate = dataModelTwoDto.PeriodStartDate,
                    PeriodEndDate = dataModelTwoDto.PeriodEndDate,
                    GainAmountThree = dataModelTwoDto.GainAmountThree
                };

                await repository.CreateDataModelTwoAsync(dataModelTwo);
                logger.LogInformation("Created a new DataModelTwo with ID {Id} successfully.", dataModelTwo.Id);
                return Results.CreatedAtRoute(GetDataModelTwoV1EndpointName, new { id = dataModelTwo.Id }, dataModelTwo);
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
        .MapToApiVersion(1.0);

        group.MapPut("/{id}", async (IDataModelTwosRepository repository, ILoggerFactory loggerFactory, int id, UpdateDataModelTwoDto updatedDataModelTwoDto) =>
        {
            var logger = loggerFactory.CreateLogger("DataModelTwos Endpoints");

            try
            {
                var existingDataModelTwo = await repository.GetDataModelTwoAsync(id);

                if (existingDataModelTwo is null)
                {
                    logger.LogWarning("DataModelTwo with ID {Id} not found.", id);
                    return Results.NotFound();
                }

                existingDataModelTwo.PeriodStartDate = updatedDataModelTwoDto.PeriodStartDate;
                existingDataModelTwo.PeriodEndDate = updatedDataModelTwoDto.PeriodEndDate;
                existingDataModelTwo.GainAmountThree = updatedDataModelTwoDto.GainAmountThree;

                await repository.UpdateDataModelTwoAsync(existingDataModelTwo);
                logger.LogInformation("Updated DataModelTwo with ID {Id} successfully.", id);

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
        .MapToApiVersion(1.0);

        group.MapDelete("/{id}", async (IDataModelTwosRepository repository, ILoggerFactory loggerFactory, int id) =>
        {
            var logger = loggerFactory.CreateLogger("DataModelTwos Endpoints");

            try
            {
                var dataModelTwo = await repository.GetDataModelTwoAsync(id);

                if (dataModelTwo is not null)
                {
                    await repository.DeleteDataModelTwoAsync(id);
                    logger.LogInformation("Deleted DataModelTwo with ID {Id} successfully.", id);
                }
                else
                {
                    logger.LogWarning("DataModelTwo with ID {Id} not found.", id);
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
        .MapToApiVersion(1.0);

        return group;
    }
}
