using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace INCHEQS.Security.User
{
    public interface IUserDao
    {
        List<UserModel> ListApprovedUser(string userId);
        List<UserModel> ListUnapprovedUser(string userId);
        void CreateInUserMaster(string userAbb);
        void DeleteInUserMasterTemp(string userAbb);
        void DeleteInUserMaster(string userAbb);
        void AddUserToUserMasterTempToDelete(string id);
        UserModel GetUser(string id);
        UserModel InsertUserDedicatedBranch(FormCollection col, string userId);
        void UpdateUserDedicatedBranch(string userId);
        void DeleteUSerDedicatedBranch(string userId);
        void DeleteDedicatedBranch(string userId);
        UserModel GetUserTemp(string userId);
        //UserModel CreateUserToUserTemp(FormCollection col, string userId, string crtUser);
        //UserModel UpdateUser(FormCollection col, string updUser);
        UserModel GetOfficerId(string userId);
        List<UserModel> ListBranch(string userId);
        List<UserModel> ListOfficer(string userId, string abb);
        List<UserModel> ListSelectedBranch(string userId, string strUser);
        List<UserModel> ListAvailableBranch(string userId);
        List<UserModel> ListCity();
        List<UserModel> ListVerificationClass();
        UserModel GetCity(string cityCode);
        UserModel GetSpickCode();
        UserModel GetSpickCode(string spikeCode);
        bool CheckUserExist(string userAbb);
        bool CheckUserExistInTempUser(string userId);
        bool CheckUserInLDAP(string userAbbr);
        string GetPasswordInLDAP(string userAbbr);
        //List<string> ValidateUser(FormCollection col, string action,string userId);
        UserModel getUserByAbbr(string strUserAbbr,string macAddress);
        void updateLastLoginDate(string userId);
        void updateLoginCounterForUser(string userAbbr, int counter, string isDisabled, bool result);
        Dictionary<string, string> GetTasksIdsForUser(string userId, string[] groupIds);
        string[] GetUserBranchCodes(string userId);
        string GetClearingDate();
        //string GetOCSClearingDate();
        
        void InsertUserPasswordHistory(FormCollection col, string strUserId);
        bool CheckUserPasswordHistory(string encryptedPassword, string action);
    }
}
