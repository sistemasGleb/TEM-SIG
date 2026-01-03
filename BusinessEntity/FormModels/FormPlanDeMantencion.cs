using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;

namespace BusinessEntity.FormModels
{
    // BusinessEntity.FormModels.FormPlanDeMantencion.
    public class FormPlanDeMantencion
    {
        public class PlanReporte
        {
            public List<SelectListItem> allMaintenanceType { get; set; }
            public string MaintenanceTypeSelected { get; set; }
            public Nullable<DateTime> startDate { get; set; }
            public Nullable<DateTime> endDate { get; set; }

            public List<PlanDeMantencionBusinessEntity.PlanIngreso> AllPlanIngresoList { get; set; }

            public Nullable<bool> status { get; set; }
            public string message { get; set; }

        }
        public class PlanDeMantencion
        {
            public DataTable allPlanDeMantencionList { get; set; }
            public List<SelectListItem> AllYearList { get; set; }
            public List<SelectListItem> AllPlanningList { get; set; }
            public String PlanningSelected { get; set; }
            public String observations { get; set; }
        }
        public class PlanFiltros
        {
            public List<SelectListItem> AllYearList { get; set; }
            public List<SelectListItem> allUnitOfMeasure { get; set; }
            public List<SelectListItem> allMaintenanceType { get; set; }
            public List<SelectListItem> allActivity { get; set; }
            public List<SelectListItem> allElementType { get; set; }
            public List<SelectListItem> allElementCode { get; set; }
            public List<SelectListItem> allResponsible { get; set; }


            public Nullable<DateTime> eventDate { get; set; }
            public String manager { get; set; }
            public Nullable<Decimal> quantity { get; set; }
            public String observations { get; set; }
            public String uniqueIdentifier { get; set; }

            public String YearSelected { get; set; }
            public String MaintenanceTypeSelected { get; set; }
            public String ActivitySelected { get; set; }
            public String ElementTypeSelected { get; set; }
            public String ElementCodeSelected { get; set; }
            public String ResponsibleSelected { get; set; }
            public Nullable<DateTime> startDate { get; set; }
            public Nullable<DateTime> endDate { get; set; }
            public String MaintenanceDate { get; set; }
            public String unit { get; set; }
            public List<SelectListItem> AllowedFileExtensions { get; set; }
            public Nullable<Int64> maxFileSize { get; set; }
            public String maxFileSizeIso { get; set; }
            public String version { get; set; }
    }

        public class Parametros
        {
            public String currentUserSelected { get; set; }
            public Nullable<Int32> YearMaintenancePlanning { get; set; }
            public Nullable<Int32> IdMaintenancePlanning { get; set; }
            public Nullable<Int16> IdMaintenanceType { get; set; }
            public Nullable<Int32> IdActivity { get; set; }
            public Nullable<Int32> ElementType { get; set; }
            public Nullable<Int32> ElementCode { get; set; }
            public Nullable<DateTime> MaintenanceDate { get; set; }
            public Nullable<Int32> IdResponsible { get; set; }
            public Nullable<Decimal> quantity { get; set; }
            public String observations { get; set; }
            public Nullable<Int16> unit { get; set; }
            public Nullable<Int16> version { get; set; }
            public Nullable<Int32> year { get; set; }
            public DataTable CategoryTable { get; set; }
            public DataTable PlanningTable { get; set; }
            public String fileName { get; set; }
            public Guid guidIdentifier { get; set; }
            public String IsoMaintenanceDate { get; set; }
        }
    }



}