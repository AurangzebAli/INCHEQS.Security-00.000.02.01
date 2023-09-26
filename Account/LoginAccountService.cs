//using INCHEQS.Areas.ICS.Models.SystemProfile;
//using INCHEQS.Helpers;
using INCHEQS.Security.SystemProfile;
using INCHEQS.Security.AuditTrail;
//using INCHEQS.Models.Group;
//using INCHEQS.Security.SecurityProfile;
//using INCHEQS.Models.User;
using INCHEQS.Security.UserSession;
//using INCHEQS.Resources;
//using INCHEQS.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using INCHEQS.Security.User;
using INCHEQS.Security.SecurityProfile;
using INCHEQS.Security.Group;
//using INCHEQS.ViewModels;
using INCHEQS.Security.Resources;
using INCHEQS.Common;
using System.Web;
using System.Data;
using INCHEQS.DataAccessLayer;
using System.Data.SqlClient;

namespace INCHEQS.Security.Account
{

    public class LoginAccountService : ILoginAccountService
    {
        private readonly Security.User.IUserDao userDao;
        private readonly ApplicationDbContext dbContext;
        private readonly Security.SecurityProfile.ISecurityProfileDao securityProfileDao;
        private readonly Security.Group.IGroupDao groupDao;
        private readonly IUserSessionDao userSessionDao;
        private readonly IAuditTrailDao auditTrailDao;
        private readonly ISystemProfileDao systemProfileDao;


        public LoginAccountService(IUserDao userProfileDao, ApplicationDbContext dbContext, ISecurityProfileDao securityProfileDao,
            IGroupDao groupDao, IUserSessionDao userSessionDao, IAuditTrailDao auditTrailDao, ISystemProfileDao systemProfileDao)
        {
            this.userDao = userProfileDao;
            this.dbContext = dbContext;
            this.securityProfileDao = securityProfileDao;
            this.groupDao = groupDao;
            this.userSessionDao = userSessionDao;
            this.auditTrailDao = auditTrailDao;
            this.systemProfileDao = systemProfileDao;
        }

        public async Task<UserModel> GetUser(string userAbbr, string macAddress)
        {
            UserModel userModel = await Task.Run(() => userDao.getUserByAbbr(userAbbr, macAddress));
            return userModel;
        }

        public async Task<AccountModel> convertUserModelToAccountModelAsync(UserModel userModel)
        {
            return await Task.Run(() => convertUserModelToAccountModel(userModel));
        }

        public async Task<AccountModel> setLofFilePathAsync()
        {
            return await Task.Run(() => setLofFilePath());
        }

        public async Task<Dictionary<string, string>> ValidateLoginAsync(UserModel user, string strPassword, string sessionId, string RemoteAddr, string loginAD, string Domain)
        {
            return await Task.Run(() => ValidateLogin(user, strPassword, sessionId, RemoteAddr, loginAD, Domain));
        }

        public AccountModel setLofFilePath()
        {
            AccountModel account = new AccountModel();
            account.Logindicator = ConfigureSetting.GetLogFileIndicator();//ConfigurationManager.AppSettings["LogFileIndicator"].Trim();
            account.LogPath = ConfigureSetting.GetLogFile();//.AppSettings["LogFile"].Trim(); // add by shamil 20170315 to write log file for performance test
            return account;
        }
        public List<UserModel> ListUserAuthMethod()
        {
            DataTable resultTable = new DataTable();
            List<UserModel> UserAuthMethodList = new List<UserModel>();
            List<SqlParameter> sqlParameterNext = new List<SqlParameter>();
            resultTable = dbContext.GetRecordsAsDataTableSP("spcgUserAuthMethod", sqlParameterNext.ToArray());
            if (resultTable.Rows.Count > 0)
            {
                foreach (DataRow row in resultTable.Rows)
                {
                    UserModel UserAuthMethod = new UserModel();
                    UserAuthMethod.fldUserAuthMethod = row["fldUserAuthMethod"].ToString();
                    UserAuthMethod.fldUserAuthMethodDesc = row["fldUserAuthMethodDesc"].ToString();
                    UserAuthMethodList.Add(UserAuthMethod);
                }
            }
            return UserAuthMethodList;
        }


        public AccountModel convertUserModelToAccountModel(UserModel userModel)
        {
            AccountModel account = new AccountModel();
            account.UserAbbr = userModel.fldUserAbb;
            account.UserId = userModel.fldUserId;
            account.UserType = userModel.userType;
            account.UserBankType = userModel.userBankType;
            account.VerificationClass = userModel.fldClass;
            account.VerificationLimit = userModel.fldVerificationLimit;
            account.BankDesc = userModel.fldBankDesc;
            account.BankCode = userModel.fldBankCode;
            account.SpickCode = userModel.fldSpickCode;
            account.UserEmail = userModel.fldEmail;
            account.SystemID = userModel.fldSystemID;
            account.BranchCode = userModel.fldBranchCode.ToString().Trim();
            // xx start 20210624
            account.BranchCode2 = userModel.fldBranchCode2.ToString().Trim();
            account.BranchCode3 = userModel.fldBranchCode3.ToString().Trim();
            // xx end 20210624
            account.LogPath = ConfigureSetting.GetLogFile(); //ConfigurationManager.AppSettings["LogFile"].Trim();  // add by shamil 20170315 to write log file for performance test
            account.Logindicator = ConfigureSetting.GetLogFileIndicator();//ConfigurationManager.AppSettings["LogFileIndicator"].Trim();
            account.LastLoginDate = userModel.fldLastLoginDate;
            account.PasswordExpDate = userModel.fldPasswordExpDate;
            account.LogoPath = systemProfileDao.GetLogoPath(userModel.fldBankCode);
            account.GroupIds = userModel.GroupIds;
            account.TaskIds = userDao.GetTasksIdsForUser(userModel.fldUserId, userModel.GroupIds);
            account.BranchCodes = new[] { userModel.fldBranchCode, userModel.fldBranchCode1 };// userDao.GetUserBranchCodes(userModel.fldUserId);
            account.ClearingDate = userDao.GetClearingDate();
            //account.OCSClearingDate = userDao.GetOCSClearingDate();
            account.SessionTimeOut = securityProfileDao.GetSecurityProfile().fldUserSessionTimeOut;
            account.BranchHubCodes = userModel.fldBranchHubCodes.ToArray();
            account.macAddress = userModel.macAddress;
            account.bicCode = userModel.bicCode;
            account.StateCode = userModel.fldStateCode;
            account.fldZeroMaker = userModel.fldZeroMaker;

            return account;
        }

        public Dictionary<string, string> ValidateLogin(UserModel user, string strPassword, string sessionId, string RemoteAddr, string selectLoginAD, string Domain)
        {
            //LoginViewModel lvm = new LoginViewModel();
            Dictionary<string, string> errors = new Dictionary<string, string>();
            Dictionary<string, string> errors2 = new Dictionary<string, string>();
            SecurityProfileModel securityProfile = securityProfileDao.GetSecurityProfile();
            string systemLoginAD = systemProfileDao.GetValueFromSystemProfileWithoutCurrentUser("LoginAD").Trim();
            int loginCounter = 0;
            string PwdAction = "N";
            if (user == null)
            {
                loginCounter = 0;
            }
            else
            {
                loginCounter = user.fldCounter;
            }
            //check if user exist
            if (user != null)
            {

                //User is not belong to group
                if (user.GroupIds.Length == 0)
                {
                    errors.Add("", Locale.Youhavenoanyprivilegestoenterthesystem);
                    auditTrailDao.Log("User " + user.fldUserAbb + " have no any privilege to enter the system" + " - " + Convert.ToString(RemoteAddr), convertUserModelToAccountModel(user));
                    goto LoginUpdate;
                }
                //Concurrent Connection Restrict except for admin
                if (userSessionDao.IsMultipleUserLoggedInAndConcurrentNotAllowed(user.fldUserId, securityProfile.fldUserCNCR, securityProfile.fldUserSessionTimeOut))
                {
                    if (StringUtils.LCase(user.fldUserAbb) != "admin")
                    {
                        errors.Add("", Locale.ConcurrentConnectionIsNotAllowed);
                        //errors.Add("", Locale.InvalidLogonSession);
                        auditTrailDao.Log("Not allow concurrent connection" + " - " + Convert.ToString(RemoteAddr), convertUserModelToAccountModel(user));
                        goto LoginUpdate;
                    }
                }

                //Check Disabled Login except for admin
                if (StringUtils.Trim(user.fldDisableLogin) == "Y")
                {
                    if (StringUtils.LCase(user.fldUserAbb) != "admin")
                    {
                        //errors.Add("", Locale.YourIDhadbeendisabled); 
                        errors.Add("", Locale.UserAccounthasbeenDisabled);
                        auditTrailDao.Log("User " + user.fldUserAbb + " ID had been disabled" + " - " + Convert.ToString(RemoteAddr), convertUserModelToAccountModel(user));
                        goto LoginUpdate;
                    }
                }



                //check auth method if ad or lp
                if (Domain.Equals("AD"))
                {
                    string passwordLDAP = userDao.GetPasswordInLDAP(user.fldUserAbb);
                    //Check user in LDAP
                    if (!userDao.CheckUserInLDAP(user.fldUserAbb))
                    {
                        //errors.Add("", "Invalid User");
                        if (loginCounter == securityProfile.fldUserLoginAttempt)
                        {
                            errors2.Add("", Locale.InvalidUserLoginIdorPassword);
                        }
                        else
                        {
                            errors.Add("", Locale.InvalidUserLoginIdorPassword);
                        }
                        auditTrailDao.Log("Invalid User in LDAP - User " + user.fldUserAbb + " " + " - " + Convert.ToString(RemoteAddr), convertUserModelToAccountModel(user));
                        if (StringUtils.LCase(user.fldUserAbb) != "admin")
                        {
                            //Count login attemp
                            loginCounter += 1;
                        }
                        //check if user exceeds login attempt
                        if (loginCounter > securityProfile.fldUserLoginAttempt)
                        {
                            errors.Add("", Locale.UserAccountExceededMaximumAttempts);
                            auditTrailDao.Log("Your ID had been locked" + " - " + Convert.ToString(RemoteAddr), convertUserModelToAccountModel(user));
                            goto LoginUpdate;
                        }
                        else if (StringUtils.Trim(user.fldDisableLogin) == "Y") //check if user is disabled
                        {
                            errors.Add("", Locale.UserAccounthasbeenDisabled);
                            auditTrailDao.Log("User " + user.fldUserAbb + " ID had been disabled" + " - " + Convert.ToString(RemoteAddr), convertUserModelToAccountModel(user));
                            goto LoginUpdate;
                        }

                    }
                    //validate password via ad
                    else if (StringUtils.Trim(strPassword) != StringUtils.Trim(passwordLDAP))
                    {
                        //errors.Add("", Locale.InvalidUserLoginIdorPassword);
                        if (loginCounter == securityProfile.fldUserLoginAttempt)
                        {
                            errors2.Add("", Locale.InvalidUserLoginIdorPassword);
                        }
                        else
                        {
                            errors.Add("", Locale.InvalidUserLoginIdorPassword);
                        }

                        if (StringUtils.LCase(user.fldUserAbb) != "admin")
                        {
                            //Count login attempt
                            auditTrailDao.Log("Invalid Password in LDAP - User " + user.fldUserAbb + " " + " - " + Convert.ToString(RemoteAddr), convertUserModelToAccountModel(user));
                            loginCounter += 1;

                        }
                        //check if user exceeds login attempt
                        if (loginCounter > securityProfile.fldUserLoginAttempt)
                        {
                            errors.Add("", Locale.UserAccountExceededMaximumAttempts);
                            auditTrailDao.Log("Your ID had been locked" + " - " + Convert.ToString(RemoteAddr), convertUserModelToAccountModel(user));
                            goto LoginUpdate;
                        }
                        else if (StringUtils.Trim(user.fldDisableLogin) == "Y") //check if user is disabled
                        {
                            errors.Add("", Locale.UserAccounthasbeenDisabled);
                            auditTrailDao.Log("User " + user.fldUserAbb + " ID had been disabled" + " - " + Convert.ToString(RemoteAddr), convertUserModelToAccountModel(user));
                            goto LoginUpdate;
                        }
                    }
                }
                else
                {
                    //Validate Password via local db
                    if (errors.Count == 0)
                    {
                        if (StringUtils.Trim(strPassword) != StringUtils.Trim(user.fldPassword))
                        {

                            if (loginCounter == securityProfile.fldUserLoginAttempt)
                            {
                                errors2.Add("", Locale.InvalidUserLoginIdorPassword);
                            }
                            else
                            {
                                errors.Add("", Locale.InvalidUserLoginIdorPassword);
                            }
                            if (StringUtils.LCase(user.fldUserAbb) != "admin")
                            {
                                auditTrailDao.Log("Invalid password - User " + user.fldUserAbb + " " + " - " + Convert.ToString(RemoteAddr), convertUserModelToAccountModel(user));
                                loginCounter += 1;
                            }
                            //check if user exceeds login attempt
                            if (loginCounter > securityProfile.fldUserLoginAttempt)
                            {
                                errors.Add("", Locale.UserAccountExceededMaximumAttempts);
                                auditTrailDao.Log("Your ID had been locked" + " - " + Convert.ToString(RemoteAddr), convertUserModelToAccountModel(user));
                                goto LoginUpdate;
                            }
                            else if (StringUtils.Trim(user.fldDisableLogin) == "Y")  //check if user is disabled
                            {
                                errors.Add("", Locale.UserAccounthasbeenDisabled);
                                auditTrailDao.Log("User " + user.fldUserAbb + " ID had been disabled" + " - " + Convert.ToString(RemoteAddr), convertUserModelToAccountModel(user));
                                goto LoginUpdate;
                            }


                        }
                    }
                }


                if (errors.Count == 0 || errors2.Count == 0)
                {
                    //check if first time login
                    if (string.IsNullOrEmpty(user.fldPassLastUpdDate))
                    {
                        errors.Add("PASSWORDEXPIRED", Locale.FirsttimeloginPleasechangeyourpassword);
                        //errors.Add("", Locale.InvalidLogonCredential);
                        userDao.updateLastLoginDate(user.fldUserId);
                        userSessionDao.updateSessionTrack(user.fldUserId, sessionId);
                        auditTrailDao.Log("User " + user.fldUserAbb + " First time login." + " - " + Convert.ToString(RemoteAddr), convertUserModelToAccountModel(user));
                        goto LoginUpdate;

                    }

                    //check last login date and compare to security profile
                    //Check if acc is inactive
                    if (!user.fldLastLoginDate.Equals(""))
                    {
                        if (DateTime.Now > DateUtils.AddDate(securityProfile.fldUserPwdExpiryInt, Convert.ToInt16(securityProfile.fldUserPwdExpiry), DateTime.Parse(user.fldLastLoginDate)))
                        {
                            errors.Add("", Locale.UserAccountHasBeenInactive);
                            auditTrailDao.Log("Account has been inactive. User " + user.fldUserAbb + "  cannot log in" + " - " + Convert.ToString(RemoteAddr), convertUserModelToAccountModel(user));
                            goto LoginUpdate;
                        }
                    }
                    //Check if ID is expired 
                    /*if (!user.fldIDExpDate.Equals("") && DateTime.Parse(user.fldIDExpDate) <= DateTime.Now) 
					{
                        errors.Add("", Locale.UserAccountHasBeenExpired);
                        auditTrailDao.Log("User " + user.fldUserAbb + " ID is expired" + " - " + Convert.ToString(RemoteAddr), convertUserModelToAccountModel(user));
                        goto LoginUpdate;
                    }*/
                    //check if ID will expire in less than 7 days
                    if (DateTime.Parse(user.fldPasswordExpDate) >= DateTime.Now)
                    {
                        TimeSpan ts = DateTime.Parse(user.fldPasswordExpDate).Subtract(DateTime.Now);
                        int NumberOfDays = ts.Days;
                        if (NumberOfDays <= 7)
                        {
                            errors.Add("PASSWORDEXPIRED", "Your Password will expired in " + NumberOfDays + " days. Please change your password.");
                            goto LoginUpdate;
                        }
                    }
                    //CHECK PASSWORD IF EXPIRED
                    if (DateTime.Now >= DateTime.Parse(user.fldPasswordExpDate))
                    {
                        if (securityProfile.fldUserPwdExpiry == 0)
                        {
                            errors.Add("", Locale.YourPasswordIsExpired);
                            auditTrailDao.Log("User " + user.fldUserAbb + " Password is expired" + " - " + Convert.ToString(RemoteAddr), convertUserModelToAccountModel(user));
                            goto LoginUpdate;
                        }
                        else
                        {
                            if (securityProfile.fldUserPwdExpAction == "D")
                            {
                                PwdAction = "Y";
                                userDao.updateLastLoginDate(user.fldUserId);
                                errors.Add("", Locale.UserAccounthasbeenDisabled);
                                auditTrailDao.Log("User " + user.fldUserAbb + " ID had been disabled" + " - " + Convert.ToString(RemoteAddr), convertUserModelToAccountModel(user));
                                goto LoginUpdate;
                            }
                            else
                            {
                                errors.Add("PASSWORDEXPIRED", Locale.PasswordhasbeenexpiredPleasechangeyourpassword);
                                userDao.updateLastLoginDate(user.fldUserId);
                                userSessionDao.updateSessionTrack(user.fldUserId, sessionId);
                                auditTrailDao.Log("User " + user.fldUserAbb + " Password has been expired." + " - " + Convert.ToString(RemoteAddr), convertUserModelToAccountModel(user));
                                goto LoginUpdate;
                            }
                        }
                    }


                }


            LoginUpdate:
                //Update login counter
                if (loginCounter >= (securityProfile.fldUserLoginAttempt) - 1 || StringUtils.Trim(user.fldDisableLogin) == "Y" || PwdAction == "Y")
                {
                    //exceed counter
                    loginCounter = 0;
                    userDao.updateLoginCounterForUser(user.fldUserAbb, loginCounter, "Y", false);
                }
                else
                {
                    //not exceed counter
                    userDao.updateLoginCounterForUser(user.fldUserAbb, loginCounter, "N", true);
                }


            }
            else
            {
                //errors.Add("", Locale.Userdoesnotexist);
                errors.Add("", Locale.InvalidUserLoginIdorPassword);
            }

            return errors;


        }

    }
}