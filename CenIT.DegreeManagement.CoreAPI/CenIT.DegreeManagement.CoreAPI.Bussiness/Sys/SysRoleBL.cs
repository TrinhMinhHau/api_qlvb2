using CenIT.DegreeManagement.CoreAPI.Core.Business;
using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Processor;
using CenIT.DegreeManagement.CoreAPI.Core.Provider;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Sys;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Sys;
using Npgsql;
using System.Data.Common;

namespace CenIT.DegreeManagement.CoreAPI.Bussiness.Sys
{
    public class SysRoleBL : NpgsqlConnector
    {
        private string _connectionString;

        #region Name function or procedure
        private string _role_getall = "fn_sys_role_getall";
        private string _role_getid = "fn_sys_role_getbyid";
        private string _role_save = "p_sys_role_save";
        private string _role_delete = "p_sys_role_delete";
        #endregion

        #region Parameter
        private string p_user_id = "@p_user_id";
        private string p_role_id = "@p_role_id";
        private string p_name = "@p_name";
        #endregion

        public SysRoleBL(string connectionString) : base(connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Lấy danh sách quyền
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<RoleModel> GetAll(SearchParamModel model)
        {

            DbParameter[] parameters = new DbParameter[]
                 {
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.search), model.Search),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.order), model.Order),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.order_dir), model.OrderDir),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.page_index), model.StartIndex),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.page_size), model.PageSize),
                 };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteStoreProcedureReturnQuery(_role_getall, parameters);

            var list = ModelProvider.CreateListFromTable<RoleModel>(returnValue);

            return list;

        }

        /// <summary>
        /// Lấy quyền theo id
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public RoleModel GetById(int? id)
        {
            DbParameter[] parameters = new DbParameter[]
              {
                    new NpgsqlParameter(p_user_id,id),
              };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteStoreProcedureReturnQuery(_role_getid, parameters);

            var list = ModelProvider.CreateListFromTable<RoleModel>(returnValue);
            RoleModel role = list.FirstOrDefault()!;

            return role;
        }

        /// <summary>
        /// Lưu quyền
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Save(RoleInputModel model)
        {
            DbParameter[] parameters = new DbParameter[]
              {
                    new NpgsqlParameter(p_role_id, model.RoleId),
                    new NpgsqlParameter(p_name, model.Name),
              };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteProcedureReturnValue(_role_save, parameters);

            return returnValue;

        }

        /// <summary>
        /// Xóa quyền
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Delete(int id)
        {
            DbParameter[] parameters = new DbParameter[]
               {
                    new NpgsqlParameter(p_role_id, id),

               };
            var returnValue = new ConnectionProcessor(_connectionString).ExcuteProcedureReturnValue(_role_delete, parameters);
            return returnValue;

        }
    }
}
