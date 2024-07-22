namespace DataManager.WebApi.DTOs;

public record GetModelOneDto(
    int PageNumber = 1,
    int PageSize = 1,
    string Filter = null
);

public record GetModelOneDtoV1(
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

public record CreateModelOneDto(
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

public record UpdateModelOneDto(
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