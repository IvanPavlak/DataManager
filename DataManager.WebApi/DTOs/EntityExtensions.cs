using DataManager.Core.DBModels;

namespace DataManager.WebApi.DTOs;

public static class EntityExtensions
{
    public static GetDataModelOneDtoV1 AsDtoV1(this DataModelOne dataModelOne)
    {
        return new GetDataModelOneDtoV1(
            dataModelOne.ExitId,
            dataModelOne.Port,
            dataModelOne.UserGroup,
            dataModelOne.Country,
            dataModelOne.MemberId,
            dataModelOne.Date,
            dataModelOne.GainAmountOne,
            dataModelOne.GainAmountTwo,
            dataModelOne.Loss,
            dataModelOne.Total

        );
    }

    public static GetDataModelTwoDtoV1 AsDtoV1(this DataModelTwo dataModelTwo)
    {
        return new GetDataModelTwoDtoV1(
            dataModelTwo.ExitId,
            dataModelTwo.PeriodStartDate,
            dataModelTwo.PeriodEndDate,
            dataModelTwo.GainAmountThree
        );
    }
}