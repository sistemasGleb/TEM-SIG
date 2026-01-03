
using System;
using System.Web.Mvc;
using System.Collections.Generic;

namespace BusinessEntity
{
    public class UsuarioBusinessEntity
    {
        public class FormUsuarios
        { public List<UsuarioViewModel> listUsuario { get; set; }
        }
        public class UsuarioCU
        {
            // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
            public class RolUnico
            {
                public string DV { get; set; }
                public int numero { get; set; }
                public string tipo { get; set; }
            }

            public class Name
            {
                public List<string> apellidos { get; set; }
                public List<string> nombres { get; set; }
            }

            public class Root
            {
                public string sub { get; set; }
                public RolUnico RolUnico { get; set; }
                public Name name { get; set; }
            }
        }

        public class UsuarioViewModel
        {
            public int usr_id { get; set; }
            public string usr_rut_com { get; set; }

            public int usr_emp { get; set; }
            public int usr_rut { get; set; }
            public string usr_rut_dv { get; set; }
            public string usr_psw { get; set; }
            public string usr_tel { get; set; }
            public string usr_lgn { get; set; }
            public string usr_full_nom { get; set; }
            public string usr_nom { get; set; }
            public string usr_ape_pat { get; set; }
            public string usr_ape_mat { get; set; }
            public string usr_nom_full { get; set; }
            public string usr_car { get; set; }
            public string usr_cre_usr { get; set; }
            public DateTime usr_cre_fec { get; set; }
            public string usr_mod_usr { get; set; }
            public DateTime usr_mod_fec { get; set; }
            public bool usr_vig { get; set; }
            public bool usr_blq { get; set; }
            public string usr_mail { get; set; }
            public string perfil { get; set; }
            public List<SelectListItem> listado_cargos { get; set; }
            public short usr_car_cod { get; set; }
            public string usr_new_psw { get; set; }
            public bool usr_lgn_est { get; set; }
            public string usr_lgn_msg { get; set; }
            //public List<Usuario_Rol> usuario_rol { get; set; }

        }

        //public class Usuario_Rol
        //{
        //    public int Id { get; set; }
        //    public int Id_Usuario { get; set; }
        //    public int Id_Rol { get; set; }
        //    public string Rol_Nombre { get; set; }

        //    public virtual Rol Rol { get; set; }
        //}
        //public class Rol
        //{
        //    public int Id { get; set; }
        //    public string Nombre { get; set; }
        //}
    }
}
