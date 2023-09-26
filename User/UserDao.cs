using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
//using INCHEQS.Helpers;
using INCHEQS.Security;
using System.Data;
using System.Web.Mvc;
using System.Text.RegularExpressions;
//using INCHEQS.Resources;
using System.Configuration;
using INCHEQS.Security.SystemProfile;
using INCHEQS.Security.SecurityProfile;
//using INCHEQS.Models.Password;
using INCHEQS.DataAccessLayer;
using INCHEQS.Common;
using INCHEQS.Security.Resources;
using INCHEQS.Security.User;


namespace INCHEQS.Security.User
{
    public class UserDao : IUserDao
    {

        private readonly ApplicationDbContext dbContext;
        private readonly ISystemProfileDao systemProfileDao;
        private readonly ISecurityProfileDao securityProfileDao;
        public UserDao(ApplicationDbContext dbContext, ISystemProfileDao systemProfileDao, ISecurityProfileDao securityProfileDao)
        {
            this.dbContext = dbContext;
            this.systemProfileDao = systemProfileDao;
            this.securityProfileDao = securityProfileDao;
        }

        public DataTable ListBranchCode(string bankCode)
        {
            string stmt = "Select fldbranchcode,fldbranchdesc FROM tblbranchmaster where fldactive='Y' order by fldbranchdesc    ";
            DataTable dataTable = new DataTable();
            return this.dbContext.GetRecordsAsDataTable(stmt);
        }
        public DataTable ListBranchDesc(string bankCode)
        {
            string stmt = "Select fldbranchcode,fldbranchdesc FROM tblbranchmaster order by fldbranchdesc  ";
            DataTable dataTable = new DataTable();
            return this.dbContext.GetRecordsAsDataTable(stmt);
        }
        public void updateLoginCounterForUser(string userAbbr, int counter, string isDisabled, bool result)
        {

            List<SqlParameter> listParams = new List<SqlParameter>();
            listParams.AddRange(new[] {
                new SqlParameter("@counter", counter),
                new SqlParameter("@disabled", isDisabled),
                new SqlParameter("@userAbbr", userAbbr)
            });
            //success
            if (result)
            {
                listParams.Add(new SqlParameter("@failLoginDate", DBNull.Value));
            }
            else
            {
                listParams.Add(new SqlParameter("@failLoginDate", DateTime.Now));
            }

            string mySqlUpd = "Update tblUserMaster SET fldCounter = @counter , fldDisableLogin = @disabled , fldFailLoginDate = @failLoginDate WHERE fldUserAbb = @userAbbr";
            dbContext.ExecuteNonQuery(mySqlUpd, listParams.ToArray());
        }
        public void updateLastLoginDate(string userId)
        {
            Console.WriteLine("Masuk");
            string mySqlUpd = "Update tblUserMaster SET fldLastLoginDate = GetDate() WHERE fldUserID = @userId ";
            dbContext.ExecuteNonQuery(mySqlUpd, new[] { new SqlParameter("@userId", userId) });
        }
        public UserModel getUserByAbbr(string strUserAbbr, string macAddress)
        {
            UserModel user = new UserModel();
            //string mySQL = "SELECT  isNULL(fldBranchCode,'')[fldBranchCode], fldUserDesc,fldPasswordExpDate,fldUserId,fldUserAbb,fldPassword, fldDisableLogin,fldPassLastUpdDate,fldCounter,fldIDExpDate,fldIDExpStatus, fldLastLoginDate,  fldBankDesc, fldSpickDesc, um.fldBankCode , um.* FROM tblUserMaster um, tblBankMaster bm, tblSpickMaster sm WHERE fldUserAbb = @strUserAbbr AND um.fldBankCode = bm.fldBankCode";
            //string mySQL = "SELECT isNULL(fldBranchCode,'')[fldBranchCode], fldUserDesc,fldPasswordExpDate,fldUserId,fldUserAbb,fldPassword, fldDisableLogin,fldPassLastUpdDate,fldCounter,fldIDExpDate,fldIDExpStatus, fldLastLoginDate,  fldBankDesc, fldSpickDesc, um.fldBankCode , um.* FROM tblUserMaster um, tblBankMaster bm, tblSpickMaster sm, tblInternalBranchMaster ibm WHERE fldUserAbb = @strUserAbbr AND um.fldBankCode = bm.fldBankCode";
            string mySQL = "SELECT isNULL(fldBranchCode,'')[fldBranchCode], fldUserDesc,fldPasswordExpDate,fldUserId,fldUserAbb,fldPassword, fldDisableLogin,fldPassLastUpdDate,fldCounter,fldIDExpDate,fldIDExpStatus, fldLastLoginDate,  fldBankDesc, fldSpickDesc, um.fldBankCode , um.*, sma.fldStateCode  FROM tblUserMaster um, tblBankMaster bm, tblSpickMaster sm, tblInternalBranchMaster ibm,tblStateMaster sma WHERE fldUserAbb = @strUserAbbr AND um.fldBankCode = bm.fldBankCode";

            DataTable dt = dbContext.GetRecordsAsDataTable(mySQL, new[] { new SqlParameter("@strUserAbbr", strUserAbbr) });

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];

                user.fldIDExpStatus = StringUtils.convertToInt(row["fldIDExpStatus"].ToString());
                user.fldCounter = StringUtils.convertToInt(row["fldCounter"].ToString());
                user.fldLastLoginDate = row["fldLastLoginDate"].ToString();
                user.fldPassLastUpdDate = row["fldPassLastUpdDate"].ToString();
                //user.fldSpickCode = row["fldSpickCode"].ToString();
                user.fldSpickDesc = row["fldSpickDesc"].ToString();
                //user.fldVerifyChequeFlag = row["fldVerifyChequeFlag"].ToString();

                // xx start 20210624
                user.fldBranchCode2 = row["fldBranchCode2"].ToString();
                user.fldBranchCode3 = row["fldBranchCode3"].ToString();
                // xx end 20210624

                user.fldVerificationLimit = row["fldVerificationLimit"].ToString();
                //user.fldStateDesc = row["fldStateDesc"].ToString();
                
                user.fldUserAbb = row["fldUserAbb"].ToString();
               user.fldClass = row["fldVerificationClass"].ToString();
                user.fldUserId = row["fldUserId"].ToString();
                user.fldUserDesc = row["fldUserDesc"].ToString();
               // user.fldEmail = row["fldEmail"].ToString();
                user.fldBankCode = row["fldBankCode"].ToString();
                user.fldBankDesc = row["fldBankDesc"].ToString();
                //user.fldCity = row["fldCity"].ToString();
                user.fldBranchCode = row["fldBranchCode"].ToString();
                user.fldBranchCode1 = row["fldBranchCode3"].ToString();
                //user.fldVerificationLimit = row["fldVerificationLimit"].ToString();
                user.fldDisableLogin = row["fldDisableLogin"].ToString();
                user.fldFailLoginDate = row["fldFailLoginDate"].ToString();
                user.fldPasswordExpDate = row["fldPasswordExpDate"].ToString();
                user.fldIDExpDate = row["fldIDExpDate"].ToString();
                user.fldCreateTimeStamp = row["fldCreateTimeStamp"].ToString();
                user.fldPassword = decryptPassword(row["fldPassword"].ToString());
                //user.fldAdminFlag = row["fldAdminFlag"].ToString();
                user.macAddress = macAddress;
                user.GroupIds = GetGroupIds(user.fldUserId);
                user.fldStateCode = row["fldStateCode"].ToString();
                user.fldZeroMaker = row["fldZeroMaker"].ToString();

                user.fldBranchHubCodes = new List<string> { "" };

                //if (user.fldAdminFlag != null)
                //{
                //    List<string> branchAvailable = new List<string>();
                //    if (user.fldAdminFlag == "N")
                //    {
                //        string mySQLVer = "SELECT distinct isNULL(fldBranchId,'') as fldbranchcode from tblDedicatedBranchDate where fldUserId = @strUserAbbr";

                //        DataTable dtVer = dbContext.GetRecordsAsDataTable(mySQLVer, new[] { new SqlParameter("@strUserAbbr", strUserAbbr) });
                //        if (dtVer.Rows.Count > 0)
                //        {
                //            foreach (DataRow rowVer in dtVer.Rows)
                //            {
                //                branchAvailable.Add(rowVer["fldbranchcode"].ToString());
                //            }
                //            user.fldBranchHubCodes = branchAvailable;
                //        }
                //    }
                //    else if ((user.fldAdminFlag == "Y") || (user.fldAdminFlag == "S"))
                //    {
                //        string mySQLoff = "SELECT distinct isNULL(fldBranchId,'') as fldbranchcode from tblDedicatedBranchDate where fldUserId = @strUserAbbr";

                //        DataTable dtoff = dbContext.GetRecordsAsDataTable(mySQLoff, new[] { new SqlParameter("@strUserAbbr", strUserAbbr) });
                //        if (dtoff.Rows.Count > 0)
                //        {
                //            foreach (DataRow rowOff in dtoff.Rows)
                //            {
                //                branchAvailable.Add(rowOff["fldbranchcode"].ToString());
                //            }
                //            user.fldBranchHubCodes = branchAvailable;
                //        }
                //    }
                //}
                return user;
            }

            return null;
        }
        public string[] GetGroupIds(string userId)
        {
            string mySQL = null;
            string[] groupArray = new string[0];
            //mySQL = "select fldGroupId from tblGroupUser where fldUserId = @userId "; //Commented by Michelle 20200518
            mySQL = "select fldGroupCode from tblGroupUser where fldUserId = @userId ";
            DataTable dataTable = dbContext.GetRecordsAsDataTable(mySQL, new[] { new SqlParameter("@userId", userId) });
            groupArray = new string[dataTable.Rows.Count + 1];
            Int32 i = 0;
            foreach (DataRow row in dataTable.Rows)
            {
                //groupArray[i] = row["fldGroupId"].ToString();
                groupArray[i] = row["fldGroupCode"].ToString();
                i = i + 1;
            }
            return groupArray;
        }
        public List<UserModel> ListApprovedUser(string userId)
        {
            string stmt = " SELECT a.fldUserID,a.fldApproveBranchCode,a.fldUserAbb,a.fldUserDesc, a.fldPasswordExpDate,a.fldCreateTimeStamp,a.fldDisableLogin, CASE WHEN isNULL(a.fldBranchCode,'') ='' THEN 'CCU' ELSE 'Branch User' END as userType FROM tblUserMaster a, tblBankMaster b WHERE a.fldBankCode=b.fldBankCode AND a.fldBankCode=@fldBankCode AND fldDeleted is null order by a.fldUserAbb ";

            List<UserModel> userList = new List<UserModel>();
            DataTable ds = dbContext.GetRecordsAsDataTable(stmt, new[] {
                new SqlParameter("@fldBankCode",userId /*CurrentUser.Account.BankCode*/)
            });

            foreach (DataRow row in ds.Rows)
            {
                UserModel user = new UserModel();
                user.fldUserId = row["fldUserId"].ToString();
                user.fldUserAbb = row["fldUserAbb"].ToString();
                user.fldUserDesc = row["fldUserDesc"].ToString();
                user.userType = row["userType"].ToString();
                user.fldDisableLogin = row["fldDisableLogin"].ToString();
                user.fldPasswordExpDate = DateUtils.formatDateFromSql(row["fldPasswordExpDate"].ToString());
                user.fldCreateTimeStamp = DateUtils.formatDateFromSql(row["fldCreateTimeStamp"].ToString());
                userList.Add(user);
            }

            return userList;

        }
        public List<UserModel> ListUnapprovedUser(string userId)
        {
            string stmt = "SELECT  fldVerificationLimit,fldUserID,fldUserAbb,fldUserDesc, CASE WHEN isNULL(fldBranchCode,'') ='' THEN 'CCU' ELSE 'Branch User' END as userType , fldBranchCode, fldDisableLogin, CASE WHEN isNULL(fldApproveStatus,'') ='A' THEN 'A'  WHEN isNULL(fldApproveStatus,'') ='U' THEN 'U'  ELSE 'D' END as status  FROM tblUserMasterTemp WHERE  fldBankCode=@fldBankCode  order by fldUserAbb ";

            List<UserModel> userList = new List<UserModel>();
            DataTable ds = new DataTable();
            ds = dbContext.GetRecordsAsDataTable(stmt, new[] { new SqlParameter("@fldBankCode", userId/*CurrentUser.Account.BankCode*/) });

            foreach (DataRow row in ds.Rows)
            {
                UserModel user = new UserModel();
                user.fldUserAbb = row["fldUserAbb"].ToString();
                user.fldUserDesc = row["fldUserDesc"].ToString();
                //user.fldEmail = row["fldEmail"].ToString();
                user.userType = row["userType"].ToString();
                user.fldBranchCode = row["fldBranchCode"].ToString();
                user.fldVerificationLimit = row["fldVerificationLimit"].ToString();
                user.fldDisableLogin = row["fldDisableLogin"].ToString();
                user.status = row["status"].ToString();
                userList.Add(user);
            }


            return userList;

        }
        public void CreateInUserMaster(string userAbb)
        {
            string stmt = "UPDATE tblUserMasterTemp SET fldApproveStatus=@fldApproveStatus,fldApproveBranchCode=@fldApproveBranchCode WHERE fldUserAbb=@fldUserAbb ";

            dbContext.ExecuteNonQuery(stmt, new[] {
                new SqlParameter("@fldApproveStatus", "Y"),
                new SqlParameter("@fldApproveBranchCode", "Y"),
                new SqlParameter("@fldUserAbb", userAbb),
            });

            string stmt2 = "Insert into tblUserMaster" +

                "(fldUserAbb, fldUserDesc, fldAppRight, fldBankCode, fldPassword, fldDisableLogin, fldCounter, fldBranchCode, fldCreateUserId, fldCreateTimeStamp, fldUpdateUserId, fldUpdateTimeStamp, fldApproveStatus, fldApproveBranchCode, fldPasswordExpDate,fldIDExpStatus,fldIDExpDate) "

                + " Select " +

                " fldUserAbb, fldUserDesc, fldAppRight, fldBankCode, fldPassword, fldDisableLogin, fldCounter, fldBranchCode, fldCreateUserId, fldCreateTimeStamp, fldUpdateUserId, fldUpdateTimeStamp, fldApproveStatus, fldApproveBranchCode, fldPasswordExpDate,fldIDExpStatus,fldIDExpDate " +

                " from tblUserMasterTemp WHERE fldUserAbb = @fldUserAbb ";
            dbContext.ExecuteNonQuery(stmt2, new[] {
                new SqlParameter("@fldUserAbb", userAbb) });


        }
        public void DeleteInUserMasterTemp(string userAbb)
        {
            string stmt = " Delete from tblUserMasterTemp where fldUserAbb=@fldUserAbb";
            dbContext.ExecuteNonQuery(stmt, new[] { new SqlParameter("@fldUserAbb", userAbb) });
        }
        public void DeleteInUserMaster(string userAbb)
        {
            //string stmt = " Delete from tblUserMaster where fldUserAbb=@fldUserAbb";
            string stmt = "update tblusermaster set fldUserAbb = flduserabb + '_' + Convert(varchar(20), getdate(), 121), " +
                " fldUpdateTimeStamp = getdate() , fldDisableLogin = 'Y' , fldDeleted = 1 where fldUserAbb=@fldUserAbb ";
            dbContext.ExecuteNonQuery(stmt, new[] { new SqlParameter("@fldUserAbb", userAbb) });
        }
        public void AddUserToUserMasterTempToDelete(string userId)
        {
            string stmt = "Insert into tblUserMasterTemp (fldUserId, fldUserAbb, fldUserDesc, fldAppRight, fldBankCode, fldPassword, fldDisableLogin, fldCounter, fldBranchCode, fldCreateUserId, fldCreateTimeStamp, fldUpdateUserId, fldUpdateTimeStamp, fldApproveStatus,fldApproveBranchCode) " +
                " Select fldUserId, fldUserAbb, fldUserDesc, fldAppRight, fldBankCode, fldPassword, fldDisableLogin, fldCounter, fldBranchCode, fldCreateUserId, fldCreateTimeStamp, fldUpdateUserId, fldUpdateTimeStamp, fldApproveStatus, fldApproveBranchCode from tblUserMaster WHERE fldUserId=@fldUserId  Update tblUserMasterTemp SET fldApproveStatus=@fldApproveStatus WHERE fldUserId=@fldUserId ";
            UserModel user = new UserModel();

            dbContext.ExecuteNonQuery(stmt, new[] {
                new SqlParameter("@fldUserId", userId),
                new SqlParameter("@fldApproveStatus", "D")
            });
        }
        public UserModel GetUser(string id)
        {
            UserModel user = new UserModel();
            DataTable ds = new DataTable();
            string stmt = "Select a.*,b.fldBankDesc FROM tblUserMaster a, tblBankMaster b WHERE a.fldUserId = @fldUserId And a.fldBankCode = b.fldBankCode";


            ds = dbContext.GetRecordsAsDataTable(stmt, new[] { new SqlParameter("@fldUserId", id) });
            if (ds.Rows.Count > 0)
            {
                DataRow row = ds.Rows[0];
                user.fldUserAbb = row["fldUserAbb"].ToString();
                user.fldClass = row["fldVerificationClass"].ToString();
                user.fldUserId = row["fldUserId"].ToString();
                user.fldUserDesc = row["fldUserDesc"].ToString();
                user.fldPassword = row["fldPassword"].ToString();
                //user.fldEmail = row["fldEmail"].ToString();
                //user.fldAdminFlag = row["fldAdminFlag"].ToString();
                user.fldBankCode = row["fldBankCode"].ToString();
                user.fldBankDesc = row["fldBankDesc"].ToString();
                //user.fldCity = row["fldCity"].ToString();
                user.fldBranchCode = row["fldBranchCode"].ToString();
                user.fldVerificationLimit = row["fldVerificationLimit"].ToString();
                user.fldDisableLogin = row["fldDisableLogin"].ToString();
                user.fldFailLoginDate = DateUtils.formatDateFromSql(row["fldFailLoginDate"].ToString());
                user.fldPasswordExpDate = DateUtils.formatDateFromSql(row["fldPasswordExpDate"].ToString());
                user.fldIDExpDate = DateUtils.formatDateFromSql(row["fldIDExpDate"].ToString());
                user.fldCreateTimeStamp = DateUtils.formatDateFromSql(row["fldCreateTimeStamp"].ToString());
                user.fldPassword = decryptPassword(user.fldPassword);
            }


            return user;
        }
        //public UserModel CreateUserToUserTemp(FormCollection col, string userId, string crtUser)
        //{
        //    dynamic bankCode = userId/*CurrentUser.Account.BankCode*/;

        //    string stmt = " INSERT INTO tblUserMasterTemp " +

        //        "(fldUserAbb, fldUserDesc, fldAppRight, fldBankCode, fldPassword, fldDisableLogin, fldCounter,fldBranchCode , fldCreateUserId, fldCreateTimeStamp, fldUpdateUserId,fldUpdateTimeStamp, fldApproveStatus, fldPasswordExpDate,fldIDExpStatus,fldIDExpDate) "

        //        + " VALUES " +

        //        " (@fldUserAbb, @fldUserDesc, @fldAppRight, @fldBankCode, @fldPassword, @fldDisableLogin, @fldCounter, @fldBranchCode, @fldCreateUserId, @fldCreateTimeStamp, @fldUpdateUserId, @fldUpdateTimeStamp ,@fldApproveStatus,@fldPasswordExpDate,@fldIDExpStatus,@fldIDExpDate) ";

        //    //Insert fldCity and Branch Code based on branchCode
        //    string strFldCity = "";
        //    string strFldBranchCode = "";
        //    string strAdminFlag = "";
        //    if ((col["userType"].Equals("branch")))
        //    {
        //        strFldCity = col["fldBranchCode"].Substring(0, 2);
        //        strFldBranchCode = col["fldBranchCode"];
        //        strAdminFlag = "N";
        //    }
        //    else if ((col["userType"].Equals("ccu")))
        //    {
        //        //if (!(string.IsNullOrEmpty(col["fldCity"])))
        //        //{
        //        //    strFldCity = col["fldCity"];
        //        //}

        //        strFldBranchCode = "";
        //        if (!(string.IsNullOrEmpty(col["userType2"])))
        //        {
        //            strAdminFlag = col["userType2"].ToString();
        //        }
        //        else
        //        {
        //            strAdminFlag = "N";
        //        }

        //    }

        //    //Initialize SpickCode
        //    //TODO: HARDCODED SPIKE CODE
        //    //UserModel spickCode = GetSpickCode();
        //    //string strSpickCode = spickCode.fldSpickCode;

        //    //Initilize disable login checkbox with N if not checked
        //    string strFldDisableLogin = col["fldDisableLogin"];
        //    if (strFldDisableLogin == null)
        //    {
        //        strFldDisableLogin = "N";
        //    }

        //    //Initilize password expiry date based on security profile
        //    SecurityProfileModel securityProfile = securityProfileDao.GetSecurityProfile();
        //    string secPwdExp = securityProfile.fldUserPwdExpiry + securityProfile.fldUserPwdExpiryInt;
        //    DateTime passwordExpDate = FormatExpDate(secPwdExp);
        //    //DateTime IDExpDate = DateUtils.AddDate(securityProfile.fldAcctExpIntDigit2, Convert.ToInt16(securityProfile.fldAcctExpIntDigit), DateTime.Now);
        //    UserModel user = new UserModel();
        //    //Julian 20180325 - handle if user does not right for Cheque Verification
        //    //string strVerificationLimit = "0";
        //    //string strVerificationClass = "C";
        //    //if (!(string.IsNullOrEmpty(col["fldVerificationLimit"])))
        //    //{
        //    //    strVerificationLimit = col["fldVerificationLimit"].ToString();
        //    //}

        //    //if (!(string.IsNullOrEmpty(col["fldVerificationClass"])))
        //    //{
        //    //    strVerificationClass = col["fldVerificationClass"].ToString();
        //    //}

        //    SqlParameter[] cmdParms = new SqlParameter[16];

        //    cmdParms[0] = new SqlParameter("@fldUserAbb", col["fldUserAbb"].ToString());
        //    cmdParms[1] = new SqlParameter("@fldUserDesc", col["fldUserDesc"].ToString());
        //    //cmdParms[2] = new SqlParameter("@fldEmail", col["fldEmail"]);
        //    cmdParms[2] = new SqlParameter("@fldAppRight", "");
        //    cmdParms[3] = new SqlParameter("@fldBankCode", userId);
        //    //cmdParms[5] = new SqlParameter("@fldSpickCode", strSpickCode);
        //    //cmdParms[4] = new SqlParameter("@fldAdminFlag", strAdminFlag);
        //    //cmdParms[6] = new SqlParameter("@fldLoginIP1", "");
        //    cmdParms[4] = new SqlParameter("@fldPassword", encryptPassword(col["fldPassword"]));
        //    cmdParms[5] = new SqlParameter("@fldDisableLogin", strFldDisableLogin);
        //    cmdParms[6] = new SqlParameter("@fldCounter", "0");
        //    //cmdParms[10] = new SqlParameter("@fldVerifyChequeFlag", "0");
        //    cmdParms[7] = new SqlParameter("@fldBranchCode", strFldBranchCode);
        //    //cmdParms[11] = new SqlParameter("@fldVerificationLimit", strVerificationLimit);
        //    cmdParms[8] = new SqlParameter("@fldCreateUserId", crtUser);
        //    cmdParms[9] = new SqlParameter("@fldCreateTimeStamp", DateTime.Now);
        //    cmdParms[10] = new SqlParameter("@fldUpdateUserId", "");
        //    cmdParms[11] = new SqlParameter("@fldUpdateTimeStamp", "");
        //    //cmdParms[14] = new SqlParameter("@fldCity", strFldCity.ToString());
        //    cmdParms[12] = new SqlParameter("@fldApproveStatus", "A");
        //    cmdParms[13] = new SqlParameter("@fldPasswordExpDate", passwordExpDate);
        //    //cmdParms[18] = new SqlParameter("@fldVerificationClass", strVerificationClass);
        //    cmdParms[14] = new SqlParameter("@fldIDExpStatus", "0");
        //    //cmdParms[15] = new SqlParameter("@fldIDExpDate", IDExpDate);

        //    dbContext.ExecuteNonQuery(stmt, cmdParms);
        //    return user;
        //}
        public UserModel InsertUserDedicatedBranch(FormCollection col, string userId)
        {
            dynamic bankCode = userId /*CurrentUser.Account.BankCode*/;
            UserModel user = new UserModel();
            if (String.IsNullOrEmpty(col["selectedTask"]))
            {
                string stmt = "Delete from tblDedicatedBranch where fldUserid = '" + col["fldUserAbb"].ToString() + "'";
            }
            else
            {
                string[] branchArray = (col["selectedTask"].ToString()).Split(',');
                foreach (string branchArrayList in branchArray)
                {
                    string stmt = "Insert into tblDedicatedBranch" +
                    "(fldUserId,fldBranchId,fldOfficerId,fldApproved) VAlUES (@fldUserAbb, @fldBranchId, @fldOfficerId,@fldApproved)";
                    dbContext.ExecuteNonQuery(stmt, new[] {
                    new SqlParameter("@fldUserAbb", col["fldUserAbb"].ToString()),
                    new SqlParameter("@fldBranchId", branchArrayList.ToString()),
                    new SqlParameter("@fldOfficerId", col["SelectUserOfficer"].ToString()),
                    new SqlParameter("@fldApproved", "N")
                });
                }
            }
            return user;
        }
        public void UpdateUserDedicatedBranch(string userId)
        {
            string stm = "Delete from tblDedicatedBranch where fldUSerId = '" + userId + "' and fldApproved = 'Y'";
            dbContext.GetRecordsAsDataTable(stm);
            string stmt = "Update tblDedicatedBranch set fldApproved = 'Y' where fldUSerId = '" + userId + "' and fldApproved = 'N'";
            dbContext.GetRecordsAsDataTable(stmt);
        }
        public void DeleteUSerDedicatedBranch(string userId)
        {
            string stmt = "Delete from tblDedicatedBranch where fldUSerId = '" + userId + "' and fldApproved <> 'Y'";
            dbContext.GetRecordsAsDataTable(stmt);
        }
        public void DeleteDedicatedBranch(string userId)
        {
            string stmt = "Delete from tblDedicatedBranch where fldUSerId = '" + userId + "'";
            dbContext.GetRecordsAsDataTable(stmt);
        }
        //public UserModel UpdateUser(FormCollection col, string updUser)
        //{
        //    dynamic passwordChecker = "";
        //    dynamic encryptedPassword = "";

        //    /*string strfldCounter = "";

        //    if (col["fldDisableLogin"]== "N") {
        //        strfldCounter= "0";
        //    }*/

        //    string stmt = " UPDATE tblUserMaster Set fldUserAbb=@fldUserAbb, fldUserDesc =@fldUserDesc, fldBranchCode =@fldBranchCode, fldIDExpDate =@fldIDExpDate, fldUpdateTimeStamp =@fldUpdateTimeStamp, fldUpdateUserId =@fldUpdateUserId, fldDisableLogin=@fldDisableLogin, fldCounter=@fldCounter,fldLastLoginDate=@fldLastLoginDate ";

        //    //Insert fldCity and Branch Code based on branchCode
        //    //dynamic fldCity = "";
        //    dynamic fldBranchCode = "";
        //    dynamic fldUserType2 = "";
        //    if ((col["userType"].Equals("branch")))
        //    {
        //        fldUserType2 = "";
        //      //  fldCity = col["fldBranchCode"].Substring(0, 2);
        //        fldBranchCode = col["fldBranchCode"];
        //    }
        //    else if ((col["userType"].Equals("ccu")))
        //    {
        //        //fldCity = col["fldCity"];
        //        //fldUserType2 = col["userType2"].ToString();
        //        if (!(string.IsNullOrEmpty(col["userType2"])))
        //        {
        //            fldUserType2 = col["userType2"].ToString();
        //        }
        //        else
        //        {
        //            fldUserType2 = "N";
        //        }
        //        fldBranchCode = "";
        //    }


        //    //if (col.AllKeys.Contains("fldCity"))
        //    //{
        //    //    fldCity = col["fldCity"];
        //    //}
        //    //else
        //    //{
        //    //    fldCity = "";
        //    //}



        //    //Check Password if changed then update password and password update date
        //    if (!(col["fldPassword"].Equals(col["passwordChecker"])))
        //    {
        //        passwordChecker = "1";
        //        stmt = stmt + " , fldPassword =@fldPassword, fldPassLastUpdDate =@fldPassLastUpdDate";
        //        encryptedPassword = encryptPassword(col["fldPassword"]);
        //    }

        //    //string strVerificationLimit = "0";
        //   // string strVerificationClass = "C";
        //    //if (!(string.IsNullOrEmpty(col["fldVerificationLimit"])))
        //    //{
        //    //    strVerificationLimit = col["fldVerificationLimit"].ToString();
        //    //}

        //    //if (!(string.IsNullOrEmpty(col["fldVerificationClass"])))
        //    //{
        //    //    strVerificationClass = col["fldVerificationClass"].ToString();
        //    //}

        //    //Continue the statement
        //    stmt = stmt + " WHERE fldUserId=@fldUserId";
        //    List<SqlParameter> sqlParams = new List<SqlParameter>();
        //    sqlParams.Add(new SqlParameter("@fldUserId", col["fldUserId"].ToString()));
        //    sqlParams.Add(new SqlParameter("@fldUserAbb", col["fldUserAbb"].ToString()));
        //    sqlParams.Add(new SqlParameter("@fldUserDesc", col["fldUserDesc"].ToString()));
        //    //sqlParams.Add(new SqlParameter("@fldEmail", col["fldEmail"].ToString()));
        //    sqlParams.Add(new SqlParameter("@fldBranchCode", fldBranchCode));
        //    //sqlParams.Add(new SqlParameter("@fldVerificationLimit", strVerificationLimit));
        //    //sqlParams.Add(new SqlParameter("@fldCity", fldCity));
        //    sqlParams.Add(new SqlParameter("@fldIDExpDate", DateUtils.formatDateToSql(col["fldIDExpDate"])));
        //    sqlParams.Add(new SqlParameter("@fldUpdateTimeStamp", DateTime.Now));
        //    sqlParams.Add(new SqlParameter("@fldUpdateUserId", updUser));
        //    //sqlParams.Add(new SqlParameter("@fldVerificationClass", strVerificationClass));
        //    sqlParams.Add(new SqlParameter("@fldDisableLogin", col["fldDisableLogin"].ToString()));
        //    sqlParams.Add(new SqlParameter("@fldCounter", "0"));
        //    sqlParams.Add(new SqlParameter("@fldLastLoginDate", DateTime.Now));//update last login date, so user can login again
        //    //sqlParams.Add(new SqlParameter("@fldAdminFlag", fldUserType2));

        //    if ((passwordChecker.Equals("1")))
        //    {
        //        sqlParams.Add((new SqlParameter("@fldPassword", encryptedPassword)));
        //        sqlParams.Add((new SqlParameter("@fldPassLastUpdDate", DateTime.Now)));
        //    }

        //    UserModel user = new UserModel();
        //    dbContext.ExecuteNonQuery(stmt, sqlParams.ToArray());
        //    return user;
        //}
        public UserModel GetUserTemp(string userId)
        {
            string stmt = "Select fldBranchCode from tblUserMasterTemp where fldUserAbb = '" + userId + "'";
            DataTable ds = new DataTable();
            UserModel getUserMaster = new UserModel();
            ds = dbContext.GetRecordsAsDataTable(stmt);
            if (ds.Rows.Count == 0)
            {
                //getUserMaster.fldAdminFlag = "";
                getUserMaster.fldCCUFlag = "";
            }
            else
            {
                foreach (DataRow row in ds.Rows)
                {
                    //getUserMaster.fldAdminFlag = row["fldAdminFlag"].ToString();
                    getUserMaster.fldCCUFlag = row["fldBranchCode"].ToString();
                }
            }

            return getUserMaster;
        }
        public UserModel GetOfficerId(string userId)
        {
            string stmt = "Select distinct fldOfficerId from tblDedicatedBranch where fldUserId = '" + userId + "'and fldApproved ='Y'";
            DataTable ds = new DataTable();
            UserModel getOfficerId = new UserModel();
            ds = dbContext.GetRecordsAsDataTable(stmt);
            if (ds.Rows.Count == 0)
            {
                getOfficerId.fldOfficerId = "";
            }
            else
            {
                foreach (DataRow row in ds.Rows)
                {
                    getOfficerId.fldOfficerId = row["fldOfficerId"].ToString();
                }
            }

            return getOfficerId;
        }
        public List<UserModel> ListAvailableBranch(string userId)
        {

            string stmt = "Select isNULL(fldConBranchCode,'')[fldConBranchCode], fldBranchDesc From tblMapBranch Where fldConBranchCode not in (Select fldBranchId from tblDedicatedBranch where fldApproved='Y' ) and fldbankcode=@fldBanckCode Order By fldBranchDesc ";
            DataTable ds = new DataTable();
            List<UserModel> branchAvailable = new List<UserModel>();
            ds = dbContext.GetRecordsAsDataTable
                (stmt, new[] {
                    new SqlParameter("@fldBanckCode", userId /*CurrentUser.Account.BankCode*/)
                });
            foreach (DataRow row in ds.Rows)
            {
                UserModel branchAvailableLsit = new UserModel();
                branchAvailableLsit.fldConBranchCode = row["fldConBranchCode"].ToString();
                branchAvailableLsit.fldBranchDesc = row["fldBranchDesc"].ToString();
                branchAvailable.Add(branchAvailableLsit);
            }
            return branchAvailable;
        }
        public List<UserModel> ListSelectedBranch(string userId, string strUser)
        {
            if (userId == null)
            {
                userId = "";
            }
            string stmt = "Select isNULL(a.fldConBranchCode,'')[fldConBranchCode], fldBranchDesc,fldOfficerId From tblMapBranch a " +
                          "inner join tblDedicatedBranch b on a.fldConBranchCode = b.fldBranchId " +
                          "Where b.fldUserId = @userid and b.fldApproved='Y' and a.fldbankcode=@fldBankCode Order By a.fldBranchDesc ";
            DataTable ds = new DataTable();
            List<UserModel> branchSelected = new List<UserModel>();
            ds = dbContext.GetRecordsAsDataTable
                (stmt, new[] {
                    new SqlParameter("@fldBankCode", userId),
                    new SqlParameter("@userid", strUser)
                });
            foreach (DataRow row in ds.Rows)
            {
                UserModel branchSelectedList = new UserModel();
                branchSelectedList.fldConBranchCode = row["fldConBranchCode"].ToString();
                branchSelectedList.fldBranchDesc = row["fldBranchDesc"].ToString();
                branchSelectedList.fldOfficerId = row["fldOfficerId"].ToString();
                branchSelected.Add(branchSelectedList);
            }

            return branchSelected;
        }
        public List<UserModel> ListBranch(string userId)
        {

            string stmt = "Select fldBranchDesc, isNULL(fldConBranchCode,'')[fldConBranchCode], isNULL(fldIsBranchCode,'')[fldIsBranchCode],isNULL(fldIsBranchCode2,'')[fldIsBranchCode2] From tblMapBranch Where 1=1 And right(fldConBranchCode,6) not in (Select fldCollapsebranchcode from tblrationalbranch)" +
                "And fldBankCode=@fldBankCode Order By fldConbranchCode ";
            DataTable ds = new DataTable();
            List<UserModel> branchList = new List<UserModel>();
            ds = dbContext.GetRecordsAsDataTable
                (stmt, new[] {
                    new SqlParameter("@fldBankCode", userId/*CurrentUser.Account.BankCode*/)
                });
            foreach (DataRow row in ds.Rows)
            {
                UserModel branch = new UserModel();
                branch.fldConBranchCode = row["fldConBranchCode"].ToString();
                branch.fldBranchDesc = row["fldBranchDesc"].ToString();
                branchList.Add(branch);
            }
            return branchList;
        }
        public List<UserModel> ListOfficer(string userId, string abb)
        {

            string stmt = "Select fldUserAbb, fldUserDesc, isNULL(fldAdminFlag,'')[fldAdminFlag]," +
                "fldAdminFLagN =CASE WHEN fldAdminFlag='Y' Then 'Officer' WHEN fldAdminFlag='S' Then 'Supervisor' End From tblUserMaster Where fldAdminFlag <> 'N' and flduserabb<>@abb and fldbankcode=@fldbankcode Order By fldUserAbb ";
            DataTable ds = new DataTable();
            List<UserModel> officerList = new List<UserModel>();
            ds = dbContext.GetRecordsAsDataTable
                (stmt, new[] {
                    new SqlParameter("@fldbankcode", userId/*CurrentUser.Account.BankCode*/),
                    new SqlParameter("@abb", abb)
                });
            foreach (DataRow row in ds.Rows)
            {
                UserModel officer = new UserModel();
                officer.fldUserAbbOfficer = row["fldUserAbb"].ToString();
                officer.fldUserDescOfficer = row["fldUserDesc"].ToString();
                officer.fldAdminFlag = row["fldAdminFlagN"].ToString();
                officerList.Add(officer);
            }
            return officerList;
        }
        public List<UserModel> ListCity()
        {

            string stmt = " SELECT * FROM tblstatemaster ";
            List<UserModel> cityList = new List<UserModel>();
            DataTable ds = dbContext.GetRecordsAsDataTable(stmt);
            if (((ds.Rows.Count > 0)))
            {
                foreach (DataRow row in ds.Rows)
                {
                    UserModel branch = new UserModel();
                    branch.fldStateCode = row["fldStateCode"].ToString();
                    branch.fldStateDesc = row["fldStateDesc"].ToString();
                    cityList.Add(branch);
                }
            }
            return cityList;
        }
        public List<UserModel> ListVerificationClass()
        {

            string stmt = " SELECT * FROM tblVerificationBatchSizeLimit ";
            List<UserModel> verificationList = new List<UserModel>();

            DataTable ds = dbContext.GetRecordsAsDataTable(stmt);
            foreach (DataRow row in ds.Rows)
            {
                UserModel verification = new UserModel();
                verification.fldClass = row["fldClass"].ToString();
                verification.fldLimitDesc = row["fldLimitDesc"].ToString();
                verificationList.Add(verification);
            }

            return verificationList;
        }
        public UserModel GetCity(string cityCode)
        {

            string stmt = " SELECT * FROM tblstatemaster WHERE fldStateCode=@fldStateCode ";
            UserModel city = new UserModel();
            DataTable ds = dbContext.GetRecordsAsDataTable(stmt, new[] {
                new SqlParameter("@fldStateCode", cityCode)
            });

            if (((ds.Rows.Count > 0)))
            {
                DataRow row = ds.Rows[0];
                city.fldStateCode = row["fldStateCode"].ToString();
                city.fldStateDesc = row["fldStateDesc"].ToString();
            }

            return city;
        }
        public UserModel GetSpickCode()
        {

            string stmt = " SELECT TOP(1) fldSpickCode FROM tblSpickMaster ";
            UserModel result = new UserModel();
            DataTable ds = dbContext.GetRecordsAsDataTable(stmt);

            if (((ds.Rows.Count > 0)))
            {
                DataRow row = ds.Rows[0];
                result.fldSpickCode = row["fldSpickCode"].ToString();
            }

            return result;
        }
        public UserModel GetSpickCode(string spickCode)
        {

            string stmt = " SELECT TOP(1) fldSpickCode FROM tblSpickMaster WHERE fldSpickCode=@fldSpickCode";
            UserModel result = new UserModel();
            DataTable ds = dbContext.GetRecordsAsDataTable(stmt, new[] {
                new SqlParameter("@fldSpickCode", spickCode)
            });

            if (((ds.Rows.Count > 0)))
            {
                DataRow row = ds.Rows[0];
                result.fldSpickCode = row["fldSpickCode"].ToString();
            }

            return result;
        }
        public bool CheckUserExist(string userAbb)
        {

            string stmt = " SELECT * FROM tblUserMaster WHERE fldUserAbb=@fldUserAbb";
            return dbContext.CheckExist(stmt, new[] { new SqlParameter("@fldUserAbb", userAbb) });

        }
        public bool CheckUserExistInTempUser(string userId)
        {
            string stmt = " SELECT * FROM tblUserMasterTemp WHERE fldUserId=@fldUserId";
            return dbContext.CheckExist(stmt, new[] { new SqlParameter("@fldUserId", userId) });
        }
        public void InsertUserPasswordHistory(FormCollection col, string strUserId)
        {
            Dictionary<string, dynamic> sqlPasswordHistory = new Dictionary<string, dynamic>();
            sqlPasswordHistory.Add("fldUserId", strUserId);
            sqlPasswordHistory.Add("fldPassword", encryptPassword(col["fldPassword"]));
            sqlPasswordHistory.Add("fldLastUpdateDate", DateUtils.GetCurrentDatetimeForSql());
            dbContext.ConstructAndExecuteInsertCommand("tblUserPwdHistory", sqlPasswordHistory);
        }
        public bool CheckUserPasswordHistory(string encryptedPassword, string userId)
        {
            int usablePassword = securityProfileDao.GetSecurityProfile().fldUserPwdHisRA;
           
            string stmt = String.Format(" SELECT TOP({0}) * FROM tblUserPwdHistory WHERE fldUserId=@fldUserId ORDER BY fldLastUpdateDate DESC", usablePassword);
            DataTable ds = new DataTable();
            ds = dbContext.GetRecordsAsDataTable(stmt, new[] { new SqlParameter("@fldUserId", userId /*CurrentUser.Account.UserId*/) });

            foreach (DataRow row in ds.Rows)
            {
                if (row["fldPassword"].Equals(encryptedPassword))
                {
                    return false;
                }
            }
            return true;
        }
        //public List<string> ValidateUser(FormCollection col, string action, string userId)
        //{

        //    string systemLoginAD = systemProfileDao.GetValueFromSystemProfileWithoutCurrentUser("LoginAD").Trim();
        //    dynamic securityProfile = securityProfileDao.GetSecurityProfile();
        //    List<string> err = new List<string>();

        //    if ((action.Equals("Create")))
        //    {
        //        if ((CheckUserExist(col["fldUserAbb"])))
        //        {
        //            err.Add(Locale.UserAbbreviationalreadytaken);
        //        }
        //    }

        //    if ((col["fldUserAbb"].Equals("")))
        //    {
        //        err.Add(Locale.UserAbbreviationcannotbeblank);
        //    }
        //    else if ((col["fldUserAbb"].Length < securityProfile.fldUserIdLengthMin))
        //    {
        //        err.Add(Locale.UserAbbreviationcannotbelessthan + securityProfile.fldUserIdLengthMin + Locale.character);
        //    }
        //    else if ((col["fldUserAbb"].Length > securityProfile.fldUserIdLengthMax))
        //    {
        //        err.Add(Locale.UserAbbreviationcannotbemorethan + securityProfile.fldUserIdLengthMax + Locale.character);
        //    }
        //    //if (!(Regex.IsMatch(col["fldUserAbb"], "[A-Z].*[0-9]|[0-9].*[A-Z]"))) {
        //    else if (!(Regex.IsMatch(col["fldUserAbb"], "^[a-zA-Z0-9]*$")))
        //    {
        //        err.Add(Locale.UserAbbreviationmustbealphanumeric);
        //    }
        //    if ((col["fldUserDesc"].Equals("")))
        //    {
        //        err.Add(Locale.UserDescriptioncannotbeblank);
        //    }

        //    if (!"Y".Equals(systemLoginAD))
        //    {
        //        if ((col["fldPassword"].Equals("")))
        //        {
        //            err.Add(Locale.Passwordcannotbeblank);
        //        }
        //        else if ((col["fldPassword"].Length < securityProfile.fldUserPwdLengthMin))
        //        {
        //            err.Add(Locale.Passwordcannotbelessthan + securityProfile.fldUserPwdLengthMin + Locale.character);
        //        }
        //        else if ((col["fldPassword"].Length > securityProfile.fldUserPwdLengthMax))
        //        {
        //            err.Add(Locale.Passwordcannotbemorethan + securityProfile.fldUserPwdLengthMax + Locale.character);
        //        }
        //        else if (!(Regex.IsMatch(col["fldPassword"], "[a-z].*[0-9]|[0-9].*[a-z]")))
        //        {
        //            err.Add(Locale.Passwordmustbealphanumeric);
        //        }
        //        else if ((col["fldPassword"].Equals(col["fldUserAbb"])))
        //        {
        //            err.Add(Locale.PasswordcannotbesameasUserAbbreviation);
        //        }
        //        else if ((col["fldPassword"].Equals(col["fldUserDesc"])))
        //        {
        //            err.Add(Locale.PasswordcannotbesameasUserDescription);
        //        }
        //        //if ((char.IsLower(col["fldPassword"], 0))) {
        //        //    err.Add(Locale.ThefirstcharacterofnewpasswordmustbeaCapitalLetter);
        //        //}

        //        else if (!CheckUserPasswordHistory(encryptPassword(col["fldPassword"]), userId))
        //        {
        //            err.Add("Password cannot be used as previous password");
        //        }
        //    }

        //    //if (!(string.IsNullOrEmpty(col["fldEmail"])))
        //    //{
        //    //    if (!(Regex.IsMatch(col["fldEmail"], "^([0-9a-zA-Z]([-\\.\\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\\w]*[0-9a-zA-Z]\\.)+[a-zA-Z]{2,9})$")))
        //    //    {
        //    //        err.Add(Locale.Incorrectemailformat);
        //    //    }
        //    //}
        //    //int temp;
        //    //if (!(string.IsNullOrEmpty(col["fldVerificationLimit"])))
        //    //{
        //    //    if ((!int.TryParse(col["fldVerificationLimit"], out temp)))
        //    //    {
        //    //        err.Add(Locale.Verificationlimitmustbeinteger);
        //    //    }
        //    //    else if ((int.Parse((col["fldVerificationLimit"])) <= 0))
        //    //    {
        //    //        err.Add(Locale.VerificationLimitmustbemorethan0);
        //    //    }
        //    //}

        //    if ((col["userType"].Equals("branch")))
        //    {
        //        if ((col["fldBranchCode"].Equals("")))
        //        {
        //            err.Add(Locale.Pleaseselectabranch);
        //        }
        //    }
        //    else if ((col["userType"].Equals("ccu")))
        //    {
        //        if (col.AllKeys.Contains("userType2"))
        //        {
        //            if ((col["userType2"].Equals("")))
        //            {
        //                err.Add(Locale.PleaseSelectCCDType);
        //            }
        //            else if ((col["userType2"].Equals("N")))
        //            {
        //                if (col.AllKeys.Contains("SelectUserOfficer"))
        //                {
        //                    if ((col["SelectUserOfficer"].Equals("")))
        //                    {
        //                        err.Add(Locale.Pleaseselectofficer);
        //                    }
        //                }
        //                else
        //                {
        //                    err.Add(Locale.Pleaseselectofficer);
        //                }

        //            }
        //        }
        //        else
        //        {
        //            if (!(string.IsNullOrEmpty(col["userType2"])))
        //            {
        //                err.Add(Locale.PleaseSelectCCDType);
        //            }

        //        }
        //    }
        //    return err;
        //}
        public Dictionary<string, string> GetTasksIdsForUser(string userId, string[] groupIds)
        {
            //oyj 20040914
            Dictionary<string, string> taskArray = new Dictionary<string, string>();

            //string mySql = "SELECT * FROM tblGroupTask a, tblGroupUser b, tblUserMaster c , tblTaskMaster t WHERE a.fldGroupId = b.fldGroupId AND b.fldUserId = c.fldUserId AND b.fldGroupId in (@groups) AND b.fldUserId = @userId AND c.fldDisableLogin = 'N' AND t.fldTaskId in (a.fldTaskId) ";  //Commented by Michelle 20200518

            string mySql = "SELECT * FROM tblGroupTask a, tblGroupUser b, tblUserMaster c , tblTaskMaster t WHERE a.fldGroupCode = b.fldGroupCode AND b.fldUserId = c.fldUserId AND b.fldGroupCode in (@groups) AND b.fldUserId = @userId AND c.fldDisableLogin = 'N' AND t.fldTaskId in (a.fldTaskId) ";

            try
            {
                DataTable dataTable = dbContext.GetRecordsAsDataTable(mySql, new[] {
                    new SqlParameter("@userId" , userId),
                    new SqlParameter("@groups" , string.Join(",", groupIds).TrimEnd(','))
                });

                foreach (DataRow row in dataTable.Rows)
                {
                    taskArray.Add(row["fldTaskId"].ToString(), row["fldMvcUrl"].ToString());
                }
                taskArray.Add("all", "");

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return taskArray;
        }
        public string[] GetUserBranchCodes(string userId)
        {
            string mySQL = null;
            string[] resultArray = new string[1];
            mySQL = "SELECT fldBranchCode FROM dbo.view_hubbranch WITH (nolock) WHERE fldUserId = @userId ";

            try
            {
                DataTable dataTable = dbContext.GetRecordsAsDataTable(mySQL, new[] { new SqlParameter("@userId", userId) });
                resultArray = new string[dataTable.Rows.Count + 1];
                Int32 i = 0;
                foreach (DataRow row in dataTable.Rows)
                {
                    resultArray[i] = row["fldBranchCode"].ToString();
                    i = i + 1;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

            }
            return resultArray;
        }
        public string GetClearingDate()
        {
            string clearingDate = "";
            string stmt = "Select top 1 fldcleardate from tblinwardcleardate order by fldcleardate desc";
            DataTable configTable = dbContext.GetRecordsAsDataTable(stmt);
            if (configTable.Rows.Count > 0)
            {
                clearingDate = configTable.Rows[0]["fldcleardate"].ToString();
            }
            else
            {
                clearingDate = DateTime.Today.ToString();
            }


            return DateUtils.formatDateFromSql(clearingDate);
        }
        /*public string GetOCSClearingDate()
        {
            string clearingDate = "";
            string stmt = "Select top 1 fldprocessdate from tblprocessdate where fldstatus='Y' order by fldprocessdate desc";
            DataTable configTable = dbContext.GetRecordsAsDataTable(stmt);
            if (configTable.Rows.Count > 0)
            {
                clearingDate = configTable.Rows[0]["fldprocessdate"].ToString();
            }
            else
            {
                clearingDate = DateTime.Today.ToString();
            }


            return DateUtils.formatDateFromSql(clearingDate);
        }*/
        public bool CheckUserInLDAP(string userAbbr)
        {

            //string domainLDAP = systemProfileDao.GetValueFromSystemProfileWithoutCurrentUser("DomainLDAP").Trim();
            string ADDomain = securityProfileDao.GetSecurityProfile().fldUserADDomain;

            if (string.IsNullOrEmpty(ADDomain) || ADDomain.Equals(""))
            {
                return false;
            }
            else
            {

                try
                {
                string stmt = string.Format("SELECT * FROM 'LDAP://{0}' WHERE objectClass='*' AND sAMAccountName =@sAMAccountName", ADDomain);
                    return dbContext.CheckExist(stmt, new[] { new SqlParameter("@sAMAccountName", userAbbr) }); ;
                }
                catch (Exception ex)
                {
                    return false;
                }


            }
        }
        public string GetPasswordInLDAP(string userAbbr)
        {

            string result = "";


            //string domainLDAP = systemProfileDao.GetValueFromSystemProfileWitho
            //utCurrentUser("DomainLDAP").Trim();
            string ADDomain = securityProfileDao.GetSecurityProfile().fldUserADDomain;
            DataTable dt;
            if (string.IsNullOrEmpty(ADDomain) || ADDomain.Equals(""))
            {
                result = "";
            }
            else
            {
                try
                {
                    string stmt = string.Format("SELECT * FROM 'LDAP://{0}' WHERE objectClass='*' AND sAMAccountName =@sAMAccountName", ADDomain);
                    dt = dbContext.GetRecordsAsDataTable(stmt, new[] { new SqlParameter("@sAMAccountName", userAbbr) });
                    if (dt.Rows.Count > 0)
                    {
                        result = dt.Rows[0]["userPassword"].ToString();
                    }
                }
                catch (Exception ex)
                {
                    result ="";
                }
            }
           

    

            return result;
        }
        public DateTime FormatExpDate(string expiryDate)
        {//99D
            DateTime result = new DateTime();
            string expiryDigit = new string(expiryDate.Where(char.IsDigit).ToArray());
            string expiryStr = StringUtils.UCase(StringUtils.Right(expiryDate.Trim(), 1).ToString());

            if (expiryStr == "D")
            {
                result = DateTime.Now.AddDays(Convert.ToDouble(expiryDigit));
            }
            else if (expiryStr == "M")
            {
                result = DateTime.Now.AddMonths(Convert.ToInt16(expiryDigit));
            }
            else if (expiryStr == "Y")
            {
                result = DateTime.Now.AddYears(Convert.ToInt16(expiryDigit));
            }
            return result;
        }
        public string decryptPassword(string encryptedPassword)
        {
            string result = "";
            try
            {
                ICSSecurity.EncryptDecrypt encryptDecrypt = new ICSSecurity.EncryptDecrypt();
                encryptDecrypt.FilePath = systemProfileDao.GetDLLPath();
                result = encryptDecrypt.DecryptString128Bit(encryptedPassword);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        public string encryptPassword(string stringPassword)
        {
            string result = "";
            try
            {
                ICSSecurity.EncryptDecrypt encryptDecrypt = new ICSSecurity.EncryptDecrypt();
                encryptDecrypt.FilePath = systemProfileDao.GetDLLPath();
                result = encryptDecrypt.EncryptString128Bit(stringPassword);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

    }
}