using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
//using INCHEQS.Helpers;
//using INCHEQS.Security;
using INCHEQS.Security.Resources;
using System.Text.RegularExpressions;
using INCHEQS.Security.User;
using INCHEQS.Security.Password;
using INCHEQS.Security;
using INCHEQS.Security.SecurityProfile;
using INCHEQS.DataAccessLayer;
using System;
using INCHEQS.Security.SystemProfile;
using INCHEQS.Common;
//using INCHEQS.DataAccessLayer;
//using INCHEQS.Security.User;
//using INCHEQS.Security.SystemProfile;
//using INCHEQS.Security.SecurityProfile;

public class PasswordDao : IPasswordDao {//get current user pass


    private readonly ApplicationDbContext dbContext;
    private readonly IUserDao userDao;
    private readonly ISecurityProfileDao securityProfileDao;
    private readonly ISystemProfileDao systemProfileDao;

    public PasswordDao(ApplicationDbContext dbContext, ISecurityProfileDao securityProfileDao, IUserDao userDao, ISystemProfileDao systemProfileDao) {
        this.dbContext = dbContext;
        this.userDao = userDao;
        this.securityProfileDao = securityProfileDao;
        this.systemProfileDao = systemProfileDao;
    }

    public DataTable getUserPassword(string userId) {
        DataTable ds = new DataTable();
        //string stmt = "SELECT fldPassword, fldUserAbb FROM tblUserMaster WHERE fldUserId = @userid";

        //ds = dbContext.GetRecordsAsDataTable( stmt, new[] { new SqlParameter("@userid", userId) });
        List<SqlParameter> sqlParameters = new List<SqlParameter>();

        sqlParameters.Add(new SqlParameter("@userid", userId));
        ds = dbContext.GetRecordsAsDataTableSP("spcgGetUserPassword", sqlParameters.ToArray());

        return ds;
    }

    public List<string> Validate(string userId , string userPassword,  string oldPassword , string newPassword, string confirmPassword) {
        SecurityProfileModel securityProfile = securityProfileDao.GetSecurityProfile();
        List<string> err = new List<string>();

        if ((oldPassword.Equals("") && newPassword.Equals("") && confirmPassword.Equals("")))
        {
            err.Add(Locale.Pleasefillinallthemandatoryfield);
        }
        else if ((oldPassword.Equals("")))
        {
            err.Add(Locale.PleaseenterOldPassword);
        }
        else if ((newPassword.Equals("")))
        {
            err.Add(Locale.PleaseenterNewPassword);
        }
        else if ((confirmPassword.Equals("")))
        {
            err.Add(Locale.PleaseenterConfirmNewPassword);
        }
        else if (oldPassword.ToString() != userPassword)
        {
            err.Add(Locale.WrongOldPassword);
        }
        else if ((oldPassword.Length < securityProfile.fldUserPwdLengthMin))
        {
            err.Add(Locale.ÓldPasswordmustcontainatleast + securityProfile.fldUserPwdLengthMin + Locale.character);
        }
        else if ((oldPassword.Length > securityProfile.fldUserPwdLengthMax))
        {
            err.Add(Locale.OldPasswordshouldnotexceed + securityProfile.fldUserPwdLengthMax + Locale.character);
        }
        else if ((newPassword.Length < securityProfile.fldUserPwdLengthMin))
        {
            err.Add(Locale.NewPasswordmustcontainatleast + securityProfile.fldUserPwdLengthMin + Locale.character);
        }
        else if ((newPassword.Length > securityProfile.fldUserPwdLengthMax))
        {
            err.Add(Locale.NewPasswordshouldnotexceed + securityProfile.fldUserPwdLengthMax + Locale.character);
        }
        else if ((confirmPassword.Length < securityProfile.fldUserPwdLengthMin))
        {
            err.Add(Locale.ConfirmnewPasswordmustcontainatleast + securityProfile.fldUserPwdLengthMin + Locale.character);
        }
        else if ((confirmPassword.Length > securityProfile.fldUserPwdLengthMax))
        {
            err.Add(Locale.ConfirmnewPasswordshouldnotexceed + securityProfile.fldUserPwdLengthMax + Locale.character);
        }
        else if ((newPassword.Equals(oldPassword)))
        {
            err.Add(Locale.NewPasswordmustbedifferentfromoldpassword);
        }
        //Modified 20220616
        //Not allowing space
        else if (!(Regex.IsMatch(newPassword, @"^[^ ]*([a-zA-Z\S*])+((\S*[a-zA-Z]+\S*\d+\S*)|(\S*\d+\S*[a-zA-Z]+\S*))$")))
        {
            err.Add(Locale.Passwordmustbealphanumeric);
        }//Check if Alphanumeric
        else if (!(Regex.IsMatch(newPassword, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*\W).*$")))
        {
            err.Add(Locale.Passwordmustbealphanumeric);
        }//End of Modified
        else if (!(newPassword.Equals(confirmPassword)))
        {
            err.Add(Locale.NewpasswordAndConfirmPasswordDoNotmatchPleasereenterNewpassword);
            //} else if ((char.IsLower(newPassword, 0))) {
            //    err.Add(Locale.ThefirstcharacterofnewpasswordmustbeaCapitalLetter);
        }
        else if ((pwdHistory(userId, newPassword)))
        {
           // err.Add(Locale.Newpasswordcannotbesameasprevious + " " + securityProfile.fldPwdHisListRA + " " + Locale.passwordsinhistory);
        }
        return err;
    }

    public bool isOldPasswordSameAsNew(string oldPassword , string newPassword) {

        bool result = true;
        //Check Password if changed then update password and password update date
        if (!(oldPassword.Equals(newPassword))) {
            result = false;
        }
        return result;
    }

    public void UpdatePwd(string userId , string passwordExpiryDate,  string oldPassword, string newPassword) {
        SecurityProfileModel securityProfile = securityProfileDao.GetSecurityProfile();

        //Construct Sql Statement
        //string stmt = " Update tblUserMaster Set fldPassLastUpdDate= getdate() , fldPasswordExpDate = @passwordExpiredDate"; --old
        //string stmt = " Update tblUserMaster Set fldPassLastUpdDate= getdate()";//by fadzuan
        int stmt = 0;

        if (!isOldPasswordSameAsNew(oldPassword, newPassword)) {
            //stmt = stmt + " , fldPassword =@fldPassword";
            stmt = 1;
        }
        //stmt = stmt + " WHERE fldUserId=@fldUserId";
        string encryptedPassword = getEncryptPassword(newPassword);

        //Construct Parameters
        List<SqlParameter> sqlParameters = new List<SqlParameter>();
        sqlParameters.Add(new SqlParameter("@stmt", stmt));
        sqlParameters.Add(new SqlParameter("@fldUserId", userId));
        //sqlParameters.Add(new SqlParameter("@passwordExpiredDate", userId));
        sqlParameters.Add(new SqlParameter("@passwordExpiredDate", passwordExpiryDate));
        sqlParameters.Add(new SqlParameter("@fldPassword", encryptedPassword.ToString().Trim()));

        //dbContext.ExecuteNonQuery( stmt, sqlParameters.ToArray());
        dbContext.ExecuteNonQuery(CommandType.StoredProcedure, "spcuUserPassword", sqlParameters.ToArray());
    }

    public void AddPwdHistory(string userId , string newPassword) {

        string pwdHistory = getEncryptPassword(newPassword);
        //string stmt = "insert into tblUserPwdHistory(fldUserId, fldPassword, fldLastUpdateDate) values(@id, @password, getdate())";

        //dbContext.ExecuteNonQuery( stmt, new[] {
        //                new SqlParameter("@id",userId ),
        //                new SqlParameter("@password", pwdHistory.ToString())
        //            });
        List<SqlParameter> sqlParameters = new List<SqlParameter>();
        sqlParameters.Add(new SqlParameter("@id", userId));
        sqlParameters.Add(new SqlParameter("@password", pwdHistory.ToString()));
        
        dbContext.ExecuteNonQuery(CommandType.StoredProcedure, "spciUserPasswordHistory", sqlParameters.ToArray());
    }

    public void DeleteUserSessionTrack(string userId)
    {

        //string pwdHistory = getEncryptPassword(newPassword);
        //string stmt = "Delete from tblUserSessionTrack where fldUserId=@id";

        //dbContext.ExecuteNonQuery(stmt, new[] {
        //                new SqlParameter("@id",userId ),
                 
        //            });

        List<SqlParameter> sqlParameters = new List<SqlParameter>();
        sqlParameters.Add(new SqlParameter("@id", userId));

        dbContext.ExecuteNonQuery(CommandType.StoredProcedure, "spcdUserSessionTrack", sqlParameters.ToArray());
    }

    public bool pwdHistory(string userId,string newPassword) {

        string encryptedNewPassword = getEncryptPassword(newPassword);
        SecurityProfileModel securityProfile = securityProfileDao.GetSecurityProfile();
        bool result = false;
        DataTable ds = new DataTable();
        List<SqlParameter> sqlParameters = new List<SqlParameter>();
        sqlParameters.Add(new SqlParameter("@id", userId));
        sqlParameters.Add(new SqlParameter("@fldPassword", securityProfile.fldUserPwdHisRA));
        ds = dbContext.GetRecordsAsDataTableSP("spcgGetUserPasswordHistory", sqlParameters.ToArray());

        foreach (DataRow row in ds.Rows) {
            if (encryptedNewPassword == row["fldPassword"].ToString()) {
                result = true;
            }
        }
        return result;
    }


    public string getDecryptPassword(string encryptedPassword) {
        string result = "";
        try {
            ICSSecurity.EncryptDecrypt encryptDecrypt = new ICSSecurity.EncryptDecrypt();
            encryptDecrypt.FilePath = systemProfileDao.GetDLLPath();
            result = encryptDecrypt.DecryptString128Bit(encryptedPassword);
        } catch (Exception ex) {
            throw ex;
        }
        return result;
    }

    public string getEncryptPassword(string stringPassword) {
        string result = "";
        try {
            ICSSecurity.EncryptDecrypt encryptDecrypt = new ICSSecurity.EncryptDecrypt();
            encryptDecrypt.FilePath = systemProfileDao.GetDLLPath();
            result = encryptDecrypt.EncryptString128Bit(stringPassword);
        } catch (Exception ex) {
            throw ex;
        }
        return result;
    }

    public bool GetTotalPwdChangeTime(string userID)
    {
        bool status = false;
        int pwdchangetime;
        
        string total = gettotalchangepwdsequence(userID);

        SecurityProfileModel securityProfile = new SecurityProfileModel();
        securityProfile.fldPwdChangeTime = StringUtils.convertToInt(StringUtils.Trim(securityProfileDao.GetValueFromSecurityMaster("fldPwdChangeTime").ToString()));
        pwdchangetime = securityProfile.fldPwdChangeTime;
        
        if (Convert.ToInt32(total) > pwdchangetime)
        {
            status = true;
        }
        else
        {
            status = false;
        }

        return status;
    }

    public string gettotalchangepwdsequence(string userID)
    {
        string result = "";
        try
        {
            DataTable dataResult = new DataTable();
            List<SqlParameter> sqlParameterNext = new List<SqlParameter>();
            sqlParameterNext.Add(new SqlParameter("@action", "gettotal"));
            sqlParameterNext.Add(new SqlParameter("@fldUserId", userID));
            dataResult = dbContext.GetRecordsAsDataTableSP("spcgPwdChangeTime", sqlParameterNext.ToArray());
            if (dataResult.Rows.Count > 0)
            {
                result = dataResult.Rows[0]["total"].ToString();
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return result;
    }
}
