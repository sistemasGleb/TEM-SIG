using BusinessEntity;
using CrosscuttingUtiles;
using DataAccessCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DataAccessImpl
{
    public class ReportDataAccessImpl
    {
        public int intError { get; set; }
        public string strTextoError { get; set; }


        public DataSetSQL get_Report_Filter(string strUsuario, DataTable TblTramos, DataTable TblTipoElementos, DataTable TblElementos)
        {
            DatosBaseSQL baseSQL = new DatosBaseSQL();
            DataSetSQL dataSetSQL = new DataSetSQL();

            List<SqlParameter> _listParametros = new List<SqlParameter>();

            try
            {
                var strConexion = Credential.ConnString("SQL");

                baseSQL.sqlCon = baseSQL.fncAbrirBD(strConexion);

                if (baseSQL.sqlCon.State == ConnectionState.Closed)
                    throw new Exception("No se pudo conectar a la base de datos");

                _listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    ParameterName = "USUARIO",
                    Size = 15,
                    Value = strUsuario
                });

                _listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.Structured,
                    ParameterName = "TRAMOS",
                    Value = TblTramos
                });

                _listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.Structured,
                    ParameterName = "TIPO",
                    Value = TblTipoElementos
                });

                _listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.Structured,
                    ParameterName = "ELEMENTOS",
                    Value = TblElementos
                });

                dataSetSQL = baseSQL.fncRetornaRegistros("[reportes].[sp_app_extrae_reportes_filtros]", "Table", _listParametros);

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
        ///  Metodo que lista reportes - sp_app_extrae_reportes
        /// </summary>
        /// <param name="strUsuario"></param>
        /// <param name="IdTipo"></param>
        /// <param name="IdElemento"></param>
        /// <returns></returns>

        public DataSetSQL get_Report(string strUsuario, Int16 IdTipo, Int16 IdElemento)
        {
            DatosBaseSQL baseSQL = new DatosBaseSQL();
            DataSetSQL dataSetSQL = new DataSetSQL();
            List<SqlParameter> _listParametros = new List<SqlParameter>();

            try
            {
                var strConexion = Credential.ConnString("SQL");

                baseSQL.sqlCon = baseSQL.fncAbrirBD(strConexion);

                if (baseSQL.sqlCon.State == ConnectionState.Closed)
                    throw new Exception("No se pudo conectar a la base de datos");

                _listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    ParameterName = "USUARIO",
                    Size = 10,
                    Value = strUsuario
                });

                _listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.SmallInt,
                    ParameterName = "TIPO_ELEMENTO",
                    Value = IdTipo
                });

                _listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.SmallInt,
                    ParameterName = "ELEMENTO",
                    Value = IdElemento
                });

                dataSetSQL = baseSQL.fncRetornaRegistros("[reportes].[sp_app_extrae_reportes]", "Table", _listParametros);

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
