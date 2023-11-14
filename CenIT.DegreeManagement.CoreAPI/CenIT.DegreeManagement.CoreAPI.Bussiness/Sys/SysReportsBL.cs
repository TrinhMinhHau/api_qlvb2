using CenIT.DegreeManagement.CoreAPI.Core.Business;
using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Processor;
using CenIT.DegreeManagement.CoreAPI.Core.Provider;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Sys;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Sys;
using MongoDB.Driver.Core.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Bussiness.Sys
{
    public class SysReportsBL : NpgsqlConnector
    {
        private string _connectionString;

        #region Name function or procedure
        private string _report_delete = "p_sys_report_delete";
        private string _report_getall = "fn_sys_report_getall";
        private string _report_save = "p_sys_report_save";
        private string _report_getbyid = "fn_sys_report_getbyid";

        #endregion

        #region Parameter
        private string p_report_id = "@p_report_id";
        private string p_name = "@p_name";
        private string p_url = "@p_url";
        #endregion

        public SysReportsBL(string connectionString) : base(connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Lấy danh sách báo cáo
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<ReportModel> GetAll(SearchParamModel model)
        {

            DbParameter[] parameters = new DbParameter[]
                 {
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.search), model.Search),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.order), model.Order),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.order_dir), model.OrderDir),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.page_index), model.StartIndex),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.page_size), model.PageSize),
                 };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteStoreProcedureReturnQuery(_report_getall, parameters);

            var list = ModelProvider.CreateListFromTable<ReportModel>(returnValue);

            return list;

        }

        public ReportModel GetById(int? id)
        {
            DbParameter[] parameters = new DbParameter[]
              {
                    new NpgsqlParameter(p_report_id,id),
              };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteStoreProcedureReturnQuery(_report_getbyid, parameters);

            var list = ModelProvider.CreateListFromTable<ReportModel>(returnValue);
            ReportModel report = list.FirstOrDefault()!;

            return report;
        }

        /// <summary>
        /// Lưu báo cáo
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Save(ReportInputModel model)
        {
            DbParameter[] parameters = new DbParameter[]
              {
                    new NpgsqlParameter(p_report_id, model.ReportId),
                    new NpgsqlParameter(p_name, model.Name),
                    new NpgsqlParameter(p_url, model.Url),
              };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteProcedureReturnValue(_report_save, parameters);

            return returnValue;
        }

        public int Delete(int id)
        {
            DbParameter[] parameters = new DbParameter[]
               {
                    new NpgsqlParameter(p_report_id, id),

               };
            var returnValue = new ConnectionProcessor(_connectionString).ExcuteProcedureReturnValue(_report_delete, parameters);
            return returnValue;

        }

    }
}
