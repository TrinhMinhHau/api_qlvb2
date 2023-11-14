using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using Microsoft.Extensions.Logging;

namespace CenIT.DegreeManagement.CoreAPI.Core.Business
{
    public class NpgsqlConnector 
    {
        private string _connectionString;

        public string ConnectionString { get; }

        public NpgsqlConnector(string connectionString)
        {
            _connectionString = connectionString;
        }

        public object ExcuteQuery(string queryString, DbParameter[] parameters)
        {
            using (DbConnection connection = new NpgsqlConnection(_connectionString))
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = queryString;
                    command.CommandType = CommandType.Text;
                    if (parameters != null)
                    {
                        foreach (DbParameter p in parameters)
                        {
                            command.Parameters.Add(p);
                        }
                    }

                    DbDataReader reader = command.ExecuteReader();

                    DataTable dt = new DataTable();
                    dt.Load(reader);
                    reader.Close();
                    connection.Close();
                    return dt;
                }
            }
        }

        public bool ExcuteNonQuery(string queryString, DbParameter[] parameters)
        {
            using (DbConnection connection = new NpgsqlConnection(_connectionString))
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = queryString;
                    command.CommandType = CommandType.Text;
                    foreach (DbParameter p in parameters)
                    {
                        command.Parameters.Add(p);
                    }
                    int iResult = command.ExecuteNonQuery();
                    connection.Close();
                    return iResult == -1 ? false : true;
                }
            }
        }

        public object ExcuteStoreProcedure(string procedureName, DbParameter[] parameters)
        {
            using (DbConnection connection = new NpgsqlConnection(_connectionString))
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = procedureName;
                    command.CommandType = CommandType.StoredProcedure;
                    foreach (DbParameter p in parameters)
                    {
                        command.Parameters.Add(p);
                    }
                    DbDataReader reader = command.ExecuteReader();
                    DataTable dt = new DataTable();
                    dt.Load(reader);
                    reader.Close();
                    connection.Close();
                    return dt;
                }
            }
        }

        public object ExcuteStoreProcedureWithReturn(string procedureName, DbParameter[] parameters)
        {
            using (DbConnection connection = new NpgsqlConnection(_connectionString))
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = procedureName;
                    command.CommandType = CommandType.StoredProcedure;
                    foreach (DbParameter p in parameters)
                    {
                        command.Parameters.Add(p);
                    }
                    DbParameter returnParameter = command.CreateParameter();
                    returnParameter.DbType = DbType.Int32;
                    returnParameter.ParameterName = "@ReturnVal";
                    returnParameter.Direction = ParameterDirection.ReturnValue;

                    command.Parameters.Add(returnParameter);

                    int iResult = command.ExecuteNonQuery();
                    connection.Close();
                    return returnParameter.Value;
                }
            }
        }


        public int ExcuteStoreProcedureReturnINT(string procedureName, DbParameter[] parameters)
        {
            NpgsqlConnection connection = new NpgsqlConnection(_connectionString);
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            NpgsqlCommand command = new NpgsqlCommand(procedureName, connection);

            command.CommandType = System.Data.CommandType.StoredProcedure;
            NpgsqlCommandBuilder.DeriveParameters(command);

            foreach (DbParameter p in parameters)
            {
                command.Parameters[p.ParameterName].Value = p.Value;
            }
            command.Parameters["@resuilt"].Value = 0;
            command.ExecuteNonQuery();
            int count = command.Parameters.Count;
            int returnValue = Convert.ToInt32(command.Parameters[count - 1].Value);
            connection.Close();
            return returnValue;
        }


    }
}
