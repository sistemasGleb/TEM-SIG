using System;
using System.Collections.Generic;

namespace BusinessEntity
{
    public class CargoBusinessEntity
    {
        public class Cargo
        {
            public int car_cod { get; set; }
            public string car_key { get; set; }
            public string car_nom { get; set; }
            public string car_des { get; set; }

            public bool car_vig { get; set; }
            public string car_cre_usr { get; set; }
            public DateTime car_cre_fec { get; set; }
            public string car_mod_usr { get; set; }
            public DateTime? car_mod_fec { get; set; }
            public bool car_mod { get; set; }
            public bool car_del { get; set; }

            public List<MenuBusinessEntity.Menu> listMenu { get; set; }
            public string menus { get; set; }
        }


    }
}