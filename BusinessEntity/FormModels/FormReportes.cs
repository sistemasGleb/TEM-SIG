using System.Collections.Generic;
using System.Web.Mvc;


namespace BusinessEntity.FormModels
{
    public class FormReportes
    {
        public string reportes { get; set; }
        //public List<SelectListItem> reportList { get; set; }

        //public List<Report> allReportList { get; set; }
        //public List<SelectListItem> reportList { get; set; }

        public string   filtro_reportes { get; set; }
        public List<SelectListItem> allReportList { get; set; }

        public string filtro_tramos { get; set; }
        public List<SelectListItem> AllTramoList { get; set; }
        public List<SelectListItem> AllTipoElementoList { get; set; }
        public List<SelectListItem> AllElementoList { get; set; }

        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class Root
        {
            public string switch_case { get; set; }
            public List<string> tramos { get; set; }
            public List<string> tipo_elementos { get; set; }
            public List<string> elementos { get; set; }
        }



        //public class Root
        //{
        //    public List<string> reportes { get; set; }
        //}
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

}
