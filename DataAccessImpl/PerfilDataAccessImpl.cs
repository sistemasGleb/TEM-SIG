using BusinessEntity;
using CrosscuttingUtiles;
using DataAccessCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using static BusinessEntity.PerfilBusinessEntity;


namespace DataAccessImpl
{
    public class PerfilDataAccessImpl
    {
        public int intError { get; set; }
        public string strTextoError { get; set; }
        public string strConexion = Credential.ConnString("SQL");

        /// <summary>
        /// Método que lista la totalidad de los perfiles de la Tabla
        /// </summary>
        public DataSetSQL ListAll(string strCurrentUser)
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
                    SqlDbType = SqlDbType.VarChar,
                    Size = 15,
                    ParameterName = "USUARIO",
                    Value = strCurrentUser
                };

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "ID_PERFIL",
                    Value = (int)0
                };

                _listParametros.Add(sqlParameter);


                dataSetSQL = baseSQL.fncRetornaRegistros("[configuracion].[sp_app_perfil_listar]", "Table", _listParametros);

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

        /// <summary>
        /// Método que devuelve un perfil segun su id
        /// </summary>
        public DataSetSQL ListById(string strCurrentUser, int iPerfil)
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
                    SqlDbType = SqlDbType.VarChar,
                    Size = 15,
                    ParameterName = "USUARIO",
                    Value = strCurrentUser
                };

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "ID_PERFIL",
                    Value = iPerfil
                };

                _listParametros.Add(sqlParameter);


                dataSetSQL = baseSQL.fncRetornaRegistros("[configuracion].[sp_app_perfil_listar]", "Table", _listParametros);

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

        #region DETALLE - MENU
        public DataSetSQL fncDevolverMenus(int? intCodigoPerfil = 0)
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

                sqlParameter = new SqlParameter();
                sqlParameter.SqlDbType = SqlDbType.Int;
                sqlParameter.ParameterName = "PER_COD";
                sqlParameter.Value = intCodigoPerfil;
                listParametros.Add(sqlParameter);

                dataSetSQL = baseSQL.fncRetornaRegistros("[configuracion].[sp_app_menu_listar]", "Menus", listParametros);

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

        public List<MenuBusinessEntity.Menu> fncDevolverSubMenus(int strCodigoMenu, int strCodigoSubMenu  , int? intCodigoPerfil = 0)
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

                sqlParameter = new SqlParameter();
                sqlParameter.SqlDbType = SqlDbType.Int;
                sqlParameter.ParameterName = "MENU_COD";
                sqlParameter.Value = strCodigoMenu;
                listParametros.Add(sqlParameter);

                sqlParameter = new SqlParameter();
                sqlParameter.SqlDbType = SqlDbType.Int;
                sqlParameter.ParameterName = "SUBMENU_COD";
                sqlParameter.Value = strCodigoSubMenu;
                listParametros.Add(sqlParameter);

                sqlParameter = new SqlParameter();
                sqlParameter.SqlDbType = SqlDbType.Int;
                sqlParameter.ParameterName = "PER_COD";
                sqlParameter.Value = intCodigoPerfil;
                listParametros.Add(sqlParameter);

                dataSetSQL = baseSQL.fncRetornaRegistros("[configuracion].[sp_app_menusub_listar]", "Menus", listParametros);

                if (dataSetSQL.intError != 0)
                    throw new Exception(dataSetSQL.strError);

                foreach (DataRow dataRow in dataSetSQL.dsSQL.Tables["Menus"].Rows)
                {
                    menu = new MenuBusinessEntity.Menu();
                    menu.menu_cod = ((Int32)dataRow["menu_cod"]).ToString();
                    menu.menu_nom = (string)dataRow["menu_desc"];
                    menu.menu_sel = (bool)dataRow["menu_sel"];
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

        public bool fncGuardarPerfil(Perfil perfil)
        {
            bool blnActualizado;
            DatosBaseSQL baseSQL = new DatosBaseSQL();
            DataSetSQL dataSetSQL = new DataSetSQL();
            SqlParameter sqlParameter;
            List<SqlParameter> listParametros = new List<SqlParameter>();

            try
            {
                baseSQL.sqlCon = baseSQL.fncAbrirBD(strConexion);

                if (baseSQL.sqlCon.State == ConnectionState.Closed)
                    throw new Exception("No se pudo conectar a la base de datos");

                listParametros = new List<SqlParameter>();

                sqlParameter = new SqlParameter();
                sqlParameter.SqlDbType = SqlDbType.Int;
                sqlParameter.ParameterName = "PER_COD";
                sqlParameter.Value = perfil.per_cod;
                listParametros.Add(sqlParameter);

                sqlParameter = new SqlParameter();
                sqlParameter.SqlDbType = SqlDbType.VarChar;
                sqlParameter.Size = 50;
                sqlParameter.ParameterName = "PER_NOM";
                sqlParameter.Value = perfil.per_nom;
                listParametros.Add(sqlParameter);

                sqlParameter = new SqlParameter();
                sqlParameter.SqlDbType = SqlDbType.VarChar;
                sqlParameter.Size = 200;
                sqlParameter.ParameterName = "PER_DES";
                sqlParameter.Value = perfil.per_des == null ? (object)DBNull.Value : perfil.per_des;
                listParametros.Add(sqlParameter);

                sqlParameter = new SqlParameter();
                sqlParameter.SqlDbType = SqlDbType.VarChar;
                sqlParameter.Size = 15;
                sqlParameter.ParameterName = "PER_CRE_USR";
                sqlParameter.Value = perfil.per_cre_usr;
                listParametros.Add(sqlParameter);

                DataTable dtMenus = new DataTable();
                dtMenus.Columns.Add("menu_cod", Type.GetType("System.String"));

                foreach (MenuBusinessEntity.Menu menu in perfil.listMenu)
                    dtMenus.Rows.Add(menu.menu_cod);

                sqlParameter = new SqlParameter();
                sqlParameter.SqlDbType = SqlDbType.Structured;
                sqlParameter.ParameterName = "MENUS";
                sqlParameter.Value = dtMenus;
                listParametros.Add(sqlParameter);

                dataSetSQL = baseSQL.fncRetornaRegistros("[configuracion].[sp_app_perfil_agregar_editar]", "PERFIL", listParametros);

                if (dataSetSQL.intError != 0)
                    throw new Exception(dataSetSQL.strError);

                blnActualizado = true;
            }
            catch (Exception ex)
            {
                blnActualizado = false;
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

            return blnActualizado;
        }
        #endregion

        public DataSetSQL DeletePerfil(int intIdPerfil, string strCurrentUser= "")
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
                    ParameterName = "USR_PER_ID",
                    Value = intIdPerfil
                };
                _listParametros.Add(sqlParameter);

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 15,
                    ParameterName = "USR_PER_LOGIN",
                    Value = strCurrentUser
                };
                _listParametros.Add(sqlParameter);

                dataSetSQL = baseSQL.fncRetornaRegistros("[configuracion].[sp_app_perfil_eliminar]", "Table", _listParametros);

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

        public DataSetSQL ListAsignedById(string strCurrentUser, int iPerfil)
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
                    SqlDbType = SqlDbType.VarChar,
                    Size = 15,
                    ParameterName = "USUARIO",
                    Value = strCurrentUser
                };

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "ID_PERFIL",
                    Value = iPerfil
                };

                _listParametros.Add(sqlParameter);


                dataSetSQL = baseSQL.fncRetornaRegistros("[configuracion].[sp_app_perfil_cargo_listar]", "Table", _listParametros);

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