using System;
using System.Data;
using System.Linq;

namespace CrosscuttingUtiles
{
    public class Mapa
    {
        #region MAPAS
        public static Func<object, DataRow> tableMenuCreateRow = (object data) =>
        {
            var tbl = new System.Data.DataTable(); // 1

            //Add column to table:
            tbl.Columns.Add(new DataColumn { ColumnName = "user_lgn", DataType = typeof(string), AllowDBNull = true });
            tbl.Columns.Add(new DataColumn { ColumnName = "tramo_id", DataType = typeof(short), AllowDBNull = true });
            tbl.Columns.Add(new DataColumn { ColumnName = "menu_id", DataType = typeof(short), AllowDBNull = true });
            tbl.Columns.Add(new DataColumn { ColumnName = "selected", DataType = typeof(bool), AllowDBNull = true });
            tbl.Columns.Add(new DataColumn { ColumnName = "title", DataType = typeof(string), AllowDBNull = true });

            var row = tbl.NewRow();
            row.ItemArray = data.GetType().GetProperties().Select(a => a.GetValue(data)).ToArray();
            return row;
        };

        public static Func<DataTable> tableMenuCreate = () =>
        {
            var tbl = new System.Data.DataTable(); // 1

            //Add column to table:
            tbl.Columns.Add(new DataColumn { ColumnName = "user_lgn", DataType = typeof(string), AllowDBNull = true });
            tbl.Columns.Add(new DataColumn { ColumnName = "tramo_id", DataType = typeof(short), AllowDBNull = true });
            tbl.Columns.Add(new DataColumn { ColumnName = "menu_id", DataType = typeof(short), AllowDBNull = true });
            tbl.Columns.Add(new DataColumn { ColumnName = "selected", DataType = typeof(bool), AllowDBNull = true });
            tbl.Columns.Add(new DataColumn { ColumnName = "title", DataType = typeof(string), AllowDBNull = true });
 
            return tbl;
        };

        public static Func<object, DataRow> tableConfigCreateRow = (object data) =>
        {
            var tbl = new System.Data.DataTable(); // 1

            //Add column to table:
            tbl.Columns.Add(new DataColumn
            { ColumnName = "cnf_cod", DataType = typeof(int), AllowDBNull = true }); // [int] NOT NULL,
            tbl.Columns.Add(new DataColumn
            { ColumnName = "cnf_tip_cod", DataType = typeof(short), AllowDBNull = true }); // [smallint] NOT NULL,
            tbl.Columns.Add(new DataColumn
            { ColumnName = "cnf_key", DataType = typeof(string), AllowDBNull = true }); // [varchar](5) NOT NULL,
            tbl.Columns.Add(new DataColumn
            { ColumnName = "cnf_nom", DataType = typeof(string), AllowDBNull = true }); // [varchar](50) NULL,
            tbl.Columns.Add(new DataColumn
            { ColumnName = "cnf_des", DataType = typeof(string), AllowDBNull = true }); // [varchar](200) NULL,
            tbl.Columns.Add(new DataColumn
            { ColumnName = "cnf_num_001", DataType = typeof(short), AllowDBNull = true }); // [smallint] NULL,
            tbl.Columns.Add(new DataColumn
            { ColumnName = "cnf_str_001", DataType = typeof(string), AllowDBNull = true }); // [varchar](10) NOT NULL,
            tbl.Columns.Add(new DataColumn
            { ColumnName = "cnf_cre_usr", DataType = typeof(string), AllowDBNull = true }); // [varchar](10) NOT NULL,
            tbl.Columns.Add(
                new DataColumn
                {
                    ColumnName = "cnf_cre_fec",
                    DataType = typeof(DateTime),
                    AllowDBNull = true
                }); // [datetime] NOT NULL,
            tbl.Columns.Add(new DataColumn
            { ColumnName = "cnf_mod_usr", DataType = typeof(string), AllowDBNull = true }); // [varchar](10) NULL,
            tbl.Columns.Add(
                new DataColumn
                { ColumnName = "cnf_mod_fec", DataType = typeof(DateTime), AllowDBNull = true }); // [datetime] NULL

            var row = tbl.NewRow();
            row.ItemArray = data.GetType().GetProperties().Select(a => a.GetValue(data)).ToArray();
            return row;
        };

        public static Func< DataTable> tableConfigCreate = () =>
        {
            var tbl = new System.Data.DataTable(); // 1

            //Add column to table:
            tbl.Columns.Add(new DataColumn
            { ColumnName = "cnf_cod", DataType = typeof(int), AllowDBNull = true }); // [int] NOT NULL,
            tbl.Columns.Add(new DataColumn
            { ColumnName = "cnf_tip_cod", DataType = typeof(short), AllowDBNull = true }); // [smallint] NOT NULL,
            tbl.Columns.Add(new DataColumn
            { ColumnName = "cnf_key", DataType = typeof(string), AllowDBNull = true }); // [varchar](5) NOT NULL,
            tbl.Columns.Add(new DataColumn
            { ColumnName = "cnf_nom", DataType = typeof(string), AllowDBNull = true }); // [varchar](50) NULL,
            tbl.Columns.Add(new DataColumn
            { ColumnName = "cnf_des", DataType = typeof(string), AllowDBNull = true }); // [varchar](200) NULL,
            tbl.Columns.Add(new DataColumn
            { ColumnName = "cnf_num_001", DataType = typeof(short), AllowDBNull = true }); // [smallint] NULL,
            tbl.Columns.Add(new DataColumn
            { ColumnName = "cnf_str_001", DataType = typeof(string), AllowDBNull = true }); // [varchar](10) NOT NULL,
            tbl.Columns.Add(new DataColumn
            { ColumnName = "cnf_cre_usr", DataType = typeof(string), AllowDBNull = true }); // [varchar](10) NOT NULL,
            tbl.Columns.Add(
                new DataColumn
                {
                    ColumnName = "cnf_cre_fec",
                    DataType = typeof(DateTime),
                    AllowDBNull = true
                }); // [datetime] NOT NULL,
            tbl.Columns.Add(new DataColumn
            { ColumnName = "cnf_mod_usr", DataType = typeof(string), AllowDBNull = true }); // [varchar](10) NULL,
            tbl.Columns.Add(
                new DataColumn
                { ColumnName = "cnf_mod_fec", DataType = typeof(DateTime), AllowDBNull = true }); // [datetime] NULL

            return tbl;
        };
        #endregion
    }
}
