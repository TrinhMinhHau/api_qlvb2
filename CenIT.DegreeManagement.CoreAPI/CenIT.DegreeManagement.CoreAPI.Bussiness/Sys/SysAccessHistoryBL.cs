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


    public class SysAccessHistoryBL : NpgsqlConnector
    {
        #region Name function or procedure
        private string _connectionString;
        private string _access_history_getall = "fn_sys_access_history_getall";
        private string _access_history_export = "fn_sys_access_history_export";
        private string _access_history_get_alluser = "fn_sys_access_history_get_alluser";
        private string _access_history_save = "sys_access_history_save";

        #endregion


        #region Parameter
        private string _user_id = "@_user_id";
        private string _fromDate = "@_fromDate";
        private string _toDate = "@_toDate";
        private string p_username = "@p_username";
        private string p_token = "@p_token";
        private string p_function = "@p_function";
        private string p_action = "@p_action";
        private string p_is_success = "@p_is_success";
        #endregion

        public SysAccessHistoryBL(string connectionString) : base(connectionString)
        {
            _connectionString = connectionString;
        }


        #region Access History

        /// <summary>
        /// Lấy danh sách thông tin lịch sử truy cập
        /// </summary>
        /// <param name="SearchParamFilterDateModel"></param>
        /// <returns></returns>
        public List<AccessHistoryModel> GetAllAccessHistory(SearchParamFilterDateModel model)
        {

            DbParameter[] parameters = new DbParameter[]
                 {
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.search), model.Search),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.order), model.Order),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.order_dir), model.OrderDir),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.page_index), model.StartIndex),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.page_size), model.PageSize),
                    new NpgsqlParameter(_fromDate, model.FromDate == null ? DBNull.Value : model.FromDate),
                    new NpgsqlParameter(_toDate,  model.ToDate == null ? DBNull.Value : model.ToDate),
                    new NpgsqlParameter(p_username,  model.Username == null ? DBNull.Value : model.Username),
                    new NpgsqlParameter(p_function,  model.Function == null ? DBNull.Value : model.Function),
                    new NpgsqlParameter(p_action,  model.Action == null ? DBNull.Value : model.Action),
                 };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteStoreProcedureReturnQuery(_access_history_getall, parameters);

            var list = ModelProvider.CreateListFromTable<AccessHistoryModel>(returnValue);

            return list;

        }

        /// <summary>
        /// Lấy danh sách thông tin lịch sử truy cập theo từng username hoặc khoảng thời gian
        /// </summary>
        /// <param name="AccessHistoryInputModel"></param>
        /// <returns></returns>

        public List<AccessHistoryModel> GetAllAccessHistoryByUsernameOrDate(AccessHistorySearchModel model)
        {

            DbParameter[] parameters = new DbParameter[]
                 {
                    new NpgsqlParameter(_user_id, model.UserId == null ? DBNull.Value : model.UserId),
                    new NpgsqlParameter(_fromDate, model.FromDate == null ? DBNull.Value : model.FromDate),
                    new NpgsqlParameter(_toDate, model.ToDate == null ? DBNull.Value : model.ToDate),
                 };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteStoreProcedureReturnQuery(_access_history_export, parameters);

            var list = ModelProvider.CreateListFromTable<AccessHistoryModel>(returnValue);

            return list;
        }

        /// <summary>
        /// Lấy danh sách người dùng đã từng truy cập
        /// </summary>
        /// <returns></returns>

        public List<UserAccessHistoryModel> GetAllUserInAccessHistory()
        {
            var returnValue = new ConnectionProcessor(_connectionString).ExcuteStoreProcedureReturnQuery(_access_history_get_alluser);
            var list = ModelProvider.CreateListFromTable<UserAccessHistoryModel>(returnValue);

            return list;
        }

        public int Save(AccessHistoryInputModel model)
        {
            DbParameter[] parameters = new DbParameter[]
              {
                    new NpgsqlParameter(p_username, model.UserName),
                    new NpgsqlParameter(p_token, model.Token),
                    new NpgsqlParameter(p_function,model.Function),
                    new NpgsqlParameter(p_action, model.Action),
                    new NpgsqlParameter(p_is_success, model.IsSuccess),
              };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteProcedureReturnValue(_access_history_save, parameters);

            return returnValue;
        }

        #endregion

    }
}
