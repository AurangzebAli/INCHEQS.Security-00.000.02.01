using INCHEQS.Security.User;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace INCHEQS.Security.Password {
    public interface IPasswordDao {
        DataTable getUserPassword(string userId);
        List<string> Validate(string userId , string userPassword, string oldPassword, string newPassword, string confirmPassword);        
        void UpdatePwd(string userId, string passwordExpDate,  string oldPassword, string newPassword);
        void AddPwdHistory(string userId, string newPassword);
        void DeleteUserSessionTrack(string userId);
        bool pwdHistory(string userId, string newPassword);
        bool GetTotalPwdChangeTime(string userID);
        string gettotalchangepwdsequence(string userID);
    }
}