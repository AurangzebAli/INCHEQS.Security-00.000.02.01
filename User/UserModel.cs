﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace INCHEQS.Security.User
{
    public class UserModel
    {

        public string fldUserId { get; set; }
        public string fldUserAbb { get; set; }
        public string fldBPCSpick { get; set; }
        public string fldUserDesc { get; set; }
        public string fldPassword { get; set; }
        public string fldEmail { get; set; }
        public string fldBankType { get; set; }

        public string fldBankDesc { get; set; }
        public string fldBankCode { get; set; }
        public string fldVerificationLimit { get; set; }

        public string fldBranchDesc { get; set; }
        public string fldConBranchCode { get; set; }
        public string fldOfficerId { get; set; }
        public string fldAdminFlag { get; set; }
        public string fldCCUFlag { get; set; }

        public string fldStateCode { get; set; }
        public string fldStateDesc { get; set; }
        public string fldCity { get; set; }
        public string fldBranchCode { get; set; }
        public string fldBranchCode2 { get; set; }
        public string fldBranchCode3 { get; set; }

        public string fldDisableLogin { get; set; }
        public string fldZeroMaker { get; set; }
        
        public string fldFailLoginDate { get; set; }
        public string fldPasswordExpDate { get; set; }
        public string fldIDExpDate { get; set; }
        public string fldCreateTimeStamp { get; set; }

        public string userType { get; set; }
        public string userBankType { get; set; }
        public string status { get; set; }

        public string fldClass { get; set; }
        public string fldLimitDesc { get; set; }

        public string fldBranchCode1 { get; set; }
        public string fldBranchDesc1 { get; set; }

        public int fldIDExpStatus { get; set; }
        public int fldCounter { get; set; }
        public string fldLastLoginDate { get; set; }
        public string fldPassLastUpdDate { get; set; }
        public string fldSpickCode { get; set; }
        public string fldSpickDesc { get; set; }


        public string fldVerifyChequeFlag { get; set; }
        public string[] GroupIds { get; set; }

        public string fldUserAbbOfficer { get; set; }
        public string fldUserDescOfficer { get; set; }
        public string fldApprovalBankCode { get; set; }
        public string fldSystemID { get; set; }
        public List<string> fldBranchHubCodes { get; set; }
        public string macAddress { get; set; }
        public string bicCode { get; set; }
       
        public string fldUserAuthMethod { get; set; }
        public string fldUserAuthMethodDesc { get; set; }

    }


}