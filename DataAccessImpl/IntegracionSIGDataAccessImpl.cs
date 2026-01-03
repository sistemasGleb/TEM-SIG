using BusinessEntity;
using DataAccessCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Remoting.Messaging;

namespace DataAccessImpl
{
    public class IntegracionSIGDataAccessImpl
    {
        #region Campos Privados
        public int intError { get; set; }
        public string strTextoError { get; set; }

        readonly string strConexion =CrosscuttingUtiles.Credential.ConnString("SQL");
        readonly DatosBaseSQL baseSQL ;
        internal DataSetSQL dataSetSQL ;
        #endregion

        public IntegracionSIGDataAccessImpl()
        {
            baseSQL = new DatosBaseSQL();
            dataSetSQL = new DataSetSQL();
        }

        #region Metodos Publicos
        public DataSetSQL CuardarNuevaMantencionIntegracion(int idMantencion, BusinessEntity.FormModels.FormPlanDeMantencion.Parametros collection)
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
                    ParameterName = "USUARIO",
                    Value = collection.currentUserSelected
                };
                _listParametros.Add(sqlParameter);          // USUARIO

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.DateTime,
                    ParameterName = "Fecha_Registro",
                    Value = collection.MaintenanceDate
                };
                _listParametros.Add(sqlParameter);          // Fecha_Registro

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "Id_Elemento",
                    Value = collection.ElementCode
                };
                _listParametros.Add(sqlParameter);          // Id_Elemento

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "Id_Tipo_Elemento",
                    Value = collection.ElementType
                };
                _listParametros.Add(sqlParameter);          // Id_Tipo_Elemento

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.TinyInt,
                    ParameterName = "Id_Accion",
                    Value = collection.IdActivity
                };
                _listParametros.Add(sqlParameter);          // Id_Accion

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "Id_Mantencion",
                    Value = idMantencion
                };
                _listParametros.Add(sqlParameter);          // Id_Accion

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    ParameterName = "Id_Guid",
                    Value = collection.guidIdentifier
                };
                _listParametros.Add(sqlParameter);          // Id_Accion

                dataSetSQL = baseSQL.fncRetornaRegistros("[integracion].[sp_app_interfaz_agregar_editar]", "Table", _listParametros);

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

        public DataSetSQL LogIntegracionWS(int idMantencion,  string sEndpointUrl,
                                            BusinessEntity.FormModels.FormPlanDeMantencion.Parametros iCollection,  bool bResult,   string sMessage)
        {
            //////DatosBaseSQL baseSQL = new DatosBaseSQL();
            //////DataSetSQL dataSetSQL = new DataSetSQL();
            try
            {
                baseSQL.sqlCon = baseSQL.fncAbrirBD(strConexion);

                if (baseSQL.sqlCon.State == ConnectionState.Closed)
                    throw new Exception("No se pudo conectar a la base de datos");

               var _listParametros = new List<SqlParameter> {new SqlParameter
                                                            {
                                                                SqlDbType = SqlDbType.VarChar,
                                                                Size = 10,
                                                                ParameterName = "USUARIO",
                                                                Value = iCollection.currentUserSelected
                                                            }, new SqlParameter
                                                            {
                                                                SqlDbType = SqlDbType.Bit,
                                                                ParameterName = "Estado",
                                                                Value = bResult
                                                            },new SqlParameter
                                                            {
                                                                SqlDbType = SqlDbType.NVarChar,
                                                                ParameterName = "Endpoint",
                                                                Size = 1000,
                                                                Value = sEndpointUrl
                                                            },new SqlParameter
                                                            {
                                                                SqlDbType = SqlDbType.NVarChar,
                                                                ParameterName = "Mensage",
                                                                Size = 4000,
                                                                Value = sMessage
                                                            },new SqlParameter
                                                            {
                                                                SqlDbType = SqlDbType.DateTime,
                                                                ParameterName = "Fecha_Registro",
                                                                Value = iCollection.MaintenanceDate
                                                            }, new SqlParameter
                                                            {
                                                                SqlDbType = SqlDbType.Int,
                                                                ParameterName = "Id_Elemento",
                                                                Value = iCollection.ElementCode
                                                            }, new SqlParameter
                                                            {
                                                                SqlDbType = SqlDbType.Int,
                                                                ParameterName = "Id_Tipo_Elemento",
                                                                Value = iCollection.ElementType
                                                            }, new SqlParameter
                                                            {
                                                                SqlDbType = SqlDbType.TinyInt,
                                                                ParameterName = "Id_Accion",
                                                                Value = iCollection.IdActivity
                                                            }, new SqlParameter
                                                            {
                                                                SqlDbType = SqlDbType.Int,
                                                                ParameterName = "Id_Mantencion",
                                                                Value = idMantencion
                                                            },new SqlParameter
                                                            {
                                                                SqlDbType = SqlDbType.UniqueIdentifier,
                                                                ParameterName = "Id_Guid",
                                                                Value = iCollection.guidIdentifier
                                                            } };

                dataSetSQL = baseSQL.fncRetornaRegistros("[integracion].[sp_app_interfaz_ws_log_envio]", "Table", _listParametros);

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

        public DataSetSQL ListarLogIntegracionWS(string strCurrentUser, int iEstado, DateTime dFechaInicio, DateTime dFechaFin, string sUsuario, int iPlanMAntencion)
        {
            try
            {
                baseSQL.sqlCon = baseSQL.fncAbrirBD(strConexion);

                if (baseSQL.sqlCon.State == ConnectionState.Closed)
                    throw new Exception("No se pudo conectar a la base de datos");

                var _listParametros = new List<SqlParameter> {new SqlParameter
                                                            {
                                                                SqlDbType = SqlDbType.VarChar,
                                                                Size = 15,
                                                                ParameterName = "USUARIO",
                                                                Value = strCurrentUser
                                                            }, new SqlParameter
                                                            {
                                                                SqlDbType = SqlDbType.Int,
                                                                ParameterName = "ESTADO",
                                                                Value = iEstado
                                                            },new SqlParameter
                                                            {
                                                                SqlDbType = SqlDbType.DateTime,
                                                                ParameterName = "FECHA_INICIO",
                                                                Value = dFechaInicio
                                                            },new SqlParameter
                                                            {
                                                                SqlDbType = SqlDbType.DateTime,
                                                                ParameterName = "FECHA_FIN",
                                                                Value = dFechaFin
                                                            },new SqlParameter
                                                            {
                                                                SqlDbType = SqlDbType.VarChar,
                                                                Size = 15,
                                                                ParameterName = "ID_USUARIO",
                                                                Value = strCurrentUser
                                                            },new SqlParameter
                                                            {
                                                                SqlDbType = SqlDbType.Int,
                                                                ParameterName = "ID_PLAN",
                                                                Value = iPlanMAntencion
                                                            }};

                dataSetSQL = baseSQL.fncRetornaRegistros("[integracion].[sp_app_interfaz_ws_listar]", "Table", _listParametros);

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
        #endregion


    }
}
