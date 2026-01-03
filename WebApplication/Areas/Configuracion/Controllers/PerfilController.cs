//using BusinessEntity;
using BusinessImpl;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace WebApplication.Areas.Configuracion.Controllers
{
    public class PerfilController : Controller
    {
        private readonly PerfilBusinessImpl perfilBusinessImpl = new PerfilBusinessImpl();

        // GET: Configuracion/Perfil
        public ActionResult Index()
        {
            var perfilBusinessEntity = new BusinessEntity.FormModels.FormPerfiles();

            try
            {
                var dataSetSQL = perfilBusinessImpl.ListAll(User.Identity.Name);

                if (dataSetSQL.intError != 0)
                    throw new Exception(dataSetSQL.strError);

                if (TempData["mensaje"] != null)
                {
                    ViewData["mensaje"] = TempData["mensaje"];
                    ViewData["tipo"] = TempData["tipo"];
                    TempData["mensaje"] = null;
                }

                perfilBusinessEntity.listPerfiles = dataSetSQL.dsSQL.Tables[0].AsEnumerable()
                  .Select(m => new BusinessEntity.PerfilBusinessEntity.Perfil
                  {
                      per_cod = (int)m["per_cod"],
                      per_key = (string)m["per_key"],
                      per_nom = (string)m["per_nom"],
                      per_des = m["per_des"] == DBNull.Value ? null : (string)m["per_des"],
                      per_vig = (bool)m["per_vig"],
                      per_cre_usr = (string)m["per_cre_usr"],
                      per_cre_fec = (DateTime)m["per_cre_fec"],
                      per_mod_usr = m["per_mod_usr"] == DBNull.Value ? null : (string)m["per_mod_usr"],
                      per_mod_fec = m["per_mod_fec"] == DBNull.Value ? null : (DateTime?)m["per_mod_fec"],
                      per_mod = (bool)m["per_mod"],
                      per_del = (bool)m["per_del"],

                  }).OrderByDescending(x => x.per_cre_fec).ThenByDescending(x => x.per_cod).ToList();
            }
            catch (Exception ex)
            {
                ViewData["mensaje"] = ex.Message;
                ViewData["tipo"] = "error";

                perfilBusinessEntity.listPerfiles = new List<BusinessEntity.PerfilBusinessEntity.Perfil>();
            }

            return View("Listar", perfilBusinessEntity);
        }

        // GET: Configuracion/Perfil/Edit/5
        public ActionResult Edit(int id)
        {
            var perfilBusinessEntity = new BusinessEntity.PerfilBusinessEntity.Perfil();

            try
            {
                var dataSetSQL = perfilBusinessImpl.ListById(User.Identity.Name, id);

                perfilBusinessEntity = dataSetSQL.dsSQL.Tables[0].AsEnumerable()
                  .Select(m => new BusinessEntity.PerfilBusinessEntity.Perfil
                  {
                      per_cod = (int)m["per_cod"],
                      per_key = (string)m["per_key"],
                      per_nom = (string)m["per_nom"],
                      per_des = m["per_des"] == DBNull.Value ? null : (string)m["per_des"],
                      per_vig = (bool)m["per_vig"],
                      per_cre_usr = (string)m["per_cre_usr"],
                      per_cre_fec = (DateTime)m["per_cre_fec"],
                      per_mod_usr = m["per_mod_usr"] == DBNull.Value ? null : (string)m["per_mod_usr"],
                      per_mod_fec = m["per_mod_fec"] == DBNull.Value ? null : (DateTime?)m["per_mod_fec"],
                      per_mod = (bool)m["per_mod"],
                      per_del = (bool)m["per_del"],

                  }).FirstOrDefault();
            }
            catch
            {
                perfilBusinessEntity = new BusinessEntity.PerfilBusinessEntity.Perfil();
            }

            return View(perfilBusinessEntity);
        }


        // GET: Configuracion/Perfil/Create
        public ActionResult Create()
        {
            var perfilBusinessEntity = new BusinessEntity.PerfilBusinessEntity.Perfil();

            try
            {
                perfilBusinessEntity.per_cod = (int)0;
                perfilBusinessEntity.per_nom = string.Empty;
                perfilBusinessEntity.menus = string.Empty;
            }
            catch
            {
                perfilBusinessEntity = new BusinessEntity.PerfilBusinessEntity.Perfil();
            }

            return View(perfilBusinessEntity);
        }

        // POST: Configuracion/Perfil/Create
        [HttpPost]
        public ActionResult Create(BusinessEntity.PerfilBusinessEntity.Perfil perfil)
        {
            TreeJSON menuJSON;
            bool blnActualizar;

            try
            {
                menuJSON = JsonConvert.DeserializeObject<TreeJSON>(perfil.menus);

                if (menuJSON == null)
                    return View(perfil);

                perfil.listMenu = new List<BusinessEntity.MenuBusinessEntity.Menu>();
                perfil.per_cre_usr = User.Identity.Name;
                perfil.per_nom = perfil.per_nom.ToUpper();

                foreach (TreeJSON menu in menuJSON.children)
                {
                    if (menu.selected || menu.partsel)
                    {
                        perfil.listMenu.Add(new BusinessEntity.MenuBusinessEntity.Menu()
                        {
                            menu_cod = menu.key,
                            menu_nom = menu.title,
                            menu_sel = true
                        });

                        if (menu.children != null)
                        {
                            foreach (TreeJSON menu2 in menu.children)
                            {
                                if (menu2.selected || menu2.partsel)
                                {
                                    perfil.listMenu.Add(new BusinessEntity.MenuBusinessEntity.Menu()
                                    {
                                        menu_cod = menu2.key,
                                        menu_nom = menu2.title,
                                        menu_sel = true
                                    });

                                    if (menu2.children != null)
                                    {
                                        foreach (TreeJSON menu3 in menu2.children)
                                        {
                                            if (menu3.selected)
                                            {
                                                perfil.listMenu.Add(new BusinessEntity.MenuBusinessEntity.Menu()
                                                {
                                                    menu_cod = menu3.key,
                                                    menu_nom = menu3.title,
                                                    menu_sel = true
                                                });
                                            }
                                        }
                                    }
                                }
                            }
                        }

                    }
                }

                // PASO 2) -  ACTUALIZAR 
                blnActualizar = perfilBusinessImpl.fncGuardarPerfil(perfil);

                //if (perfilBusinessImpl != 0)
                //    throw new Exception(datosPerfil.strTextoError);

                if (blnActualizar)
                {
                    TempData["mensaje"] = "El perfil " + perfil.per_nom + " se ha agregado satisfactoriamente.";
                    TempData["tipo"] = "ok";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ViewData["mensaje"] = ex.Message;
                ViewData["tipo"] = "error";
            }
            return View("Listar", perfil);
        }

        // POST: Configuracion/Perfil/Edit/5
        [HttpPost]
        public ActionResult Edit(BusinessEntity.PerfilBusinessEntity.Perfil perfil)
        {
            TreeJSON menuJSON;
            bool blnActualizar;

            try
            {
                menuJSON = JsonConvert.DeserializeObject<TreeJSON>(perfil.menus);

                if (menuJSON == null)
                    return View(perfil);

                perfil.listMenu = new List<BusinessEntity.MenuBusinessEntity.Menu>();
                perfil.per_cre_usr = User.Identity.Name;
                perfil.per_nom = perfil.per_nom.ToUpper();

                foreach (TreeJSON menu in menuJSON.children)
                {
                    if (menu.selected || menu.partsel)
                    {
                        perfil.listMenu.Add(new BusinessEntity.MenuBusinessEntity.Menu()
                        {
                            menu_cod = menu.key,
                            menu_nom = menu.title,
                            menu_sel = true
                        });

                        if (menu.children != null)
                        {
                            foreach (TreeJSON menu2 in menu.children)
                            {
                                if (menu2.selected || menu2.partsel)
                                {
                                    perfil.listMenu.Add(new BusinessEntity.MenuBusinessEntity.Menu()
                                    {
                                        menu_cod = menu2.key,
                                        menu_nom = menu2.title,
                                        menu_sel = true
                                    });

                                    if (menu2.children != null)
                                    {
                                        foreach (TreeJSON menu3 in menu2.children)
                                        {
                                            if (menu3.selected)
                                            {
                                                perfil.listMenu.Add(new BusinessEntity.MenuBusinessEntity.Menu()
                                                {
                                                    menu_cod = menu3.key,
                                                    menu_nom = menu3.title,
                                                    menu_sel = true
                                                });
                                            }
                                        }
                                    }
                                }
                            }
                        }

                    }
                }

                // PASO 2) -  ACTUALIZAR 
                blnActualizar = perfilBusinessImpl.fncGuardarPerfil(perfil);

                //if (perfilBusinessImpl != 0)
                //    throw new Exception(datosPerfil.strTextoError);

                if (blnActualizar)
                {
                    TempData["mensaje"] = "El perfil " + perfil.per_nom + " se ha modificado satisfactoriamente.";
                    TempData["tipo"] = "ok";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ViewData["mensaje"] = ex.Message;
                ViewData["tipo"] = "error";
            }
            return View("Listar", perfil);
        }

        [Authorize]
        public ActionResult Delete(string id, string perfil)
        {
            try
            {
                if (id is null)
                {
                    TempData["mensaje"] = "El Perfil no es válido.";
                    TempData["tipo"] = "error";
                    return RedirectToAction("Index");
                }

                var usuario = perfilBusinessImpl.DeletePerfil(Int32.Parse(id), User.Identity.Name);

                if (usuario.intError != 0)
                    throw new Exception(usuario.strError);

                TempData["mensaje"] = $"El Perfil {perfil.ToUpper()} se ha eliminado satisfactoriamente.";
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

        public JsonResult Menu(int? id)
        {
            //DatosMenu datosMenu = new DatosMenu(this._configuracion);
            List<BusinessEntity.MenuBusinessEntity.Menu> listMenu, listSubMenu;
            List<TreeJSON> listMenuDevolver = new List<TreeJSON>();
            TreeJSON menuJSON, menuJSON2;
            TreeJSON menuJSONDevolver = new TreeJSON();
            List<BusinessEntity.MenuBusinessEntity.Menu> listMenus = new List<BusinessEntity.MenuBusinessEntity.Menu>();
            var idPerfil = (id == null ? (int)0 : id.Value);

            var dataSetSQL = perfilBusinessImpl.fncDevolverMenus(idPerfil);

            if (dataSetSQL.intError != 0)
                throw new Exception(dataSetSQL.strError);

            listMenu = dataSetSQL.dsSQL.Tables[0].AsEnumerable().Select(m => new BusinessEntity.MenuBusinessEntity.Menu
            {
                menu_cod = ((Int32)m["menu_cod"]).ToString(),
                menu_nom = (string)m["menu_desc"],
                menu_sel = (bool)m["menu_sel"]
            }).ToList();

            foreach (BusinessEntity.MenuBusinessEntity.Menu menu in listMenu)
            {
                menuJSON = new TreeJSON();
                menuJSON.key = menu.menu_cod;
                menuJSON.title = menu.menu_nom;
                menuJSON.selected = menu.menu_sel;
                menuJSON.expanded = true;
                listMenuDevolver.Add(menuJSON);
            }

            foreach (TreeJSON menu2 in listMenuDevolver)
            {
                listMenu = new List<BusinessEntity.MenuBusinessEntity.Menu>();
                listMenu = perfilBusinessImpl.fncDevolverSubMenus(Int32.Parse(menu2.key), 0, (id == null ? 0 : id));

                menu2.children = new List<TreeJSON>();

                foreach (BusinessEntity.MenuBusinessEntity.Menu menu in listMenu)
                {
                    menuJSON = new TreeJSON();
                    menuJSON.key = menu.menu_cod;
                    menuJSON.title = menu.menu_nom;
                    menuJSON.selected = menu.menu_sel;
                    menuJSON.expanded = true;

                    listSubMenu = new List<BusinessEntity.MenuBusinessEntity.Menu>();
                    // listSubMenu = datosMenu.fncDevolverSubMenus(menu2.key.Substring(0, 1), menuJSON.key.Substring(3, 1), (id == null ? 0 : id));

                    menuJSON.children = new List<TreeJSON>();

                    foreach (BusinessEntity.MenuBusinessEntity.Menu menu3 in listSubMenu)
                    {
                        menuJSON2 = new TreeJSON();
                        menuJSON2.key = menu3.menu_cod;
                        menuJSON2.title = menu3.menu_nom;
                        menuJSON2.selected = menu3.menu_sel;
                        menuJSON.children.Add(menuJSON2);
                    }

                    menu2.children.Add(menuJSON);
                }
            }

            menuJSONDevolver.title = "Menu";
            menuJSONDevolver.key = "1";
            menuJSONDevolver.expanded = true;
            menuJSONDevolver.children = listMenuDevolver;

            string output = JsonConvert.SerializeObject(menuJSONDevolver);

            return Json(menuJSONDevolver, JsonRequestBehavior.AllowGet);
        }


        #region Validaciones
        [AllowAnonymous]
        [HttpPost]
        public JsonResult JSON_ValidIsAsigned(string id)
        {

            try
            {
                var dataSetSQL = perfilBusinessImpl.ListAsignedById(string.Empty, Int32.Parse(id));

                if (dataSetSQL.intError != 0)
                    throw new Exception(dataSetSQL.strError);

                var isCount = dataSetSQL.dsSQL.Tables[0].AsEnumerable().Where(m => (Int32)m["per_cod"] == Int32.Parse(id)).Count();

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

        [AllowAnonymous]
        [HttpPost]
        public JsonResult JSON_ValidExists(string perfil)
       {
            try
            {
                if (string.IsNullOrEmpty(perfil))
                    return Json(false);

                var array = perfil.Split('|');

                var idPerfil = Int32.Parse(array[0]);
                var strPerfil = array[1];

                var dataSetSQL = perfilBusinessImpl.ListAll(string.Empty);

                if (dataSetSQL.intError != 0)
                    throw new Exception(dataSetSQL.strError);

                var isCount = dataSetSQL.dsSQL.Tables[0].AsEnumerable().Where(m => (Int32)m["per_cod"] != idPerfil &&
                                                                      (string)m["per_nom"] == (strPerfil != null ? strPerfil.Trim() : "")).Count();

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
