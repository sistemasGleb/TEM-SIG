using BusinessEntity;
using CrosscuttingUtiles;
using DataAccessCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DataAccessImpl
{
    public class MapDataAccessImpl
    {
        public int intError { get; set; }
        public string strTextoError { get; set; }
        public DataSetSQL GetAllMarkers(string strCurrentUser, List<int> listElementos)
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



                // AGREGAMOS LOS EJECUTIVOS DEL FILTRO
                var elementosList = listElementos.Where(x => x > 0).ToList();


                var dtElementos = new DataTable();
                dtElementos.Columns.Add("ate_id", Type.GetType("System.Int16"));

                foreach (var ejecutivo in elementosList)
                    dtElementos.Rows.Add(ejecutivo);
                // /. AGREGAMOS LOS EJECUTIVOS DEL FILTRO


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
                    ParameterName = "ELEMENTOS",
                    Value = dtElementos
                };
                _listParametros.Add(sqlParameter);

                dataSetSQL = baseSQL.fncRetornaRegistros("[dbo].[sp_app_extrae_componentes]", "TBL_componentes", _listParametros);

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

        public DataSetSQL GetMarkerDetail(string strCurrentUser, int idTipoMarker,int idMarker)
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
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "IDELEMENTO",
                    Value = idTipoMarker
                };
                _listParametros.Add(sqlParameter);

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "IDMARKER",
                    Value = idMarker
                };
                _listParametros.Add(sqlParameter);

                dataSetSQL = baseSQL.fncRetornaRegistros("[dbo].[sp_app_extrae_marcador_detalle]", "TBL_MARKER", _listParametros);

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
        /// Método que actualiza las preferencias del Usuario
        /// </summary>
        /// <param name="strCurrentUser"></param>
        /// <param name="dataTableCnf">Tabla de configuración</param>
        /// <param name="dataTableComp">Tabla de Elementos</param>
        /// <returns></returns>
        public DataSetSQL UpdateUserPreferences(string strCurrentUser, DataTable dataTableCnf, DataTable dataTableComp)
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
                    ParameterName = "CONFIGURACION",
                    Value = dataTableCnf
                };
                _listParametros.Add(sqlParameter);

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.Structured,
                    ParameterName = "ELEMENTOS",
                    Value = dataTableComp
                };
                _listParametros.Add(sqlParameter);

                dataSetSQL = baseSQL.fncRetornaRegistros("[dbo].[sp_app_guardar_editar_configuracion]", "USUARIO", _listParametros);

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
