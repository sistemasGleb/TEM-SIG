
using System;

namespace BusinessEntity
{
    public class IntegracionSIGBusinessEntity
    {
        // Nullable<int> example;
        public class IntegracionWS
        {
            public Int64 integracion_log_id { get; set; }
            public Nullable<DateTime> integracion_log_fecha { get; set; }
            public string integracion_log_usuario { get; set; }
            public string integracion_log_endpoint { get; set; }
            public bool integracion_log_estado { get; set; }

            //public Int16 tra_tip_mant_id { get; set; }
            //public string tra_tip_mant_nom { get; set; }
            //public Int32 tra_act_id { get; set; }
            //public string tra_act_nom { get; set; }
            //public Int32 tra_tip_ele_id { get; set; }
            //public string tra_tip_ele_nom { get; set; }
            //public Int32 tra_ele_id { get; set; }
            //public string tra_ele_nom { get; set; }
            //public Nullable<DateTime> tra_fec_ing { get; set; } 
        }
    }
}
