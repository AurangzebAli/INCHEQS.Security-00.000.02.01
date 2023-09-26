
using System.Collections.Generic;
using INCHEQS.Common;

namespace INCHEQS.Security.Account
{
    public class AccountModel
    {
        public string UserId { get; set; }
        
        public string BranchCode { get; set; }
        public string BranchCode2 { get; set; }
        public string BranchCode3 { get; set; }
        public string BranchDesc { get; set; }

        public string UserAbbr { get; set; }

        public string UserEmail { get; set; }

        public string UserType { get; set; }

        public string UserBankType { get; set; }

        public string VerificationClass { get; set; }

        public string VerificationLimit { get; set; }

        public string[] BranchCodes { get; set; }

        public string BankDesc { get; set; }

        public string LogPath { get; set; }

        public string Logindicator { get; set; }

        public string BankCode { get; set; }
        public string fldZeroMaker { get; set; }
        

        public string SystemID { get; set; }

        public string StateCode { get; set; }

        public string StateDesc { get; set; }

        public string SpickCode { get; set; }

        public string ClearingDate { get; set; }
        public string OCSClearingDate { get; set; }

        public int SessionTimeOut { get; set; }

        public string[] GroupIds { get; set; }

        public string LastLoginDate { get; set; }

        public string PasswordExpDate { get; set; }

        public string ApprovalBankCode { get; set; }


        public string LogoPath { get; set; }

        public Dictionary<string,string> TaskIds { get; set; }

        public string[] BranchHubCodes { get; set; }

        public string macAddress { get; set; }

        public string bicCode { get; set; }

        public string TaskId { get; set; }
        public string Status { get; set; }

        public AccountModel()
        {
            this.UserId = "";
            this.UserAbbr = "";
            this.UserEmail = "";
            this.UserType = "";
            this.VerificationClass = "";
            this.VerificationLimit = "";
            this.BankDesc = "";
            this.BankCode = "";
            this.SystemID = "";
            this.SpickCode = "";
            this.ClearingDate = "";
            //this.OCSClearingDate = "";
            this.SessionTimeOut = 0;
            this.GroupIds = new string[0];
            this.TaskIds = new Dictionary<string, string>();
            this.BranchCodes = new string[1];
            this.BranchCode = "";
            this.BranchCode2 = "";
            this.BranchCode3 = "";
            this.LastLoginDate = "";
            this.PasswordExpDate = "";
            this.LogPath = "";
            this.ApprovalBankCode = "";
            this.LogoPath = "";
            this.Logindicator = "";
            this.BranchHubCodes = new string[0] ;
            this.macAddress = "";
            this.bicCode = "";
            this.TaskId = "";

        }

        public Dictionary<string, string> ToDictionary()
        {
            Dictionary<string, string> userParams = new Dictionary<string, string>();
            userParams.Add("currentUserId", this.UserId);
            userParams.Add("currentUserAbbr", this.UserAbbr);
            userParams.Add("currentUserEmail", this.UserEmail);
            userParams.Add("currentUserType", this.UserType);
            userParams.Add("currentUserVerificationClass", this.VerificationClass);
            userParams.Add("currentUserVerificationLimit", this.VerificationLimit);
            userParams.Add("currentUserTaskIds", string.Join(",", this.TaskIds));
            userParams.Add("currentUsergroupIds", string.Join(",", this.GroupIds));
            userParams.Add("currentUserBankCode", this.BankCode);
            userParams.Add("currentUserSystemID", this.SystemID);
            userParams.Add("currentLogindicator", this.Logindicator);
            userParams.Add("currentLogPath", this.LogPath);
            userParams.Add("currentApprovalBankCode", this.ApprovalBankCode);
            userParams.Add("currentLogoPath", this.LogoPath);
            userParams.Add("currentUserBranchCode", this.BranchCode);
            userParams.Add("currentUserBranchCodes", string.Join(",", this.BranchCodes));
            userParams.Add("currentUserBranchHubCodes", string.Join(",", this.BranchHubCodes));
            userParams.Add("currentmacAddress", this.macAddress);
            userParams.Add("currentbicCode", this.bicCode);
            userParams.Add("currentUserTaskId", this.TaskId);
            userParams.Add("stateCode", this.StateCode);
            return userParams;
        }

        public bool isBranchUser() {
            bool result = false;
            if(BranchCodes.Length > 0) {
                result = !string.IsNullOrEmpty(StringUtils.Trim(BranchCodes[0]));
            }
            return result;
        }        

        public List<string> TaskIdsToList() {
            return new List<string>(TaskIds.Keys);
        }
              
    }
}