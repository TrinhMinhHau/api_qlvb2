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
    public class SysDeviceTokenBL : NpgsqlConnector
    {
        private string _connectionString;

        #region Name function or procedure
        private string _device_token_save = "sys_device_token_save";
        private string _sys_device_token_getbyiddonvi = "fn_sys_device_token_getbyiddonvi";

        #endregion

        #region Parameter
        private string p_device_token = "@p_device_token";
        private string p_user_id = "@p_user_id";
        private string p_id_donvi = "@p_id_donvi";
        private string p_token = "@p_token";


        #endregion

        public SysDeviceTokenBL(string connectionString) : base(connectionString)
        {
            _connectionString = connectionString;
        }

        public int Save(DeviceTokenInputModel model)
        {
            DbParameter[] parameters = new DbParameter[]
              {
                    new NpgsqlParameter(p_device_token, model.DeviceToken),
                    new NpgsqlParameter(p_user_id, model.UserId),
                    new NpgsqlParameter(p_id_donvi, model.DonViId),
                    new NpgsqlParameter(p_token, model.Token),
              };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteProcedureReturnValue(_device_token_save, parameters);

            return returnValue;
        }

        public List<DeviceTokenModel> GetByIdDonVi(string idDonVi)
        {

            DbParameter[] parameters = new DbParameter[]
                 {

                    new NpgsqlParameter(p_id_donvi, idDonVi),
                 };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteStoreProcedureReturnQuery(_sys_device_token_getbyiddonvi, parameters);

            var list = ModelProvider.CreateListFromTable<DeviceTokenModel>(returnValue);

            return list;
        }
    }
}
