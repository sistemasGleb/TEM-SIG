//using BusinessEntity;
using BusinessEntity;
using CrosscuttingUtiles;
using DataAccessCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
//using static BusinessEntity.PerfilBusinessEntity;

namespace DataAccessImpl
{
    public class CargoDataAccessImpl
    {
        public int intError { get; set; }
        public string strTextoError { get; set; }
        public string strConexion = Credential.ConnString("SQL");

        /// <summary>
        /// Método que lista la totalidad de los cargos de la Tabla
        /// </summary>
        public BusinessEntity.DataSetSQL ListAll(string strCurrentUser)
        {
            DatosBaseSQL baseSQL = new DatosBaseSQL();
            BusinessEntity.DataSetSQL dataSetSQL = new BusinessEntity.DataSetSQL();

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
                    ParameterName = "ID_CARGO",
                    Value = (int)0
                };

                _listParametros.Add(sqlParameter);


                dataSetSQL = baseSQL.fncRetornaRegistros("[configuracion].[sp_app_cargo_listar]", "Table", _listParametros);

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
        /// Método que devuelve un cargo segun su id
        /// </summary>
        public BusinessEntity.DataSetSQL ListById(string strCurrentUser, int iPerfil)
        {
            DatosBaseSQL baseSQL = new DatosBaseSQL();
            BusinessEntity.DataSetSQL dataSetSQL = new BusinessEntity.DataSetSQL();

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
                    ParameterName = "ID_CARGO",
                    Value = iPerfil
                };

                _listParametros.Add(sqlParameter);

                dataSetSQL = baseSQL.fncRetornaRegistros("[configuracion].[sp_app_cargo_listar]", "Table", _listParametros);

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
        public bool fncGuardarCargo(BusinessEntity.CargoBusinessEntity.Cargo cargo)
        {
            bool blnActualizado;
            DatosBaseSQL baseSQL = new DatosBaseSQL();
            BusinessEntity.DataSetSQL dataSetSQL = new BusinessEntity.DataSetSQL();
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
                sqlParameter.ParameterName = "CAR_COD";
                sqlParameter.Value = cargo.car_cod;
                listParametros.Add(sqlParameter);

                sqlParameter = new SqlParameter();
                sqlParameter.SqlDbType = SqlDbType.VarChar;
                sqlParameter.Size = 50;
                sqlParameter.ParameterName = "CAR_NOM";
                sqlParameter.Value = cargo.car_nom;
                listParametros.Add(sqlParameter);

                sqlParameter = new SqlParameter();
                sqlParameter.SqlDbType = SqlDbType.VarChar;
                sqlParameter.Size = 200;
                sqlParameter.ParameterName = "CAR_DES";
                sqlParameter.Value = cargo.car_des == null ? (object)DBNull.Value : cargo.car_des;
                listParametros.Add(sqlParameter);

                sqlParameter = new SqlParameter();
                sqlParameter.SqlDbType = SqlDbType.Bit;
                sqlParameter.ParameterName = "CAR_DEL";
                sqlParameter.Value = cargo.car_del;
                listParametros.Add(sqlParameter);

                sqlParameter = new SqlParameter();
                sqlParameter.SqlDbType = SqlDbType.Bit;
                sqlParameter.ParameterName = "CAR_MOD";
                sqlParameter.Value = cargo.car_mod;
                listParametros.Add(sqlParameter);

                sqlParameter = new SqlParameter();
                sqlParameter.SqlDbType = SqlDbType.VarChar;
                sqlParameter.Size = 15;
                sqlParameter.ParameterName = "CAR_CRE_USR";
                sqlParameter.Value = cargo.car_cre_usr;
                listParametros.Add(sqlParameter);

                DataTable dtMenus = new DataTable();
                dtMenus.Columns.Add("menu_cod", Type.GetType("System.String"));

                foreach (BusinessEntity.MenuBusinessEntity.Menu menu in cargo.listMenu)
                    dtMenus.Rows.Add(menu.menu_cod);

                sqlParameter = new SqlParameter();
                sqlParameter.SqlDbType = SqlDbType.Structured;
                sqlParameter.ParameterName = "MENUS";
                sqlParameter.Value = dtMenus;
                listParametros.Add(sqlParameter);

                dataSetSQL = baseSQL.fncRetornaRegistros("[configuracion].[sp_app_cargo_agregar_editar]", "PERFIL", listParametros);

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

        public DataSetSQL DeleteCargo(int intIdCargo, string strCurrentUser = "")
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
                    ParameterName = "USR_CGO_ID",
                    Value = intIdCargo
                };
                _listParametros.Add(sqlParameter);

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 15,
                    ParameterName = "USR_CGO_LOGIN",
                    Value = strCurrentUser
                };
                _listParametros.Add(sqlParameter);

                dataSetSQL = baseSQL.fncRetornaRegistros("[configuracion].[sp_app_cargo_eliminar]", "Table", _listParametros);

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
        public BusinessEntity.DataSetSQL fncDevolverMenus(int? intCodigoPerfil = 0)
        {
            DatosBaseSQL baseSQL = new DatosBaseSQL();
            BusinessEntity.DataSetSQL dataSetSQL = new BusinessEntity.DataSetSQL();
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
                sqlParameter.ParameterName = "CGO_COD";
                sqlParameter.Value = intCodigoPerfil;
                listParametros.Add(sqlParameter);

                dataSetSQL = baseSQL.fncRetornaRegistros("[configuracion].[sp_app_menu_cargo_listar]", "Menus", listParametros);

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

        public List<BusinessEntity.MenuBusinessEntity.Menu> fncDevolverSubMenus(int strCodigoMenu, int strCodigoSubMenu, int? intCodigoPerfil = 0)
        {
            DatosBaseSQL baseSQL = new DatosBaseSQL();
            BusinessEntity.DataSetSQL dataSetSQL = new BusinessEntity.DataSetSQL();
            SqlParameter sqlParameter;
            List<SqlParameter> listParametros = new List<SqlParameter>();
            List<BusinessEntity.MenuBusinessEntity.Menu> listMenus = new List<BusinessEntity.MenuBusinessEntity.Menu>();
            BusinessEntity.MenuBusinessEntity.Menu menu;

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
                sqlParameter.ParameterName = "CGO_COD";
                sqlParameter.Value = intCodigoPerfil;
                listParametros.Add(sqlParameter);

                dataSetSQL = baseSQL.fncRetornaRegistros("[configuracion].[sp_app_menusub_listar]", "Menus", listParametros);

                if (dataSetSQL.intError != 0)
                    throw new Exception(dataSetSQL.strError);

                foreach (DataRow dataRow in dataSetSQL.dsSQL.Tables["Menus"].Rows)
                {
                    menu = new BusinessEntity.MenuBusinessEntity.Menu();
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


        #endregion
    }
}
