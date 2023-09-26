
using System;
using INCHEQS.DataAccessLayer;
using System.Data;
//using INCHEQS.DataAccessLayer;
using System.Data.SqlClient;
using System.Data.Entity;
using INCHEQS.Common;
using INCHEQS.DataAccessLayer.OCS;
using System.Collections.Generic;
//using INCHEQS.ViewModel;

namespace INCHEQS.Security.UserSession {
    public class UserSessionDao : IUserSessionDao
    {

        private readonly ApplicationDbContext dbContext;
        private readonly OCSDbContext ocsDbContext;

        public UserSessionDao(ApplicationDbContext dbContext, OCSDbContext ocsDbContext)
        {
            this.dbContext = dbContext;
            this.ocsDbContext = ocsDbContext;
        }
        public void ClearSession(string sSessionUserID)
        {
            List<SqlParameter> sqlParameterNext = new List<SqlParameter>();
            sqlParameterNext.Add(new SqlParameter("@UserSessionId", sSessionUserID));
            this.dbContext.ExecuteNonQuery(CommandType.StoredProcedure, "spcdClearSession", sqlParameterNext.ToArray());

        }
        public DataTable ListAll(string userId)
        {
            DataTable dataTable = new DataTable();
            List<SqlParameter> sqlParameterNext = new List<SqlParameter>();
            sqlParameterNext.Add(new SqlParameter("@fldBankCode", userId));
            return this.dbContext.GetRecordsAsDataTableSP("spcgListAll", sqlParameterNext.ToArray());

        }
        public void DeleteSessionForUser(string userId, string sessionId)
        {

            List<SqlParameter> sqlParameterNext = new List<SqlParameter>();
            try
            {
                sqlParameterNext.Add(new SqlParameter("@UserId", userId));
                sqlParameterNext.Add(new SqlParameter("@SessionId", sessionId));
                this.dbContext.ExecuteNonQuery(CommandType.StoredProcedure, "spcdSessionForUser", sqlParameterNext.ToArray());


            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public bool HasManyAndNotAllowedConcurrent(string userId, string sessionId)
        {
            bool strs = false;
            DataTable dataTable = new DataTable();
            List<SqlParameter> sqlParameterNext = new List<SqlParameter>();
            sqlParameterNext.Add(new SqlParameter("@UserId", userId));
            sqlParameterNext.Add(new SqlParameter("@SessionId", sessionId));
            dataTable = dbContext.GetRecordsAsDataTableSP("spcgHasManyAndNotAllowedConcurrent", sqlParameterNext.ToArray());


            if ((Convert.ToInt32(dataTable.Rows[0]["NCConnection"]) != 1 ? false : Convert.ToInt32(dataTable.Rows[0]["count"]) > 1))
            {
                strs = true;
            }
            if (Convert.ToInt32(dataTable.Rows[0]["count"]) == 0)
            {
                strs = true;
            }
            return strs;
        }

        public void InsertSessionToOcs(string userId, string userSessionId)
        {
            DataTable dataTable = new DataTable();
            List<SqlParameter> sqlParameterNext = new List<SqlParameter>();
            sqlParameterNext.Add(new SqlParameter("@UserId", userId));
            sqlParameterNext.Add(new SqlParameter("@SessionId", userSessionId));
            dataTable = dbContext.GetRecordsAsDataTableSP("spcgInsertSessionToOcs", sqlParameterNext.ToArray());

            if (dataTable.Rows.Count > 0)
            {
                DataRow item = dataTable.Rows[0];
                string str1 = item["fldUserId"].ToString();
                string str2 = item["fldSessionId"].ToString();
                List<SqlParameter> sqlParameterNext2 = new List<SqlParameter>();
                sqlParameterNext2.Add(new SqlParameter("@UserId", str1));
                sqlParameterNext2.Add(new SqlParameter("@SessionId", str2));
                this.dbContext.ExecuteNonQuery(CommandType.StoredProcedure, "spciInsertSessionToOcs", sqlParameterNext2.ToArray());

            }
        }

        public bool isLoginSessionExceededAndRefresh(string userId, string sessionId)
        {

            bool flag;
            DataTable dataTable = new DataTable();
            List<SqlParameter> sqlParameterNext = new List<SqlParameter>();
            sqlParameterNext.Add(new SqlParameter("@UserId", userId));
            sqlParameterNext.Add(new SqlParameter("@SessionId", sessionId));
            dataTable = dbContext.GetRecordsAsDataTableSP("spcgisLoginSessionExceededAndRefresh", sqlParameterNext.ToArray());

            if (dataTable.Rows.Count <= 0)
            {
                flag = true;
            }
            else
            {
                flag = (Convert.ToInt32(dataTable.Rows[0]["isSessionExceeded"]) != 0 ? true : false);

                if (flag == false)
                {
                    List<SqlParameter> sqlParameterNext2 = new List<SqlParameter>();
                    sqlParameterNext2.Add(new SqlParameter("@UserId", userId));
                    sqlParameterNext2.Add(new SqlParameter("@SessionId", sessionId));
                    this.dbContext.ExecuteNonQuery(CommandType.StoredProcedure, "spcuisLoginSessionExceededAndRefresh", sqlParameterNext2.ToArray());
                }

            }
            return flag;
        }

        public bool IsMultipleUserLoggedInAndConcurrentNotAllowed(string userId, string isAllowedConcurrentConnection, int sessionTimeoutInMinutes)
        {
            bool flag = false;
            DataTable dataTable = new DataTable();
            List<SqlParameter> sqlParameterNext = new List<SqlParameter>();
            sqlParameterNext.Add(new SqlParameter("@UserId", userId));
            dataTable = dbContext.GetRecordsAsDataTableSP("spcgIsMultipleUserLoggedInAndConcurrentNotAllowed", sqlParameterNext.ToArray());

            int count = dataTable.Rows.Count;
            if (count > 0)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    if ("Y".Equals(row["exceededTimeout"]))
                    {
                        string str1 = row["fldSessionId"].ToString();
                        List<SqlParameter> sqlParameterNext2 = new List<SqlParameter>();
                        sqlParameterNext2.Add(new SqlParameter("@UserId", userId));
                        sqlParameterNext2.Add(new SqlParameter("@SessionId", str1));
                        this.dbContext.ExecuteNonQuery(CommandType.StoredProcedure, "spcdIsMultipleUserLoggedInAndConcurrentNotAllowed", sqlParameterNext2.ToArray());

                        count--;
                    }
                }
            }
            if ((isAllowedConcurrentConnection != "N" ? false : count > 0))
            {
                flag = true;
            }
            return flag;
        }

        public void updateSessionTrack(string userId, string userSessionId)
        {
            DataTable dataTable = new DataTable();
            List<SqlParameter> sqlParameterNext = new List<SqlParameter>();
            sqlParameterNext.Add(new SqlParameter("@UserId", userId));
            sqlParameterNext.Add(new SqlParameter("@SessionId", userSessionId));

            if (this.dbContext.ExecuteNonQuery(CommandType.StoredProcedure, "spcuupdateSessionTrack", sqlParameterNext.ToArray()) == 0)
            {
                List<SqlParameter> sqlParameterNext2 = new List<SqlParameter>();
                sqlParameterNext2.Add(new SqlParameter("@UserId", userId));
                sqlParameterNext2.Add(new SqlParameter("@SessionId", userSessionId));
                this.dbContext.ExecuteNonQuery(CommandType.StoredProcedure, "spciupdateSessionTrack", sqlParameterNext2.ToArray());
            }
            }

        }
    }