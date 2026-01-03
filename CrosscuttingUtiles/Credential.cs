using System;
using System.Configuration;

namespace CrosscuttingUtiles
{
    public class Credential
    {
        public static string ConnString(string Connection)
        {
            string conn;

            int _ambiente = Convert.ToInt32(ConfigurationManager.AppSettings["Ambiente"].ToString());

            if (_ambiente < 4)
            {
                conn = ConfigurationManager.AppSettings.Get(Connection + "_Desarrollo_Conex");  //"DATA SOURCE=DESA11;PERSIST SECURITY INFO=True;USER ID=pacabre; password=p4c4br3; Pooling= False; ";
            }
            else
            {
                conn = ConfigurationManager.AppSettings.Get(Connection + "_Produccion_Conex");  //"DATA SOURCE=DESA11;PERSIST SECURITY INFO=True;USER ID=pacabre; password=p4c4br3; Pooling= False; ";
            }

            return conn;
        }
    }
}
