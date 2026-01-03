using BusinessEntity;
using CrosscuttingUtiles;
using DataAccessCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DataAccessImpl
{
    public class UsuarioDataAccessImpl
    {
        public string strConexion = Credential.ConnString("SQL");
        public int intError { get; set; }
        public string strTextoError { get; set; }

        public DataSetSQL ListUser(string strUsuario)
        {
            DatosBaseSQL baseSQL = new DatosBaseSQL();
            DataSetSQL dataSetSQL = new DataSetSQL();

            SqlParameter sqlParameter;
            List<SqlParameter> _listParametros = new List<SqlParameter>();

            try
            {
                baseSQL.sqlCon = baseSQL.fncAbrirBD(strConexion);

                if (baseSQL.sqlCon.State == ConnectionState.Closed)
                    throw new Exception("No se pudo conectar a la base de datos");


                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    ParameterName = "USUARIO",
                    Size = 15,
                    Value = strUsuario
                };
                _listParametros.Add(sqlParameter);

                dataSetSQL = baseSQL.fncRetornaRegistros("[configuracion].[sp_app_usuario_listar]", "Table", _listParametros);

                if (dataSetSQL.intError != 0)
                    throw new Exception(dataSetSQL.strError);
            }
            catch (Exception ex)
            {
                this.intError = 1;
                this.strTextoError = (ex.InnerException == null ? ex.Message : ex.InnerException.Message);
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
        public DataSetSQL DeleteUser(int intUsuario, string strCurrentUser = "")
        {
            DatosBaseSQL baseSQL = new DatosBaseSQL();
            DataSetSQL dataSetSQL = new DataSetSQL();

            SqlParameter sqlParameter;
            List<SqlParameter> _listParametros = new List<SqlParameter>();

            try
            {
               baseSQL.sqlCon = baseSQL.fncAbrirBD(strConexion);

                if (baseSQL.sqlCon.State == ConnectionState.Closed)
                    throw new Exception("No se pudo conectar a la base de datos");

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "USR_MOD_ID",
                    Value = intUsuario
                };
                _listParametros.Add(sqlParameter);

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 10,
                    ParameterName = "USR_MOD_LOGIN",
                    Value = strCurrentUser
                };
                _listParametros.Add(sqlParameter);

                dataSetSQL = baseSQL.fncRetornaRegistros("[configuracion].[sp_app_usuario_eliminar]", "USUARIO", _listParametros);

                if (dataSetSQL.intError != 0)
                    throw new Exception(dataSetSQL.strError);
            }
            catch (Exception ex)
            {
                this.intError = 1;
                this.strTextoError = (ex.InnerException == null ? ex.Message : ex.InnerException.Message);
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
        public DataSetSQL CreateUser(UsuarioBusinessEntity.UsuarioViewModel collection, string strCurrentUser = "")
        {
            DatosBaseSQL baseSQL = new DatosBaseSQL();
            DataSetSQL dataSetSQL = new DataSetSQL();

            SqlParameter sqlParameter;
            List<SqlParameter> _listParametros = new List<SqlParameter>();

            try
            {
                baseSQL.sqlCon = baseSQL.fncAbrirBD(strConexion);

                if (baseSQL.sqlCon.State == ConnectionState.Closed)
                    throw new Exception("No se pudo conectar a la base de datos");

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 10,
                    ParameterName = "usr_cre_lgn",
                    Value = strCurrentUser
                };
                _listParametros.Add(sqlParameter);

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "usr_id",
                    Value = collection.usr_id
                };
                _listParametros.Add(sqlParameter);

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 10,
                    ParameterName = "usr_rut",
                    Value = collection.usr_rut
                };
                _listParametros.Add(sqlParameter);

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 1,
                    ParameterName = "usr_rut_dv",
                    Value = collection.usr_rut_dv
                };
                _listParametros.Add(sqlParameter);

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 10,
                    ParameterName = "usr_lgn",
                    Value = collection.usr_lgn
                };
                _listParametros.Add(sqlParameter);

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 50,
                    ParameterName = "usr_psw",
                    Value = collection.usr_psw
                };
                _listParametros.Add(sqlParameter);

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 50,
                    ParameterName = "usr_nom",
                    Value = collection.usr_nom
                };
                _listParametros.Add(sqlParameter);

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 50,
                    ParameterName = "usr_ape_pat",
                    Value = collection.usr_ape_pat
                };
                _listParametros.Add(sqlParameter);

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 50,
                    ParameterName = "usr_ape_mat",
                    Value = collection.usr_ape_mat
                };
                _listParametros.Add(sqlParameter);

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 100,
                    ParameterName = "usr_mail",
                    Value = collection.usr_mail
                };
                _listParametros.Add(sqlParameter);

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 10,
                    ParameterName = "usr_tel",
                    Value = collection.usr_tel
                };
                _listParametros.Add(sqlParameter);

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.Bit,
                    ParameterName = "usr_vig",
                    Value = collection.usr_vig
                };
                _listParametros.Add(sqlParameter);

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.Bit,
                    ParameterName = "usr_blq",
                    Value = collection.usr_blq
                };
                _listParametros.Add(sqlParameter);

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "usr_per_cod",
                    Value = collection.usr_car_cod
                };
                _listParametros.Add(sqlParameter);

                dataSetSQL = baseSQL.fncRetornaRegistros("[configuracion].[sp_app_usuario_agregar_editar]", "USUARIO", _listParametros);

                if (dataSetSQL.intError != 0)
                    throw new Exception(dataSetSQL.strError);
            }
            catch (Exception ex)
            {
                this.intError = 1;
                this.strTextoError = (ex.InnerException == null ? ex.Message : ex.InnerException.Message);
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

        public DataSetSQL PasswordRecovery(PasswordBusinessEntity collection)
        {
            DatosBaseSQL baseSQL = new DatosBaseSQL();
            DataSetSQL dataSetSQL = new DataSetSQL();

            SqlParameter sqlParameter;
            List<SqlParameter> _listParametros = new List<SqlParameter>();

            try
            {
                baseSQL.sqlCon = baseSQL.fncAbrirBD(strConexion);

                if (baseSQL.sqlCon.State == ConnectionState.Closed)
                    throw new Exception("No se pudo conectar a la base de datos");

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 15,
                    ParameterName = "USUARIO",
                    Value = collection.recovery_user_lgn
                };
                _listParametros.Add(sqlParameter);

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 36,
                    ParameterName = "tpr_gui",
                    Value = collection.recovery_guid.ToString()
                };
                _listParametros.Add(sqlParameter);

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 100,
                    ParameterName = "tpr_usr_mail",
                    Value = collection.recovery_user_email
                };
                _listParametros.Add(sqlParameter);

                dataSetSQL = baseSQL.fncRetornaRegistros("[configuracion].[sp_app_usuario_agregar_editar]", "Table", _listParametros);

                if (dataSetSQL.intError != 0)
                    throw new Exception(dataSetSQL.strError);
            }
            catch (Exception ex)
            {
                this.intError = 1;
                this.strTextoError = (ex.InnerException == null ? ex.Message : ex.InnerException.Message);
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
