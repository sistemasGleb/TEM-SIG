using System.ComponentModel.DataAnnotations;


namespace BusinessEntity
{
    public class LoginBusinessEntity
    {
       public string Usuario { get; set; }
       public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class InicioSesionBusinessEntity
    {
        public string bloque { get; set; }
        public string usuario { get; set; }
        public string password { get; set; }
        public long usuario_session { get; set; }
        public string pregunta { get; set; }
        public string respuesta { get; set; }
        public string new_password { get; set; }
        public int setpreg { get; set; }
        public int preg_id { get; set; }
        public string resp_desc { get; set; }
        public string aspx { get; set; }
        public string ip { get; set; }
        public int cultura { get; set; }
    }
}
