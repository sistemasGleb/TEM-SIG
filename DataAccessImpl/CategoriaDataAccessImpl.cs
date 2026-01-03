using BusinessEntity;
using CrosscuttingUtiles;
using DataAccessCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DataAccessImpl
{
    public class CategoriaDataAccessImpl
    {
        public int intError { get; set; }
        public string strTextoError { get; set; }

        public DataSetSQL ListAll(string strCurrentUser)
        {
            var baseSQL = new DatosBaseSQL();
            var dataSetSQL = new DataSetSQL();

            var _listParametros = new List<SqlParameter>();

            try
            {
                var strConexion = Credential.ConnString("SQL");

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

                dataSetSQL = baseSQL.fncRetornaRegistros("[dbo].[sp_app_categorias_listar]", "Table", _listParametros);

                if (dataSetSQL.intError != 0)
                    throw new Exception(dataSetSQL.strError);
            }
            catch (Exception ex)
            {
                intError = 1;
                strTextoError = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
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

        public DataSetSQL Create(CategoriaBusinessEntity collection, string strCurrentUser = "")
        {
            DatosBaseSQL baseSQL = new DatosBaseSQL();
            DataSetSQL dataSetSQL = new DataSetSQL();

            List<SqlParameter> _listParametros = new List<SqlParameter>();

            try
            {
                baseSQL.sqlCon = baseSQL.fncAbrirBD(Credential.ConnString("SQL"));

                if (baseSQL.sqlCon.State == ConnectionState.Closed)
                    throw new Exception("No se pudo conectar a la base de datos");

                _listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 15,
                    ParameterName = "usr_cre_lgn",
                    Value = strCurrentUser
                });

                _listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "doc_cat_cod",
                    Value = collection.doc_cat_cod
                });

                _listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 60,
                    ParameterName = "doc_cat_nom",
                    Value = collection.doc_cat_nom == null ? "" : collection.doc_cat_nom.ToUpper()
                });

                _listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 1000,
                    ParameterName = "doc_cat_des",
                    Value = collection.doc_cat_des == null ? "" : collection.doc_cat_des.ToUpper()
                });

                _listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.Bit,
                    ParameterName = "doc_cat_est",
                    Value = collection.doc_cat_est
                });

                _listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 15,
                    ParameterName = "doc_cat_cre_usr",
                    Value = collection.doc_cat_cre_usr
                });

                _listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.DateTime,
                    ParameterName = "doc_cat_cre_fec",
                    Value = collection.doc_cat_cre_fec == null ? DateTime.Now : collection.doc_cat_cre_fec
                });

                _listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 15,
                    ParameterName = "doc_cat_mod_usr",
                    Value = collection.doc_cat_mod_usr
                });

                _listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.DateTime,
                    ParameterName = "doc_cat_mod_fec",
                    Value = collection.doc_cat_mod_fec == null ? DateTime.Now : collection.doc_cat_mod_fec
                });

                dataSetSQL = baseSQL.fncRetornaRegistros("[dbo].[sp_app_categoria_agregar_editar]", "Table", _listParametros);

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