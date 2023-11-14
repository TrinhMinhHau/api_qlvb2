using CenIT.DegreeManagement.CoreAPI.Core.Business;
using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Processor;
using CenIT.DegreeManagement.CoreAPI.Core.Provider;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Sys;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Sys;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Bussiness.Sys
{
    public class SysNotificationBL : NpgsqlConnector
    {

        #region Name function or procedure
        private string _connectionString;
        private string _sys_notification_save = "p_sys_notification_save";
        private string _sys_notification_getall = "fn_sys_notification_getall";
        private string _sys_notification_getbyid = "fn_sys_notification_getbyid";

        #endregion


        #region Parameter
        private string p_notification_id = "@p_notification_id";
        private string p_type = "@p_type";
        private string p_recipient = "@p_recipient";
        private string p_subject = "@p_subject";
        private string p_message = "@p_message";
        private string p_action = "@p_action";
        private string p_created_by = "@p_created_by";
        private string _fromDate = "@_fromDate";
        private string _toDate = "@_toDate";
        #endregion

        public SysNotificationBL(string connectionString) : base(connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Lấy danh sách nin nhắn
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<NotificationModel> GetAllNotification(SearchParamFilterDateModel model)
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
                 };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteStoreProcedureReturnQuery(_sys_notification_getall, parameters);

            var list = ModelProvider.CreateListFromTable<NotificationModel>(returnValue);

            return list;
        }

        public int Save(NotificationInputModel model)
        {
            DbParameter[] parameters = new DbParameter[]
              {
                    new NpgsqlParameter(p_type, model.Type),
                    new NpgsqlParameter(p_recipient, model.Recipient),
                    new NpgsqlParameter(p_subject, model.Subject),
                    new NpgsqlParameter(p_message, model.Message),
                    new NpgsqlParameter(p_action, model.Action),
                    new NpgsqlParameter(p_created_by, model.CreatedBy),
              };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteProcedureReturnValue(_sys_notification_save, parameters);

            return returnValue;
        }


        public NotificationModel GetNotificationById(int? id)
        {
            DbParameter[] parameters = new DbParameter[]
            {
                    new NpgsqlParameter(p_notification_id,id),
            };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteStoreProcedureReturnQuery(_sys_notification_getbyid, parameters);

            var list = ModelProvider.CreateListFromTable<NotificationModel>(returnValue);
            NotificationModel notification = list.FirstOrDefault()!;

            return notification;
        }
    }
}
