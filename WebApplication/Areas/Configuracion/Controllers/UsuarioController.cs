using BusinessEntity;
using BusinessImpl;
using CrosscuttingUtiles;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace WebApplication.Areas.Configuracion.Controllers
{
    public class UsuarioController : WebApplication.Controllers.BaseController
    {
        private readonly UsuarioBusinessImpl usuarioBusinessImpl = new UsuarioBusinessImpl();
        private readonly LogBusinessImpl _logBusinessImpl = new LogBusinessImpl();

        // GET: Configuracion/Usuario
        public ActionResult Index()
        {
            var usuarioBusinessEntity = new UsuarioBusinessEntity.FormUsuarios();

            try
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
                    tla_des = null
                });
                #endregion

                var dataSetSQL = usuarioBusinessImpl.ListUser(string.Empty);

                var list = dataSetSQL.dsSQL.Tables[0].AsEnumerable()
                    .Select(m => new UsuarioBusinessEntity.UsuarioViewModel
                    {
                        usr_id = (int)m["usr_id"],
                        usr_lgn = (string)m["usr_lgn"],
                        usr_nom_full = $"{(string.IsNullOrEmpty((string)m["usr_nom"]) ? "" : (string)m["usr_nom"])} {(string.IsNullOrEmpty((string)m["usr_ape_pat"]) ? "" : (string)m["usr_ape_pat"])} {(string.IsNullOrEmpty((string)m["usr_ape_mat"]) ? "" : (string)m["usr_ape_mat"])}",
                        usr_car = m["car_nom"] == DBNull.Value ? string.Empty : (string)m["car_nom"],
                        usr_cre_usr = (string)m["usr_cre_usr"],
                        usr_cre_fec = (DateTime)m["usr_cre_fec"],
                        usr_mod_usr = (string)m["usr_mod_usr"] ?? string.Empty,
                        usr_mod_fec = (DateTime)m["usr_mod_fec"],
                        usr_mail = (string)m["usr_mail"] ?? string.Empty,
                        usr_vig = (bool)m["usr_vig"],
                        usr_blq = (bool)m["usr_blq"]

                    }).OrderByDescending(x => x.usr_mod_fec).ThenByDescending(x => x.usr_cre_fec).ToList();

                usuarioBusinessEntity.listUsuario = list;

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

                usuarioBusinessEntity.listUsuario = null;
            }

            return View("Listar", usuarioBusinessEntity);
        }


        // GET: Configuracion/Usuario/Create
        public ActionResult Create()
        {
            try
            {
                UsuarioBusinessEntity.UsuarioViewModel usuarioVan = new UsuarioBusinessEntity.UsuarioViewModel
                {
                    listado_cargos = new List<SelectListItem> { new SelectListItem { Text = "Seleccione Cargo...", Value = "0", Selected = true } }
                };

                var dataSetSQL = usuarioBusinessImpl.ListUser(string.Empty);

                if (dataSetSQL.intError != 0)
                    throw new Exception(dataSetSQL.strError);

                usuarioVan.usr_vig = true;
                usuarioVan.usr_blq = false;

                var allcargosList = dataSetSQL.dsSQL.Tables[1].AsEnumerable()
                    .Select(m => new SelectListItem
                    {
                        Value = ((int)m["car_cod"]).ToString(),
                        Text = (string)m["car_nom"],
                        Selected = usuarioVan != null
                            ? usuarioVan.usr_car_cod == (int)m["car_cod"] ? true : false
                            : false
                    }).ToList();

                usuarioVan.listado_cargos.AddRange(allcargosList);

                return View(usuarioVan);
            }
            catch
            {
                return View(new UsuarioBusinessEntity.UsuarioViewModel());
            }
        }

        // POST: Configuracion/Usuario/Create  -- FormCollection collection
        [HttpPost]
        public ActionResult Create(UsuarioBusinessEntity.UsuarioViewModel collection)
        {
            try
            {
                collection.usr_cre_usr = User.Identity.Name;

                if (string.IsNullOrEmpty(collection.usr_rut_com))
                    collection.usr_rut_com = "0-0";

                if (!collection.usr_rut_com.Contains("-"))
                    collection.usr_rut_com =
                        collection.usr_rut_com.Insert(collection.usr_rut_com.Trim().Length - 1, "-");

                collection.usr_rut = int.Parse(collection.usr_rut_com.Split('-')[0]);
                collection.usr_rut_dv = collection.usr_rut_com.Split('-')[1];
                collection.usr_psw = Cifrado.Cifrar(collection.usr_psw);
                collection.usr_ape_mat = collection.usr_ape_mat == null ? "" : collection.usr_ape_mat.Trim();

                var dataSetSQL = usuarioBusinessImpl.CreateUser(collection, collection.usr_cre_usr);

                if (dataSetSQL.intError != 0)
                    throw new Exception(dataSetSQL.strError);


                TempData["mensaje"] = $"El usuario {collection.usr_lgn.Trim().ToUpper()} se ha agregado satisfactoriamente.";
                TempData["tipo"] = "ok";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewData["mensaje"] = ex.Message;
                ViewData["tipo"] = "error";

                return View("Create", new UsuarioBusinessEntity.UsuarioViewModel
                {
                    listado_cargos = new List<SelectListItem> { new SelectListItem { Text = "Seleccione Cargo...", Value = "0", Selected = true } }
                });
            }


        }

        // GET: Configuracion/Usuario/Edit/5
        public ActionResult Edit(string strCurrentUser)
        {
            try
            {
                UsuarioBusinessEntity.UsuarioViewModel usuarioVan = new UsuarioBusinessEntity.UsuarioViewModel();

                var dataSetSQL = usuarioBusinessImpl.ListUser(strCurrentUser);

                if (dataSetSQL.intError != 0)
                    throw new Exception(dataSetSQL.strError);

                usuarioVan = dataSetSQL.dsSQL.Tables[0].AsEnumerable()
                    .Select(m => new UsuarioBusinessEntity.UsuarioViewModel
                    {
                        usr_id = (int)m["usr_id"],
                        usr_lgn = (string)m["usr_lgn"],
                        usr_nom_full = $"{(string.IsNullOrEmpty((string)m["usr_nom"]) ? "" : (string)m["usr_nom"])} {(string.IsNullOrEmpty((string)m["usr_ape_pat"]) ? "" : (string)m["usr_ape_pat"])} {(string.IsNullOrEmpty((string)m["usr_ape_mat"]) ? "" : (string)m["usr_ape_mat"])}",
                        usr_rut_com = $"{(string)m["usr_rut"]}-{(string)m["usr_rut_dv"]}",
                        usr_nom = (string)m["usr_nom"],
                        usr_ape_pat = (string)m["usr_ape_pat"],
                        usr_ape_mat = (string)m["usr_ape_mat"],
                        usr_tel = (string)m["usr_tel"],
                        usr_car = m["car_nom"] == DBNull.Value ? string.Empty : (string)m["car_nom"],
                        usr_cre_usr = (string)m["usr_cre_usr"],
                        usr_cre_fec = (DateTime)m["usr_cre_fec"],
                        usr_mod_usr = (string)m["usr_mod_usr"] ?? string.Empty,
                        usr_mod_fec = (DateTime)m["usr_mod_fec"],
                        usr_mail = (string)m["usr_mail"] ?? string.Empty,
                        usr_vig = (bool)m["usr_vig"],
                        usr_blq = (bool)m["usr_blq"],
                        usr_car_cod = (short)m["usr_car_cod"],
                        listado_cargos = new List<SelectListItem> { new SelectListItem { Text = "Seleccione Cargo...", Value = "0", Selected = true } }

                    }).DefaultIfEmpty(new UsuarioBusinessEntity.UsuarioViewModel()).FirstOrDefault();

                var allCargosList = dataSetSQL.dsSQL.Tables[1].AsEnumerable()
                    .Select(m => new SelectListItem
                    {
                        Value = ((int)m["car_cod"]).ToString(),
                        Text = (string)m["car_nom"],
                        Selected = usuarioVan != null ? usuarioVan.usr_car_cod == (int)m["car_cod"] ? true : false : false
                    }).ToList();

                usuarioVan.listado_cargos.AddRange(allCargosList);

                if (TempData["mensaje"] != null)
                {
                    ViewData["mensaje"] = TempData["mensaje"];
                    ViewData["tipo"] = TempData["tipo"];
                    TempData["mensaje"] = null;
                }

                return View(usuarioVan);
            }
            catch (Exception ex)
            {
                ViewData["mensaje"] = ex.Message;
                ViewData["tipo"] = "error";

                return View("Edit", new UsuarioBusinessEntity.UsuarioViewModel
                {
                    listado_cargos = new List<SelectListItem> { new SelectListItem { Text = "Seleccione Cargo...", Value = "0", Selected = true } }
                });
            }


        }

        // POST: Configuracion/Usuario/Edit/5
        [HttpPost]
        public ActionResult Edit(UsuarioBusinessEntity.UsuarioViewModel collection)
        {
            try
            {
                collection.usr_cre_usr = User.Identity.Name;

                if (string.IsNullOrEmpty(collection.usr_rut_com))
                    collection.usr_rut_com = "0-0";

                if (string.IsNullOrEmpty(collection.usr_ape_mat))
                    collection.usr_ape_mat = "";

                if (!collection.usr_rut_com.Contains("-"))
                    collection.usr_rut_com =
                        collection.usr_rut_com.Insert(collection.usr_rut_com.Trim().Length - 1, "-");

                collection.usr_rut = int.Parse(collection.usr_rut_com.Split('-')[0]);
                collection.usr_rut_dv = collection.usr_rut_com.Split('-')[1];
                collection.usr_psw = string.Empty;


                var dataSetSQL = usuarioBusinessImpl.CreateUser(collection, collection.usr_cre_usr);

                if (dataSetSQL.intError != 0)
                    throw new Exception(dataSetSQL.strError);

                TempData["mensaje"] =
                    $"El usuario {collection.usr_lgn.Trim().ToUpper()} se ha actualizao satisfactoriamente.";
                TempData["tipo"] = "ok";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewData["mensaje"] = ex.Message;
                ViewData["tipo"] = "error";

                return View("Edit", new UsuarioBusinessEntity.UsuarioViewModel
                {
                    listado_cargos = new List<SelectListItem> { new SelectListItem { Text = "Seleccione Cargo...", Value = "0", Selected = true } }
                });
            }
        }

        // GET: Configuracion/Usuario/Delete/5
        [Authorize]
        public ActionResult Delete(string id, string login)
        {
            try
            {
                if (id is null)
                {
                    TempData["mensaje"] = "El usuario ingresado no es válido.";
                    TempData["tipo"] = "error";
                    return RedirectToAction("Index");
                }

                if (string.IsNullOrEmpty(login))
                    login = id;

                var usuario = usuarioBusinessImpl.DeleteUser(Int32.Parse(id), string.Empty);

                if (usuario.intError != 0)
                    throw new Exception(usuario.strError);

                TempData["mensaje"] = $"El usuario {login.ToUpper()} se ha eliminado satisfactoriamente.";
                TempData["tipo"] = "ok";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["mensaje"] = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
                TempData["tipo"] = "error";
                return RedirectToAction("Index");
            }
        }



        #region JSON
        [AllowAnonymous]
        [HttpPost]
        public JsonResult JSON_ValidUserExists(string username)
        {
            try
            {
                var dataSetSQL = usuarioBusinessImpl.ListUser(string.Empty);

                if (dataSetSQL.intError != 0)
                    throw new Exception(dataSetSQL.strError);

                var isCount = dataSetSQL.dsSQL.Tables[0].AsEnumerable().Where(m => ((string)m["usr_lgn"]).Contains(username)).Count();
                if (isCount > 0)
                {
                    return Json(false);
                }
                else
                {
                    return Json(true);
                }
            }
            catch
            {
                return Json(false);
            }
        }

        #endregion
    }
}
