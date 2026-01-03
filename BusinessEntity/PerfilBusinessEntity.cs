using System;
using System.Collections.Generic;

namespace BusinessEntity
{
    public class PerfilBusinessEntity
    {
        public class Perfil
        {
            public int per_cod { get; set; }
            public string per_key { get; set; }
            public string per_nom { get; set; }
            public string per_des { get; set; }

            public bool per_vig { get; set; }
            public string per_cre_usr { get; set; }
            public DateTime per_cre_fec { get; set; }
            public string per_mod_usr { get; set; }
            public DateTime? per_mod_fec { get; set; }
            public bool per_mod { get; set; }
            public bool per_del { get; set; }

            public List<MenuBusinessEntity.Menu> listMenu { get; set; }
            public string menus { get; set; }
        }


    }
}