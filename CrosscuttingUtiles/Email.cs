using System.Configuration;
using System.Net.Mail;
using System.Text;

namespace CrosscuttingUtiles
{
    public class Email
    {

        /// <summary>
        /// recupera plantilla segun tipo de EMAIL
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public static StringBuilder GetMailTemplate(string template)
        {
            StringBuilder sb = new StringBuilder();

            try
            {

                sb.Append("<table width='100%' cellpadding='0' cellspacing='0' border='0' class='message'>");
                sb.Append("<tbody>");
                sb.Append("<tr>");
                sb.Append("<td colspan='2'><table width='100%' cellpadding='12' cellspacing='0' border='0'><tbody><tr><td><div style='overflow: hidden;'><font size='-1'>");
                sb.Append("<div>");
                sb.Append("Estimado #NOMBRE#, <br>");
                sb.Append("<br>");
                sb.Append("Hemos recibido exitosamente tu solicitud para reestablecer tu contraseña. <br>");
                sb.Append("Para reestablecer tu contraseña haz click <a href='#TOKEN#' target='_blank'>");
                sb.Append("aquí</a> <br>");
                sb.Append("<br>");
                sb.Append("<br>");
                sb.Append("<a href='#MAIL_SOPORTE#' target='_blank'>#MAIL_SOPORTE#</a>");
                sb.Append("</div>");
                sb.Append("</div>");
                sb.Append("</font></div></td></tr></tbody></table>");
                sb.Append("</td>");
                sb.Append("</tr>");
                sb.Append("</tbody>");
                sb.Append("</table>");
 
                return sb;
            }
            catch
            {
                return sb;
            }
        }


        /// <summary>
        /// Método genérico de envio de email
        /// </summary>
        /// <param name="pAsunto"></param>
        /// <param name="pCuerpo"></param>
        /// <param name="pMailTo"></param>
        public static void Enviar(string pAsunto, AlternateView pCuerpo, string pMailTo)
        {
            string _MailSender;
            string _MailFrom;
            string _MailSMTP;
            string _MailPuerto;
            string _MailClave;
            bool _EnableSsl;
            bool _UseDefaultCredentials;

            try
            {

                _MailSender = ConfigurationManager.AppSettings["MailSender"].ToString();
                _MailFrom = ConfigurationManager.AppSettings["MailCuenta"].ToString();
                _MailSMTP = ConfigurationManager.AppSettings["MailSMTP"].ToString();
                _MailPuerto = ConfigurationManager.AppSettings["MailPuerto"].ToString();
                _MailClave = ConfigurationManager.AppSettings["MailClave"].ToString();
                _EnableSsl = bool.Parse(ConfigurationManager.AppSettings["EnableSsl"].ToString());
                _UseDefaultCredentials =bool.Parse(ConfigurationManager.AppSettings["UseDefaultCredentials"].ToString());

                MailMessage correo = new MailMessage();
                correo.From = new MailAddress(_MailFrom, "Soporte Sig");
                correo.To.Add(new MailAddress(pMailTo));
                correo.Bcc.Add(new MailAddress("ivillanueva@iteradata.cl"));
                correo.Subject = pAsunto;
                correo.AlternateViews.Add(pCuerpo);
                correo.Priority = MailPriority.Normal;

                SmtpClient smtp = new SmtpClient();
                smtp.Host = _MailSMTP;
                smtp.UseDefaultCredentials = _UseDefaultCredentials;
                if (_UseDefaultCredentials == true)
                {
                    smtp.Credentials = new System.Net.NetworkCredential(_MailFrom, _MailClave);
                }

                if (_MailPuerto != "0")
                {
                    smtp.Port = int.Parse(_MailPuerto);
                }

                smtp.EnableSsl = _EnableSsl;

                smtp.Send(correo);
                correo.Dispose();

            }
            catch
            {
                throw;
            }
        }
    }
}
