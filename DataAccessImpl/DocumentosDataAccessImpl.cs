using BusinessEntity;
using CrosscuttingUtiles;
using DataAccessCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DataAccessImpl
{
    public class DocumentosDataAccessImpl
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


                dataSetSQL = baseSQL.fncRetornaRegistros("[dbo].[sp_App_documentos_listar]", "Table", _listParametros);

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
        public DataSetSQL DeleteByGuid(string strGUID, string strCurrentUser)
        {
            DatosBaseSQL baseSQL = new DatosBaseSQL();
            DataSetSQL dataSetSQL = new DataSetSQL();

            //SqlParameter sqlParameter;
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
                    Size = 15,
                    ParameterName = "USUARIO",
                    Value = strCurrentUser
                });
 
                _listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 50,
                    ParameterName = "doc_guid",
                    Value = strGUID
                });

                dataSetSQL = baseSQL.fncRetornaRegistros("[dbo].[sp_app_documentos_eliminar]", "Table", _listParametros);

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

        public DataSetSQL SaveDocument(DocumentoBusinessEntity collection)
        {
            //bool blnActualizado;
            var baseSQL = new DatosBaseSQL();
            DataSetSQL dataSetSQL = new DataSetSQL();
            //SqlParameter sqlParameter;
            var listParametros = new List<SqlParameter>();

            try
            {
                var strConexion = Credential.ConnString("SQL");

                baseSQL.sqlCon = baseSQL.fncAbrirBD(strConexion);

                if (baseSQL.sqlCon.State == ConnectionState.Closed)
                    throw new Exception("No se pudo conectar a la base de datos");

                //listParametros = new List<SqlParameter>();

                listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 50,
                    ParameterName = "doc_guid",
                    Value = collection.doc_guid.Trim().ToUpper()
                });

                listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "doc_cat",
                    Value = collection.doc_cat_cod
                });

                listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 100,
                    ParameterName = "doc_nombre",
                    Value = collection.doc_nombre.Trim().ToUpper()
                });

                listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 50,
                    ParameterName = "doc_titulo",
                    Value = collection.doc_titulo.Trim().ToUpper()
                });

                listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 200,
                    ParameterName = "doc_descripcion",
                    Value = collection.doc_descripcion.Trim().ToUpper()
                });

                listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 200,
                    ParameterName = "doc_directorio",
                    Value = collection.doc_directorio.Trim().ToUpper()
                });

                listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 5,
                    ParameterName = "doc_extension",
                    Value = collection.doc_extension.Trim().ToUpper()
                });

                listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "doc_size",
                    Value = collection.doc_size
                });

                listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 15,
                    ParameterName = "doc_usr_cre_usr",
                    Value = collection.doc_usr_cre_usr.Trim().ToUpper()
                });

                listParametros.Add(new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    Size = 15,
                    ParameterName = "doc_usr_mod_usr",
                    Value = collection.doc_usr_mod_usr.Trim().ToUpper()
                });

                dataSetSQL = baseSQL.fncRetornaRegistros("[dbo].[sp_app_documentos_agregar_editar]", "Table", listParametros);

                if (dataSetSQL.intError != 0)
                    throw new Exception(dataSetSQL.strError);

                //if ((int)dataSetSQL.dsSQL.Tables["DOCUMENTOS"].Rows[0]["ID_ERROR"] != 0)
                //    throw new Exception((string)dataSetSQL.dsSQL.Tables["DOCUMENTOS"].Rows[0]["DESCRIPCION_ERROR"]);

                //blnActualizado = true;
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
    }
}