using System;
using INCHEQS.Security.Account;


namespace INCHEQS.Security.AuditTrail {
    public interface IAuditTrailDao {
        void Log(string remarks , AccountModel currentUserAccount);
        void SecurityLog(string ActionPerformed, string ActionDetails, string TaskId, AccountModel currentUserAccount = null);
    }
}