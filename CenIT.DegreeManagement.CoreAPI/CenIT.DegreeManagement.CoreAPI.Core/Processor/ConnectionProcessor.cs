using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CenIT.DegreeManagement.CoreAPI.Core.Business;
using Npgsql;
using CenIT.DegreeManagement.CoreAPI.Core.Provider;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace CenIT.DegreeManagement.CoreAPI.Core.Processor
{
    public class ConnectionProcessor
    {
        private string _connectString;

        public ConnectionProcessor(string connectString)
        {
            _connectString = connectString;
        }

        /// <summary>
        /// Trả về datatable từ SQL
        /// </summary>
        /// <param name="procedureName">Tên store</param>
        /// <param name="parameters">Danh sách tham số</param>
        /// <returns></returns>
        public DataTable ExcuteStoreProcedureReturnQuery(string procedureName, DbParameter[] parameters = null)
        {
            string str = "";
            string result = "";
            var connector = new NpgsqlConnector(_connectString);
            var _params = new List<DbParameter>();

            if (parameters != null)
            {
                foreach (DbParameter p in parameters)
                {
                    _params.Add(p);
                    str = str + p.ParameterName + ",";
                }
                result = str.Substring(0, str.Length - 1);
            }
                
            return (DataTable)connector.ExcuteQuery("SELECT * FROM " + procedureName + "(" + (string.IsNullOrEmpty(result) ? "" : result) + ")", _params.ToArray());
        }

        /// <summary>
        /// Trả về giá trị số
        /// </summary>
        /// <param name="procedureName">Tên store</param>
        /// <param name="parameters">Danh sách tham số</param>
        /// <returns></returns>
        public int ExcuteProcedureReturnValue(string procedureName, DbParameter[] parameters)
        {
            return new NpgsqlConnector(_connectString).ExcuteStoreProcedureReturnINT(procedureName, parameters);
        }

    }
}
