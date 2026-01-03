using BusinessEntity;
using CrosscuttingUtiles;
using DataAccessCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DataAccessImpl
{
    public class ConfiguracionDataAccessImpl
    {
        public int intError { get; set; }
        public string strTextoError { get; set; }

        public DataSetSQL ListAll(string strCurrentUser, string strKey)
        {
            var baseSQL = new DatosBaseSQL();
            var dataSetSQL = new DataSetSQL();

            var _listParametros = new List<SqlParameter>();

            try
            {
                var strConexion = Credential.ConnString("SQL");

                baseSQL.sqlCon = baseSQL.fncAbrirBD(strConexion);

                if (baseSQL.sqlCon.State == ConnectionState.Closed)
                    throw new Exception("No se pudo conectar a la base de datos");


                _listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 15,
                    ParameterName = "USUARIO",
                    Value = strCurrentUser
                });

                _listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 40,
                    ParameterName = "GUID",
                    Value = DBNull.Value
                });

                _listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 10,
                    ParameterName = "KEY",
                    Value = string.IsNullOrEmpty(strKey) ? DBNull.Value : (object)strKey
                });

                _listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.SmallInt,
                    ParameterName = "TIPO",
                    Value = DBNull.Value
                });

                _listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.SmallInt,
                    ParameterName = "SUBTIPO",
                    Value = DBNull.Value
                });

                dataSetSQL = baseSQL.fncRetornaRegistros("[configuracion].[sp_app_listar_generica]", "Table", _listParametros);

                if (dataSetSQL.intError != 0)
                    throw new Exception(dataSetSQL.strError);
            }
            catch (Exception ex)
            {
                intError = 1;
                strTextoError = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            }
            finally
            {
                if (baseSQL.sqlCon.State != ConnectionState.Closed)
                {
                    baseSQL.sqlCon.Close();
                    baseSQL.sqlCon.Dispose();
                }
            }

            return dataSetSQL;
        }
    }
}
