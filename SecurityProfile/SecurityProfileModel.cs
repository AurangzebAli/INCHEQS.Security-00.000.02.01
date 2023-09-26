using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
public class SecurityProfileModel {
    
    public Int32 fldSecurityId { get; set; }
    public string fldUserAuthMethod { get; set; }
    public Int32 fldUserIdLengthMin { get; set; }
    public Int32 fldUserIdLengthMax { get; set; }
    public Int32 fldUserLoginAttempt { get; set; }
    public string fldUserCNCR { get; set; }
    public Int32 fldUserSessionTimeOut { get; set; }
    public string fldDualApproval { get; set; }
    public string fldUserADDomain { get; set; }
    public Int32 fldUserAcctExpiry { get; set; }
    public string fldUserAcctExpiryInt { get; set; }
    public Int32 fldUserPwdLengthMin { get; set; }
    public Int32 fldUserPwdLengthMax { get; set; }
    public Int32 fldUserPwdHisRA { get; set; }
    public Int32 fldUserPwdExpiry { get; set; }
    public string fldUserPwdExpiryInt { get; set; }
    public Int32 fldUserPwdNotification { get; set; }
    public string fldUserPwdNotificationInt { get; set; }
    public string fldUserPwdExpAction { get; set; }
    public string fldUserAuthMethodDesc { get; set; }
    public string fldSecurityValue { get; set; }
    public Int32 fldPwdChangeTime { get; set; }
    

}