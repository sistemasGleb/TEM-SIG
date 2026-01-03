

using System.Collections.Generic;
using System;

namespace BusinessEntity
{
    public class ElementoBusinessEntity
    {
        public class Elemento
        {
            public short ele_id { get; set; }
            public byte ele_tip_id { get; set; }
            public byte ele_sub_tip_id { get; set; }
            public string ele_nom { get; set; }
            public string ele_des { get; set; }
            public bool ele_vig { get; set; }
           public bool ele_opc_vis { get; set; }
            public string ele_cre_usr { get; set; }
            public DateTime ele_cre_fec { get; set; }
            public string ele_mod_usr { get; set; }
            public DateTime ele_mod_fec { get; set; }
            public string ele_hab { get; set; }
            public bool ele_chk { get; set; }
            public short ele_num_com { get; set; }
            public string ele_obs { get; set; }
            public string ele_his { get; set; } 
            public List<Imagen> ele_img_list { get; set; }
            public string ele_cre_usr_nom { get; set; }
        }

        public class Imagen
        {
            public Int32 img_id { get; set; }
            public Int16 img_tip_ele { get; set; }
            public Int32 img_ele_id { get; set; }
            public string img_nom { get; set; }
            public string img_des { get; set; }
            public string img_pat { get; set; }
        }
        public class Poligono {
            public Int16 corr { get; set; }
            public string descripcion { get; set; }
            public string lat { get; set; }
            public string lon { get; set; }
        }
        public class ElementoHistoria
        {
            public Int32 his_id { get; set; }
            public Int16 his_tip_ele { get; set; }
            public Int32 his_ele_id { get; set; }
            public string his_txt { get; set; }
            public bool his_vig { get; set; }
            public string his_cre_usr { get; set; }
            public DateTime his_cre_fec { get; set; }
            public string his_cre_fec_iso { get; set; }
            public string his_cre_usr_nom { get; set; }
        }
    }
}
