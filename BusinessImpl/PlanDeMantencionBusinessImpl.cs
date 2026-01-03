using DataAccessImpl;
using System;

namespace BusinessImpl
{
    public class PlanDeMantencionBusinessImpl
    {
        private readonly PlanDeMantencionDataAccessImpl planDeMantencionDataAccessImpl = new PlanDeMantencionDataAccessImpl();

        public BusinessEntity.DataSetSQL ListAllPlanning(string strCurrentUser, string strGuid)
        {
            return planDeMantencionDataAccessImpl.ListAllPlanning(strCurrentUser, strGuid);
        }

        public BusinessEntity.DataSetSQL ListFilterByGuid(string strCurrentUser,string strGuid)
        {
            return planDeMantencionDataAccessImpl.ListFilterByGuid(strCurrentUser, strGuid);
        }

        public BusinessEntity.DataSetSQL ListIngresoFiltered(string strCurrentUser, Int32 maintenanceTypeSelected, Nullable<DateTime> startDate, Nullable<DateTime> endDate)
        {
            return planDeMantencionDataAccessImpl.ListIngresoFiltered(strCurrentUser, maintenanceTypeSelected, startDate, endDate);
        }

        public BusinessEntity.DataSetSQL ListElementsByType(string strCurrentUser, Int32 elementTypeSelected)
        {
            return planDeMantencionDataAccessImpl.ListElementsByType(strCurrentUser, elementTypeSelected);
        }

        public BusinessEntity.DataSetSQL guardarMantencion(BusinessEntity.FormModels.FormPlanDeMantencion.Parametros collection)
        {
            return planDeMantencionDataAccessImpl.CuardarNuevaMantencion(collection);
        }

        public BusinessEntity.DataSetSQL ListFilterActivityByPlan(BusinessEntity.FormModels.FormPlanDeMantencion.Parametros collection)
        {
            return planDeMantencionDataAccessImpl.ListFilterActivityByPlan(collection);
        }

        public BusinessEntity.DataSetSQL SubirArchivoPlanMantencion(BusinessEntity.FormModels.FormPlanDeMantencion.Parametros collection)
        {
            return planDeMantencionDataAccessImpl.SubirArchivoPlanMantencion(collection);
        }
    }
}
