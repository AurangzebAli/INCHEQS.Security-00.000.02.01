using INCHEQS.Security.User;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using INCHEQS.Security.Group;
using System.Web.Mvc;

namespace INCHEQS.Security.Group {
    public interface IGroupDao {
//        DataTable ListAllGroup();
        GroupModel GetGroup(string groupId);
        GroupModel GetGroup(string groupId, string BankCode);
        List<UserModel> ListAvailableUserInGroup(string strBankCode);
        List<UserModel> ListSelectedUserInGroup(string groupId);
        List<UserModel> ListSelectedUserInGroup(string groupId, string strBankCode);
        void DeleteGroup(string groupId);
        void DeleteGroup(string groupId, string strBankCode);
        void DeleteAllUserInGroup(string groupId);
        void DeleteAllUserInGroup(string groupId, string strBankCode);
        void DeleteAllTaskInGroup(string groupId);
        void DeleteAllTaskInGroup(string groupId, string strBankCode);
        void DeleteUserNotSelected(string groupId, string userIds);
        void DeleteUserNotSelected(string groupId, string userIds, string strBankCode);
        bool CheckUserExistInGroup(string groupId, string userId);
        bool CheckUserExistInGroup(string groupId, string userId, string strBankCode);
        bool CheckGroupExist(string groupId, string strBankCode);
        void CreateInGroupMaster(string GroupId);
        void DeleteInGroupMaster(string Group);
        void DeleteInGroupMasterTemp(string Group);
        void AddGroupToGroupMasterTempToDelete(string id);
        void UpdateSelectedUser(string groupId, string UserId, string strUpdate);
        void UpdateSelectedUser(string groupId, string UserId, string strUpdate, string strBankCode);
        void InsertUserInGroup(string groupId, string userId, string strUpdate, string strUpdateId);
        void InsertUserInGroup(string groupId, string userId, string strUpdate, string strUpdateId, string strBankCode);
        void UpdateGroupMaster(FormCollection col, string strUpdate);
        void CreateGroupMaster(FormCollection col, string strBankCode, string strUpdate, string spickCode, string strUpdateId);
        List<string> ValidateGroup(FormCollection col, string strBankCode);
        string GetMaintenanceChecker();
        void CreateGroupMasterTemp(FormCollection col, string strUpdate, string strBankCode, string spickCode, string strUpdateId);

    }
}