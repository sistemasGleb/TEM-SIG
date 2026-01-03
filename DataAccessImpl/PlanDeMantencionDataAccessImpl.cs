using BusinessEntity;
using DataAccessCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DataAccessImpl
{
    public class PlanDeMantencionDataAccessImpl
    {
        public int intError { get; set; }
        public string strTextoError { get; set; }

        /// <summary>
        /// Lista todos los planes de mantencion existentes
        /// </summary>
        /// <param name="strCurrentUser"></param>
        /// <param name="strGuid"></param>
        /// <returns></returns>
        public DataSetSQL ListAllPlanning(string strCurrentUser, string strGuid)
        {
            DatosBaseSQL baseSQL = new DatosBaseSQL();
            DataSetSQL dataSetSQL = new DataSetSQL();
            List<SqlParameter> _listParametros = new List<SqlParameter>();

            try
            {
                var strConexion = CrosscuttingUtiles.Credential.ConnString("SQL");

                baseSQL.sqlCon = baseSQL.fncAbrirBD(strConexion);

                if (baseSQL.sqlCon.State == ConnectionState.Closed)
                    throw new Exception("No se pudo conectar a la base de datos");

                _listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 15,
                    ParameterName = "USUARIO",
                    Value = strCurrentUser
                });

                _listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 36,
                    ParameterName = "GUID_ID",
                    Value = strGuid
                });

                dataSetSQL = baseSQL.fncRetornaRegistros("[dbo].[sp_app_plan_conservacion_listar]", "Table", _listParametros);

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
        /// Método que lista plan de mantención segun año seleccionado
        /// </summary>
        public DataSetSQL ListFilterByGuid(string strCurrentUser,string strGuid)
        {
            DatosBaseSQL baseSQL = new DatosBaseSQL();
            DataSetSQL dataSetSQL = new DataSetSQL();
            List<SqlParameter> _listParametros = new List<SqlParameter>();

            try
            {
                var strConexion = CrosscuttingUtiles.Credential.ConnString("SQL");

                baseSQL.sqlCon = baseSQL.fncAbrirBD(strConexion);

                if (baseSQL.sqlCon.State == ConnectionState.Closed)
                    throw new Exception("No se pudo conectar a la base de datos");

                _listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 15,
                    ParameterName = "USUARIO",
                    Value = strCurrentUser
                });

                _listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 36,
                    ParameterName = "GUID_ID",
                    Value = strGuid
                });

                dataSetSQL = baseSQL.fncRetornaRegistros("[dbo].[sp_app_plan_conservacion_listar_detalle]", "Table", _listParametros);

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

        public DataSetSQL ListIngresoFiltered(string strCurrentUser, Int32 maintenanceTypeSelected, Nullable<DateTime> startDate, Nullable<DateTime> endDate)
        {
            DatosBaseSQL baseSQL = new DatosBaseSQL();
            DataSetSQL dataSetSQL = new DataSetSQL();
            List<SqlParameter> _listParametros = new List<SqlParameter>();

            try
            {
                var strConexion = CrosscuttingUtiles.Credential.ConnString("SQL");

                baseSQL.sqlCon = baseSQL.fncAbrirBD(strConexion);

                if (baseSQL.sqlCon.State == ConnectionState.Closed)
                    throw new Exception("No se pudo conectar a la base de datos");

                _listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 15,
                    ParameterName = "USUARIO",
                    Value = strCurrentUser
                });

                _listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.SmallInt,
                    ParameterName = "TIPO",
                    Value = maintenanceTypeSelected
                });

                _listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.DateTime,
                    ParameterName = "FCHA_INICIO",
                    Value = startDate
                });

                _listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.DateTime,
                    ParameterName = "FECHA_FIN",
                    Value = endDate
                });

                dataSetSQL = baseSQL.fncRetornaRegistros("[dbo].[sp_app_plan_conservacion_ingreso_listar]", "Table", _listParametros);

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

        public DataSetSQL ListElementsByType(string strCurrentUser, Int32 elementTypeSelected)
        {
            DatosBaseSQL baseSQL = new DatosBaseSQL();
            DataSetSQL dataSetSQL = new DataSetSQL();
            List<SqlParameter> _listParametros = new List<SqlParameter>();

            try
            {
                var strConexion = CrosscuttingUtiles.Credential.ConnString("SQL");

                baseSQL.sqlCon = baseSQL.fncAbrirBD(strConexion);

                if (baseSQL.sqlCon.State == ConnectionState.Closed)
                    throw new Exception("No se pudo conectar a la base de datos");

                _listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 15,
                    ParameterName = "USUARIO",
                    Value = strCurrentUser
                });

                _listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "TIP_ELE",
                    Value = elementTypeSelected
                });

                dataSetSQL = baseSQL.fncRetornaRegistros("[dbo].[sp_app_plan_conservacion_listar_elementos]", "Table", _listParametros);

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

        public DataSetSQL ListFilterActivityByPlan(BusinessEntity.FormModels.FormPlanDeMantencion.Parametros collection)
        {
            DatosBaseSQL baseSQL = new DatosBaseSQL();
            DataSetSQL dataSetSQL = new DataSetSQL();
            List<SqlParameter> _listParametros = new List<SqlParameter>();

            try
            {
                var strConexion = CrosscuttingUtiles.Credential.ConnString("SQL");

                baseSQL.sqlCon = baseSQL.fncAbrirBD(strConexion);

                if (baseSQL.sqlCon.State == ConnectionState.Closed)
                    throw new Exception("No se pudo conectar a la base de datos");

                _listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 15,
                    ParameterName = "USUARIO",
                    Value = collection.currentUserSelected
                });

                _listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "PLAN_ID",
                    Value = collection.IdMaintenancePlanning
                });

                _listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "ANHO_ID",
                    Value = collection.YearMaintenancePlanning
                });

                dataSetSQL = baseSQL.fncRetornaRegistros("[dbo].[sp_app_plan_conservacion_listar_activiades]", "Table", _listParametros);

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

        public DataSetSQL CuardarNuevaMantencion(BusinessEntity.FormModels.FormPlanDeMantencion.Parametros collection)
        {
            DatosBaseSQL baseSQL = new DatosBaseSQL();
            DataSetSQL dataSetSQL = new DataSetSQL();

            SqlParameter sqlParameter;
            List<SqlParameter> _listParametros = new List<SqlParameter>();

            try
            {
                var strConexion = CrosscuttingUtiles.Credential.ConnString("SQL");

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
                _listParametros.Add(sqlParameter);

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "tra_pla_id",
                    Value = collection.IdMaintenancePlanning
                };
                _listParametros.Add(sqlParameter);

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.SmallInt,
                    ParameterName = "tra_tip_mant_id",
                    Value = collection.IdMaintenanceType
                };
                _listParametros.Add(sqlParameter);

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "tra_act_id",
                    Value = collection.IdActivity
                };
                _listParametros.Add(sqlParameter);

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "tra_tip_ele_id",
                    Value = collection.ElementType
                };
                _listParametros.Add(sqlParameter);

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "tra_ele_id",
                    Value = collection.ElementCode
                };
                _listParametros.Add(sqlParameter);

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.DateTime,
                    ParameterName = "tra_fec_ing",
                    Value = collection.MaintenanceDate
                };
                _listParametros.Add(sqlParameter);

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "tra_res_id",
                    Value = collection.IdResponsible
                };
                _listParametros.Add(sqlParameter);

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.Decimal,
                    Precision = 18,
                    Scale = 4,
                    ParameterName = "tra_can",
                    Value = collection.quantity
                };
                _listParametros.Add(sqlParameter);

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 1000,
                    ParameterName = "tra_obs",
                    Value = collection.observations
                };
                _listParametros.Add(sqlParameter);

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.SmallInt,
                    ParameterName = "tra_uni",
                    Value = collection.unit
                };
                _listParametros.Add(sqlParameter);

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    ParameterName = "tra_guid",
                    Value = collection.guidIdentifier
                };
                _listParametros.Add(sqlParameter);

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "Identity",
                    Value = 0,
                    Direction = ParameterDirection.Output
                };
                _listParametros.Add(sqlParameter);

                dataSetSQL = baseSQL.fncRetornaRegistros("[dbo].[sp_app_plan_mantencion_agregar_editar]", "Table", _listParametros);

                if (dataSetSQL.intError <0)
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

        public DataSetSQL SubirArchivoPlanMantencion(BusinessEntity.FormModels.FormPlanDeMantencion.Parametros collection)
        {
            DatosBaseSQL baseSQL = new DatosBaseSQL();
            DataSetSQL dataSetSQL = new DataSetSQL();

            SqlParameter sqlParameter;
            List<SqlParameter> _listParametros = new List<SqlParameter>();

            try
            {
                var strConexion = CrosscuttingUtiles.Credential.ConnString("SQL");

                baseSQL.sqlCon = baseSQL.fncAbrirBD(strConexion);

                if (baseSQL.sqlCon.State == ConnectionState.Closed)
                    throw new Exception("No se pudo conectar a la base de datos");

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 15,
                    ParameterName = "USUARIO",
                    Value = collection.currentUserSelected
                };
                _listParametros.Add(sqlParameter);

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 40,
                    ParameterName = "GUID_IDD",
                    Value = collection.guidIdentifier.ToString()
                };
                _listParametros.Add(sqlParameter);

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "YEAR",
                    Value = collection.year
                };
                _listParametros.Add(sqlParameter);

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.SmallInt,
                    ParameterName = "VERSION",
                    Value = collection.version
                };
                _listParametros.Add(sqlParameter);

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 250,
                    ParameterName = "FILE",
                    Value = collection.fileName
                };
                _listParametros.Add(sqlParameter);

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 500,
                    ParameterName = "OBSERVACIONES",
                    Value = collection.observations
                };
                _listParametros.Add(sqlParameter);

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.Structured,
                    ParameterName = "TABLA",
                    Value = collection.PlanningTable
                };
                _listParametros.Add(sqlParameter);

                sqlParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.Structured,
                    ParameterName = "CATEGORIAS",
                    Value = collection.CategoryTable
                };
                _listParametros.Add(sqlParameter);


                dataSetSQL = baseSQL.fncRetornaRegistros("[dbo].[sp_app_plan_mantencion_subir archivo]", "Table", _listParametros);

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

