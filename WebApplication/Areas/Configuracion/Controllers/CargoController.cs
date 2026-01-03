using BusinessImpl;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace WebApplication.Areas.Configuracion.Controllers
{
    public class CargoController : WebApplication.Controllers.BaseController
    {
        private readonly CargoBusinessImpl cargoBusinessImpl = new CargoBusinessImpl();
        private readonly UsuarioBusinessImpl usuarioBusinessImpl = new UsuarioBusinessImpl();

        [Authorize]
        public ActionResult Index()
        {
            var cargoBusinessEntity = new BusinessEntity.FormModels.FormCargos();

            try
            {
                var dataSetSQL = cargoBusinessImpl.ListAll(User.Identity.Name);

                if (dataSetSQL.intError != 0)
                    throw new Exception(dataSetSQL.strError);

                if (TempData["mensaje"] != null)
                {
                    ViewData["mensaje"] = TempData["mensaje"];
                    ViewData["tipo"] = TempData["tipo"];
                    TempData["mensaje"] = null;
                }

                cargoBusinessEntity.listCargos = dataSetSQL.dsSQL.Tables[0].AsEnumerable()
                  .Select(m => new BusinessEntity.CargoBusinessEntity.Cargo
                  {
                      car_cod = (int)m["car_cod"],
                      car_key = (string)m["car_key"],
                      car_nom = (string)m["car_nom"],
                      car_des = m["car_des"] == DBNull.Value ? null : (string)m["car_des"],
                      car_vig = (bool)m["car_vig"],
                      car_cre_usr = (string)m["car_cre_usr"],
                      car_cre_fec = (DateTime)m["car_cre_fec"],
                      car_mod_usr = m["car_mod_usr"] == DBNull.Value ? null : (string)m["car_mod_usr"],
                      car_mod_fec = m["car_mod_fec"] == DBNull.Value ? null : (DateTime?)m["car_mod_fec"],
                      car_mod = (bool)m["car_mod"],
                      car_del = (bool)m["car_del"],

                  }).OrderByDescending(x => x.car_cre_fec).ThenByDescending(x => x.car_cod).ToList();
            }
            catch (Exception ex)
            {
                ViewData["mensaje"] = ex.Message;
                ViewData["tipo"] = "error";

                cargoBusinessEntity.listCargos = new List<BusinessEntity.CargoBusinessEntity.Cargo>();
            }

            return View("Listar", cargoBusinessEntity);
        }

        // GET: Configuracion/Cargo/Create
        public ActionResult Create()
        {
            var cargoBusinessEntity = new BusinessEntity.CargoBusinessEntity.Cargo();

            try
            {
                cargoBusinessEntity.car_cod = (int)0;
                cargoBusinessEntity.car_nom = string.Empty;
                cargoBusinessEntity.car_des = string.Empty;
                cargoBusinessEntity.car_del = false;
                cargoBusinessEntity.car_mod = false;
                cargoBusinessEntity.menus = string.Empty;
            }
            catch
            {
                cargoBusinessEntity = new BusinessEntity.CargoBusinessEntity.Cargo();
            }

            return View(cargoBusinessEntity);
        }

        // POST: Configuracion/Cargo/Create
        [HttpPost]
        public ActionResult Create(BusinessEntity.CargoBusinessEntity.Cargo cargo)
        {
            TreeJSON menuJSON;
            bool blnActualizar;

            try
            {
                menuJSON = JsonConvert.DeserializeObject<TreeJSON>(cargo.menus);

                if (menuJSON == null)
                    return View(cargo);

                cargo.listMenu = new List<BusinessEntity.MenuBusinessEntity.Menu>();
                cargo.car_cre_usr = User.Identity.Name;
                cargo.car_nom = cargo.car_nom.ToUpper();

                foreach (TreeJSON menu in menuJSON.children)
                {
                    if (menu.selected || menu.partsel)
                    {
                        cargo.listMenu.Add(new BusinessEntity.MenuBusinessEntity.Menu()
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
                                    cargo.listMenu.Add(new BusinessEntity.MenuBusinessEntity.Menu()
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
                                                cargo.listMenu.Add(new BusinessEntity.MenuBusinessEntity.Menu()
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
                blnActualizar = cargoBusinessImpl.fncGuardarCargo(cargo);

                //if (perfilBusinessImpl != 0)
                //    throw new Exception(datosPerfil.strTextoError);

                if (blnActualizar)
                {
                    TempData["mensaje"] = "El cargo " + cargo.car_nom + " se ha agregado satisfactoriamente.";
                    TempData["tipo"] = "ok";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ViewData["mensaje"] = ex.Message;
                ViewData["tipo"] = "error";
            }
            return View("Listar", cargo);
        }

        // GET: Configuracion/Cargo/Edit/5
        public ActionResult Edit(int id)
        {
            var cargoBusinessEntity = new BusinessEntity.CargoBusinessEntity.Cargo();

            try
            {
                var dataSetSQL = cargoBusinessImpl.ListById(User.Identity.Name, id);

                cargoBusinessEntity = dataSetSQL.dsSQL.Tables[0].AsEnumerable()
                  .Select(m => new BusinessEntity.CargoBusinessEntity.Cargo
                  {
                      car_cod = (int)m["car_cod"],
                      car_key = (string)m["car_key"],
                      car_nom = (string)m["car_nom"],
                      car_des = m["car_des"] == DBNull.Value ? null : (string)m["car_des"],
                      car_vig = (bool)m["car_vig"],
                      car_mod = (bool)m["car_mod"],
                      car_del = (bool)m["car_del"],
                      car_cre_usr = (string)m["car_cre_usr"],
                      car_cre_fec = (DateTime)m["car_cre_fec"],
                      car_mod_usr = m["car_mod_usr"] == DBNull.Value ? null : (string)m["car_mod_usr"],
                      car_mod_fec = m["car_mod_fec"] == DBNull.Value ? null : (DateTime?)m["car_mod_fec"],
                  }).FirstOrDefault();
            }
            catch
            {
                cargoBusinessEntity = new BusinessEntity.CargoBusinessEntity.Cargo();
            }

            return View(cargoBusinessEntity);
        }

        // POST: Configuracion/Cargo/Edit/5
        [HttpPost]
        public ActionResult Edit(BusinessEntity.CargoBusinessEntity.Cargo cargo)
        {
            TreeJSON menuJSON;
            bool blnActualizar;

            try
            {
                menuJSON = JsonConvert.DeserializeObject<TreeJSON>(cargo.menus);

                if (menuJSON == null)
                    return View(cargo);

                cargo.listMenu = new List<BusinessEntity.MenuBusinessEntity.Menu>();
                cargo.car_cre_usr = User.Identity.Name;
                cargo.car_nom = cargo.car_nom.ToUpper();

                foreach (TreeJSON menu in menuJSON.children)
                {
                    if (menu.selected || menu.partsel)
                    {
                        cargo.listMenu.Add(new BusinessEntity.MenuBusinessEntity.Menu()
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
                                    cargo.listMenu.Add(new BusinessEntity.MenuBusinessEntity.Menu()
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
                                                cargo.listMenu.Add(new BusinessEntity.MenuBusinessEntity.Menu()
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
                blnActualizar = cargoBusinessImpl.fncGuardarCargo(cargo);

                //if (perfilBusinessImpl != 0)
                //    throw new Exception(datosPerfil.strTextoError);

                if (blnActualizar)
                {
                    TempData["mensaje"] = "El cargo " + cargo.car_nom + " se ha modificado satisfactoriamente.";
                    TempData["tipo"] = "ok";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ViewData["mensaje"] = ex.Message;
                ViewData["tipo"] = "error";
            }
            return View("Listar", cargo);
        }

        [Authorize]
        public ActionResult Delete(string id, string cargo)
        {
            try
            {
                if (id is null)
                {
                    TempData["mensaje"] = "El cargo no es válido.";
                    TempData["tipo"] = "error";
                    return RedirectToAction("Index");
                }

                var usuario = cargoBusinessImpl.DeleteCargo(Int32.Parse(id), User.Identity.Name);

                if (usuario.intError != 0)
                    throw new Exception(usuario.strError);

                TempData["mensaje"] = $"El Cargo {cargo.ToUpper()} se ha eliminado satisfactoriamente.";
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

        [AllowAnonymous]
        [HttpPost]
        public JsonResult JSON_ValidExists(string cargo)
        {
            try
            {
                if (string.IsNullOrEmpty(cargo))
                    return Json(false);

                var array=cargo.Split('|');

                var idCargo = Int32.Parse(array[0]);
                var strCargo = array[1];

                var dataSetSQL = cargoBusinessImpl.ListAll(string.Empty);

                if (dataSetSQL.intError != 0)
                    throw new Exception(dataSetSQL.strError);

                var isCount = dataSetSQL.dsSQL.Tables[0].AsEnumerable().Where(m => (Int32)m["car_cod"] != idCargo && 
                                                                                    (string)m["car_nom"] == (strCargo != null ? strCargo.Trim() : "")).Count();

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
        public JsonResult JSON_ValidIsAsigned(string cargo)
        {
            
            try
            {
                var dataSetSQL = usuarioBusinessImpl.ListUser(string.Empty);

                if (dataSetSQL.intError != 0)
                    throw new Exception(dataSetSQL.strError);

                var isCount = dataSetSQL.dsSQL.Tables[0].AsEnumerable().Where(m => ((string)m["car_nom"]).Contains(cargo)).Count();

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
        public JsonResult Menu(int? id)
        {
            List<BusinessEntity.MenuBusinessEntity.Menu> listMenu, listSubMenu;
            List<TreeJSON> listMenuDevolver = new List<TreeJSON>();
            TreeJSON menuJSON, menuJSON2;
            TreeJSON menuJSONDevolver = new TreeJSON();
            List<BusinessEntity.MenuBusinessEntity.Menu> listMenus = new List<BusinessEntity.MenuBusinessEntity.Menu>();
            var idPerfil = (id == null ? (int)0 : id.Value);

            var dataSetSQL = cargoBusinessImpl.fncDevolverMenus(idPerfil);

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
                //listMenu = cargoBusinessImpl.fncDevolverSubMenus(Int32.Parse(menu2.key), 0, (id == null ? 0 : id));

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
    }
}