namespace DataManager.WebApi.DTOs;

public record GetModelTwoDto(
    int PageNumber = 1,
    int PageSize = 1,
    string Filter = null
);

public record GetModelTwoDtoV1(
    int ExitId,
    DateOnly PeriodStartDate,
    DateTime PeriodEndDate,
    int GainAmountThree
);

public record CreateModelTwoDto(
    int ExitId,
    DateOnly PeriodStartDate,
    DateTime PeriodEndDate,
    int GainAmountThree
);

public record UpdateModelTwoDto(
    DateOnly PeriodStartDate,
    DateTime PeriodEndDate,
    int GainAmountThree
);