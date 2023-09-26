using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Web.Mvc;
using System.Linq;
using INCHEQS.Security.Resources;
using INCHEQS.Security;
using INCHEQS.Common;
using INCHEQS.DataAccessLayer;
using INCHEQS.Security.SecurityProfile;
using INCHEQS.Security.SystemProfile;

public class SecurityProfileDao : ISecurityProfileDao{


    private readonly ApplicationDbContext dbContext;
    protected readonly ISystemProfileDao systemProfileDao;
    public SecurityProfileDao(ApplicationDbContext dbContext, ISystemProfileDao systemProfileDao) {
        this.dbContext = dbContext;
        this.systemProfileDao = systemProfileDao;
    }
    


    public SecurityProfileModel GetSecurityProfile()
    {
        SecurityProfileModel securityProfile = new SecurityProfileModel();

        securityProfile.fldUserAuthMethod = GetValueFromSecurityMaster("fldUserAuthMethod");
        securityProfile.fldUserIdLengthMin = StringUtils.convertToInt(StringUtils.Trim(GetValueFromSecurityMaster("fldUserIdLengthMin").ToString()));
        securityProfile.fldUserIdLengthMax = StringUtils.convertToInt(StringUtils.Trim(GetValueFromSecurityMaster("fldUserIdLengthMax").ToString()));
        securityProfile.fldUserAcctExpiry = StringUtils.convertToInt(StringUtils.Trim(GetValueFromSecurityMaster("fldUserAcctExpiry").ToString()));
        securityProfile.fldUserAcctExpiryInt = GetValueFromSecurityMaster("fldUserAcctExpiryInt");
        securityProfile.fldUserLoginAttempt = StringUtils.convertToInt(StringUtils.Trim(GetValueFromSecurityMaster("fldUserLoginAttemptMax").ToString()));
        securityProfile.fldUserCNCR = GetValueFromSecurityMaster("fldUserCNCR");
        securityProfile.fldUserSessionTimeOut = StringUtils.convertToInt(StringUtils.Trim(GetValueFromSecurityMaster("fldUserSessionTimeOut").ToString()));
        securityProfile.fldDualApproval = GetValueFromSecurityMaster("fldDualApproval");
        //AD
        securityProfile.fldUserADDomain = GetValueFromSecurityMaster("fldUserADDomain");
        //LP
        securityProfile.fldUserPwdLengthMin = StringUtils.convertToInt(StringUtils.Trim(GetValueFromSecurityMaster("fldUserPwdLengthMin").ToString()));
        securityProfile.fldUserPwdLengthMax = StringUtils.convertToInt(StringUtils.Trim(GetValueFromSecurityMaster("fldUserPwdLengthMax").ToString()));
        securityProfile.fldUserPwdHisRA = StringUtils.convertToInt(StringUtils.Trim(GetValueFromSecurityMaster("fldUserPwdHistoryMax").ToString()));
        securityProfile.fldUserPwdExpiry = StringUtils.convertToInt(StringUtils.Trim(GetValueFromSecurityMaster("fldUserPwdExpiry").ToString()));
        securityProfile.fldUserPwdExpiryInt = GetValueFromSecurityMaster("fldUserPwdExpiryInt");
        securityProfile.fldUserPwdNotification = StringUtils.convertToInt(StringUtils.Trim(GetValueFromSecurityMaster("fldUserPwdNotification").ToString()));
        securityProfile.fldUserPwdNotificationInt = GetValueFromSecurityMaster("fldUserPwdNotificationInt");
        securityProfile.fldUserPwdExpAction = GetValueFromSecurityMaster("fldUserPwdExpAction");
        securityProfile.fldPwdChangeTime = StringUtils.convertToInt(StringUtils.Trim(GetValueFromSecurityMaster("fldPwdChangeTime").ToString()));

        return securityProfile;
    }

    public SecurityProfileModel GetSecurityProfileTemp()
    {
        SecurityProfileModel securityProfileTemp = new SecurityProfileModel();

        securityProfileTemp.fldUserAuthMethod = GetValueFromSecurityMasterTemp("fldUserAuthMethod");
        securityProfileTemp.fldUserIdLengthMin = StringUtils.convertToInt(StringUtils.Trim(GetValueFromSecurityMasterTemp("fldUserIdLengthMin").ToString()));
        securityProfileTemp.fldUserIdLengthMax = StringUtils.convertToInt(StringUtils.Trim(GetValueFromSecurityMasterTemp("fldUserIdLengthMax").ToString()));
        securityProfileTemp.fldUserAcctExpiry = StringUtils.convertToInt(StringUtils.Trim(GetValueFromSecurityMasterTemp("fldUserAcctExpiry").ToString()));
        securityProfileTemp.fldUserAcctExpiryInt = GetValueFromSecurityMasterTemp("fldUserAcctExpiryInt");
        securityProfileTemp.fldUserLoginAttempt = StringUtils.convertToInt(StringUtils.Trim(GetValueFromSecurityMasterTemp("fldUserLoginAttemptMax").ToString()));
        securityProfileTemp.fldUserCNCR = GetValueFromSecurityMasterTemp("fldUserCNCR");
        securityProfileTemp.fldUserSessionTimeOut = StringUtils.convertToInt(StringUtils.Trim(GetValueFromSecurityMasterTemp("fldUserSessionTimeOut").ToString()));
        securityProfileTemp.fldDualApproval = GetValueFromSecurityMasterTemp("fldDualApproval");
        //AD
        securityProfileTemp.fldUserADDomain = GetValueFromSecurityMasterTemp("fldUserADDomain");
        //LP
        securityProfileTemp.fldUserPwdLengthMin = StringUtils.convertToInt(StringUtils.Trim(GetValueFromSecurityMasterTemp("fldUserPwdLengthMin").ToString()));
        securityProfileTemp.fldUserPwdLengthMax = StringUtils.convertToInt(StringUtils.Trim(GetValueFromSecurityMasterTemp("fldUserPwdLengthMax").ToString()));
        securityProfileTemp.fldUserPwdHisRA = StringUtils.convertToInt(StringUtils.Trim(GetValueFromSecurityMasterTemp("fldUserPwdHistoryMax").ToString()));
        securityProfileTemp.fldUserPwdExpiry = StringUtils.convertToInt(StringUtils.Trim(GetValueFromSecurityMasterTemp("fldUserPwdExpiry").ToString()));
        securityProfileTemp.fldUserPwdExpiryInt = GetValueFromSecurityMasterTemp("fldUserPwdExpiryInt");
        securityProfileTemp.fldUserPwdNotification = StringUtils.convertToInt(StringUtils.Trim(GetValueFromSecurityMasterTemp("fldUserPwdNotification").ToString()));
        securityProfileTemp.fldUserPwdNotificationInt = GetValueFromSecurityMasterTemp("fldUserPwdNotificationInt");
        securityProfileTemp.fldUserPwdExpAction = GetValueFromSecurityMasterTemp("fldUserPwdExpAction");
        securityProfileTemp.fldPwdChangeTime = StringUtils.convertToInt(StringUtils.Trim(GetValueFromSecurityMasterTemp("fldPwdChangeTime").ToString()));

        return securityProfileTemp;
    }

    public List<SecurityProfileModel> GetAuthSecurityProfile()
    {
        DataTable resultTable = new DataTable();
        List<SecurityProfileModel> AuthSecurityProfileList = new List<SecurityProfileModel>();
        List<SqlParameter> sqlParameterNext = new List<SqlParameter>();

        resultTable = dbContext.GetRecordsAsDataTableSP("spcgUserAuthMethodProfile", sqlParameterNext.ToArray());
        if (resultTable.Rows.Count > 0)
        {
            foreach (DataRow row in resultTable.Rows)
            {
                SecurityProfileModel AuthSecurityProfile = new SecurityProfileModel();
                AuthSecurityProfile.fldUserAuthMethod = row["fldUserAuthMethod"].ToString();
                AuthSecurityProfile.fldUserAuthMethodDesc = row["fldUserAuthMethodDesc"].ToString();
                AuthSecurityProfileList.Add(AuthSecurityProfile);
            }
        }
        return AuthSecurityProfileList;
    }

    public bool CheckMaxSession(string SessionTimeOut)
    {
        bool status = false;
        //Get MaxSessionTimeOut setting from tblsystemprofile
        string maxtimeOut = systemProfileDao.GetValueFromSystemProfileWithoutCurrentUser("MaxSessionTimeOut");
        if (Convert.ToInt32(maxtimeOut) < Convert.ToInt32(SessionTimeOut))
        {
            status = true;
        }
        else
        {
            status = false;
        }
        return status;
    }

    public List<string> ValidateSecurity(FormCollection col)
    {
        List<string> err = new List<string>();
        int result;
        string x = col["fldUserAuthMethod"];
        
        if (x == "AD")
        {
            if (col["fldUserIdLengthMin"].Equals(""))
            {
                err.Add("User Id min cannot be blank");
            }
            else if (col["fldUserIdLengthMin"].Equals("0"))
            {
                err.Add("User Id min cannot be zero");
            }
            else if (!int.TryParse(col["fldUserIdLengthMin"], out result))
            {
                err.Add("User Id min must be numeric");
            }
            else if (col["fldUserIdLengthMax"].Equals(""))
            {
                err.Add("User Id max cannot be blank");
            }
            else if (col["fldUserIdLengthMax"].Equals("0"))
            {
                err.Add("User Id max cannot be zero");
            }
            else if (!int.TryParse(col["fldUserIdLengthMax"], out result))
            {
                err.Add("User Id max must be numeric");
            }
            else if (int.Parse(col["fldUserIdLengthMin"]) >= int.Parse(col["fldUserIdLengthMax"]))
            {
                err.Add("User Id min cannot more than User Id max");
            }
            else if (col["fldUserAcctExpiry"].Equals(""))
            {
                err.Add("User account expiry cannot be blank");
            }
            else if (col["fldUserAcctExpiry"].Equals("0"))
            {
                err.Add("User account expiry cannot be zero");
            }
            else if (!int.TryParse(col["fldUserAcctExpiry"], out result))
            {
                err.Add("User account expiry must be numeric");
            }
            else if (col["fldUserLoginAttemptMax"].Equals(""))
            {
                err.Add("User maximum login attempt cannot be blank");
            }
            else if (col["fldUserLoginAttemptMax"].Equals("0"))
            {
                err.Add("User maximum login attempt cannot be zero");
            }
            else if (!int.TryParse(col["fldUserLoginAttemptMax"], out result))
            {
                err.Add("User maximum login attempt must be numeric");
            }
            else if (col["fldUserSessionTimeOut"].Equals(""))
            {
                err.Add("User session timeout cannot be blank");
            }
            else if (col["fldUserSessionTimeOut"].Equals("0"))
            {
                err.Add("User session timeout cannot be zero");
            }
            else if (!int.TryParse(col["fldUserSessionTimeOut"], out result))
            {
                err.Add("User session timeout must be numeric");
            }
            else if (CheckMaxSession(col["fldUserSessionTimeOut"]) == true)
            {
                err.Add("User session timeout cannot be more than timeout setting");
            }
        }
        else if (x == "LP")
        {
            if (col["fldUserIdLengthMin"].Equals(""))
            {
                err.Add("User Id min cannot be blank");
            }
            else if (col["fldUserIdLengthMin"].Equals("0"))
            {
                err.Add("User Id min cannot be zero");
            }
            else if (!int.TryParse(col["fldUserIdLengthMin"], out result))
            {
                err.Add("User Id min must be numeric");
            }
            else if (col["fldUserIdLengthMax"].Equals(""))
            {
                err.Add("User Id max cannot be blank");
            }
            else if (col["fldUserIdLengthMax"].Equals("0"))
            {
                err.Add("User Id max cannot be zero");
            }
            else if (!int.TryParse(col["fldUserIdLengthMax"], out result))
            {
                err.Add("User Id max must be numeric");
            }
            else if (int.Parse(col["fldUserIdLengthMin"]) >= int.Parse(col["fldUserIdLengthMax"]))
            {
                err.Add("User Id min cannot more than User Id max");
            }
            else if (col["fldUserAcctExpiry"].Equals(""))
            {
                err.Add("User account expiry cannot be blank");
            }
            else if (col["fldUserAcctExpiry"].Equals("0"))
            {
                err.Add("User account expiry cannot be zero");
            }
            else if (!int.TryParse(col["fldUserAcctExpiry"], out result))
            {
                err.Add("User account expiry must be numeric");
            }
            else if (col["fldUserLoginAttemptMax"].Equals(""))
            {
                err.Add("User maximum login attempt cannot be blank");
            }
            else if (col["fldUserLoginAttemptMax"].Equals("0"))
            {
                err.Add("User maximum login attempt cannot be zero");
            }
            else if (!int.TryParse(col["fldUserLoginAttemptMax"], out result))
            {
                err.Add("User maximum login attempt must be numeric");
            }
            else if (col["fldUserSessionTimeOut"].Equals(""))
            {
                err.Add("User session timeout cannot be blank");
            }
            else if (col["fldUserSessionTimeOut"].Equals("0"))
            {
                err.Add("User session timeout cannot be zero");
            }
            else if (!int.TryParse(col["fldUserSessionTimeOut"], out result))
            {
                err.Add("User session timeout must be numeric");
            }
            else if (CheckMaxSession(col["fldUserSessionTimeOut"]) == true)
            {
                err.Add("User session timeout cannot be more than timeout setting");
            }
            else if (col["fldUserPwdLengthMin"].Equals(""))
            {
                err.Add("User password length min cannot be blank");
            }
            else if (col["fldUserPwdLengthMin"].Equals("0"))
            {
                err.Add("User password length min cannot be zero");
            }
            else if (!int.TryParse(col["fldUserPwdLengthMin"], out result))
            {
                err.Add("User password length min must be numeric");
            }
            else if (col["fldUserPwdLengthMax"].Equals(""))
            {
                err.Add("User password length max cannot be blank");
            }
            else if (col["fldUserPwdLengthMax"].Equals("0"))
            {
                err.Add("User password length max cannot be zero");
            }
            else if (!int.TryParse(col["fldUserPwdLengthMax"], out result))
            {
                err.Add("User password length max must be numeric");
            }
            else if (int.Parse(col["fldUserPwdLengthMin"]) >= int.Parse(col["fldUserPwdLengthMax"]))
            {
                err.Add("User password min cannot more than user password max");
            }
            else if (col["fldUserPwdHistoryMax"].Equals(""))
            {
                err.Add("Password History Reusable after cannot be blank");
            }
            else if (col["fldUserPwdExpiry"].Equals(""))
            {
                err.Add("User password expiry cannot be blank");
            }
            else if (col["fldUserPwdExpiry"].Equals("0"))
            {
                err.Add("User password expiry cannot be zero");
            }
            else if (!int.TryParse(col["fldUserPwdExpiry"], out result))
            {
                err.Add("User password expiry must be numeric");
            }
            else if (col["fldUserPwdNotification"].Equals(""))
            {
                err.Add("User password notification cannot be blank");
            }
            else if (col["fldUserPwdNotification"].Equals("0"))
            {
                err.Add("User password notification cannot be zero");
            }
            else if (!int.TryParse(col["fldUserPwdNotification"], out result))
            {
                err.Add("User password notification must be numeric");
            }
            else if (CheckExpIntervalSetting(col) == false)
            {
                err.Add("User password Expiry Interval must be longer than User Password Notification Interval");
            }
            else if (col["fldPwdChangeTime"].Equals(""))
            {
                err.Add("User Password Change Sequence cannot be blank");
            }
            else if (col["fldPwdChangeTime"].Equals("0"))
            {
                err.Add("User Password Change Sequence cannot be zero");
            }
            else if (!int.TryParse(col["fldPwdChangeTime"], out result))
            {
                err.Add("User Password Change Sequence must be numeric");
            }
        }

        return err;
    }

    public bool CheckExpIntervalSetting(FormCollection col)
    {
        bool status = false;
        dynamic fldUserPwdExpiry = null;
        dynamic fldUserPwdExpiryInt = null;
        dynamic fldUserPwdNotification = null;
        dynamic fldUserPwdNotificationInt = null;

        fldUserPwdExpiry = Convert.ToDouble(col["fldUserPwdExpiry"]);
        fldUserPwdExpiryInt = col["fldUserPwdExpiryInt"];

        fldUserPwdNotification = Convert.ToDouble(col["fldUserPwdNotification"]);
        fldUserPwdNotificationInt = col["fldUserPwdNotificationInt"];

        if (fldUserPwdExpiryInt == "Y")
        {
            fldUserPwdExpiry = fldUserPwdExpiry * 365;
        }
        else if (fldUserPwdExpiryInt == "M")
        {
            fldUserPwdExpiry = fldUserPwdExpiry * 30;
        }

        if (fldUserPwdNotificationInt == "Y")
        {
            fldUserPwdNotification = fldUserPwdNotification * 365;
        }
        else if (fldUserPwdNotificationInt == "M")
        {
            fldUserPwdNotification = fldUserPwdNotification * 30;
        }

        if (fldUserPwdExpiry >= fldUserPwdNotification)
        {
            status = true;
        }
        else
        {
            status = false;
        }
        return status;
    }

    public string GetValueFromSecurityMaster(string securityCode)
    {
        string result = "";
        try
        {

            DataTable dataResult = new DataTable();
            List<SqlParameter> sqlParameterNext = new List<SqlParameter>();
            sqlParameterNext.Add(new SqlParameter("@fldBankCode", ""));
            sqlParameterNext.Add(new SqlParameter("@fldSecurityCode", securityCode));
            sqlParameterNext.Add(new SqlParameter("@Action", "Master"));
            dataResult = dbContext.GetRecordsAsDataTableSP("spcgValueFromSecurityMaster", sqlParameterNext.ToArray());

            if (dataResult.Rows.Count > 0)
            {
                result = dataResult.Rows[0]["fldSecurityValue"].ToString();
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return result;
    }

    public string GetValueFromSecurityMasterTemp(string securityCode)
    {
        string result = "";
        try
        {

            DataTable dataResult = new DataTable();
            List<SqlParameter> sqlParameterNext = new List<SqlParameter>();
            sqlParameterNext.Add(new SqlParameter("@fldSecurityCode", securityCode));
            dataResult = dbContext.GetRecordsAsDataTableSP("spcgValueFromSecurityMasterTemp", sqlParameterNext.ToArray());

            if (dataResult.Rows.Count > 0)
            {
                result = dataResult.Rows[0]["fldSecurityValue"].ToString();
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return result;
    }

    public void UpdateSecurityMaster(FormCollection col, string userId)
    {
        string x = col["fldUserAuthMethod"];

        if (x == "AD")
        {
            List<SqlParameter> sqlParameterNext = new List<SqlParameter>();

            sqlParameterNext.Add(new SqlParameter("@fldUserAuthMethod", col["fldUserAuthMethod"]));
            sqlParameterNext.Add(new SqlParameter("@fldUserIdLengthMin", col["fldUserIdLengthMin"]));
            sqlParameterNext.Add(new SqlParameter("@fldUserIdLengthMax", col["fldUserIdLengthMax"]));
            sqlParameterNext.Add(new SqlParameter("@fldUserAcctExpiry", col["fldUserAcctExpiry"]));
            sqlParameterNext.Add(new SqlParameter("@fldUserAcctExpiryInt", col["fldUserAcctExpiryInt"]));
            sqlParameterNext.Add(new SqlParameter("@fldUserLoginAttemptMax", col["fldUserLoginAttemptMax"]));
            sqlParameterNext.Add(new SqlParameter("@fldUserCNCR", col["fldUserCNCR"]));
            sqlParameterNext.Add(new SqlParameter("@fldUserSessionTimeout", col["fldUserSessionTimeout"]));
            sqlParameterNext.Add(new SqlParameter("@fldDualApproval", col["fldDualApproval"]));
            sqlParameterNext.Add(new SqlParameter("@fldUserADDomain", col["fldUserADDomain"]));
            sqlParameterNext.Add(new SqlParameter("@fldUserPwdLengthMin", ""));
            sqlParameterNext.Add(new SqlParameter("@fldUserPwdLengthMax", ""));
            sqlParameterNext.Add(new SqlParameter("@fldUserPwdHistoryMax", ""));
            sqlParameterNext.Add(new SqlParameter("@fldUserPwdExpiry", ""));
            sqlParameterNext.Add(new SqlParameter("@fldUserPwdExpiryInt", ""));
            sqlParameterNext.Add(new SqlParameter("@fldUserPwdNotification", ""));
            sqlParameterNext.Add(new SqlParameter("@fldUserPwdNotificationInt",""));
            sqlParameterNext.Add(new SqlParameter("@fldUserPwdExpAction",""));
            sqlParameterNext.Add(new SqlParameter("@fldPwdChangeTime", col["fldPwdChangeTime"]));

            this.dbContext.ExecuteNonQuery(CommandType.StoredProcedure, "spcuSecurityMaster", sqlParameterNext.ToArray());
        }
        else
        {
            List<SqlParameter> sqlParameterNext = new List<SqlParameter>();
            sqlParameterNext.Add(new SqlParameter("@fldUserAuthMethod", col["fldUserAuthMethod"]));
            sqlParameterNext.Add(new SqlParameter("@fldUserIdLengthMin", col["fldUserIdLengthMin"]));
            sqlParameterNext.Add(new SqlParameter("@fldUserIdLengthMax", col["fldUserIdLengthMax"]));
            sqlParameterNext.Add(new SqlParameter("@fldUserAcctExpiry", col["fldUserAcctExpiry"]));
            sqlParameterNext.Add(new SqlParameter("@fldUserAcctExpiryInt", col["fldUserAcctExpiryInt"]));
            sqlParameterNext.Add(new SqlParameter("@fldUserLoginAttemptMax", col["fldUserLoginAttemptMax"]));
            sqlParameterNext.Add(new SqlParameter("@fldUserCNCR", col["fldUserCNCR"]));
            sqlParameterNext.Add(new SqlParameter("@fldUserSessionTimeout", col["fldUserSessionTimeout"]));
            sqlParameterNext.Add(new SqlParameter("@fldDualApproval", col["fldDualApproval"]));
            sqlParameterNext.Add(new SqlParameter("@fldUserADDomain", ""));
            sqlParameterNext.Add(new SqlParameter("@fldUserPwdLengthMin", col["fldUserPwdLengthMin"]));
            sqlParameterNext.Add(new SqlParameter("@fldUserPwdLengthMax", col["fldUserPwdLengthMax"]));
            sqlParameterNext.Add(new SqlParameter("@fldUserPwdHistoryMax", col["fldUserPwdHistoryMax"]));
            sqlParameterNext.Add(new SqlParameter("@fldUserPwdExpiry", col["fldUserPwdExpiry"]));
            sqlParameterNext.Add(new SqlParameter("@fldUserPwdExpiryInt", col["fldUserPwdExpiryInt"]));
            sqlParameterNext.Add(new SqlParameter("@fldUserPwdNotification", col["fldUserPwdNotification"]));
            sqlParameterNext.Add(new SqlParameter("@fldUserPwdNotificationInt", col["fldUserPwdNotificationInt"]));
            sqlParameterNext.Add(new SqlParameter("@fldUserPwdExpAction", col["fldUserPwdExpAction"]));
            sqlParameterNext.Add(new SqlParameter("@fldPwdChangeTime", col["fldPwdChangeTime"]));

            this.dbContext.ExecuteNonQuery(CommandType.StoredProcedure, "spcuSecurityMaster", sqlParameterNext.ToArray());
        }
    }

    public void CreateSecurityMasterTemp(FormCollection col, string userId)
    {
        string x = col["fldUserAuthMethod"];

        if (x == "LP")
        {
            List<SqlParameter> sqlParameterNext = new List<SqlParameter>();

            sqlParameterNext.Add(new SqlParameter("@fldUserAuthMethod", col["fldUserAuthMethod"]));
            sqlParameterNext.Add(new SqlParameter("@fldUserIdLengthMin", col["fldUserIdLengthMin"]));
            sqlParameterNext.Add(new SqlParameter("@fldUserIdLengthMax", col["fldUserIdLengthMax"]));
            sqlParameterNext.Add(new SqlParameter("@fldUserAcctExpiry", col["fldUserAcctExpiry"]));
            sqlParameterNext.Add(new SqlParameter("@fldUserAcctExpiryInt", col["fldUserAcctExpiryInt"]));
            sqlParameterNext.Add(new SqlParameter("@fldUserLoginAttemptMax", col["fldUserLoginAttemptMax"]));
            sqlParameterNext.Add(new SqlParameter("@fldUserCNCR", col["fldUserCNCR"]));
            sqlParameterNext.Add(new SqlParameter("@fldUserSessionTimeout", col["fldUserSessionTimeout"]));
            sqlParameterNext.Add(new SqlParameter("@fldDualApproval", col["fldDualApproval"]));
            sqlParameterNext.Add(new SqlParameter("@fldUserADDomain", ""));
            sqlParameterNext.Add(new SqlParameter("@fldUserPwdLengthMin", col["fldUserPwdLengthMin"]));
            sqlParameterNext.Add(new SqlParameter("@fldUserPwdLengthMax", col["fldUserPwdLengthMax"]));
            sqlParameterNext.Add(new SqlParameter("@fldUserPwdHistoryMax", col["fldUserPwdHistoryMax"]));
            sqlParameterNext.Add(new SqlParameter("@fldUserPwdExpiry", col["fldUserPwdExpiry"]));
            sqlParameterNext.Add(new SqlParameter("@fldUserPwdExpiryInt", col["fldUserPwdExpiryInt"]));
            sqlParameterNext.Add(new SqlParameter("@fldUserPwdNotification", col["fldUserPwdNotification"]));
            sqlParameterNext.Add(new SqlParameter("@fldUserPwdNotificationInt", col["fldUserPwdNotificationInt"]));
            sqlParameterNext.Add(new SqlParameter("@fldUserPwdExpAction", col["fldUserPwdExpAction"]));
            sqlParameterNext.Add(new SqlParameter("@fldPwdChangeTime", col["fldPwdChangeTime"]));
            sqlParameterNext.Add(new SqlParameter("@fldUserId", userId));

            this.dbContext.ExecuteNonQuery(CommandType.StoredProcedure, "spciSecurityMasterTemp", sqlParameterNext.ToArray());
        }
        else if (x == "AD")
        {
            List<SqlParameter> sqlParameterNext = new List<SqlParameter>();

            sqlParameterNext.Add(new SqlParameter("@fldUserAuthMethod", col["fldUserAuthMethod"]));
            sqlParameterNext.Add(new SqlParameter("@fldUserIdLengthMin", col["fldUserIdLengthMin"]));
            sqlParameterNext.Add(new SqlParameter("@fldUserIdLengthMax", col["fldUserIdLengthMax"]));
            sqlParameterNext.Add(new SqlParameter("@fldUserAcctExpiry", col["fldUserAcctExpiry"]));
            sqlParameterNext.Add(new SqlParameter("@fldUserAcctExpiryInt", col["fldUserAcctExpiryInt"]));
            sqlParameterNext.Add(new SqlParameter("@fldUserLoginAttemptMax", col["fldUserLoginAttemptMax"]));
            sqlParameterNext.Add(new SqlParameter("@fldUserCNCR", col["fldUserCNCR"]));
            sqlParameterNext.Add(new SqlParameter("@fldUserSessionTimeout", col["fldUserSessionTimeout"]));
            sqlParameterNext.Add(new SqlParameter("@fldDualApproval", col["fldDualApproval"]));
            sqlParameterNext.Add(new SqlParameter("@fldUserADDomain", col["fldUserADDomain"]));
            sqlParameterNext.Add(new SqlParameter("@fldUserPwdLengthMin", ""));
            sqlParameterNext.Add(new SqlParameter("@fldUserPwdLengthMax", ""));
            sqlParameterNext.Add(new SqlParameter("@fldUserPwdHistoryMax", ""));
            sqlParameterNext.Add(new SqlParameter("@fldUserPwdExpiry", ""));
            sqlParameterNext.Add(new SqlParameter("@fldUserPwdExpiryInt", ""));
            sqlParameterNext.Add(new SqlParameter("@fldUserPwdNotification", ""));
            sqlParameterNext.Add(new SqlParameter("@fldUserPwdNotificationInt", ""));
            sqlParameterNext.Add(new SqlParameter("@fldUserPwdExpAction", ""));
            sqlParameterNext.Add(new SqlParameter("@fldPwdChangeTime", col["fldPwdChangeTime"]));
            sqlParameterNext.Add(new SqlParameter("@fldUserId", userId));

            this.dbContext.ExecuteNonQuery(CommandType.StoredProcedure, "spciSecurityMasterTemp", sqlParameterNext.ToArray());
        }

    }

    public void CreateSecurityProfileChecker(string securityProfile, string Update,string currentUser)
    {
        List<SqlParameter> sqlParameterNext = new List<SqlParameter>();
        sqlParameterNext.Add(new SqlParameter("@fldTaskModule", securityProfile));
        sqlParameterNext.Add(new SqlParameter("@fldStatus", Update));
        sqlParameterNext.Add(new SqlParameter("@fldCreateTimeStamp", (object)DateTime.Now));
        sqlParameterNext.Add(new SqlParameter("@fldCreateUserId", currentUser));
        sqlParameterNext.Add(new SqlParameter("@fldUpdateTimeStamp", (object)DateTime.Now));
        sqlParameterNext.Add(new SqlParameter("@fldUpdateUserId", currentUser));

        this.dbContext.ExecuteNonQuery(CommandType.StoredProcedure, "spciSecurityProfileChecker", sqlParameterNext.ToArray());
    }

    public void DeleteSecurityMaster()
    {
        List<SqlParameter> sqlParameterNext = new List<SqlParameter>();
        try
        {
            this.dbContext.ExecuteNonQuery(CommandType.StoredProcedure, "spcdSecurityMaster", sqlParameterNext.ToArray());
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public void MovetoSecurityMasterfromTemp()
    {
        List<SqlParameter> sqlParameterNext = new List<SqlParameter>();
        try
        {
            this.dbContext.ExecuteNonQuery(CommandType.StoredProcedure, "spciSecurityMaster", sqlParameterNext.ToArray());
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public void DeleteSecurityProfileChecker()
    {
        List<SqlParameter> sqlParameterNext = new List<SqlParameter>();
        try
        {
            this.dbContext.ExecuteNonQuery(CommandType.StoredProcedure, "spcdSecurityProfileChecker", sqlParameterNext.ToArray());
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public void DeleteSecurityMasterTemp()
    {
        List<SqlParameter> sqlParameterNext = new List<SqlParameter>();
        try
        {
            this.dbContext.ExecuteNonQuery(CommandType.StoredProcedure, "spcdSecurityMasterTemp", sqlParameterNext.ToArray());
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public bool CheckSecurityProfileTemp()
    {
        bool strs = false;
        DataTable dataTable = new DataTable();
        List<SqlParameter> sqlParameterNext = new List<SqlParameter>();
        
        dataTable = this.dbContext.GetRecordsAsDataTableSP("spcgSecurityProfileTemp", sqlParameterNext.ToArray());

        if (dataTable.Rows.Count > 0)
        {
            strs = true;


        }
        else
        {
            strs = false;

        }

        return strs;
    }

    public string GetValueFromSecurityProfile(string securityCode, string BankCode)
    {
        string result = "";
        try
        {

            DataTable dataResult = new DataTable();
            List<SqlParameter> sqlParameterNext = new List<SqlParameter>();
            sqlParameterNext.Add(new SqlParameter("@fldBankCode", BankCode));
            sqlParameterNext.Add(new SqlParameter("@fldSecurityCode", securityCode));
            sqlParameterNext.Add(new SqlParameter("@Action", "SecurityProfile"));
            dataResult = dbContext.GetRecordsAsDataTableSP("spcgValueFromSecurityMaster", sqlParameterNext.ToArray());

            if (dataResult.Rows.Count > 0)
            {
                result = dataResult.Rows[0]["fldSecurityValue"].ToString();
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return result;
    }


}
