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
    public class SysFunctionBL : NpgsqlConnector
    {
        #region Name function or procedure
        private string _connectionString;
        private string _function_getall = "fn_Sys_Function_Getall";
        private string _function_save = "p_Sys_Function_Save";
        private string _function_getid = "fn_Function_Getbyid";
        private string _function_delete = "p_sys_function_delete";
        #endregion
        
        #region parameter
        private string p_function_id = "@p_function_id";
        private string p_name = "@p_name";
        private string p_description = "@p_description";
        private string f_function_id = "@f_function_id";
        #endregion

        public SysFunctionBL(string connectionString) : base(connectionString)
        {
            _connectionString = connectionString;
        }


        /// <summary>
        /// Lấy danh sách Function
        /// </summary>
        /// <param name="SearchParamModel"></param>
        /// <returns></returns>
        public List<FunctionModel> GetAll(SearchParamModel model)
        {

            DbParameter[] parameters = new DbParameter[]
                 {
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.search), model.Search),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.order), model.Order),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.order_dir), model.OrderDir),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.page_index), model.StartIndex),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.page_size), model.PageSize),
                 };
            var returnValue = new ConnectionProcessor(_connectionString).ExcuteStoreProcedureReturnQuery(_function_getall, parameters);
            var list = ModelProvider.CreateListFromTable<FunctionModel>(returnValue);

            return list;

        }

        /// <summary>
        /// Lưu Function
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Save(FunctionInputModel model)
        {
            DbParameter[] parameters = new DbParameter[]
            {
                new NpgsqlParameter(p_function_id, model.FunctionId),
                new NpgsqlParameter(p_name, model.Name),
                new NpgsqlParameter(p_description, model.Description)
            };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteProcedureReturnValue(_function_save, parameters);
            return returnValue;
        }

        /// <summary>
        /// Lấy function theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FunctionModel GetById(int? id)
        {
            DbParameter[] parameters = new DbParameter[]
              {

                    new NpgsqlParameter(f_function_id,id)
              };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteStoreProcedureReturnQuery(_function_getid, parameters);

            var list = ModelProvider.CreateListFromTable<FunctionModel>(returnValue);
            FunctionModel function = list.FirstOrDefault()!;

            return function;
        }

        /// <summary>
        /// Xóa function
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int Delete(int id)
        {
            DbParameter[] parameters = new DbParameter[]
               {
                    new NpgsqlParameter(p_function_id, id),
               };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteProcedureReturnValue(_function_delete, parameters);
            return returnValue;
        }
    }
}
