using System.Diagnostics;
using DataManager.Core.DBModels;
using DataManager.WebApi.Authorization;
using DataManager.WebApi.DTOs;
using DataManager.WebApi.Endpoints;
using DataManager.WebApi.Repositories;

namespace DataManager.WebApi.Endpoints;

public static class ModelOnesEndpoints
{
    const string GetModelOneV1EndpointName = "GetModelOne";

    public static RouteGroupBuilder MapModelOnesEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.NewVersionedApi()
                          .MapGroup("/ModelOnes")
                          .HasApiVersion(1.0);

        group.MapGet("/", async (
            IModelOnesRepository repository,
            ILoggerFactory loggerFactory,
            [AsParameters] GetModelOneDto request,
            HttpContext http) =>
            {
                var totalCount = await repository.CountModelOneAsync(request.Filter);
                http.Response.AddPaginationHeader(totalCount, request.PageSize);

                var logger = loggerFactory.CreateLogger("ModelOnes Endpoints");

                try
                {
                    var ModelOnes = await repository.GetAllModelOnesAsync(request.PageNumber, request.PageSize, request.Filter);
                    logger.LogInformation("Retrieved all ModelOnes successfully.");
                    return Results.Ok(ModelOnes.Select(ModelOne => ModelOne.AsDtoV1()));
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

        group.MapGet("/{id}", async (IModelOnesRepository repository, ILoggerFactory loggerFactory, int id) =>
        {
            var logger = loggerFactory.CreateLogger("ModelOnes Endpoints");

            try
            {
                var ModelOne = await repository.GetModelOneAsync(id);
                if (ModelOne is not null)
                {
                    logger.LogInformation("Retrieved ModelOne with ID {Id} successfully.", id);
                    return Results.Ok(ModelOne.AsDtoV1());
                }
                else
                {
                    logger.LogWarning("ModelOne with ID {Id} not found.", id);
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
        .WithName(GetModelOneV1EndpointName)
        .RequireAuthorization(Policies.ReadAccess)
        .MapToApiVersion(1.0); ;

        group.MapPost("/", async (IModelOnesRepository repository, ILoggerFactory loggerFactory, CreateModelOneDto ModelOneDto) =>
        {
            var logger = loggerFactory.CreateLogger("ModelOnes Endpoints");

            try
            {
                var modelOne = new ModelOne
                {
                    ExitId = ModelOneDto.ExitId,
                    Port = ModelOneDto.Port,
                    UserGroup = ModelOneDto.UserGroup,
                    Country = ModelOneDto.Country,
                    MemberId = ModelOneDto.MemberId,
                    Date = ModelOneDto.Date,
                    GainAmountOne = ModelOneDto.GainAmountOne,
                    GainAmountTwo = ModelOneDto.GainAmountTwo,
                    Loss = ModelOneDto.Loss,
                    Total = ModelOneDto.Total
                };

                await repository.CreateModelOneAsync(modelOne);
                logger.LogInformation("Created a new ModelOne with ID {Id} successfully.", modelOne.Id);
                return Results.CreatedAtRoute(GetModelOneV1EndpointName, new { id = modelOne.Id }, modelOne);
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

        group.MapPut("/{id}", async (IModelOnesRepository repository, ILoggerFactory loggerFactory, int id, UpdateModelOneDto updatedModelOneDto) =>
        {
            var logger = loggerFactory.CreateLogger("ModelOnes Endpoints");

            try
            {
                var existingModelOne = await repository.GetModelOneAsync(id);

                if (existingModelOne is null)
                {
                    logger.LogWarning("ModelOne with ID {Id} not found.", id);
                    return Results.NotFound();
                }

                existingModelOne.Port = updatedModelOneDto.Port;
                existingModelOne.UserGroup = updatedModelOneDto.UserGroup;
                existingModelOne.Country = updatedModelOneDto.Country;
                existingModelOne.MemberId = updatedModelOneDto.MemberId;
                existingModelOne.Date = updatedModelOneDto.Date;
                existingModelOne.GainAmountOne = updatedModelOneDto.GainAmountOne;
                existingModelOne.GainAmountTwo = updatedModelOneDto.GainAmountTwo;
                existingModelOne.Loss = updatedModelOneDto.Loss;
                existingModelOne.Total = updatedModelOneDto.Total;

                await repository.UpdateModelOneAsync(existingModelOne);
                logger.LogInformation("Updated ModelOne with ID {Id} successfully.", id);

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

        group.MapDelete("/{id}", async (IModelOnesRepository repository, ILoggerFactory loggerFactory, int id) =>
        {
            var logger = loggerFactory.CreateLogger("ModelOnes Endpoints");

            try
            {
                var ModelOne = await repository.GetModelOneAsync(id);

                if (ModelOne is not null)
                {
                    await repository.DeleteModelOneAsync(id);
                    logger.LogInformation("Deleted ModelOne with ID {Id} successfully.", id);
                }
                else
                {
                    logger.LogWarning("ModelOne with ID {Id} not found.", id);
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