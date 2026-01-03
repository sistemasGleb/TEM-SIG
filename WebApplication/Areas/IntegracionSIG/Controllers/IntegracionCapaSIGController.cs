using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace WebApplication.Areas.IntegracionSIG.Controllers
{
    public class IntegracionCapaSIGController : WebApplication.Controllers.BaseController
    {
        #region Campos Privados
        private readonly BusinessImpl.IntegracionSIGBusinessImpl _integracionSIGBusinessImpl = new BusinessImpl.IntegracionSIGBusinessImpl();
        private readonly BusinessImpl.LogBusinessImpl _logBusinessImpl = new BusinessImpl.LogBusinessImpl();
        #endregion

        #region Controladores
        public IntegracionCapaSIGController() { }
        #endregion

        public ActionResult Listar()
        {
            var integracionSigBusinessEntity = new BusinessEntity.FormModels.FormIntegracionSIG();

            try
            {
                #region LOG
                _logBusinessImpl._AddNewLog(new BusinessEntity.LogBusinessEntity()
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

                int iEstado = 99;
                DateTime dFechaInicio=DateTime.Now;
                DateTime dFechaFin = DateTime.Now;
                string sUsuario="";
                int iPlanMAntencion = 0;

                var dataSetSQL = _integracionSIGBusinessImpl.ListarLogIntegracionWS(User.Identity.Name, iEstado,  dFechaInicio,  dFechaFin,  sUsuario,iPlanMAntencion);

                var list = dataSetSQL.dsSQL.Tables[0].AsEnumerable()
                    .Select(m => new BusinessEntity.IntegracionSIGBusinessEntity.IntegracionWS
                    {
                        integracion_log_id = (Int64)m["tiw_id"],
                        integracion_log_fecha = m["tiw_fec_cre"] == DBNull.Value ? (DateTime?)null : m.Field<DateTime>("tiw_fec_cre"),
                        integracion_log_usuario = m["tiw_usr_lgn"] == DBNull.Value ? string.Empty : m.Field<string>("tiw_usr_lgn"),
                        integracion_log_endpoint =m["tiw_url_aws"] == DBNull.Value ? string.Empty : m.Field<string>("tiw_url_aws"),
                        integracion_log_estado = (bool)m["tiw_est"]

                        //usr_nom_full = $"{(string.IsNullOrEmpty((string)m["usr_nom"]) ? "" : (string)m["usr_nom"])} {(string.IsNullOrEmpty((string)m["usr_ape_pat"]) ? "" : (string)m["usr_ape_pat"])} {(string.IsNullOrEmpty((string)m["usr_ape_mat"]) ? "" : (string)m["usr_ape_mat"])}",
                        //usr_car = m["car_nom"] == DBNull.Value ? string.Empty : (string)m["car_nom"],
                        //usr_cre_usr = (string)m["usr_cre_usr"],
                        //usr_cre_fec = (DateTime)m["usr_cre_fec"],
                        //usr_mod_usr = (string)m["usr_mod_usr"] ?? string.Empty,
                        //usr_mod_fec = (DateTime)m["usr_mod_fec"],
                        //usr_mail = (string)m["usr_mail"] ?? string.Empty,
                        //usr_vig = (bool)m["usr_vig"],
                        //usr_blq = (bool)m["tiw_est"]
                    }).OrderByDescending(x => x.integracion_log_id).ToList();
                //}).OrderByDescending(x => x.log_id).ThenByDescending(x => x.usr_cre_fec).ToList();

                integracionSigBusinessEntity.listEnvioWS = list;

                if (TempData["mensaje"] != null)
                {
                    ViewData["mensaje"] = TempData["mensaje"];
                    ViewData["tipo"] = TempData["tipo"];
                    TempData["mensaje"] = null;
                }
            }
            catch (Exception ex)
            {
                ViewData["mensaje"] = (ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                ViewData["tipo"] = "error";

                integracionSigBusinessEntity.listEnvioWS = null;
            }

            return View("Listar", integracionSigBusinessEntity);
        }


        #region Metodos ENVIO WS
        internal void EnviarIntegracionWS(int idMantencion, BusinessEntity.FormModels.FormPlanDeMantencion.Parametros iCollection)
        {
            bool bResult = false;
            string sMessage;
            var sEndpointUrl = "http://52.33.254.51/InterfazWS.asmx?op=NotficaInterfaz";
            try
            {
                #region WEBSERVICE SIG

                var iRegistro = iCollection.ElementCode;
                if (iRegistro % 2 == 0)
                {
                    //Console.WriteLine("El número es par.");
                    bResult = true; sMessage = "200 (OK)";
                }
                else
                {
                    //Console.WriteLine("El número es impar.");
                    bResult = false; sMessage = "404 ( We could not find the resource you requested. Please refer to the documentation for the list of resources.)";
                }
                #endregion

                LogIntegracionWS(  idMantencion, sEndpointUrl, iCollection, bResult, sMessage);
            }
            catch (Exception ex)
            {
                LogIntegracionWS(idMantencion, sEndpointUrl, iCollection, false, (ex.InnerException == null ? ex.Message : ex.InnerException.Message));
            }
        }

        internal void LogIntegracionWS(int idMantencion,
                                            string sEndpointUrl,
                                            BusinessEntity.FormModels.FormPlanDeMantencion.Parametros iCollection,
                                            bool bResult,
                                            string sMessage)
        {
           _integracionSIGBusinessImpl.LogIntegracionWS(idMantencion, sEndpointUrl, iCollection, bResult, sMessage);

        }
        #endregion
    }
}
