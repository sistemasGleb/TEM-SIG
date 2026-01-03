using BusinessEntity;
using BusinessImpl;
using CrosscuttingUtiles;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace WebApplication.Controllers
{
    public class AccountController : BaseController
    {
        private readonly LoginBusinessImpl loginBusinessImpl = new LoginBusinessImpl();
        private readonly LogBusinessImpl _logBusinessImpl = new LogBusinessImpl();
        private readonly UsuarioBusinessImpl _usuarioBusinessImpl = new UsuarioBusinessImpl();
        private readonly PasswordBusinessImpl _passwordBusinessImpl = new PasswordBusinessImpl();


        // GET: Account
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {

                IAuthenticationManager iAuthenticationManager = HttpContext.GetOwinContext().Authentication;
                iAuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

                returnUrl = Url.Action("Index", "Home");
            };

            ViewBag.ReturnUrl = returnUrl;
            ViewBag.version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            ViewData["mensaje"] = "";
            return View(new BusinessEntity.LoginBusinessEntity()); //login_tabbed
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginBusinessEntity data, string returnUrl)
        {
            ActionResult Result;

            try
            {
                var dataSetSQL = loginBusinessImpl.logOn(data);

                #region LOGIN
                if (dataSetSQL.intError != 0)
                    throw new Exception(dataSetSQL.strError);

                var usuarioBusinessEntity = dataSetSQL.dsSQL.Tables[0].AsEnumerable()
                    .Select(m => new UsuarioBusinessEntity.UsuarioViewModel
                    {
                        usr_id = (Int32)m["usr_id"],
                        usr_lgn = (string)m["usr_lgn"],
                        usr_rut_com = (string)m["usr_rut_com"],
                        usr_nom = (string)m["usr_nom"],
                        usr_ape_pat = (string)m["usr_ape_pat"],
                        usr_ape_mat = (string)m["usr_ape_mat"],
                        usr_mail = (string)m["usr_mail"],
                        usr_vig = (bool)m["usr_vig"],
                        usr_blq = (bool)m["usr_blq"],
                        usr_car_cod = (Int16)m["usr_car_cod"],
                        usr_cre_fec = (DateTime)m["usr_cre_fec"],
                        usr_lgn_est = (bool)m["usr_lgn_est"],
                        usr_lgn_msg = (string)m["usr_lgn_msg"],
                        usr_car = (string)m["usr_car_nom"],
                    }).FirstOrDefault();


                //if (results.estado == "USUARIO VALIDO")
                if (usuarioBusinessEntity.usr_lgn_est)
                {
                    // AUTENTICACIONDE ASP.NET IDENTITY
                    if (usuarioBusinessEntity.usr_id > 0)
                    {
                        Result = SignInUser(usuarioBusinessEntity, data.RememberMe, returnUrl);

                        // PASO 1) - LOG DE APLICACION
                        #region LOG
                        _logBusinessImpl._AddNewLog(new LogBusinessEntity()
                        {
                            tla_id = 0,
                            tla_fec_ing = DateTime.Now,
                            tla_usr_lgn = usuarioBusinessEntity.usr_lgn,
                            tla_app_id = 1,
                            tla_ipp = GetCustomerIP(),
                            tla_tip_pla = 0,
                            tla_ctr = ControllerContext.RouteData.Values["controller"].ToString(),
                            tla_ctr_act = ControllerContext.RouteData.Values["action"].ToString(),
                            tla_des = "Login success"
                        });
                        #endregion
                    }
                    else
                    {
                        Result = View(data);
                    }
                }
                else
                {
                    ViewData["mensaje"] = (usuarioBusinessEntity.usr_lgn_msg);
                    ViewData["tipo"] = "error";

                    Result = View(data);
                }
                #endregion
            }
            catch (Exception ex)
            {
                ViewData["mensaje"] = (ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                ViewData["tipo"] = "error";

                Result = View(data);
            }

            ViewBag.Name = "user.png";

            return Result;
        }
        private ActionResult SignInUser(UsuarioBusinessEntity.UsuarioViewModel user, bool rememberMe, string returnUrl)
        {
            ActionResult Result;

            List<Claim> Claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.usr_id.ToString()),
                new Claim(ClaimTypes.Email, user.usr_mail),
                new Claim(ClaimTypes.Name, user.usr_lgn),
                // Claim Personalizado
                new Claim("UserName", $"{user.usr_nom} {user.usr_ape_pat} {user.usr_ape_mat}"),
                new Claim("UserCargoName", $"{user.usr_car}")
            };

            var Identity = new ClaimsIdentity(Claims, DefaultAuthenticationTypes.ApplicationCookie);

            IAuthenticationManager iAuthenticationManager = HttpContext.GetOwinContext().Authentication;

            iAuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = rememberMe }, Identity);

            if (string.IsNullOrWhiteSpace((returnUrl)))
            {
                returnUrl = Url.Action("Index", "Home");

            }

            Result = Redirect(returnUrl);

            return Result;
        }
        public ActionResult LogOff()
        {
            #region LOG
            _logBusinessImpl._AddNewLog(new LogBusinessEntity()
            {
                tla_id = 0,
                tla_fec_ing = DateTime.Now,
                tla_usr_lgn = User.Identity.Name,
                tla_app_id = 1,
                tla_ipp = GetCustomerIP(),
                tla_tip_pla = 0,
                tla_ctr = ControllerContext.RouteData.Values["controller"].ToString(),
                tla_ctr_act = ControllerContext.RouteData.Values["action"].ToString(),
                tla_des = "Logout success"
            });
            #endregion

            IAuthenticationManager iAuthenticationManager = HttpContext.GetOwinContext().Authentication;

            iAuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            Session.Abandon();
            return RedirectToAction("Login", "Account");
        }


        #region PASSWORD RECOVERY
        [AllowAnonymous]
        [HttpGet]
        public ActionResult PasswordRecover()
        {
            UsuarioBusinessEntity model = new UsuarioBusinessEntity();
 
            #region LOG
            _logBusinessImpl._AddNewLog(new LogBusinessEntity()
            {
                tla_id = 0,
                tla_fec_ing = DateTime.Now,
                tla_usr_lgn = User.Identity.Name,
                tla_app_id = 1,
                tla_ipp = GetCustomerIP(),
                tla_tip_pla = 0,
                tla_ctr = ControllerContext.RouteData.Values["controller"].ToString(),
                tla_ctr_act = ControllerContext.RouteData.Values["action"].ToString(),
                tla_des = null
            });
            #endregion

            if (TempData["mensaje"] != null)
            {
                ViewData["mensaje"] = TempData["mensaje"];
                ViewData["tipo"] = TempData["tipo"];
            }
            else
            {
                TempData["mensaje"] = "";
                ViewData["mensaje"] = TempData["mensaje"];
                ViewData["tipo"] = "";
            }

            return View(new BusinessEntity.PasswordBusinessEntity());
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult JSON_PasswordRecover(PasswordBusinessEntity form)
        {
            System.Threading.Thread.Sleep(1000);
            try
            {
                // PASO 1) -  VALIDAMOS EL USUARIO EXISTE
                var dataSetSQL = _usuarioBusinessImpl.ListUser(String.Empty);

                if (dataSetSQL.intError != 0)
                    throw new Exception(dataSetSQL.strError);

                var user = dataSetSQL.dsSQL.Tables[0].AsEnumerable()
                      .Where(x => ((string)x["usr_mail"]).Equals(form.recovery_user_email))
                      .Select(m => new UsuarioBusinessEntity.UsuarioViewModel
                      {
                          usr_id = (int)m["usr_id"],
                          usr_lgn = (string)m["usr_lgn"],
                          usr_nom_full = $"{(string.IsNullOrEmpty((string)m["usr_nom"]) ? "" : (string)m["usr_nom"])} {(string.IsNullOrEmpty((string)m["usr_ape_pat"]) ? "" : (string)m["usr_ape_pat"])} {(string.IsNullOrEmpty((string)m["usr_ape_mat"]) ? "" : (string)m["usr_ape_mat"])}",
                          //usr_rut_com = $"{(string)m["usr_rut"]}-{(string)m["usr_rut_dv"]}",
                          //usr_nom = (string)m["usr_nom"],
                          //usr_ape_pat = (string)m["usr_ape_pat"],
                          //usr_ape_mat = (string)m["usr_ape_mat"],
                          //usr_tel = (string)m["usr_tel"],
                          //usr_car = m["per_nom"] == DBNull.Value ? "" : (string)m["per_nom"],
                          //usr_cre_usr = (string)m["usr_cre_usr"],
                          //usr_cre_fec = (DateTime)m["usr_cre_fec"],
                          //usr_mod_usr = (string)m["usr_mod_usr"] ?? string.Empty,
                          //usr_mod_fec = (DateTime)m["usr_mod_fec"],
                          //usr_mail = m["usr_mail"] == DBNull.Value ? string.Empty : (string)m["usr_mail"],
                          //usr_vig = (bool)m["usr_vig"],
                          //usr_blq = (bool)m["usr_blq"],
                          //usr_car_cod = (short)m["usr_per_cod"],
                          //listado_cargos = new List<SelectListItem> { new SelectListItem { Text = "Seleccione Perfil...", Value = "0", Selected = true } },
                          //usr_full_nom = $"{(string)m["usr_nom"]} {(string)m["usr_ape_pat"]} {(string)m["usr_ape_mat"]}"

                      }).DefaultIfEmpty(new UsuarioBusinessEntity.UsuarioViewModel()).FirstOrDefault();

                if (user.usr_id == 0)
                    throw new Exception("Tu correo electrónico no se encuentra en nuestros registros. ");

                // PASO 2) - Create and display the value of two GUIDs.
                var password = new PasswordBusinessEntity
                {
                    recovery_user_lgn = user.usr_lgn,
                    recovery_guid = Guid.NewGuid(),
                    recovery_user_email = form.recovery_user_email,
                };

                var toclick = CrosscuttingUtiles.Password.fnc_get_linkPasswordRecovery(password.recovery_guid.ToString());

                // PSAO 2.1 Genera registro de solicitude de Cambio
                var dataSetSQL2 = _passwordBusinessImpl.PasswordRecovery(password);

                // PASO 2) -  GENERAR PLANTILLA
                var htmlView = CrosscuttingUtiles.Email.GetMailTemplate("RECOVERY");
                htmlView.Replace("#NOMBRE#", user.usr_nom_full.Trim().ToUpper());
                htmlView.Replace("#TOKEN#", toclick);
                htmlView.Replace("#MAIL_SOPORTE#", ConfigurationManager.AppSettings["MailSoporte"].ToString());

                // PASO 2) -  ENVIAR EMAIL
                var htmlAlternateView = AlternateView.CreateAlternateViewFromString(htmlView.ToString(), null, "text/html");
                CrosscuttingUtiles.Email.Enviar("Recuperar Contraseña", htmlAlternateView, form.recovery_user_email);

                // PASO 3) - 
                return Json(new
                {
                    status = true,
                    title = "Correo electrónico de restablecimiento de contraseña enviado",
                    message = "Pronto recibirás un correo electrónico para restablecer tu contraseña. Si no lo encuentras, comprueba la carpeta de correo no deseado y la papelera."
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = false,
                    title = "Error al enviar la solicitud",
                    message = ex.InnerException == null ? ex.Message : ex.InnerException.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// metodo de recepcion del token de validacion de cambio de password
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ResetPassword(string token)
        {
            PasswordBusinessEntity filteredRequest = new PasswordBusinessEntity();
            try
            {
                if (string.IsNullOrEmpty(token))
                    throw new Exception("No hay token para esta solicitud");

                // PSAO 2.1 Genera registro de solicitude de Cambio
                var dataSetSQL = _passwordBusinessImpl.ListAll("");

                if (dataSetSQL.intError != 0)
                    throw new Exception(dataSetSQL.strError);

                var allPassowrdRecoveryToken = dataSetSQL.dsSQL.Tables[0].AsEnumerable()
                    .Where(m => (bool)m["tpr_sol_vig"] && (string)m["tpr_gui"] == token)
                      .Select(m => new PasswordBusinessEntity
                      {
                          recovery_user_lgn = (string)m["tpr_lgn"],
                          recovery_guid = Guid.Parse((string)m["tpr_gui"]),
                          recovery_diff = (Int32)m["tpr_sol_dif"],
                      }).ToList();

                // 1- NO EXISTE LA SOLICITUD
                if (!allPassowrdRecoveryToken.Any())
                    throw new Exception("No podemos encontrar la página que estás buscando.");

                filteredRequest = allPassowrdRecoveryToken.Where(x => x.recovery_diff <= 24).FirstOrDefault();

                // 2- SOLICITUD CADUCADA
                if (filteredRequest is null)
                    throw new Exception("Tu solicitud de crear o recuperar contraseña a expirado");

            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Exception", new { InnerexceptionMessage = ex.InnerException == null ? ex.Message : ex.InnerException.Message });
            }

            return View(filteredRequest);
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult JSON_ResetPassword(PasswordBusinessEntity collection)
        {
            PasswordBusinessEntity filteredRequest = new PasswordBusinessEntity();
            System.Threading.Thread.Sleep(1000);
            try
            {
                if (string.IsNullOrEmpty(collection.recovery_new_pswd) || string.IsNullOrEmpty(collection.recovery_confirm_pswd))
                    throw new Exception("Parametros inválidos.");

                // PASO 1) -  CIFRAMOS LA PASSWORD
                collection.recovery_new_pswd = Cifrado.Cifrar(collection.recovery_new_pswd);
                collection.recovery_confirm_pswd = Cifrado.Cifrar(collection.recovery_confirm_pswd);

                // PASO 2) -  ACTUALIZAMOS PASSWORD Y SOLICITUD
                var dataSetSQL = _passwordBusinessImpl.PasswordUpdate(collection);

                if (dataSetSQL.intError != 0)
                    throw new Exception(dataSetSQL.strError);

                // PASO 3) - 
                return Json(new
                {
                    status = true,
                    title = "¡Bien Hecho!",
                    message = "Tu nueva contraseña a sido creada exitosamente"
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = false,
                    title = "Error al enviar la solicitud",
                    message = ex.InnerException == null ? ex.Message : ex.InnerException.Message
                }, JsonRequestBehavior.AllowGet);
            }

        }
        #endregion


    }
}