
using System;

namespace BusinessEntity
{
    public class ElementoImagenBusinessEntity
    {
        public Int32 ele_img_id { get; set; }
        public Int16 ele_img_tip_ele { get; set; }
        public Int32 ele_img_ele_id { get; set; }
        public string ele_img_nom { get; set; }
        public string ele_img_des { get; set; }
        public decimal ele_img_siz { get; set; }
        public string ele_img_ext { get; set; }
        public string ele_img_pat { get; set; }
        public bool ele_img_vig { get; set; }
        public string ele_img_cre_usr { get; set; }
        public string ele_img_cre_usr_nom { get; set; }
        public DateTime? ele_img_cre_fec { get; set; }
        public string ele_img_mod_usr { get; set; }
        public string ele_img_mod_usr_nom { get; set; }
        public DateTime? ele_img_mod_fec { get; set; }
        public bool ele_img_xts { get; set; }
        public string ele_img_siz_txt { get; set; }

    }
}
