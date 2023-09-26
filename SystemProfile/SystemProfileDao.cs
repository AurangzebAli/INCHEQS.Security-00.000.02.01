using INCHEQS.DataAccessLayer;
//using INCHEQS.Models;
using INCHEQS.Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using INCHEQS.Security.User;

namespace INCHEQS.Security.SystemProfile
{
    public class SystemProfileDao : ISystemProfileDao
    {
        private readonly ApplicationDbContext dbContext;
        public SystemProfileDao(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        //modify by shamil 20170315 for a function to write log file
        public void Log(string logMessage,string Indicator,string Logpath,string userID, string userAbbr, string user)
        {
            if (Indicator == "Y")//(CurrentUser.Account.Logindicator == "Y")
            {
                string path = "";
                path = Logpath;//CurrentUser.Account.LogPath;
                if (String.IsNullOrEmpty(userID))//CurrentUser.Account.UserId))
                {
                    path = path + @"\" + user + ".log";
                }
                else
                {
                    path = path + @"\" + userAbbr/*CurrentUser.Account.UserAbbr*/ + ".log";
                }
                using (StreamWriter w = File.AppendText(path))
                {
                    w.WriteLine(logMessage);
                }
            }
        }
        // end modify
        public string GetValueFromSystemProfile(string systemProfileCode, string strBankCode)
        {
            string result = "";
            try
            {

                DataTable dataResult = new DataTable();
                string stmt = "Select fldSystemProfileValue from tblSystemProfile where fldBankCode=@fldBankCode and fldSystemProfileCode=@fldSystemProfileCode";
                dataResult = dbContext.GetRecordsAsDataTable(stmt, new[] {
                    new SqlParameter("@fldBankCode", strBankCode),
                    new SqlParameter("@fldSystemProfileCode", systemProfileCode)
                });

                if (dataResult.Rows.Count > 0)
                {
                    result = dataResult.Rows[0]["fldSystemProfileValue"].ToString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public string GetValueFromSystemProfileWithoutCurrentUser(string systemProfileCode)
        {
            string result = "";
            try
            {

                DataTable dataResult = new DataTable();
                string stmt = "Select fldSystemProfileValue from tblSystemProfile where  fldSystemProfileCode=@fldSystemProfileCode";
                dataResult = dbContext.GetRecordsAsDataTable(stmt, new[] {
                    new SqlParameter("@fldSystemProfileCode", systemProfileCode)
                });

                if (dataResult.Rows.Count > 0)
                {
                    result = dataResult.Rows[0]["fldSystemProfileValue"].ToString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public string GetDLLPath()
        {
            string result = "";
            try
            {

                DataTable dataResult = new DataTable();
                string stmt = "Select fldSystemProfileValue from tblSystemProfile where fldSystemProfileCode = 'DllPath'";
                dataResult = dbContext.GetRecordsAsDataTable(stmt);

                if (dataResult.Rows.Count > 0)
                {
                    result = dataResult.Rows[0]["fldSystemProfileValue"].ToString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public string GetLogoPath(string bankcode)
        {
            string result = "";
            try
            {

                DataTable dataResult = new DataTable();
                string stmt = "Select fldSystemProfileValue from tblSystemProfile where fldSystemProfileCode = 'LogoPath' and fldBankCode=@fldBankCode";
                dataResult = dbContext.GetRecordsAsDataTable(stmt, new[] {
                    new SqlParameter("@fldBankCode", bankcode)
                });
                if (dataResult.Rows.Count > 0)
                {
                    result = dataResult.Rows[0]["fldSystemProfileValue"].ToString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        public string GetValueFromInterfaceFileMasterICS(string interfaceFile, string strBankCode)
        {
            string result = "";
            try
            {
                DataTable dataResult = new DataTable();
                string stmt = "Select fldInterfaceFileSourcePath from tblInterfaceFileMasterICS where fldBankCode=@fldBankCode and fldInterfaceFile=@fldInterfaceFile";
                dataResult = dbContext.GetRecordsAsDataTable(stmt, new[] {
                    new SqlParameter("@fldBankCode", strBankCode),
                    new SqlParameter("@fldInterfaceFile", interfaceFile)
                });
                if (dataResult.Rows.Count > 0)
                {
                    result = dataResult.Rows[0]["fldInterfaceFileSourcePath"].ToString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
    }
}