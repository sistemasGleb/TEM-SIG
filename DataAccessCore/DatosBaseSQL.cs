using BusinessEntity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;


namespace DataAccessCore
{
    public class DatosBaseSQL
    {
        public SqlConnection sqlCon;
        public SqlTransaction sqlTran;
        public int intError = 0;
        public string strError = string.Empty;
        public SqlConnection fncAbrirBD(string strBD)
        {
            SqlConnection sqlCon = new SqlConnection();

            try
            {
                sqlCon.ConnectionString = strBD;
                sqlCon.Open();
            }
            catch (Exception e)
            {
                this.strError = e.Message;
                if (sqlCon != null)
                {
                    sqlCon.Dispose();
                }
            }

            return sqlCon;
        }
        public DataSetSQL fncExecutaSQL(string strSQL, List<SqlParameter> arrSQLParam, int intTimeOut = 0)
        {
            DataSetSQL DataSetSQL = new DataSetSQL();
            SqlCommand sqlCommand = null;

            try
            {
                if (this.sqlTran == null)
                {
                    sqlCommand = new SqlCommand(strSQL, this.sqlCon);
                }
                else
                {
                    sqlCommand = new SqlCommand(strSQL, this.sqlCon, this.sqlTran);
                }

                if (intTimeOut != 0)
                {
                    sqlCommand.CommandTimeout = intTimeOut;
                }

                foreach (SqlParameter sqlParameter in arrSQLParam)
                {
                    sqlCommand.Parameters.Add(sqlParameter);
                }

                sqlCommand.CommandType = CommandType.StoredProcedure;
                DataSetSQL.intQtyReg = sqlCommand.ExecuteNonQuery();
                DataSetSQL.intError = 0;
                DataSetSQL.strError = "Sin Error";

            }
            catch (Exception e)
            {
                DataSetSQL.intError = 1;
                DataSetSQL.strError = e.Message;
            }
            finally
            {
                if (sqlCommand != null)
                {
                    sqlCommand.Dispose();
                }
            }

            return DataSetSQL;
        }
        public DataSetSQL fncRetornaRegistros(string strNombreSP, string strDataSet, List<SqlParameter> arrSQLParam, int intTimeOut = 0, bool blnEsSP = true)
        {
            DataSetSQL DataSetSQL = new DataSetSQL();
            SqlDataAdapter sqlDataAdapter = null;
            SqlCommand sqlCommand = null;
            DataSet dsRegistros = new DataSet();

            try
            {
                if (this.sqlTran == null)
                {
                    sqlCommand = new SqlCommand(strNombreSP, this.sqlCon);
                }
                else
                {
                    sqlCommand = new SqlCommand(strNombreSP, this.sqlCon, this.sqlTran);
                }

                if (blnEsSP)
                    sqlCommand.CommandType = CommandType.StoredProcedure;

                if (intTimeOut != 0)
                {
                    sqlCommand.CommandTimeout = intTimeOut;
                }

                foreach (SqlParameter sqlParameter in arrSQLParam)
                {
                    sqlCommand.Parameters.Add(sqlParameter);
                }

                sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                sqlDataAdapter.Fill(dsRegistros, strDataSet);

                DataSetSQL.intError = 0;
                DataSetSQL.strError = "";
                DataSetSQL.dsSQL = dsRegistros;

            }
            catch (Exception e)
            {
                DataSetSQL.intError = 1;
                DataSetSQL.strError = e.Message;
                DataSetSQL.dsSQL = null;
            }
            finally
            {
                if (sqlCommand != null)
                {
                    sqlCommand.Dispose();
                }
            }

            return DataSetSQL;
        }
    }
}
