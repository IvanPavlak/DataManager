using DataManager.Core.DBModels;

namespace DataManager.WebApi.DTOs;

public static class EntityExtensions
{
    public static GetModelOneDtoV1 AsDtoV1(this ModelOne modelOne)
    {
        return new GetModelOneDtoV1(
            modelOne.ExitId,
            modelOne.Port,
            modelOne.UserGroup,
            modelOne.Country,
            modelOne.MemberId,
            modelOne.Date,
            modelOne.GainAmountOne,
            modelOne.GainAmountTwo,
            modelOne.Loss,
            modelOne.Total

        );
    }

    public static GetModelTwoDtoV1 AsDtoV1(this ModelTwo modelTwo)
    {
        return new GetModelTwoDtoV1(
            modelTwo.ExitId,
            modelTwo.PeriodStartDate,
            modelTwo.PeriodEndDate,
            modelTwo.GainAmountThree
        );
    }
}