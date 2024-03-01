﻿using API.Param.Enums;

namespace API.Helper
{
    public class GetStatusName
    {
        public string GetRealEstateStatusName(int status)
        {
            RealEstateEnum realEstateStatus = (RealEstateEnum)status;
            string statusName = realEstateStatus.ToString();
            return statusName;
        }

        public string GetDepositAmountStatusName(int status)
        {
            UserDepositEnum depositAmountStatus = (UserDepositEnum)status;
            string statusName = depositAmountStatus.ToString();
            return statusName;
        }

        public string GetStatusAccountName(int status)
        {
            AccountStatus AccountStatus = (AccountStatus)status;
            string statusName = AccountStatus.ToString();
            return statusName;
        }
    }
}