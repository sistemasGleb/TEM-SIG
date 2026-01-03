
using System;
using System.Collections.Generic;

namespace BusinessEntity
{
    public class MenuBusinessEntity
    {
        public class Menu
        {
            public string menu_cod { get; set; }
            public string menu_nom { get; set; }
            public bool menu_sel { get; set; }
            public short menu_row { get; set; }
        }


        public class MenuOpciones
        {
            public int mum_id { get; set; }
            public short mum_root_id { get; set; }
            public short mum_level_id { get; set; }
            public short mum_order_id { get; set; }
            public short mum_emp_id { get; set; }
            public string mum_area { get; set; }
            public string mum_controller { get; set; }
            public string mum_method { get; set; }
            public string mum_view { get; set; }
            public string mum_icon { get; set; }
            public string mum_caption { get; set; }
            public bool mum_vig { get; set; }
            public List<UrlMenu> child { get; set; }

        }


    public class MenuUsuario
        {
            public List<MenuOpciones> menu_opciones { get; set; }
        }

            public class UrlMenu
        {
            public int mum_id { get; set; }
            public short mum_root_id { get; set; }
            public short mum_level_id { get; set; }
            public short mum_order_id { get; set; }
            public short mum_emp_id { get; set; }
            public string mum_area { get; set; }
            public string mum_controller { get; set; }
            public string mum_method { get; set; }
            public string mum_view { get; set; }
            public string mum_icon { get; set; }
            public string mum_caption { get; set; }
            public bool mum_vig { get; set; }
            public DateTime mum_fec_cre { get; set; }
            public  string mum_usr_cre { get; set; }
            public DateTime mum_fec_mod { get; set; }
            public string mum_usr_mod { get; set; }
        }

    }
}
