using CenIT.DegreeManagement.CoreAPI.Core.Business;
using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Processor;
using CenIT.DegreeManagement.CoreAPI.Core.Provider;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Sys;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Sys;
using Npgsql;
using NpgsqlTypes;
using System.Data.Common;


namespace CenIT.DegreeManagement.CoreAPI.Bussiness.Sys
{
    public class SysPermissionBL : NpgsqlConnector
    {
        private string _connectionString;

        #region Name function or procedure
        private string _permission_isallow = "p_permission_isallow";
        private string _permission_getbyroleid = "fn_permission_getbyroleid";
        private string _permission_getbyid = "fn_permission_getbyid";
        private string _permission_save = "p_permission_save";
        private string _sys_permission_delete = "p_sys_permission_delete";
        private string _sys_get_permissions = "fn_sys_get_permissions";
        #endregion

        #region Parameter
        private string p_permission_id = "@p_permission_id";
        private string p_username = "@p_username";
        private string p_function = "@p_function";
        private string p_action = "@p_action";
        private string p_role_id = "@p_role_id";
        private string p_function_action_ids = "@p_function_action_ids";
        #endregion

        public SysPermissionBL(string connectionString) : base(connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Kiểm tra quyền truy cập theo username, function, action
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="function"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public int Permission_IsAllow(string UserName, string function, string action)
        {
            DbParameter[] parameters = new DbParameter[]
               {
                    new NpgsqlParameter(p_username, UserName),
                    new NpgsqlParameter(p_function, function),
                    new NpgsqlParameter(p_action, action),

               };


            var returnValue = new ConnectionProcessor(_connectionString).ExcuteProcedureReturnValue(_permission_isallow, parameters);
            return returnValue;
        }

        /// <summary>
        /// Lấy danh sách permission theo roleid có serachParam
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<PermissionModel> GetPermissionById(int id, SearchParamModel model)
        {

            DbParameter[] parameters = new DbParameter[]
                 {
                    new NpgsqlParameter(p_role_id, id),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.search), model.Search),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.order), model.Order),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.order_dir), model.OrderDir),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.page_index), model.StartIndex),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.page_size), model.PageSize),
                 };
            var returnValue = new ConnectionProcessor(_connectionString).ExcuteStoreProcedureReturnQuery(_sys_get_permissions, parameters);
            var list = ModelProvider.CreateListFromTable<PermissionModel>(returnValue);

            return list;

        }

        /// <summary>
        /// Lấy danh sách permission theo roleid 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<PermissionModel> GetByRoleId(int id)
        {
            DbParameter[] parameters = new DbParameter[]
              {
                    new NpgsqlParameter(p_role_id,id),
              };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteStoreProcedureReturnQuery(_permission_getbyroleid, parameters);

            var list = ModelProvider.CreateListFromTable<PermissionModel>(returnValue);
            return list;
        }

        /// <summary>
        /// Lấy permission theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PermissionModel GetById(int id)
        {
            DbParameter[] parameters = new DbParameter[]
              {
                    new NpgsqlParameter(p_permission_id,id),
              };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteStoreProcedureReturnQuery(_permission_getbyid, parameters);

            var list = ModelProvider.CreateListFromTable<PermissionModel>(returnValue);
            PermissionModel permission = list.FirstOrDefault()!;

            return permission;
        }

        /// <summary>
        /// Lưu permission
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Save(PermissionInputModel model)
        {
            NpgsqlParameter roleIdParameter = new NpgsqlParameter(p_role_id, model.RoleId);
            NpgsqlParameter functionActionIdsParameter = new NpgsqlParameter(p_function_action_ids, NpgsqlDbType.Array | NpgsqlDbType.Integer);
            functionActionIdsParameter.Value = model.FunctionActionId;

            DbParameter[] parameters = new DbParameter[]
            {
                roleIdParameter,
                functionActionIdsParameter
            };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteProcedureReturnValue(_permission_save, parameters);

            return returnValue;

        }

        /// <summary>
        /// Xóa permission
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int Delete(int id)
        {
            DbParameter[] parameters = new DbParameter[]
               {
                    new NpgsqlParameter(p_permission_id, id),

               };


            var returnValue = new ConnectionProcessor(_connectionString).ExcuteProcedureReturnValue(_sys_permission_delete, parameters);
            return returnValue;

        }
    }
}
