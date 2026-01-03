using BusinessEntity;
using CrosscuttingUtiles;
using DataAccessCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DataAccessImpl
{
    public class LoginDataAccessImpl
    {
        public int intError { get; set; }
        public string strTextoError { get; set; }
 
        public DataSetSQL logOn(LoginBusinessEntity collection)
        {
            var baseSQL = new DatosBaseSQL();
            var dataSetSQL = new DataSetSQL();

            SqlParameter sqlParameter;
            var _listParametros = new List<SqlParameter>();

            try
            {
                var strConexion = Credential.ConnString("SQL");

                baseSQL.sqlCon = baseSQL.fncAbrirBD(strConexion);

                if (baseSQL.sqlCon.State == ConnectionState.Closed)
                    throw new Exception("No se pudo conectar a la base de datos");

                #region Parametros

                _listParametros = new List<SqlParameter>();

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 50,
                    ParameterName = "usuario",
                    Value = collection.Usuario
                };
                _listParametros.Add(sqlParameter);

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 100,
                    ParameterName = "pass_usuario",
                    Value = Cifrado.Cifrar(collection.Password) 
                };
                _listParametros.Add(sqlParameter);

                #endregion

                dataSetSQL = baseSQL.fncRetornaRegistros("[configuracion].[sp_app_login]", "Login", _listParametros);

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
