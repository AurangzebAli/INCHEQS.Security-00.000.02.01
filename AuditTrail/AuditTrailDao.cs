using INCHEQS.Common;
using INCHEQS.Security.Account;
using System.Collections.Generic;
using INCHEQS.DataAccessLayer;
//using INCHEQS.DataAccessLayer.ApplicationDbContext;
using System.Data.SqlClient;
using System.Text;
using System;

namespace INCHEQS.Security.AuditTrail {
    public class AuditTrailDao : IAuditTrailDao {

        private readonly ApplicationDbContext dbContext;
        public AuditTrailDao(ApplicationDbContext dbContext) {
            this.dbContext = dbContext;
        }

        public void Log(string remarks, AccountModel currentUserAccount = null) {
            Dictionary<string, dynamic> keyValue = new Dictionary<string, dynamic>();
            if (currentUserAccount != null) {
                keyValue.Add("fldUserId", StringUtils.Trim(currentUserAccount.UserId));
                keyValue.Add("fldBankCode", StringUtils.Trim(currentUserAccount.BankCode));
                keyValue.Add("fldTaskId", StringUtils.Trim(currentUserAccount.TaskId));
                keyValue.Add("fldStatus", StringUtils.Trim(currentUserAccount.Status));
            } else {
                keyValue.Add("fldUserId", "");
                keyValue.Add("fldBankCode", "");
            }
            keyValue.Add("fldRemarks", remarks);
            keyValue.Add("fldCreateTimeStamp", DateUtils.GetCurrentDatetimeForSql());

            dbContext.ConstructAndExecuteInsertCommand("tblAuditTrail", keyValue);
        }

        public void SecurityLog(string ActionPerformed, string ActionDetails,string TaskId, AccountModel currentUserAccount = null)
        {
            Dictionary<string, dynamic> keyValue = new Dictionary<string, dynamic>();
            if (currentUserAccount != null)
            {
                keyValue.Add("fldBankCode", StringUtils.Trim(currentUserAccount.BankCode));
                keyValue.Add("fldTaskId", StringUtils.Trim(TaskId));
            }
            else
            {
                keyValue.Add("fldBankCode", "");
                keyValue.Add("fldTaskId", StringUtils.Trim(TaskId));
            }

            keyValue.Add("fldActionPerformed", StringUtils.Trim(ActionPerformed));
            keyValue.Add("fldActionDetails", StringUtils.Trim(ActionDetails));
            keyValue.Add("fldCreateUserId", StringUtils.Trim(currentUserAccount.UserId));
            keyValue.Add("fldCreateTimeStamp", DateUtils.GetCurrentDatetimeForSql());

            dbContext.ConstructAndExecuteInsertCommand("tblAuditLog", keyValue);
        }

       

        
    }

}