using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace INCHEQS.Security.SystemProfile {
    public interface ISystemProfileDao {
        string GetValueFromSystemProfile(string systemProfileCode, string strBankCode);
        string GetValueFromSystemProfileWithoutCurrentUser(string systemProfileCode);
        string GetDLLPath();
        string GetLogoPath(string BankCode);
        void Log(string msg, string Indicator, string Logpath, string userID, string userAbbr, string filename = "SystemLog");
        string GetValueFromInterfaceFileMasterICS(string interfaceFile, string strBankCode);
    }
}