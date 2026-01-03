using CrosscuttingUtiles;
using DataAccessCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DataAccessImpl
{
    public class LogDataAccessImpl
    {
        public int intError { get; set; }
        public string strTextoError { get; set; }

        public void AddNewLog(BusinessEntity.LogBusinessEntity collection)
        {
            var baseSQL = new DatosBaseSQL();
            var _listParametros = new List<SqlParameter>();

            try
            {
                baseSQL.sqlCon = baseSQL.fncAbrirBD(Credential.ConnString("SQL"));

                if (baseSQL.sqlCon.State == ConnectionState.Closed)
                    throw new Exception("No se pudo conectar a la base de datos");

                #region Parametros

                _listParametros = new List<SqlParameter>
                {
                    new SqlParameter
                    {
                        SqlDbType = SqlDbType.BigInt,
                        ParameterName = "tla_id",
                        Value = collection.tla_id
                    },
                    new SqlParameter
                    {
                        SqlDbType = SqlDbType.DateTime,
                        ParameterName = "tla_fec_ing",
                        Value = collection.tla_fec_ing == null ? (object) DBNull.Value : collection.tla_fec_ing
                    },
                    new SqlParameter
                    {
                        SqlDbType = SqlDbType.VarChar,
                        Size = 15,
                        ParameterName = "tla_usr_lgn",
                        Value = collection.tla_usr_lgn != null ? collection.tla_usr_lgn : (object) DBNull.Value
                    },
                    new SqlParameter
                    {
                        SqlDbType = SqlDbType.SmallInt,
                        ParameterName = "tla_app_id",
                        Value = collection.tla_app_id == null ? (object) DBNull.Value : collection.tla_app_id
                    },
                    new SqlParameter
                    {
                        SqlDbType = SqlDbType.VarChar,
                        Size = 20,
                        ParameterName = "tla_ipp",
                        Value = collection.tla_ipp == null ? (object) DBNull.Value : collection.tla_ipp
                    },
                    new SqlParameter
                    {
                        SqlDbType = SqlDbType.SmallInt,
                        ParameterName = "tla_tip_pla",
                        Value = collection.tla_tip_pla == null ? (object) DBNull.Value : collection.tla_tip_pla
                    },
                    new SqlParameter
                    {
                        SqlDbType = SqlDbType.VarChar,
                        Size = 100,
                        ParameterName = "tla_ctr",
                        Value = collection.tla_ctr == null ? (object) DBNull.Value : collection.tla_ctr
                    },
                    new SqlParameter
                    {
                        SqlDbType = SqlDbType.VarChar,
                        Size = 100,
                        ParameterName = "tla_ctr_act",
                        Value = collection.tla_ctr_act == null ? (object) DBNull.Value : collection.tla_ctr_act
                    },
                    new SqlParameter
                    {
                        SqlDbType = SqlDbType.VarChar,
                        Size = 400,
                        ParameterName = "tla_des",
                        Value = collection.tla_des == null ? (object) DBNull.Value : collection.tla_des
                    }
                };

                #endregion

                var dataSetSQL = baseSQL.fncRetornaRegistros("[dbo].[sp_app_log_agregar_editar]", "Table", _listParametros);

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
 
        }
    }
}
