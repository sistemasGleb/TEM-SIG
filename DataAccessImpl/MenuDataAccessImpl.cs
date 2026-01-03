using BusinessEntity;
using CrosscuttingUtiles;
using DataAccessCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DataAccessImpl
{
    public class MenuDataAccessImpl
    {
        public int intError { get; set; }
        public string strTextoError { get; set; }
        public DataSetSQL get_Elementos( int intUsuario)
        {
            DatosBaseSQL baseSQL = new DatosBaseSQL();
            DataSetSQL dataSetSQL = new DataSetSQL();

            SqlParameter sqlParameter;
            List<SqlParameter> _listParametros = new List<SqlParameter>();

            try
            {
                var strConexion = Credential.ConnString("SQL");

                baseSQL.sqlCon = baseSQL.fncAbrirBD(strConexion);

                if (baseSQL.sqlCon.State == ConnectionState.Closed)
                    throw new Exception("No se pudo conectar a la base de datos");


                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "usuario",
                    Value = intUsuario
                };
                _listParametros.Add(sqlParameter);


                dataSetSQL = baseSQL.fncRetornaRegistros("[dbo].[sp_app_elementos]", "Table", _listParametros);

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

        public List<MenuBusinessEntity.Menu> fncDevolverMenus(int? intCodigoPerfil = 0)
        {
            DatosBaseSQL baseSQL = new DatosBaseSQL();
            DataSetSQL dataSetSQL = new DataSetSQL();

            SqlParameter sqlParameter;
            List<SqlParameter> listParametros = new List<SqlParameter>();

            List<MenuBusinessEntity.Menu> listMenus = new List<MenuBusinessEntity.Menu>();
            MenuBusinessEntity.Menu menu;

            try
            {
                var strConexion = Credential.ConnString("SQL");

                baseSQL.sqlCon = baseSQL.fncAbrirBD(strConexion);

                if (baseSQL.sqlCon.State == ConnectionState.Closed)
                    throw new Exception("No se pudo conectar a la base de datos");

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "PER_COD",
                    Value = intCodigoPerfil
                };
                listParametros.Add(sqlParameter);

                dataSetSQL = baseSQL.fncRetornaRegistros("[dbo].[sp_app_menu_listar]", "Menus", listParametros);

                if (dataSetSQL.intError != 0)
                    throw new Exception(dataSetSQL.strError);

                foreach (DataRow dataRow in dataSetSQL.dsSQL.Tables["Menus"].Rows)
                {
                    menu = new MenuBusinessEntity.Menu
                    {
                        menu_cod = ((short)dataRow["menu_cod"]).ToString(),
                        menu_nom = (string)dataRow["menu_desc"],
                        menu_sel = (bool)dataRow["menu_sel"]
                    };
                    listMenus.Add(menu);
                }

            }
            catch (Exception ex)
            {
                listMenus = null;
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

            return listMenus;
        }

        public DataSetSQL fncDevolverMenu(string strCurrentUser)
        {
            DatosBaseSQL baseSQL = new DatosBaseSQL();
            DataSetSQL dataSetSQL = new DataSetSQL();

            SqlParameter sqlParameter;
            List<SqlParameter> listParametros = new List<SqlParameter>();

            try
            {
                var strConexion = Credential.ConnString("SQL");

                baseSQL.sqlCon = baseSQL.fncAbrirBD(strConexion);

                if (baseSQL.sqlCon.State == ConnectionState.Closed)
                    throw new Exception("No se pudo conectar a la base de datos");

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 15,
                    ParameterName = "USUARIO",
                    Value = strCurrentUser
                };
                listParametros.Add(sqlParameter);

                dataSetSQL = baseSQL.fncRetornaRegistros("[dbo].[sp_app_menu_listar]", "Table", listParametros);

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

        public DataSetSQL fncDevolverMenuOpciones(string strCurrentUser)
        {
            DatosBaseSQL baseSQL = new DatosBaseSQL();
            DataSetSQL dataSetSQL = new DataSetSQL();

            SqlParameter sqlParameter;
            List<SqlParameter> listParametros = new List<SqlParameter>();

            try
            {
                var strConexion = Credential.ConnString("SQL");

                baseSQL.sqlCon = baseSQL.fncAbrirBD(strConexion);

                if (baseSQL.sqlCon.State == ConnectionState.Closed)
                    throw new Exception("No se pudo conectar a la base de datos");

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 15,
                    ParameterName = "USR_LOGIN",
                    Value = strCurrentUser
                };
                listParametros.Add(sqlParameter);

                dataSetSQL = baseSQL.fncRetornaRegistros("[configuracion].[sp_app_menu_opciones_listar]", "Table", listParametros);

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
