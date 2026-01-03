using System;
using System.Configuration;
using System.Globalization;

namespace CrosscuttingUtiles
{
    public class Password
    {
        public static Func<string, string> fnc_get_linkPasswordRecovery = (cadena) =>
        {
            CultureInfo cultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;

            string baseUrl = ConfigurationManager.AppSettings["BaseUrl"].ToString();

            try
            {
                var toclick = $"{baseUrl}Account/ResetPassword?token={cadena}";
                return toclick;
            }
            catch
            {
                return "#";
            }
        };
    }
}
