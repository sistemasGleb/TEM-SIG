using BusinessEntity;
using BusinessImpl;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace WebApplication.Areas.Mapa.Controllers
{
    public class MapsController : WebApplication.Controllers.BaseController
    {
        private readonly MapBusinessImpl mapBusinessImpl = new MapBusinessImpl();
        private readonly MenuBusinessImpl menuBusinessImpl = new MenuBusinessImpl();
        private readonly LogBusinessImpl _logBusinessImpl = new LogBusinessImpl();
        private readonly PlanDeMantencionBusinessImpl planDeMantencionBusinessImpl = new PlanDeMantencionBusinessImpl();
        private readonly ElementosBusinessImpl elementosBusinessImpl = new ElementosBusinessImpl();


        #region MENU
        [Authorize]
        public ActionResult Menu()
        {
            ViewData["mensaje"] = "";

            return View();
        }

        [Authorize]
        public JsonResult ListarMenu(int? id)
        {
            List<BusinessEntity.MenuBusinessEntity.Menu> listMenu;
            var listMenuDevolver = new List<TreeJSON>();
            TreeJSON menuJSON;
            var menuJSONDevolver = new TreeJSON();

            var dataSetSQL = menuBusinessImpl.fncDevolverMenu(User.Identity.Name);

            if (dataSetSQL.intError != 0)
            {
                //error
            }
            else
            {
                var newMenuJSON = dataSetSQL.dsSQL.Tables[0].AsEnumerable()
                    .Where(m => (bool)m["menu_vig"])
                    .Select(m => new TreeJSON
                    {
                        key = ((short)m["menu_cod"]).ToString(),//menu.menu_cod,
                        title = (string)m["menu_desc"],          //menu.menu_nom,
                        selected = (bool)m["menu_sel"],          //menu.menu_sel,
                        expanded = false,
                        row = (short)m["menu_cod"]
                    }).OrderBy(m => m.row).ThenBy(m => m.title).ToList();

                listMenuDevolver.AddRange(newMenuJSON);

                foreach (TreeJSON menu2 in listMenuDevolver)
                {
                    listMenu = new List<BusinessEntity.MenuBusinessEntity.Menu>();
                    var OpcMenu = menu2.key;

                    listMenu = dataSetSQL.dsSQL.Tables[1].AsEnumerable()
                        .Where(m => ((short)m["men_tra_cod"] == short.Parse(menu2.key)) && (bool)m["men_tra_vig"] && (bool)m["men_ele_vig"] && (bool)m["men_ele_opc_vis"])
                        .Select(m => new BusinessEntity.MenuBusinessEntity.Menu
                        {
                            menu_cod = ((short)m["men_id"]).ToString(),
                            menu_nom = (string)m["men_ele_nom"],
                            menu_sel = (bool)m["men_ele_sel_chk"],
                            menu_row = (short)m["men_id"]
                        }).OrderBy(X => X.menu_nom).ToList();

                    menu2.children = new List<TreeJSON>();

                    foreach (BusinessEntity.MenuBusinessEntity.Menu menu in listMenu)
                    {
                        menuJSON = new TreeJSON
                        {
                            key = menu.menu_cod,
                            title = menu.menu_nom,
                            selected = menu.menu_sel,
                            expanded = true
                        };
                        menu2.children.Add(menuJSON);
                    }
                }

                menuJSONDevolver.title = "Menu";
                menuJSONDevolver.key = "1";
                menuJSONDevolver.expanded = true;
                menuJSONDevolver.children = listMenuDevolver;
            }

            var _List = Newtonsoft.Json.JsonConvert.SerializeObject(menuJSONDevolver);

            return Json(menuJSONDevolver, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region TEST
        [Authorize]
        [HttpGet]
        public ActionResult Test()
        {
            BusinessEntity.FormModels.FormElementos collection = new BusinessEntity.FormModels.FormElementos
            {
                allTypeElementList = new List<SelectListItem> { new SelectListItem { Text = "Seleccione ...", Value = "0" } },
                elementList = new List<SelectListItem> { new SelectListItem { Text = "Seleccione ...", Value = "0" } },
                id_elemento = 0,
                id_tipo_elemento = 0
            };

            try
            {
                // sp_app_extrae_elemento_detalle
                var dataSetSQL = elementosBusinessImpl.AllInfoElement(User.Identity.Name, 0, 0);

                if (dataSetSQL.intError != 0)
                    throw new Exception(dataSetSQL.strError);

                var allElementTypeList = dataSetSQL.dsSQL.Tables[0].AsEnumerable()
                       //.Where(x => (bool)x["ele_vig"] )
                       .Select(m => new SelectListItem
                       {
                           Value = ((short)m["ele_id"]).ToString(),
                           Text = (string)m["ele_des"],
                           Selected = false,
                           Disabled = !(bool)m["ele_edt_vig"]
                       }).OrderBy(x => x.Disabled).ThenBy(x => x.Text).ToList();

                var allElementTypeListCount = dataSetSQL.dsSQL.Tables[6].AsEnumerable()
                    .Select(m => new
                    {
                        tel_id = (byte)m["codigo"],
                        tel_id_can = (Int32)m["cantidad"]
                    }).OrderBy(x => x.tel_id).ToList();

                var filteredElementTypeList = (List<SelectListItem>)(from cnf in allElementTypeList
                                                                     join tmp in allElementTypeListCount on new { codigo = cnf.Value } equals new { codigo = tmp.tel_id.ToString() }
                                                                     into EmployeeAddressGroup
                                                                     from tmp in EmployeeAddressGroup.DefaultIfEmpty()
                                                                     select new SelectListItem
                                                                     {
                                                                         Value = cnf.Value,
                                                                         Text = tmp == null ? $"{cnf.Text} (0)" : $"{cnf.Text} ({tmp.tel_id_can})",
                                                                         Selected = false,
                                                                         Disabled = tmp == null ? true : (tmp.tel_id_can > 0 ? false : true)
                                                                     }).OrderBy(x => x.Text).ToList();

                collection.allTypeElementList.AddRange(filteredElementTypeList);

                return View(collection);
            }
            catch (Exception ex)
            {
                ViewData["mensaje"] = ex.InnerException is null ? ex.Message : ex.InnerException.Message;
                ViewData["tipo"] = "error";

                return View(new BusinessEntity.FormModels.FormElementos
                {
                    elementList = new List<SelectListItem> { new SelectListItem { Text = "Seleccione ...", Value = "0" } }
                });
            }
        }


        [Authorize]
        [HttpPost]
        public ActionResult Test(BusinessEntity.FormModels.FormElementos collection)
        {
            try
            {
                collection.allTypeElementList = new List<SelectListItem> { new SelectListItem { Text = "Seleccione ...", Value = "0" } };
                collection.elementList = new List<SelectListItem> { new SelectListItem { Text = "Seleccione ...", Value = "0" } };


                // PASO 1) -  TIPOS DE ELEMENTO
                var dataSetSQL = elementosBusinessImpl.AllInfoElement(User.Identity.Name, 0, 0);

                if (dataSetSQL.intError != 0)
                    throw new Exception(dataSetSQL.strError);

                var allElementTypeList = dataSetSQL.dsSQL.Tables[0].AsEnumerable()
                       //.Where(x => (bool)x["ele_vig"] )
                       .Select(m => new SelectListItem
                       {
                           Value = ((short)m["ele_id"]).ToString(),
                           Text = (string)m["ele_des"],
                           Selected = false,
                           Disabled = !(bool)m["ele_edt_vig"]
                       }).OrderBy(x => x.Disabled).ThenBy(x => x.Text).ToList();

                var allElementTypeListCount = dataSetSQL.dsSQL.Tables[6].AsEnumerable()
                    .Select(m => new
                    {
                        tel_id = (byte)m["codigo"],
                        tel_id_can = (Int32)m["cantidad"]
                    }).OrderBy(x => x.tel_id).ToList();

                var filteredElementTypeList = (List<SelectListItem>)(from cnf in allElementTypeList
                                                                     join tmp in allElementTypeListCount on new { codigo = cnf.Value } equals new { codigo = tmp.tel_id.ToString() }
                                                                     into EmployeeAddressGroup
                                                                     from tmp in EmployeeAddressGroup.DefaultIfEmpty()
                                                                     select new SelectListItem
                                                                     {
                                                                         Value = cnf.Value,
                                                                         Text = tmp == null ? $"{cnf.Text} (0)" : $"{cnf.Text} ({tmp.tel_id_can})",
                                                                         Selected = false,
                                                                         Disabled = tmp == null ? true : (tmp.tel_id_can > 0 ? false : true)
                                                                     }).OrderBy(x => x.Text).ToList();

                collection.allTypeElementList.AddRange(filteredElementTypeList);

                // PASO 2) - ELEMENTOS
                var allEmentList = dataSetSQL.dsSQL.Tables[1].AsEnumerable()
                       .Where(m => ((byte)m["Id Elemento"]).ToString() == collection.id_tipo_elemento.ToString())
                       .Select(m => new SelectListItem
                       {
                           Value = ((int)m["Id"]).ToString(),
                           Text = $"{(string)m["Nombre Elemento"]} ({(int)m["Id"]})",
                           Selected = false
                       }).OrderBy(x => x.Disabled).ThenBy(x => x.Text).ToList();

                collection.elementList.AddRange(allEmentList);
            }
            catch
            {


            }

            return View(collection);
        }
        #endregion

        [Authorize]
        public ActionResult Index()
        {
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

                ViewData["mensaje"] = "";
            }
            catch (Exception ex)
            {
                ViewData["mensaje"] = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
                ViewData["tipo"] = "error";

                return View();
            }

            return View();
        }


        public virtual JsonResult ListarJSON_MarkersById (string idTipoMarker, string idMarker)
        {
            System.Threading.Thread.Sleep(1000);

            try
            {
                 #region MARCADORES

                var result =
                    mapBusinessImpl.GetMarkerDetail(User.Identity.Name, Int32.Parse(idTipoMarker), Int32.Parse(idMarker));

                if (result.intError != 0)
                    throw new Exception(result.strError);


                // PASO 2) - LISTA DE MARCADORES 
                var listMarkers = result.dsSQL.Tables[0].AsEnumerable()
                    .Where(m => (bool)m["mar_vig"] && m["mar_lat_ini"] != DBNull.Value && m["mar_lon_ini"] != DBNull.Value)
                    .Select(m => new
                    {
                        com_id = m["mar_id"] == DBNull.Value ? (int)0 : (int)m["mar_id"],
                        com_categoria = (string)m["mar_tip_ele"],
                        com_id_inv = m["mar_id"] == DBNull.Value ? "" : ((Int32)m["mar_id"]).ToString(),
                        com_dm = m["mar_dm_ini"] != DBNull.Value && m["mar_dm_fin"] != DBNull.Value ? $"{((decimal)m["mar_dm_ini"]).ToString()} - {((decimal)m["mar_dm_fin"]).ToString()}"
                              : m["mar_dm_ini"] != DBNull.Value && m["mar_dm_fin"] == DBNull.Value ? $"{((decimal)m["mar_dm_ini"]).ToString()}" : "",
                        com_elemento_nombre = m["mar_ele_nom"] == DBNull.Value ? "" : (string)m["mar_ele_nom"],
                        com_ruta = (string)m["mar_rou"],
                        com_tramo_desc = (string)m["mar_tra"],
                        com_id_mop = m["mar_id_mop"] == DBNull.Value ? "" : (string)m["mar_id_mop"], 
                        com_map_ubicacion_flag = m["mar_lat_fin"] == DBNull.Value ? true :
                            (string)m["mar_lat_fin"] == (string)m["mar_lat_ini"] ? true : false,
                        com_map_ubicacion_inicio = m["mar_lat_ini"] == DBNull.Value || m["mar_lon_ini"] == DBNull.Value
                            ? ""
                            : $"{((string)m["mar_lat_ini"]).Substring(0, 9)},{((string)m["mar_lon_ini"]).Substring(0, 9)}",
                        com_map_ubicacion_fin = m["mar_lat_fin"] == DBNull.Value || m["mar_lon_fin"] == DBNull.Value
                            ? ""
                            : $"{((string)m["mar_lat_fin"]).Substring(0, 9)},{((string)m["mar_lon_fin"]).Substring(0, 9)}",
                        com_tipo = Convert.ToInt16( idTipoMarker)
                    }).ToList();
 
                #endregion

                return Json(
                    new
                    {
                        status = listMarkers.Count > 0 ? true : false,
                        List = listMarkers,
                        listLines = "[]",
                        areaLines = "[]",
                        exception = listMarkers.Count > 0
                            ? ""
                            : "No se encontraron marcadores para los elementos selecionados."
                    }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ViewData["mensaje"] = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
                ViewData["tipo"] = "error";

                return Json(
                    new
                    {
                        status = false,
                        exception = ex.InnerException == null ? ex.Message : ex.InnerException.Message
                    }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Método que extrae marcadores asociados al menu del usuario
        /// </summary>
        /// <returns>le metodo retorna un Objeto JSON</returns>
        public virtual JsonResult ListarJSON_Markers(string jsonData, bool isPostback, string iIcon)
        {
            System.Threading.Thread.Sleep(1000);

            try
            {
                var js = new JavaScriptSerializer();
                var blogObject = js.Deserialize<Root>(jsonData);
                var limyStringListst = new List<int>();
                var listMenu = new List<MenuBusinessEntity>();
                var listSubMenu = new List<MenuBusinessEntity>();
                var strInfo = new StringBuilder();

                if (blogObject != null)
                    // var chCount = blogObject.Children.Count();
                    foreach (var child in blogObject.Children)

                        if (child.Children != null)
                            foreach (var VARIABLE in child.Children)
                            {
                                var key = VARIABLE.Key;
                                var selected = VARIABLE.Selected;
                                if (selected) limyStringListst.Add(Convert.ToInt32(key));
                            }

                if (!isPostback)
                {
                    var dataSetSQL = menuBusinessImpl.fncDevolverMenu(User.Identity.Name);

                    // PASO 1) -  LISTA DE MENUS
                    var list = dataSetSQL.dsSQL.Tables[0].AsEnumerable()
                        .Where(m => (bool)m["menu_vig"])
                        .Select(m => (short)m["menu_cod"]).ToList();

                    // PASO 2)-  LISTA DE SUBMENUS
                    var listSubMenus = dataSetSQL.dsSQL.Tables[1].AsEnumerable()
                        .Where(m => list.Contains((short)m["men_tra_cod"]) && (bool)m["men_tra_vig"] &&
                                    (bool)m["men_ele_vig"] && (bool)m["men_ele_opc_vis"])
                        .Select(m => new
                        {
                            menu_cod = ((short)m["men_id"]).ToString(),
                            menu_tra = (short)m["men_tra_cod"],
                            menu_tra_nom = (string)m["men_tra_nom"],
                            menu_nom = (string)m["men_ele_nom"],
                            menu_sel = (bool)m["men_ele_sel_chk"],
                            menu_row = (short)m["men_id"]
                        }).OrderBy(x => x.menu_row).ToList();

                    var _List = JsonConvert.SerializeObject(listSubMenus);


                    var lista = listSubMenus.Where(x => x.menu_sel).Select(x => (int)x.menu_row).ToList();
                    limyStringListst.AddRange(lista);
                }

                #region MARCADORES

                var result = mapBusinessImpl.GetAllMarkers(User.Identity.Name, limyStringListst);

                if (result.intError != 0)
                    throw new Exception(result.strError);


                // PASO 2) - LISTA DE MARCADORES 
                var listMarkers = result.dsSQL.Tables[1].AsEnumerable()
                    .Where(m => (bool)m["vigente"] && m["lat_ini"] != DBNull.Value && m["lon_ini"] != DBNull.Value)
                    .Select(m => new
                    {
                        com_id = m["id"] == DBNull.Value ? (int)0 : (int)m["id"],
                        com_tipo = m["tipo"] == DBNull.Value ? (short)0 : (short)m["tipo"],
                        com_subtipo = (short)m["subtipo"],
                        com_categoria = (string)m["categoria"],
                        com_nombre = (string)m["nombre"],
                        com_lat_ini = m["lat_ini"] == DBNull.Value ? "" : (string)m["lat_ini"],
                        com_lon_ini = m["lon_ini"] == DBNull.Value ? "" : (string)m["lon_ini"],
                        com_lat_fin = m["lat_fin"] == DBNull.Value ? "" : (string)m["lat_fin"],
                        com_lon_fin = m["lon_fin"] == DBNull.Value ? "" : (string)m["lon_fin"],
                        com_vigente = (bool)m["vigente"],
                        com_creado = (DateTime)m["creado"],
                        com_map_icon = fnc_getmarkerIcon(iIcon, (string)m["icon_active"],
                            (string)m["icon_active_ext"]),
                        com_map_ubicacion_inicio = m["lat_ini"] == DBNull.Value || m["lon_ini"] == DBNull.Value
                            ? ""
                            : $"{((string)m["lat_ini"]).Substring(0, 9)},{((string)m["lon_ini"]).Substring(0, 9)}",
                        com_map_ubicacion_fin = m["lat_fin"] == DBNull.Value || m["lon_fin"] == DBNull.Value
                            ? ""
                            : $"{((string)m["lat_fin"]).Substring(0, 9)},{((string)m["lon_fin"]).Substring(0, 9)}",
                        com_map_ubicacion_flag = m["lat_fin"] == DBNull.Value ? true :
                            (string)m["lat_fin"] == (string)m["lat_ini"] ? true : false,
                        com_map_img = m["img"] == DBNull.Value ? "" : fnc_getmarkerImagePreview((string)m["img"]),
                        com_img_vig = m["img"] == DBNull.Value ? false : true,
                        com_map_tramo = (string)m["tramo"],
                        com_id_mop = m["id_mop"] == DBNull.Value ? "" : (string)m["id_mop"],
                        com_ruta = (string)m["ruta"],
                        com_tramo_desc = (string)m["tramo"],
                        com_id_inv = m["id_inv"] == DBNull.Value ? "" : (string)m["id_inv"],
                        com_title = $"{(string)m["categoria"]} {(m["id_inv"] == DBNull.Value ? "" : $"/ {(string)m["id_inv"]}")}",
                        com_elemento_nombre = m["nombre_elemento"] == DBNull.Value ? "" : (string)m["nombre_elemento"],
                        com_dm = m["dm_ini"] != DBNull.Value && m["dm_fin"] != DBNull.Value ? $"{((decimal)m["dm_ini"]).ToString()} - {((decimal)m["dm_fin"]).ToString()}"
                              : m["dm_ini"] != DBNull.Value && m["dm_fin"] == DBNull.Value ? $"{((decimal)m["dm_ini"]).ToString()}" : ""
                    }).ToList();

                // PASO 3) - TUNELES (Lista)
                var listLines = result.dsSQL.Tables[2].AsEnumerable()
                    .Select(m => new
                    {
                        com_id = (short)m["tunel"],
                        com_tipo = (short)m["corr"],
                        com_lat = (string)m["lat"],
                        com_lon = (string)m["lon"]
                    }).OrderBy(x => x.com_id).ThenBy(x => x.com_tipo).ToList();

                // PASO 3) - POLIGONO DE AREA
                var listAreaLines = result.dsSQL.Tables[3].AsEnumerable()
                    .Select(m => new
                    {
                        corr = (short)m["corr"],
                        descripcion = (string)m["descripcion"],
                        com_lat = (string)m["lat"],
                        com_lon = (string)m["lon"]
                    }).OrderBy(x => x.corr).ToList();

                #endregion

                return Json(
                    new
                    {
                        status = listMarkers.Count > 0 ? true : false,
                        List = listMarkers,
                        listLines = listLines,
                        areaLines = listAreaLines,
                        exception = listMarkers.Count > 0
                            ? ""
                            : "No se encontraron marcadores para los elementos selecionados."
                    }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ViewData["mensaje"] = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
                ViewData["tipo"] = "error";

                return Json(
                    new
                    {
                        status = false,
                        exception = ex.InnerException == null ? ex.Message : ex.InnerException.Message
                    }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Método que extrae Detalle de un marcador segun su Id y tipo
        /// </summary>
        [HttpPost]
        [Authorize]
        public virtual JsonResult ListarJSON_MarkerDetail(string idTipoMarker, string idMarker)
        {
            System.Threading.Thread.Sleep(1000);
            var str2 = new StringBuilder();
            var strInfoLeft = new StringBuilder();
            var strInfoRight = new StringBuilder();
            var markerHis = new List<ElementoBusinessEntity.Elemento>();
            var markerImgList = new List<ElementoBusinessEntity.Imagen>();
            var markerPoligonoList = new List<ElementoBusinessEntity.Poligono>();
            List<BusinessEntity.PlanDeMantencionBusinessEntity.PlanIngreso> allTableMaintenanceType = new List<BusinessEntity.PlanDeMantencionBusinessEntity.PlanIngreso>();

            var numbers = new StringBuilder();
            try
            {
                var dataSetSQL =
                    mapBusinessImpl.GetMarkerDetail(User.Identity.Name, Int32.Parse(idTipoMarker), Int32.Parse(idMarker));

                if (dataSetSQL.intError != 0)
                    throw new Exception(dataSetSQL.strError);

                //-- TUPLA UNICA --
                var marker = dataSetSQL.dsSQL.Tables[0].AsEnumerable()
                    .Select(m => new
                    {
                        marker_id = (int)m["mar_id"],
                        marker_id_mop = m["mar_id_mop"] == DBNull.Value ? "" : (string)m["mar_id_mop"],
                        marker_tra = (string)m["mar_tra"],
                        marker_tip_ele = (string)m["mar_tip_ele"],
                        marker_dm_ini = m["mar_dm_ini"] == DBNull.Value ? "" : ((decimal)m["mar_dm_ini"]).ToString(),
                        marker_dm_fin = m["mar_dm_fin"] == DBNull.Value ? "" : ((decimal)m["mar_dm_fin"]).ToString(),
                        marker_ruta = (string)m["mar_rou"],
                        marker_titulo = (string)m["mar_tit"],
                        marker_vig = (bool)m["mar_vig"],
                        marker_lat_ini = m["mar_lat_ini"] == DBNull.Value ? "" : (string)m["mar_lat_ini"],
                        marker_lon_ini = m["mar_lon_ini"] == DBNull.Value ? "" : (string)m["mar_lon_ini"],
                        marker_lat_fin = m["mar_lat_fin"] == DBNull.Value ? "" : (string)m["mar_lat_fin"],
                        marker_lon_fin = m["mar_lon_fin"] == DBNull.Value ? "" : (string)m["mar_lon_fin"],
                        marker_geo_ubicacion_ini = m["mar_lat_ini"] == DBNull.Value || m["mar_lon_ini"] == DBNull.Value
                            ? ""
                            : $"{((string)m["mar_lat_ini"]).Substring(0, 9)},{((string)m["mar_lon_ini"]).Substring(0, 9)}",
                        marker_geo_ubicacion_fin = m["mar_lat_fin"] == DBNull.Value || m["mar_lon_fin"] == DBNull.Value
                            ? ""
                            : $"{((string)m["mar_lat_fin"]).Substring(0, 9)},{((string)m["mar_lon_fin"]).Substring(0, 9)}",
                        marker_geo_ubicacion_flag = m["mar_lat_fin"] == DBNull.Value ? true :
                            (string)m["mar_lat_ini"] == (string)m["mar_lat_fin"] ? true : false,
                        marker_obs = m["mar_obs"] == DBNull.Value ? "" : (string)m["mar_obs"],
                        marker_id_inv = m["mar_id_inv"] == DBNull.Value ? "" : (string)m["mar_id_inv"],
                        marker_dm_flag = m["mar_dm_fin"] == DBNull.Value ? true : (decimal)m["mar_dm_ini"] == (decimal)m["mar_dm_fin"] ? true : false,
                        marker_ele_nom = m["mar_ele_nom"] == DBNull.Value ? "" : (string)m["mar_ele_nom"],
                    }).FirstOrDefault();
                //-- /.TUPLA UNICA --

                if (marker != null && marker.marker_tra != null)
                {
                    // TITULO
                    str2.Append($"<i class='icon-pushpin mr-2'></i> {marker.marker_titulo.ToUpper()}");

                    // CARD IZQUIERDA
                    strInfoLeft.AppendFormat("<table class='table table-borderless table-xs border-top-0 my-2'>");
                    strInfoLeft.Append("<tbody>");

                    strInfoLeft.Append("<tr>");
                    strInfoLeft.Append("    <td class='text-uppercase font-weight-semibold py-0 pl-0 pr-2 border-0'>Nombre Elemento:</td>");
                    strInfoLeft.Append($"   <td class='text-left p-0 border-0'>{marker.marker_ele_nom}</td>");
                    strInfoLeft.Append("</tr>");

                    strInfoLeft.Append("<tr>");
                    strInfoLeft.Append("    <td class='text-uppercase font-weight-semibold py-0 pl-0 pr-2 border-0'>ID:</td>");
                    strInfoLeft.Append($"   <td class='text-left p-0 border-0'>{marker.marker_id}</td>");
                    strInfoLeft.Append("</tr>");

                    // MUESTRA DM INICIO Y FIN
                    if (marker.marker_dm_flag)
                    {
                        strInfoLeft.Append("<tr>");
                        strInfoLeft.Append("    <td class='text-uppercase font-weight-semibold py-0 pl-0 pr-2 border-0'>DM:</td>");
                        strInfoLeft.Append($"   <td class='text-left p-0 border-0'>{marker.marker_dm_ini}</td>");
                        strInfoLeft.Append("</tr>");
                    }
                    else
                    {
                        strInfoLeft.Append("<tr>");
                        strInfoLeft.Append("    <td class='text-uppercase font-weight-semibold py-0 pl-0 pr-2 border-0'>DM Inicio:</td>");
                        strInfoLeft.Append($"   <td class='text-left p-0 border-0'>{marker.marker_dm_ini}</td>");
                        strInfoLeft.Append("</tr>");
                        strInfoLeft.Append("<tr>");
                        strInfoLeft.Append("    <td class='text-uppercase font-weight-semibold py-0 pl-0 pr-2 border-0'>DM Fin:</td>");
                        strInfoLeft.Append($"   <td class='text-left p-0 border-0'>{marker.marker_dm_fin}</td>");
                        strInfoLeft.Append("</tr>");
                    }

                    strInfoLeft.Append("<tr>");
                    strInfoLeft.Append("    <td class='text-uppercase font-weight-semibold py-0 pl-0 pr-2 border-0'>Ruta:</td>");
                    strInfoLeft.Append($"   <td class='text-left p-0 border-0'>{marker.marker_ruta}</td>");
                    strInfoLeft.Append("</tr>");
                    strInfoLeft.Append("<tr>");
                    strInfoLeft.Append("    <td class='text-uppercase font-weight-semibold py-0 pl-0 pr-2 border-0'>Tramo:</td>");
                    strInfoLeft.Append($"   <td class='text-left p-0 border-0'>{marker.marker_tra}</td>");
                    strInfoLeft.Append("</tr>");

                    strInfoLeft.Append("<tr>");
                    strInfoLeft.Append("    <td class='text-uppercase font-weight-semibold py-0 pl-0 pr-2 border-0'>ID Mop:</td>");
                    strInfoLeft.Append($"   <td class='text-left p-0 border-0'>{marker.marker_id_mop}</td>");
                    strInfoLeft.Append("</tr>");
                    strInfoLeft.Append("<tr>");
                    strInfoLeft.Append("    <td class='text-uppercase font-weight-semibold py-0 pl-0 pr-2 border-0'>ID Inventario:</td>");
                    strInfoLeft.Append($"   <td class='text-left p-0 border-0'>{marker.marker_id_inv}</td>");
                    strInfoLeft.Append("</tr>");

                    // MUESTRA COORDENADAS DE UBICACION INICIO Y FIN
                    if (marker.marker_geo_ubicacion_flag)
                    {
                        strInfoLeft.Append("<tr>");
                        strInfoLeft.Append("    <td class='text-uppercase font-weight-semibold py-0 pl-0 pr-2 border-0'>Ubicación:</td>");
                        strInfoLeft.Append($"   <td class='text-left p-0 border-0'>{marker.marker_geo_ubicacion_ini}</td>");
                        strInfoLeft.Append("</tr>");
                    }
                    else
                    {
                        strInfoLeft.Append("<tr>");
                        strInfoLeft.Append("    <td class='text-uppercase font-weight-semibold py-0 pl-0 pr-2 border-0'>Coordenadas Inicio:</td>");
                        strInfoLeft.Append($"   <td class='text-left p-0 border-0'>{marker.marker_geo_ubicacion_ini}</td>");
                        strInfoLeft.Append("</tr>");
                        strInfoLeft.Append("<tr>");
                        strInfoLeft.Append("    <td class='text-uppercase font-weight-semibold py-0 pl-0 pr-2 border-0'>Coordenadas Fin:</td>");
                        strInfoLeft.Append($"   <td class='text-left p-0 border-0'>{marker.marker_geo_ubicacion_fin}</td>");
                        strInfoLeft.Append("</tr>");
                    }


                    strInfoLeft.Append("</tbody>");
                    strInfoLeft.Append("</table>");

                    strInfoRight.AppendFormat($"<table class='table table-borderless table-xs border-top-0 my-2'>");
                    strInfoRight.Append("   <tbody>");

                    // PASO 2) -  ELEMENTOS VARIABLES
                    var dt = dataSetSQL.dsSQL.Tables[0];

                    foreach (var dtRow in dt.Select())
                    {
                        var i = 0;
                        foreach (DataColumn dc in dt.Columns)
                        {
                            var dataColumnData = dtRow[dc].ToString();
                            var dataColumnName = dt.Columns[i].ColumnName;

                            var str = "";

                            if (dataColumnName.Length < 4)
                                str = dataColumnName.Substring(0, dataColumnName.Length);
                            else
                                str = dataColumnName.Substring(0, 4);

                            if (str != "mar_")
                            {
                                strInfoRight.Append("<tr>");
                                strInfoRight.Append($"   <td class='text-uppercase font-weight-semibold py-0 pl-0 pr-2 align-top border-0'>{dataColumnName}:</td>");
                                strInfoRight.Append($"   <td class='text-left py-0 border-0'>{dataColumnData}</td>");
                                strInfoRight.Append("</tr>");
                            }
                            i++;
                        }
                    }

                    strInfoRight.Append("   </tbody>");
                    strInfoRight.Append("</table>");

                    // PASO 3) -  TABLA DE MANTENCION
                    DateTime date = DateTime.Now.AddDays(365); // Adds 1 days to the date
                    string formattedDate = date.ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                    var startDate = DateTime.ParseExact($"01/01/2000 00:00:00", "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                    var endDate = DateTime.ParseExact(formattedDate, "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                    var dataSetSQL2 = planDeMantencionBusinessImpl.ListIngresoFiltered(User.Identity.Name, (short)1000, startDate, endDate);

                    if (dataSetSQL2.intError != 0)
                        throw new Exception(dataSetSQL.strError);

                    allTableMaintenanceType = dataSetSQL2.dsSQL.Tables[0].AsEnumerable()
                                                .Where(r => r.Field<Int32>("tra_tip_ele_id").Equals(Int32.Parse(idTipoMarker)) &&
                                                            r.Field<Int32>("tra_ele_id").Equals(Int32.Parse(idMarker)))
                                                .Select(r => new BusinessEntity.PlanDeMantencionBusinessEntity.PlanIngreso
                                                {
                                                    tra_id = r.Field<Int32>("tra_id"),
                                                    tra_tip_mant_id = r.Field<Int16>("tra_tip_mant_id"),
                                                    tra_act_id = r.Field<Int32>("tra_act_id"),
                                                    tra_tip_ele_id = r.Field<Int32>("tra_tip_ele_id"),
                                                    tra_ele_id = r.Field<Int32>("tra_ele_id"),
                                                    tra_fec_ing = r["tra_fec_ing"] == DBNull.Value ? (DateTime?)null : r.Field<DateTime>("tra_fec_ing"),
                                                    tra_res_id = r.Field<Int32>("tra_res_id"),
                                                    tra_can = r.Field<decimal>("tra_can"),
                                                    tra_vig = r.Field<bool>("tra_vig"),
                                                    tra_tip_act_nom = $"({r.Field<String>("tra_tip_act_code")}) - {r.Field<String>("tra_tip_act_nom")}",
                                                    tra_obs = r["tra_obs"] == DBNull.Value ? null : r.Field<string>("tra_obs"),
                                                    tra_tip_mant_nom = r.Field<String>("tra_tip_mant_nom"),
                                                    tra_uni_nom = r.Field<String>("tra_uni_nom"),
                                                    tra_res_nom = r.Field<String>("tra_res_nom"),
                                                    tra_ele_nom = r.Field<Int32>("tra_ele_id") > 0 ? $"{r.Field<String>("tra_ele_nom")} ({r.Field<Int32>("tra_ele_id")})" : string.Empty,
                                                    tra_tip_ele_nom = r.Field<string>("tra_tip_ele_nom")
                                                }).OrderByDescending(x => x.tra_fec_ing).ToList();

                    // PASO 4) -  TABLA DE IMAGENES
                    markerImgList = dataSetSQL.dsSQL.Tables[3].AsEnumerable()
                        .Select(m => new ElementoBusinessEntity.Imagen()
                        {
                            img_id = (int)m["img_id"],
                            img_tip_ele = (short)m["img_tip_ele"],
                            img_ele_id = (int)m["img_ele_id"],
                            img_nom = (string)m["img_nom"],
                            img_des = m["img_des"] == DBNull.Value ? "" : (string)m["img_des"],
                            img_pat = (string)m["img_pat"]
                        }).ToList();


                    if (markerImgList.Any())
                    {
                        numbers.Append("<div class='table-responsive'>");
                        numbers.Append("	<table class='table table-xs table-bordered' id='table_element_image'>");
                        numbers.Append("		<thead>");
                        numbers.Append("			<tr>");

                        foreach (var item in markerImgList)
                        { 
                                numbers.Append("				<th class='font-weight-bold'>"+ item.img_nom + "</th>");                    
                        }

                        numbers.Append("			</tr>");
                        numbers.Append("		</thead>");
                        numbers.Append("		<tbody>	");

                        foreach (var item2 in markerImgList)
                        {
                            numbers.Append($"<td class='text-center'><img id='{item2.img_id}' height='200' src='/content/img/elementos/{item2.img_pat}/{item2.img_nom}'></td>");
                        }

                        numbers.Append("		</tbody>");
                        numbers.Append("	</table>");
                        numbers.Append("</div>");

                    }
                }
                else
                {
                    str2.Append("<i class='icon-pushpin mr-3'></i> No Data Title</h5>");
                }

                return Json(
                    new
                    {
                        status = marker != null ? true : false,
                        MarkerLeft = strInfoLeft.ToString(),
                        MarkerRight = strInfoRight.ToString(),
                        titulo = str2.ToString(),
                        obs = marker.marker_obs,
                        his = string.Join("\n", markerHis.Select(x => x.ele_his)),
                        his_rows = markerHis == null ? 1 : markerHis.Count * 2,
                        img = !markerImgList.Any() ? "" : numbers.ToString(),
                        img_flag = markerImgList.Any(),
                        poligonoList = markerPoligonoList,
                        matenciones = allTableMaintenanceType,
                        exception = marker != null ? "" : "No se encontró informacion para el elemento seleccionado."
                    }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ViewData["mensaje"] = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
                ViewData["tipo"] = "error";

                return Json(
                    new
                    {
                        status = false,
                        exception = ex.InnerException == null ? ex.Message : ex.InnerException.Message
                    }, JsonRequestBehavior.AllowGet);
            }
        }

        public virtual JsonResult EditJSON_User_Preferences(string jsonData, bool isPostback, string iIcon, string iMap)
        {
            System.Threading.Thread.Sleep(2000);

            try
            {
                var js = new JavaScriptSerializer();
                var blogObject = js.Deserialize<Root>(jsonData);
                var itemComponentList = new List<ListChild>();
                var componentsTable = new System.Data.DataTable();
                var configrationTable = new System.Data.DataTable();

                // PASO 2) - REGISTROS INVALIDOS
                var childrenRows = from drFila in blogObject.Children.AsEnumerable()
                                   select new
                                   {
                                       Children = drFila.Children,
                                       Key = drFila.Key.ToString(),
                                       Expanded = drFila.Selected,
                                       Title = drFila.Title
                                   };

                foreach (var item in childrenRows)
                {
                    var cadena = item.Children;
                    var keyString = item.Key;

                    if (item.Children != null)
                    {
                        var listChild = from drChild in item.Children
                                        select new ListChild
                                        {
                                            Expanded = false,
                                            ChildKey = drChild.Key,
                                            Selected = drChild.Selected,
                                            Title = drChild.Title,
                                            RootChild = int.Parse(item.Key)
                                        };

                        itemComponentList.AddRange(listChild);
                    }
                }

                if (itemComponentList.Any())
                { 
                          var _ListItems = JsonConvert.SerializeObject(itemComponentList);      
                }


                // PASO 2) - REGISTROS INVALIDOS
                var componentList = from drFila in itemComponentList
                                    select CrosscuttingUtiles.Mapa.tableMenuCreateRow(new
                                    {
                                        user_lgn = User.Identity.Name,
                                        tramo_id = drFila.RootChild,
                                        menu_id = drFila.ChildKey,
                                        selected = drFila.Selected,
                                        title = drFila.Title
                                    });

                if (componentList.Any())
                {
                    componentsTable = componentList.CopyToDataTable<DataRow>();
                }
                else
                {
                    componentsTable = CrosscuttingUtiles.Mapa.tableMenuCreate();
                }


                // PASO 4) - CONFIGURACION
                // Create a List of objects  
                var itemConfigurationtList = new List<ConfiguracionEntity>
                {
                    new ConfiguracionEntity
                    {
                        cnf_cod = 0, cnf_tip_cod = 99, cnf_key = "TYPE_MAP", cnf_num_001 = (short) 0, cnf_str_001 = iMap
                    },
                    new ConfiguracionEntity
                    {
                        cnf_cod = 0, cnf_tip_cod = 99, cnf_key = "ICON_SIZE", cnf_num_001 = short.Parse(iIcon),
                        cnf_str_001 = ""
                    }
                };

                var ConfiguracionList = from drFila in itemConfigurationtList
                                        select CrosscuttingUtiles.Mapa.tableConfigCreateRow(new
                                        {
                                            cnf_cod = drFila.cnf_cod,
                                            cnf_tip_cod = drFila.cnf_tip_cod,
                                            cnf_key = drFila.cnf_key,
                                            cnf_nom = "",
                                            cnf_des = "",
                                            cnf_num_001 = drFila.cnf_num_001 != null ? drFila.cnf_num_001 : 0,
                                            cnf_str_001 = drFila.cnf_str_001 != null ? drFila.cnf_str_001 : "",
                                            cnf_cre_usr = drFila.cnf_cre_usr != null ? drFila.cnf_cre_usr : User.Identity.Name,
                                            cnf_cre_fec = drFila.cnf_cre_fec != null ? drFila.cnf_cre_fec : DateTime.Now,
                                            cnf_mod_usr = User.Identity.Name,
                                            cnf_mod_fec = DateTime.Now
                                        });

                if (ConfiguracionList.Any()) configrationTable = ConfiguracionList.CopyToDataTable<DataRow>();

                var request = mapBusinessImpl.UpdateUserPreferences(User.Identity.Name, configrationTable, componentsTable);

                if (request.intError != 0)
                    throw new Exception(request.strError);

                return Json(new { status = true, responseText = "Los parámetros se actualizaron satisfactoriamente." },
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(
                    new
                    {
                        status = false,
                        responseText = ex.InnerException == null ? ex.Message : ex.InnerException.Message
                    }, JsonRequestBehavior.AllowGet);
            }
        }

        public string fnc_getmarkerIcon(string iconSize, string curFile, string ext)
        {
            try
            {
                var relativePath = $"~/content/img/icons/{iconSize}/{curFile}_{iconSize}.{ext}";
                var absolutePath = HttpContext.Server.MapPath(relativePath);

                if (System.IO.File.Exists(absolutePath))
                    return $"/content/img/icons/{iconSize}/{curFile}_{iconSize}.{ext}";
                else
                    return $"/content/img/icons/default/flag_red_18x18.png";
            }
            catch
            {
                return $"/content/img/icons/default/flag_red_18x18.png";
            }
        }

        /// <summary>
        /// Método que retorna la ruta de una imagen
        /// </summary>
        public string fnc_getmarkerImagePreview(string ext)
        {
            try
            {
                if (string.IsNullOrEmpty(ext))
                    throw new Exception();

                var relativePath = $"~/content/img/elementos/{ext}";
                var absolutePath = HttpContext.Server.MapPath(relativePath);

                if (System.IO.File.Exists(absolutePath))
                    return $"/content/img/elementos/{ext}";
                else
                    return $"/content/img/miscelaneous/img_no_disp.png";
            }
            catch
            {
                return $"/content/img/miscelaneous/img_no_disp.png";
            }
        }
    }
}