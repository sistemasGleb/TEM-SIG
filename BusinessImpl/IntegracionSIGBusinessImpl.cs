using BusinessEntity;
using DataAccessImpl;
using System;

namespace BusinessImpl
{
    public class IntegracionSIGBusinessImpl
    {
        private readonly IntegracionSIGDataAccessImpl _planDeMantencionDataAccessImpl = new IntegracionSIGDataAccessImpl();

        public DataSetSQL ListarLogIntegracionWS(string strCurrentUser,int iEstado, DateTime dFechaInicio, DateTime dFechaFin, string sUsuario, int iPlanMAntencion)
        {
            return _planDeMantencionDataAccessImpl.ListarLogIntegracionWS(strCurrentUser,iEstado, dFechaInicio, dFechaFin, sUsuario, iPlanMAntencion);
        }

        #region Envio WS
        public BusinessEntity.DataSetSQL guardarMantencionIntegracion(int idMantencion, BusinessEntity.FormModels.FormPlanDeMantencion.Parametros collection)
        {
            return _planDeMantencionDataAccessImpl.CuardarNuevaMantencionIntegracion(idMantencion, collection);
        }

        public BusinessEntity.DataSetSQL LogIntegracionWS(int idMantencion,
                                            string sEndpointUrl,
                                            BusinessEntity.FormModels.FormPlanDeMantencion.Parametros iCollection,
                                            bool bResult,
                                            string sMessage)
        {
            return _planDeMantencionDataAccessImpl.LogIntegracionWS(idMantencion, sEndpointUrl, iCollection, bResult, sMessage);
        }
        #endregion
    }
}