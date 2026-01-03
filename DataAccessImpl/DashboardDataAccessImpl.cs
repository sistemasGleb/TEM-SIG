
namespace DataAccessImpl
{
    using BusinessEntity;
    using CrosscuttingUtiles;
    using DataAccessCore;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;

    public class DashboardDataAccessImpl
    {
        //private IConfiguration _configuracion;
        public int intError { get; set; }
        public string strTextoError { get; set; }
        public DataSetSQL getDashboard()
        {
            DatosBaseSQL baseSQL = new DatosBaseSQL();
            DataSetSQL dataSetSQL = new DataSetSQL();

            SqlParameter sqlParameter;
            List<SqlParameter> _listParametros = new List<SqlParameter>();

            try
            {
                baseSQL.sqlCon = baseSQL.fncAbrirBD(Credential.ConnString("SQLGTM"));

                if (baseSQL.sqlCon.State == ConnectionState.Closed)
                    throw new Exception("No se pudo conectar a la base de datos");

                #region Parametros
                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 30,
                    ParameterName = "opcion",
                    Value = "construir"
                };
                _listParametros.Add(sqlParameter);

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "id_cultura",
                    Value = 1
                };
                _listParametros.Add(sqlParameter);

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "id_usuario",
                    Value = 1
                };
                _listParametros.Add(sqlParameter);
                #endregion

                dataSetSQL = baseSQL.fncRetornaRegistros("[dbo].[sp_web_dashboard_builder]", "DASHBOARD", _listParametros);

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
        public UsuarioBusinessEntity.UsuarioViewModel GetUserRole(UsuarioBusinessEntity.UsuarioViewModel form)
        {
            DatosBaseSQL baseSQL = new DatosBaseSQL();
            DataSetSQL dataSetSQL = new DataSetSQL();
            var usuario = new UsuarioBusinessEntity.UsuarioViewModel();

            SqlParameter sqlParameter;
            List<SqlParameter> _listParametros = new List<SqlParameter>();

            try
            {
                baseSQL.sqlCon = baseSQL.fncAbrirBD(Credential.ConnString("SQL"));

                if (baseSQL.sqlCon.State == ConnectionState.Closed)
                    throw new Exception("No se pudo conectar a la base de datos");

                #region Parametros
                _listParametros = new List<SqlParameter>();

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 30,
                    ParameterName = "bloque",
                    Value = "PRF_USR"
                };
                _listParametros.Add(sqlParameter);

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 128,
                    ParameterName = "usuario",
                    Value = form.usr_id
                };
                _listParametros.Add(sqlParameter);

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "idperfil",
                    Value = 0
                };
                _listParametros.Add(sqlParameter);
                #endregion

                dataSetSQL = baseSQL.fncRetornaRegistros("[dbo].[sp_mca_perfil_usuario]", "ROLES", _listParametros);

                if (dataSetSQL.intError != 0)
                    throw new Exception(dataSetSQL.strError);

                // PASO 1) - LISTA DE ROLES DEL USUARIO
                var rolesList = dataSetSQL.dsSQL.Tables[0].AsEnumerable()
                    .Select(m => new UsuarioBusinessEntity.Rol()
                    {
                        Id = (int)m["id_perfil"],
                        Nombre = (string)m["gls_perfil"],
                    }).ToList();

                var rol = new UsuarioBusinessEntity.Rol { Id = 0, Nombre = "Login" };

                rolesList.Add(rol);

                // PASO 2) - usuario rol
            //    form.usuario_Rol = rolesList.OrderBy(r => r.Id).Select((r, i) => new UsuarioBusinessEntity.Usuario_Rol() { Id = i, Id_Rol = r.Id, Rol_Nombre = r.Nombre, Id_Usuario = form.Id }).ToList();

                usuario = form;
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

            return usuario;
        }
    }
}
