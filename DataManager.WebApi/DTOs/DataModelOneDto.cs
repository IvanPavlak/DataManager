namespace DataManager.WebApi.DTOs;

public record GetDataModelOneDto(
    int PageNumber = 1,
    int PageSize = 1,
    string Filter = null
);

public record GetDataModelOneDtoV1(
    int ExitId,
    string Port,
    string UserGroup,
    string Country,
    int MemberId,
    DateOnly Date,
    int GainAmountOne,
    int GainAmountTwo,
    int Loss,
    int Total
);

public record CreateDataModelOneDto(
    int ExitId,
    string Port,
    string UserGroup,
    string Country,
    int MemberId,
    DateOnly Date,
    int GainAmountOne,
    int GainAmountTwo,
    int Loss,
    int Total
);

public record UpdateDataModelOneDto(
    string Port,
    string UserGroup,
    string Country,
    int MemberId,
    DateOnly Date,
    int GainAmountOne,
    int GainAmountTwo,
    int Loss,
    int Total
);