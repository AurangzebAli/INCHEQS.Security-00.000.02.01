﻿using INCHEQS.Security.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace INCHEQS.Security.Account {
    public interface ILoginAccountService {
        Task<UserModel> GetUser(string userAbbr,string macAddress);
        Task<Dictionary<string,string>> ValidateLoginAsync(UserModel userModel, string userPassword, string sessionId, string remoteAddress, string loginAD, string Domain);
        Task<AccountModel> convertUserModelToAccountModelAsync(UserModel userModel);
        Task<AccountModel> setLofFilePathAsync();
        List<UserModel> ListUserAuthMethod();
        Dictionary<string, string> ValidateLogin(UserModel user, string strPassword, string sessionId, string RemoteAddr, string selectLoginAD, string Domain);
    }
}