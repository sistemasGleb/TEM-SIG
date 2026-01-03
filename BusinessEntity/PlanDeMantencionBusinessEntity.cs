using System;

namespace BusinessEntity
{
    public class PlanDeMantencionBusinessEntity
    {
        // Nullable<int> example;
        public class PlanIngreso
        {
            public Int32 tra_id { get; set; }
            public Int16 tra_tip_mant_id { get; set; }
            public string tra_tip_mant_nom { get; set; }
            public Int32 tra_act_id { get; set; }
            public string tra_act_nom { get; set; }
            public Int32 tra_tip_ele_id { get; set; }
            public string tra_tip_ele_nom { get; set; }
            public Int32 tra_ele_id { get; set; }
            public string tra_ele_nom { get; set; }
            public Nullable<DateTime> tra_fec_ing { get; set; }
            public Int32 tra_res_id { get; set; }
            public decimal tra_can { get; set; }
            public bool tra_vig { get; set; }
            public string tra_obs { get; set; }

            //public Int32 pln_man_cod { get; set; }
            public string tra_uni_nom { get; set; }
            //public short pln_man_ano { get; set; }
            //public short pln_man_mes { get; set; }
            //public Int32 pln_man_val { get; set; }
            public string tra_res_nom { get; set; }
            //public bool pln_man_est { get; set; }
            //public string tra_ele_nom { get; set; }
            //public Nullable<DateTime> pln_man_cre_fec { get; set; }
            public string tra_tip_act_nom { get; set; }
            //public Nullable<DateTime> pln_man_mod_fec { get; set; }
        }

    }
}
