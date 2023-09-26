using INCHEQS.Common;
using INCHEQS.Security.User;
using INCHEQS.Security.Resources;
using INCHEQS.Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using INCHEQS.DataAccessLayer;
//using INCHEQS.DataAccessLayer;
using INCHEQS.Security.Group;



namespace INCHEQS.Security.Group {
    public class GroupDao: IGroupDao {

        private readonly ApplicationDbContext dbContext;
        public GroupDao(ApplicationDbContext dbContext) {
            this.dbContext = dbContext;
        }


        public DataTable ListAllGroup(string strBankCode) {
            DataTable dataTable = new DataTable();
            string stmt = "SELECT * FROM tblGroupMaster WHERE fldBankCode =@fldBankCode ";
            
                dataTable = dbContext.GetRecordsAsDataTable( stmt, new[] {
                    new SqlParameter("@fldBankCode", strBankCode)
            });
        
            return dataTable;
        }

        public GroupModel GetGroup(string groupId) {
            string stmt = "Select * FROM tblGroupMaster WHERE fldGroupCode=@fldGroupCode";
            GroupModel strGroup = new GroupModel();
                    try {
                    DataTable ds = dbContext.GetRecordsAsDataTable( stmt, new[] { new SqlParameter("@fldGroupCode", groupId) });
                
                            if (ds.Rows.Count > 0) {
                                DataRow row = ds.Rows[0];
                                string groupid = row["fldGroupCode"].ToString().Trim();
                                strGroup.fldGroupId = groupid.Remove(groupid.Length - 4);
                                strGroup.fldGroupDesc = row["fldGroupDesc"].ToString();
                                //strGroup.fldBranchGroup = "0";//row["fldBranchGroup"].ToString();
                }
                       
                    } catch (Exception ex) {
                        throw ex;
                    } 
                
            
            return strGroup;
        }

        public GroupModel GetGroup(string groupId, string strBankCode)
        {
            string stmt = "Select * FROM tblGroupMaster WHERE fldGroupCode=@fldGroupCode AND fldBankCode = @fldBankCode";
            GroupModel strGroup = new GroupModel();
            try
            {
                SqlParameter[] cmdParms = new SqlParameter[2];

                cmdParms[0] = new SqlParameter("@fldGroupCode", groupId);
                cmdParms[1] = new SqlParameter("@fldBankCode", strBankCode);
                DataTable ds = dbContext.GetRecordsAsDataTable(stmt, cmdParms);

                if (ds.Rows.Count > 0)
                {
                    DataRow row = ds.Rows[0];
                    string groupid = row["fldGroupCode"].ToString().Trim();
                    //strGroup.fldGroupId = groupid.Remove(groupid.Length - 4);
                    strGroup.fldGroupId = groupid;
                    strGroup.fldGroupDesc = row["fldGroupDesc"].ToString();
                    //strGroup.fldBranchGroup = "0";//row["fldBranchGroup"].ToString();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }


            return strGroup;
        }

        public List<UserModel> ListAvailableUserInGroup(string strBankCode) {
            List<UserModel> userList = new List<UserModel>();
            string stmt = " select fldUserId,fldUserAbb,fldUserDesc from tblUsermaster Where fldBankCode = @fldBankCode AND fldUserId not in (select fldUserId from tblGroupUser where fldBankCode = @fldBankCode ) and fldApproveStatus = 'Y' ";

            try {
                DataTable ds = new DataTable();
                ds = dbContext.GetRecordsAsDataTable(stmt, new[] {
                    new SqlParameter("@fldBankCode", strBankCode)
                });
                if (((ds.Rows.Count > 0))) {
                    foreach (DataRow row in ds.Rows) {
                        UserModel user = new UserModel();
                        user.fldUserId = row["fldUserId"].ToString();
                        user.fldUserAbb = row["fldUserAbb"].ToString();
                        userList.Add(user);
                    }
                }
            } catch (Exception ex) {
                throw ex;
            }
            return userList;
        }

        public List<UserModel> ListSelectedUserInGroup(string groupId) {
            string stmt = " select gu.fldUserId, um.fldUserAbb from tblGroupUser gu inner join tblUserMaster um on gu.fldUserID = um.fldUserID where gu.fldGroupCode = @fldGroupCode order by um.fldUserAbb";
            List<UserModel> userList = new List<UserModel>();
            try {
                DataTable ds = new DataTable();
                ds = dbContext.GetRecordsAsDataTable(stmt, new[] { new SqlParameter("@fldGroupCode", groupId) });

                if (ds.Rows.Count > 0) {
                    foreach (DataRow row in ds.Rows) {
                        UserModel user = new UserModel();
                        user.fldUserId = row["fldUserId"].ToString();
                        user.fldUserAbb = row["fldUserAbb"].ToString();
                        userList.Add(user);
                    }
                }

            } catch (Exception ex) {
                throw ex;
            } 
            return userList;
        }

        public List<UserModel> ListSelectedUserInGroup(string groupId, string strBankCode)
        {
            string stmt = " select gu.fldUserId, um.fldUserAbb from tblGroupUser gu inner join tblUserMaster um on gu.fldUserID = um.fldUserID where gu.fldGroupCode = @fldGroupCode and gu.fldBankCode = @fldBankCode order by um.fldUserAbb";
            List<UserModel> userList = new List<UserModel>();
            try
            {
                DataTable ds = new DataTable();

                SqlParameter[] cmdParms = new SqlParameter[2];

                cmdParms[0] = new SqlParameter("@fldGroupCode", groupId);
                cmdParms[1] = new SqlParameter("@fldBankCode", strBankCode);

                ds = dbContext.GetRecordsAsDataTable(stmt, cmdParms);

                if (ds.Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Rows)
                    {
                        UserModel user = new UserModel();
                        user.fldUserId = row["fldUserId"].ToString();
                        user.fldUserAbb = row["fldUserAbb"].ToString();
                        userList.Add(user);
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return userList;
        }

        public void DeleteGroup(string groupId) {
            string stmt = " Delete from tblGroupMaster where fldGroupCode=@fldGroupCode";

            try {
                dbContext.ExecuteNonQuery(stmt, new[] {new SqlParameter("@fldGroupCode", groupId)
                });
            } catch (Exception ex) {
                throw ex;
            } 
            
        }

        public void DeleteGroup(string groupId, string strBankCode)
        {
            string stmt = " Delete from tblGroupMaster where fldGroupCode=@fldGroupCode and fldBankCode = @fldBankCode";

            try
            {
                SqlParameter[] cmdParms = new SqlParameter[2];

                cmdParms[0] = new SqlParameter("@fldGroupCode", groupId);
                cmdParms[1] = new SqlParameter("@fldBankCode", strBankCode);
                dbContext.ExecuteNonQuery(stmt, cmdParms);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void DeleteAllUserInGroup(string groupId) {
            string stmt = " Delete from tblGroupUser where fldGroupCode=@fldGroupCode";
            try {
                dbContext.ExecuteNonQuery(stmt, new[] { new SqlParameter("@fldGroupCode", groupId) });

            } catch (Exception ex) {
                throw ex;
            }
        }

        public void DeleteAllUserInGroup(string groupId, string strBankCode)
        {
            string stmt = " Delete from tblGroupUser where fldGroupCode=@fldGroupCode and fldBankCode = @fldBankCode";
            try
            {
                SqlParameter[] cmdParms = new SqlParameter[2];

                cmdParms[0] = new SqlParameter("@fldGroupCode", groupId);
                cmdParms[1] = new SqlParameter("@fldBankCode", strBankCode);
                dbContext.ExecuteNonQuery(stmt, cmdParms);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteAllTaskInGroup(string groupId, string strBankCode) {
            string stmt = " Delete from tblGroupTask where fldGroupCode=@fldGroupCode and fldBankCode = @fldBankCode";
            try {
                SqlParameter[] cmdParms = new SqlParameter[2];

                cmdParms[0] = new SqlParameter("@fldGroupCode", groupId);
                cmdParms[1] = new SqlParameter("@fldBankCode", strBankCode);
                dbContext.ExecuteNonQuery(stmt, cmdParms);

            } catch (Exception ex) {
                throw ex;
            } 
        }

        public void DeleteAllTaskInGroup(string groupId)
        {
            string stmt = " Delete from tblGroupTask where fldGroupCode=@fldGroupCode";
            try
            {
                dbContext.ExecuteNonQuery(stmt, new[] { new SqlParameter("@fldGroupCode", groupId) });

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteUserNotSelected(string groupId, string userIds) {
            string[] aryText = userIds.Split(',');
            if ((aryText.Length > 0)) {
                string stmt = "Delete from tblGroupUser where fldGroupCode=@fldGroupCode AND fldUserId not in (" + DatabaseUtils.getParameterizedStatementFromArray(aryText) + ")";

                List<SqlParameter> sqlParams = DatabaseUtils.getSqlParametersFromArray(aryText);
                sqlParams.Add(new SqlParameter("@fldGroupCode", groupId));

                try {
                    dbContext.ExecuteNonQuery(stmt, sqlParams.ToArray());
                } catch (Exception ex) {
                    throw ex;
                } 
            }
        }

        public void DeleteUserNotSelected(string groupId, string userIds, string strBankCode)
        {
            string[] aryText = userIds.Split(',');
            if ((aryText.Length > 0))
            {
                string stmt = "Delete from tblGroupUser where fldGroupCode=@fldGroupCode and fldBankCode = @fldBankCode AND fldUserId not in (" + DatabaseUtils.getParameterizedStatementFromArray(aryText) + ")";

                List<SqlParameter> sqlParams = DatabaseUtils.getSqlParametersFromArray(aryText);
                sqlParams.Add(new SqlParameter("@fldGroupCode", groupId));
                sqlParams.Add(new SqlParameter("@fldBankCode", strBankCode));
                try
                {
                    dbContext.ExecuteNonQuery(stmt, sqlParams.ToArray());
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }


        public bool CheckUserExistInGroup(string groupId, string userId) {
            string stmt = " SELECT * FROM tblGroupUser WHERE fldUserId=@fldUserId AND fldGroupCode=@fldGroupCode";

            try {
                return dbContext.CheckExist(stmt, new[] {
                    new SqlParameter("@fldUserId", userId),
                    new SqlParameter("@fldGroupCode", groupId)
                });
            } catch (Exception ex) {
                throw ex;

            }
        }

        public bool CheckUserExistInGroup(string groupId, string userId, string strBankCode)
        {
            string stmt = " SELECT * FROM tblGroupUser WHERE fldUserId=@fldUserId AND fldGroupCode=@fldGroupCode and fldBankCode = @fldBankCode";

            try
            {
                return dbContext.CheckExist(stmt, new[] {
                    new SqlParameter("@fldUserId", userId),
                    new SqlParameter("@fldGroupCode", groupId),
                    new SqlParameter("@fldBankCode", strBankCode)
                });
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

        public bool CheckGroupExist(string groupId,string StrBankCode) {
            string stmt = " SELECT * FROM tblGroupMaster WHERE fldGroupCode=@fldGroupCode and fldBankCode=@fldBankCode";
            try {
                return dbContext.CheckExist(stmt, new[] 
                { new SqlParameter("@fldGroupCode", groupId) ,
                  new SqlParameter("@fldBankCode", StrBankCode)
                });                
            } catch (Exception ex) {
                throw ex;
            }
        }

        public void UpdateSelectedUser(string groupId, string UserId, string strUpdate) {
            string stmt = " update tblGroupUser set fldUpdateUserId=@fldUpdateUserId, fldUpdateTimeStamp=@fldUpdateTimeStamp where fldGroupCode=@fldGroupCode AND fldUserId=@fldUserId";

            try {
                dbContext.ExecuteNonQuery(stmt, new[] {
                new SqlParameter("@fldUpdateUserId", strUpdate),
                new SqlParameter("@fldUpdateTimeStamp", DateTime.Now),
                new SqlParameter("@fldGroupCode", groupId),
                new SqlParameter("@fldUserId", UserId)});
            } catch (Exception ex) {
                throw ex;
            } 
        }

        public void UpdateSelectedUser(string groupId, string UserId, string strUpdate, string strBankCode)
        {
            string stmt = " update tblGroupUser set fldUpdateUserId=@fldUpdateUserId, fldUpdateTimeStamp=@fldUpdateTimeStamp where fldGroupCode=@fldGroupCode AND fldUserId=@fldUserId AND fldBankCode = @fldBankCode";

            try
            {
                dbContext.ExecuteNonQuery(stmt, new[] {
                new SqlParameter("@fldUpdateUserId", strUpdate),
                new SqlParameter("@fldUpdateTimeStamp", DateTime.Now),
                new SqlParameter("@fldGroupCode", groupId),
                new SqlParameter("@fldUserId", UserId),
                new SqlParameter("@fldBankCode", strBankCode)});
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void InsertUserInGroup(string groupId, string userId, string strUpdate, string strUpdateId) {
            string stmt = " insert into tblGroupUser (fldGroupCode,fldUserId,fldCreateUserId,fldCreateTimeStamp,fldUpdateUserId,fldUpdateTimeStamp) Values(@fldGroupCode,@fldUserId,@fldCreateUserId,@fldCreateTimeStamp,@fldUpdateUserId,@fldUpdateTimeStamp) ";
            UserModel user = new UserModel();
            try {
                dbContext.ExecuteNonQuery(stmt, new[] {
                new SqlParameter("@fldGroupCode", groupId),
                new SqlParameter("@fldUserId", userId),
                new SqlParameter("@fldCreateUserId", strUpdate),
                new SqlParameter("@fldCreateTimeStamp", DateTime.Now),
                new SqlParameter("@fldUpdateUserId", strUpdateId),
                new SqlParameter("@fldUpdateTimeStamp", DateTime.Now)});
            } catch (Exception ex) {
                throw ex;
            } 
        }

        public void InsertUserInGroup(string groupId, string userId, string strUpdate, string strUpdateId, string strBankCode)
        {
            string stmt = " insert into tblGroupUser (fldGroupCode,fldUserId,fldCreateUserId,fldCreateTimeStamp,fldUpdateUserId,fldUpdateTimeStamp, fldBankCode) Values(@fldGroupCode,@fldUserId,@fldCreateUserId,@fldCreateTimeStamp,@fldUpdateUserId,@fldUpdateTimeStamp,@fldBankCode) ";
            UserModel user = new UserModel();
            try
            {
                dbContext.ExecuteNonQuery(stmt, new[] {
                new SqlParameter("@fldGroupCode", groupId),
                new SqlParameter("@fldUserId", userId),
                new SqlParameter("@fldCreateUserId", strUpdate),
                new SqlParameter("@fldCreateTimeStamp", DateTime.Now),
                new SqlParameter("@fldUpdateUserId", strUpdateId),
                new SqlParameter("@fldUpdateTimeStamp", DateTime.Now),
                new SqlParameter("@fldBankCode", strBankCode)});
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateGroupMaster(FormCollection col,string strUpdate) {
            string stmt = "Update tblGroupMaster SET fldGroupDesc=@fldGroupDesc, fldUpdateUserId=@fldUpdateUserId, fldUpdateTimeStamp=@fldUpdateTimeStamp WHERE fldGroupCode=@fldGroupCode ";
            try {
                dbContext.ExecuteNonQuery(stmt, new[]{
                    new SqlParameter("@fldGroupDesc", col["fldGroupDesc"]),
                    new SqlParameter("@fldUpdateUserId", strUpdate),
                    new SqlParameter("@fldUpdateTimeStamp", DateTime.Now),
                    new SqlParameter("@fldGroupCode", col["fldGroupCode"] + "_" + strUpdate) });
            } catch (Exception ex) {
                throw ex;
            } 
        }

        public void CreateGroupMaster(FormCollection col,string strBankCode,string strUpdate, string spickCode, string strUpdateId) {
            string stmt = " INSERT INTO tblGroupMaster (fldGroupCode,fldGroupDesc,fldBankCode, fldSpickCode,fldCreateUserId, fldCreateTimeStamp,fldUpdateUserId,fldUpdateTimeStamp) VALUES(@fldGroupCode,@fldGroupDesc,@fldBankCode,@fldSpickCode,@fldCreateUserId,@fldCreateTimeStamp,@fldUpdateUserId,@fldUpdateTimeStamp)";

            try {
                dbContext.ExecuteNonQuery(stmt, new[]{
                    new SqlParameter("@fldGroupCode", col["fldGroupCode"] + "_" + strBankCode),
                    new SqlParameter("@fldGroupDesc", col["fldGroupDesc"]),
                    new SqlParameter("@fldBankCode", strBankCode),
                    new SqlParameter("@fldSpickCode", spickCode),
                    new SqlParameter("@fldCreateUserId", strUpdate),
                    new SqlParameter("@fldCreateTimeStamp", DateTime.Now),
                    new SqlParameter("@fldUpdateUserId", strUpdateId),
                    new SqlParameter("@fldUpdateTimeStamp", DateTime.Now) });
            } catch (Exception ex) {
                throw ex;
            } 
        }
        public void CreateGroupMasterTemp(FormCollection col, string strUpdate, string strBankCode, string spickCode, string strUpdateId) {
            string stmt = " INSERT INTO tblGroupMasterTemp (fldGroupCode,fldGroupDesc,fldBankCode, fldSpickCode,fldApprovalStatus,fldCreateUserId, fldCreateTimeStamp,fldUpdateUserId,fldUpdateTimeStamp) VALUES(@fldGroupCode,@fldGroupDesc,@fldBankCode,@fldSpickCode,@fldApprovalStatus,@fldCreateUserId,@fldCreateTimeStamp,@fldUpdateUserId,@fldUpdateTimeStamp)";
            try {
                dbContext.ExecuteNonQuery(stmt, new[]{
                    //new SqlParameter("@fldGroupCode", col["fldGroupCode"] + "_" + strBankCode),
                    new SqlParameter("@fldGroupCode", col["fldGroupCode"] ),
                    new SqlParameter("@fldGroupDesc", col["fldGroupDesc"]),
                    new SqlParameter("@fldBankCode", strBankCode),
                    new SqlParameter("@fldSpickCode", spickCode),
                    new SqlParameter("@fldApprovalStatus", "A"),
                    new SqlParameter("@fldCreateUserId", strUpdate),
                    new SqlParameter("@fldCreateTimeStamp", DateTime.Now),
                    new SqlParameter("@fldUpdateUserId", strUpdateId),
                    new SqlParameter("@fldUpdateTimeStamp", DateTime.Now) });
            } catch (Exception ex) {
                throw ex;
            } 
        }

        public void CreateInGroupMaster(string GroupId)
        {
            string stmt = "UPDATE tblGroupMasterTemp SET fldApprovalStatus=@fldApprovalStatus WHERE fldGroupCode=@fldGroupCode ";

            dbContext.ExecuteNonQuery(stmt, new[] {
                new SqlParameter("@fldApprovalStatus", "Y"),
                new SqlParameter("@fldGroupCode", GroupId),
            });

            string stmt2 = "Insert into tblGroupMaster" +

            "(fldGroupCode,fldGroupDesc,fldBankCode, fldSpickCode,fldCreateUserId, fldCreateTimeStamp,fldUpdateUserId,fldUpdateTimeStamp,fldApprovalStatus)  "

                + " Select " +

                " fldGroupCode,fldGroupDesc,fldBankCode, fldSpickCode,fldCreateUserId, fldCreateTimeStamp,fldUpdateUserId,fldUpdateTimeStamp,fldApprovalStatus " +

                " from tblGroupMasterTemp WHERE fldGroupCode = @fldGroupCode ";
            dbContext.ExecuteNonQuery(stmt2, new[] {
                new SqlParameter("@fldGroupCode", GroupId) });

        }

        public void DeleteInGroupMaster(string GroupId)
        {
            string stmt = " Delete from tblGroupMaster where fldGroupCode=@fldGroupCode";
            dbContext.ExecuteNonQuery(stmt, new[] { new SqlParameter("@fldGroupCode", GroupId) });
        }

        public void DeleteInGroupMasterTemp(string GroupId)
        {
            string stmt = " Delete from tblGroupMasterTemp where fldGroupCode=@fldGroupCode";
            dbContext.ExecuteNonQuery(stmt, new[] { new SqlParameter("@fldGroupCode", GroupId) });
        }


        public List<string> ValidateGroup(FormCollection col, string strBankCode) {
            List<string> err = new List<string>();
            if (CheckGroupExist(col["fldGroupCode"] + "_" + strBankCode, strBankCode)) {
                err.Add(Locale.GroupNameAlreadyTaken);
            }
            if (col["fldGroupCode"].Length > 10)
            {
                err.Add(Locale.GroupIdLength);
            }
            return err;
        }

        public void AddGroupToGroupMasterTempToDelete(string GroupId)
        {
            string stmt = "INSERT INTO tblGroupMasterTemp (fldGroupCode,fldGroupDesc,fldBankCode, fldSpickCode,fldCreateUserId, fldCreateTimeStamp,fldUpdateUserId,fldUpdateTimeStamp) " +
                "Select fldGroupCode,fldGroupDesc,fldBankCode, fldSpickCode,fldCreateUserId, fldCreateTimeStamp,fldUpdateUserId,fldUpdateTimeStamp from tblGroupMaster WHERE fldGroupCode=@fldGroupCode  Update tblGroupMasterTemp SET fldApprovalStatus=@fldApprovalStatus WHERE fldGroupCode=@fldGroupCode ";
          
            dbContext.ExecuteNonQuery(stmt, new[] {
                new SqlParameter("@fldGroupCode", GroupId),
                new SqlParameter("@fldApprovalStatus", "D")
            });
        }

        public string GetMaintenanceChecker() {
            string result = "";

            DataTable dataResult = new DataTable();
            string stmt = "Select fldSystemProfileValue from tblSystemProfile where fldSystemProfileCode = 'MaintenanceGroupChecker'";
            dataResult = dbContext.GetRecordsAsDataTable(stmt);

            if (dataResult.Rows.Count > 0) {
                result = dataResult.Rows[0]["fldSystemProfileValue"].ToString();
            }

            return result;
        }
    }
}