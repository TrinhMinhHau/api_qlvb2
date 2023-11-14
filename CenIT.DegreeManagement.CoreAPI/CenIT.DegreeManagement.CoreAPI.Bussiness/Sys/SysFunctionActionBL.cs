using CenIT.DegreeManagement.CoreAPI.Core.Business;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Processor;
using CenIT.DegreeManagement.CoreAPI.Core.Provider;
using Npgsql;
using System.Data.Common;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Sys;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Sys;
using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;

namespace CenIT.DegreeManagement.CoreAPI.Bussiness.Sys
{
    public class SysFunctionActionBL : NpgsqlConnector
    {
        #region Name function or procedure
        private string _connectionString;
        private string _functionaction_getall = "fn_sys_function_action_getall";
        private string _p_sys_functionaction_save = "p_sys_functionaction_save";
        private string _p_sys_functionaction_delete = "p_sys_functionaction_delete";
        private string _fn_sys_functionaction_getbyfunctionid = "fn_sys_functionaction_getbyfunctionid";
        private string _function_action_getbyid = "fn_sys_function_action_getbyid";
        #endregion

        #region parameter
        private string _p_function_id = "@p_function_id";
        private string _p_function_action_id = "@p_function_action_id";
        private string _p_action = "@p_action";
        #endregion

 
        public SysFunctionActionBL(string connectionString) : base(connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Lấy danh sách functionAction
        /// </summary>
        /// <param name="SearchParamModel"></param>
        /// <returns></returns>
        public List<FunctionActionModel> GetAll(SearchParamModel model)
        {

            DbParameter[] parameters = new DbParameter[]
                 {
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.search), model.Search),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.order), model.Order),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.order_dir), model.OrderDir),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.page_index), model.StartIndex),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.page_size), model.PageSize),
                 };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteStoreProcedureReturnQuery(_functionaction_getall, parameters);

            var list = ModelProvider.CreateListFromTable<FunctionActionModel>(returnValue);

            return list;

        }

        /// <summary>
        /// Lấy danh sách functionAction theo functionId
        /// </summary>
        /// <param name="SearchParamModel"></param>
        /// <returns></returns>
        public List<FunctionActionModel> GetActionsByFunctionId(int id, SearchParamModel model)
        {
            DbParameter[] parameters = new DbParameter[]
              {
                    new NpgsqlParameter(_p_function_id,id),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.search), model.Search),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.order), model.Order),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.order_dir), model.OrderDir),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.page_index), model.StartIndex),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.page_size), model.PageSize),
              };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteStoreProcedureReturnQuery(_fn_sys_functionaction_getbyfunctionid, parameters);

            var list = ModelProvider.CreateListFromTable<FunctionActionModel>(returnValue);
            return list;
        }

        /// <summary>
        /// Lấy functionAction theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public FunctionActionModel GetById(int? id)
        {
            DbParameter[] parameters = new DbParameter[]
               {
                    new NpgsqlParameter(_p_function_action_id,id),
               };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteStoreProcedureReturnQuery(_function_action_getbyid, parameters);

            var list = ModelProvider.CreateListFromTable<FunctionActionModel>(returnValue);
            FunctionActionModel functionAction = list.FirstOrDefault()!;

            return functionAction;
        }

        /// <summary>
        /// Lưu functionAction 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Save(FunctionActionInputModel model)
        {
            DbParameter[] parameters = new DbParameter[]
              {
                    new NpgsqlParameter(_p_function_action_id, model.FunctionActionId),
                    new NpgsqlParameter(_p_function_id, model.FunctionId),
                    new NpgsqlParameter(_p_action, model.Action),
              };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteProcedureReturnValue(_p_sys_functionaction_save, parameters);

            return returnValue;
        }

        /// <summary>
        /// Xóa functionAction 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public int Delete(int id)
        {
            DbParameter[] parameters = new DbParameter[]
               {
                    new NpgsqlParameter(_p_function_action_id, id),

               };
            var returnValue = new ConnectionProcessor(_connectionString).ExcuteProcedureReturnValue(_p_sys_functionaction_delete, parameters);
            return returnValue;
        }
    }
}
