using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace INCHEQS.Security.SecurityProfile
{
    public interface ISecurityProfileDao
    {
        SecurityProfileModel GetSecurityProfile();
        SecurityProfileModel GetSecurityProfileTemp();
        List<SecurityProfileModel> GetAuthSecurityProfile();
        bool CheckMaxSession(string SessionTimeOut);
        List<string> ValidateSecurity(FormCollection col);
        bool CheckExpIntervalSetting(FormCollection col);
        string GetValueFromSecurityMaster(string securityCode);
        string GetValueFromSecurityMasterTemp(string securityCode);
        void UpdateSecurityMaster(FormCollection col, string userId);
        void CreateSecurityMasterTemp(FormCollection col, string userId);
        void CreateSecurityProfileChecker(string securityProfile, string Update, string currentUser);
        void DeleteSecurityMaster();
        void MovetoSecurityMasterfromTemp();
        void DeleteSecurityProfileChecker();
        void DeleteSecurityMasterTemp();
        bool CheckSecurityProfileTemp();
        string GetValueFromSecurityProfile(string securityCode, string BankCode);
    }
}