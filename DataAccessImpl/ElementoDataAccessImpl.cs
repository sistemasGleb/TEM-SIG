using BusinessEntity;
using CrosscuttingUtiles;
using DataAccessCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;


namespace DataAccessImpl
{
    public class ElementoDataAccessImpl
    {
        public int intError { get; set; }
        public string strTextoError { get; set; }

        public DataSetSQL CreateEditElements(string strUsuario, ParametrosBusinessEntity collection)
        {
            var baseSQL = new DatosBaseSQL();
            var dataSetSQL = new DataSetSQL();
            var _listParametros = new List<SqlParameter>();

            try
            {
                baseSQL.sqlCon = baseSQL.fncAbrirBD(Credential.ConnString("SQL"));

                if (baseSQL.sqlCon.State == ConnectionState.Closed)
                    throw new Exception("No se pudo conectar a la base de datos");

                _listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 15,
                    ParameterName = "USUARIO",
                    Value = strUsuario
                });     //@USUARIO

                _listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "Param_id",
                    Value = collection.Param_id
                });     //@Param_id

                _listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 20,
                    ParameterName = "Param_id_int",
                    Value = collection.Param_id_int == null ? (object)DBNull.Value : collection.Param_id_int.ToUpper()
                });    //@Param_id_int

                _listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 20,
                    ParameterName = "Param_id_mop",
                    Value = collection.Param_id_mop == null ? "" : collection.Param_id_mop
                });     //@Param_id_mop

                _listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 100,
                    ParameterName = "Param_ele_nom",
                    Value = collection.Param_ele_nom == null ? "" : collection.Param_ele_nom
                });     //@Param_ele_nom

                _listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.SmallInt,
                    ParameterName = "Param_tra_id",
                    Value = collection.Param_tra_id
                });     //@Param_tra_id

                _listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.TinyInt,
                    ParameterName = "Param_tip_ele",
                    Value = collection.Param_tip_ele
                });     //@Param_tip_ele

                _listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "Param_id_inv",
                    Value = collection.Param_id_inv
                });     //@Param_id_inv

                _listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.Decimal,
                    Precision = 18,
                    Scale = 4,
                    ParameterName = "Param_dm_ini",
                    Value = collection.Param_dm_ini
                });     //@Param_dm_ini

                _listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.Decimal,
                    Precision = 18,
                    Scale = 4,
                    ParameterName = "Param_dm_fin",
                    Value = (collection.Param_dm_fin == null) ? (object)DBNull.Value : collection.Param_dm_fin
                });     //@Param_dm_fin

                _listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 20,
                    ParameterName = "Param_crd_utm_est_ini",
                    Value = collection.Param_crd_utm_est_ini
                });     //@Param_crd_utm_est_ini

                _listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 20,
                    ParameterName = "Param_crd_utm_nor_ini",
                    Value = collection.Param_crd_utm_nor_ini
                });     //@Param_crd_utm_nor_ini

                _listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 20,
                    ParameterName = "Param_crd_utm_est_fin",
                    Value = collection.Param_crd_utm_est_fin
                });     //@Param_crd_utm_est_fin

                _listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 20,
                    ParameterName = "Param_crd_utm_nor_fin",
                    Value = collection.Param_crd_utm_nor_fin
                });     //@Param_crd_utm_nor_fin

                _listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 20,
                    ParameterName = "Param_crd_lat_ini",
                    Value = collection.Param_crd_lat_ini

                });     //@Param_crd_lat_ini

                _listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 20,
                    ParameterName = "Param_crd_lon_ini",
                    Value = collection.Param_crd_lon_ini

                });     //@Param_crd_lon_ini

                _listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 20,
                    ParameterName = "Param_crd_lat_fin",
                    Value = (collection.Param_crd_lat_fin == null) ? (object)DBNull.Value : collection.Param_crd_lat_fin
                });     //@Param_crd_lat_fin

                _listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 20,
                    ParameterName = "Param_crd_lon_fin",
                    Value = (collection.Param_crd_lon_fin == null) ? (object)DBNull.Value : collection.Param_crd_lon_fin
                });     //@Param_crd_lon_fin

                _listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.Bit,
                    ParameterName = "Param_vig",
                    Value = collection.Param_vig
                });     //@Param_vig

                _listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 100,
                    ParameterName = "Param_obs",
                    Value = collection.Param_obs == null ? "" : collection.Param_obs
                });     //@Param_obs

                //---TUPLA AVRIABLE-- -
                _listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.TinyInt,
                    ParameterName = "Param_lad",
                    Value = collection.Param_lad
                });     //@Param_lad

                _listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "Param_mac_id",
                    Value = collection.Param_Mac
                });     //@Param_mac_id

                dataSetSQL = baseSQL.fncExecutaSQL($"[mantencion].[sp_editar_agregar_elemento]", _listParametros, 0);


                if (dataSetSQL.intError != 0)
                    throw new Exception(dataSetSQL.strError);

            }
            catch (Exception ex)
            {
                //dataSetSQL = null;
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

        public DataSetSQL AllInfoElement(string strUsuario, Int16 IdTipo, Int32 IdElemento)
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
                    ParameterName = "USUARIO",
                    Size = 10,
                    Value = strUsuario
                };
                _listParametros.Add(sqlParameter);

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.SmallInt,
                    ParameterName = "TIPO_ELEMENTO",
                    Value = IdTipo
                };
                _listParametros.Add(sqlParameter);

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "ELEMENTO",
                    Value = IdElemento
                };
                _listParametros.Add(sqlParameter);


                dataSetSQL = baseSQL.fncRetornaRegistros("[mantencion].[sp_app_extrae_elemento_detalle]", "Table", _listParametros);

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

        public DataSetSQL DeleteElementImage(int intUsuario, string strCurrentUser = "")
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
                    ParameterName = "USR_LOGIN",
                    Value = strCurrentUser
                };

                _listParametros.Add(sqlParameter);

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "IMG_ID",
                    Value = intUsuario
                };
                _listParametros.Add(sqlParameter);

                dataSetSQL = baseSQL.fncRetornaRegistros("[mantencion].[sp_app_elemento_imagen_eliminar]", "Table", _listParametros);

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

        public DataSetSQL AddImageElements(string strCurrentUser, DataTable dt)
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

                _listParametros.Add(sqlParameter);

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.Structured,
                    ParameterName = "TABLA",
                    Value = dt
                };
                _listParametros.Add(sqlParameter);

                dataSetSQL = baseSQL.fncExecutaSQL($"[mantencion].[sp_app_elemento_imagen_agregar_editar]", _listParametros, 0);

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
