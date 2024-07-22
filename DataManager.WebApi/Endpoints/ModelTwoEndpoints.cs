using System.Diagnostics;
using DataManager.Core.DBModels;
using DataManager.WebApi.Authorization;
using DataManager.WebApi.DTOs;
using DataManager.WebApi.Repositories;

namespace DataManager.WebApi.Endpoints;

public static class ModelTwosEndpoints
{
    const string GetModelTwoV1EndpointName = "GetModelTwo";

    public static RouteGroupBuilder MapModelTwosEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.NewVersionedApi()
                          .MapGroup("/ModelTwos")
                          .HasApiVersion(1.0);

        group.MapGet("/", async (
            IModelTwosRepository repository,
            ILoggerFactory loggerFactory,
            [AsParameters] GetModelTwoDto request,
            HttpContext http) =>
            {
                var totalCount = await repository.CountModelTwoAsync();
                http.Response.AddPaginationHeader(totalCount, request.PageSize);
                var logger = loggerFactory.CreateLogger("ModelTwos Endpoints");

                try
                {
                    var ModelTwos = await repository.GetAllModelTwosAsync(request.PageNumber, request.PageSize);
                    logger.LogInformation("Retrieved all ModelTwos successfully.");
                    return Results.Ok(ModelTwos.Select(ModelTwo => ModelTwo.AsDtoV1()));
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

        group.MapGet("/{id}", async (IModelTwosRepository repository, ILoggerFactory loggerFactory, int id) =>
        {
            var logger = loggerFactory.CreateLogger("ModelTwos Endpoints");

            try
            {
                var ModelTwo = await repository.GetModelTwoAsync(id);
                if (ModelTwo is not null)
                {
                    logger.LogInformation("Retrieved ModelTwo with ID {Id} successfully.", id);
                    return Results.Ok(ModelTwo.AsDtoV1());
                }
                else
                {
                    logger.LogWarning("ModelTwo with ID {Id} not found.", id);
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
        .WithName(GetModelTwoV1EndpointName)
        .RequireAuthorization(Policies.ReadAccess)
        .MapToApiVersion(1.0);

        group.MapPost("/", async (IModelTwosRepository repository, ILoggerFactory loggerFactory, CreateModelTwoDto ModelTwoDto) =>
        {
            var logger = loggerFactory.CreateLogger("ModelTwos Endpoints");

            try
            {
                var modelTwo = new ModelTwo
                {
                    ExitId = ModelTwoDto.ExitId,
                    PeriodStartDate = ModelTwoDto.PeriodStartDate,
                    PeriodEndDate = ModelTwoDto.PeriodEndDate,
                    GainAmountThree = ModelTwoDto.GainAmountThree
                };

                await repository.CreateModelTwoAsync(modelTwo);
                logger.LogInformation("Created a new ModelTwo with ID {Id} successfully.", modelTwo.Id);
                return Results.CreatedAtRoute(GetModelTwoV1EndpointName, new { id = modelTwo.Id }, modelTwo);
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

        group.MapPut("/{id}", async (IModelTwosRepository repository, ILoggerFactory loggerFactory, int id, UpdateModelTwoDto updatedModelTwoDto) =>
        {
            var logger = loggerFactory.CreateLogger("ModelTwos Endpoints");

            try
            {
                var existingModelTwo = await repository.GetModelTwoAsync(id);

                if (existingModelTwo is null)
                {
                    logger.LogWarning("ModelTwo with ID {Id} not found.", id);
                    return Results.NotFound();
                }

                existingModelTwo.PeriodStartDate = updatedModelTwoDto.PeriodStartDate;
                existingModelTwo.PeriodEndDate = updatedModelTwoDto.PeriodEndDate;
                existingModelTwo.GainAmountThree = updatedModelTwoDto.GainAmountThree;

                await repository.UpdateModelTwoAsync(existingModelTwo);
                logger.LogInformation("Updated ModelTwo with ID {Id} successfully.", id);

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

        group.MapDelete("/{id}", async (IModelTwosRepository repository, ILoggerFactory loggerFactory, int id) =>
        {
            var logger = loggerFactory.CreateLogger("ModelTwos Endpoints");

            try
            {
                var ModelTwo = await repository.GetModelTwoAsync(id);

                if (ModelTwo is not null)
                {
                    await repository.DeleteModelTwoAsync(id);
                    logger.LogInformation("Deleted ModelTwo with ID {Id} successfully.", id);
                }
                else
                {
                    logger.LogWarning("ModelTwo with ID {Id} not found.", id);
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
