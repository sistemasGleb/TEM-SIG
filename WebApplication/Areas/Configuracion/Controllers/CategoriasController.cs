using BusinessEntity;
using BusinessEntity.FormModels;
using BusinessImpl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace WebApplication.Areas.Configuracion.Controllers
{
    public class CategoriasController : WebApplication.Controllers.BaseController
    {
        private readonly CategoriaBusinessImpl categoriaBusinessImpl = new CategoriaBusinessImpl();
        private readonly LogBusinessImpl _logBusinessImpl = new LogBusinessImpl();

        [Authorize]
        public ActionResult Listar()
        {
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

                var formCategoria = new FormCategoria();

                try
                {
                    //if (!DatosUsuario.fncValidarOpcionUsuario(User, 2))
                    //    return RedirectToAction("NotAllowed", "Error");

                    var dataSetSQL = categoriaBusinessImpl.ListAll(User.Identity.Name);

                    if (dataSetSQL.intError != 0)
                        throw new Exception(dataSetSQL.strError);

                    // PASO 1) -  OBTENEMOS TODAS LAS CATEGORIAS Y SUS DOCUMENTOS
                    formCategoria.allCategoryList = dataSetSQL.dsSQL.Tables[0].AsEnumerable()
                        .Select(m => new CategoriaBusinessEntity
                        {
                            doc_cat_cod = (int)m["doc_cat_cod"],
                            doc_cat_nom = (string)m["doc_cat_nom"],
                            doc_cat_des = (string)m["doc_cat_des"],
                            doc_cat_est = (bool)m["doc_cat_est"],
                            doc_cat_cre_usr = (string)m["doc_cat_cre_usr"],
                            doc_cat_cre_fec = m["doc_cat_cre_fec"] == null
                                ? (DateTime?)null
                                : (DateTime)m["doc_cat_cre_fec"],
                            doc_cat_mod_usr = (string)m["doc_cat_mod_usr"],
                            doc_cat_mod_fec = m["doc_cat_mod_fec"] == null
                                ? (DateTime?)null
                                : (DateTime)m["doc_cat_mod_fec"]
                        }).OrderByDescending(x => x.doc_cat_cod).ToList();

                    if (TempData["mensaje"] != null)
                    {
                        ViewData["mensaje"] = TempData["mensaje"];
                        ViewData["tipo"] = TempData["tipo"];
                        TempData["mensaje"] = null;
                    }
                }
                catch (Exception ex)
                {
                    formCategoria.allCategoryList = new List<CategoriaBusinessEntity>();
                    ViewData["mensaje"] = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
                    ViewData["tipo"] = "error";
                }

                return View(formCategoria);
            }
        }

        [Authorize]
        public ActionResult Agregar()
        {
            //if (!DatosUsuario.fncValidarOpcionUsuario(User, 2))
            //    return RedirectToAction("NotAllowed", "Error");

            CategoriaBusinessEntity categoria = new CategoriaBusinessEntity()
            {
                doc_cat_est = true
            };

            return View(categoria);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Agregar(CategoriaBusinessEntity collection)
        {
            //if (!DatosUsuario.fncValidarOpcionUsuario(User, 2))
            //    return RedirectToAction("NotAllowed", "Error");

            try
            {
                if (!ModelState.IsValid)
                    throw new Exception("MVC model error collection");

                if (string.IsNullOrEmpty(collection.doc_cat_nom))
                    throw new Exception($"Debe ingresar el nombre de una categoria!.");

                var dataSetSQL = categoriaBusinessImpl.ListAll(User.Identity.Name);

                if (dataSetSQL.intError != 0)
                    throw new Exception("Error al validar la biblioteca.");

                // PASO 1) -  OBTENEMOS TODAS LAS CATEGORIAS Y SUS DOCUMENTOS
                var allCategoryList = dataSetSQL.dsSQL.Tables[0].AsEnumerable()
                    .Where(m => (string)m["doc_cat_nom"] == collection.doc_cat_nom.Trim())
                    .Select(m => new CategoriaBusinessEntity
                    {
                        doc_cat_cod = (int)m["doc_cat_cod"],
                        doc_cat_nom = (string)m["doc_cat_nom"],
                        doc_cat_des = (string)m["doc_cat_des"],
                        doc_cat_est = (bool)m["doc_cat_est"],
                        doc_cat_cre_usr = (string)m["doc_cat_cre_usr"],
                        doc_cat_cre_fec = m["doc_cat_cre_fec"] == null
                            ? (DateTime?)null
                            : (DateTime)m["doc_cat_cre_fec"],
                        doc_cat_mod_usr = (string)m["doc_cat_mod_usr"],
                        doc_cat_mod_fec = m["doc_cat_mod_fec"] == null
                            ? (DateTime?)null
                            : (DateTime)m["doc_cat_mod_fec"]
                    }).OrderByDescending(x => x.doc_cat_cod).ToList();

                if (allCategoryList.Count > 0) throw new Exception($"La biblioteca ({collection.doc_cat_nom.Trim().ToUpper()}) ya existe!.");

                // PASO 3) - SETEAMOS USUARIO ACTUAL QUE REALIZA LOS CAMBIOS
                collection.doc_cat_cre_usr = User.Identity.Name;
                collection.doc_cat_mod_usr = User.Identity.Name;

                var dataSetSQL2 = categoriaBusinessImpl.Create(collection, User.Identity.Name);

                if (dataSetSQL2.intError != 0)
                    throw new Exception(dataSetSQL2.strError);

                TempData["mensaje"] = $"La biblioteca ({collection.doc_cat_nom.Trim().ToUpper()}) se ha agregado satisfactoriamente.";
                TempData["tipo"] = "ok";
                return RedirectToAction("Listar");
            }
            catch (Exception ex)
            {
                ViewData["mensaje"] = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
                ViewData["tipo"] = "error";
            }

            return View(collection);
        }

        [Authorize]
        public ActionResult Editar(int id)
        {
            CategoriaBusinessEntity singleCategory = new CategoriaBusinessEntity();

            try
            {
                //if (!DatosUsuario.fncValidarOpcionUsuario(User, 2))
                //    return RedirectToAction("NotAllowed", "Error");

                var dataSetSQL = categoriaBusinessImpl.ListAll(User.Identity.Name);

                if (dataSetSQL.intError != 0)
                    throw new Exception(dataSetSQL.strError);

                // PASO 1) -  OBTENEMOS TODAS LAS CATEGORIAS Y SUS DOCUMENTOS
                singleCategory = dataSetSQL.dsSQL.Tables[0].AsEnumerable()
                    .Where(m => (int)m["doc_cat_cod"] == id)
                    .Select(m => new CategoriaBusinessEntity
                    {
                        doc_cat_cod = (int)m["doc_cat_cod"],
                        doc_cat_nom = (string)m["doc_cat_nom"],
                        doc_cat_des = (string)m["doc_cat_des"],
                        doc_cat_est = (bool)m["doc_cat_est"],
                        doc_cat_cre_usr = (string)m["doc_cat_cre_usr"],
                        doc_cat_cre_fec = m["doc_cat_cre_fec"] == null
                            ? (DateTime?)null
                            : (DateTime)m["doc_cat_cre_fec"],
                        doc_cat_mod_usr = (string)m["doc_cat_mod_usr"],
                        doc_cat_mod_fec = m["doc_cat_mod_fec"] == null
                            ? (DateTime?)null
                            : (DateTime)m["doc_cat_mod_fec"]
                    }).FirstOrDefault();

                if (singleCategory is null)
                {
                    TempData["mensaje"] = "La biblioteca seleccionada no existe!.";
                    TempData["tipo"] = "error";

                    return RedirectToAction("Listar");
                }
            }
            catch (Exception ex)
            {
                ViewData["mensaje"] = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
                ViewData["tipo"] = "error";
            }

            return View(singleCategory);
        }
        [HttpPost]
        public ActionResult Editar(CategoriaBusinessEntity collection)
        {
            try
            {
                // PASO 3) - SETEAMOS USUARIO ACTUAL QUE REALIZA LOS CAMBIOS
                collection.doc_cat_mod_usr = User.Identity.Name;
                collection.doc_cat_mod_fec = DateTime.Now;

                var dataSetSQL = categoriaBusinessImpl.Create(collection, User.Identity.Name);

                if (dataSetSQL.intError != 0)
                    throw new Exception(dataSetSQL.strError);

                TempData["mensaje"] = $"La biblioteca ({collection.doc_cat_nom.Trim().ToUpper()}) se ha modificado satisfactoriamente.";
                TempData["tipo"] = "ok";
                return RedirectToAction("Listar");
            }
            catch (Exception ex)
            {
                ViewData["mensaje"] = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
                ViewData["tipo"] = "error";

                return View("Editar", new CategoriaBusinessEntity());
            }
        }
    }
}