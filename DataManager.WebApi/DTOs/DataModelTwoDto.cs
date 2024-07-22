namespace DataManager.WebApi.DTOs;

public record GetDataModelTwoDto(
    int PageNumber = 1,
    int PageSize = 1,
    string Filter = null
);

public record GetDataModelTwoDtoV1(
    int ExitId,
    DateOnly PeriodStartDate,
    DateTime PeriodEndDate,
    int GainAmountThree
);

public record CreateDataModelTwoDto(
    int ExitId,
    DateOnly PeriodStartDate,
    DateTime PeriodEndDate,
    int GainAmountThree
);

public record UpdateDataModelTwoDto(
    DateOnly PeriodStartDate,
    DateTime PeriodEndDate,
    int GainAmountThree
);