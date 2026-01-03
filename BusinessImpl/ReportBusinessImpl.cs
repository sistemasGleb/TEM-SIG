using BusinessEntity;
using DataAccessImpl;
using System;
using System.Data;

namespace BusinessImpl
{
    public class ReportBusinessImpl
    {
        private readonly ReportDataAccessImpl reportDataAccessImpl = new ReportDataAccessImpl();

        public DataSetSQL get_Report(string strUsuario, Int16 IdTipo, Int16 IdElemento)
        {
            return reportDataAccessImpl.get_Report(strUsuario, IdTipo, IdElemento);
        }

        public DataSetSQL get_Report_Filter(string strUsuario, DataTable TblTramos, DataTable TblTipoElementos, DataTable TblElementos)
        {
            return reportDataAccessImpl.get_Report_Filter( strUsuario,  TblTramos,  TblTipoElementos,  TblElementos);
        }
    }

}
