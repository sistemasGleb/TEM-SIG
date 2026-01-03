
using System.Globalization;
using BusinessEntity;
using BusinessImpl;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.IO;

namespace WebApplication.Areas.Configuracion.Controllers
{
    public class ElementosController : WebApplication.Controllers.BaseController
    {
        private readonly ElementosBusinessImpl elementosBusinessImpl = new ElementosBusinessImpl();
        private readonly ReportBusinessImpl reportBusinessImpl = new ReportBusinessImpl();
        private readonly LogBusinessImpl _logBusinessImpl = new LogBusinessImpl();
        private readonly ConfiguracionBusinessImpl configuracionBusinessImpl = new ConfiguracionBusinessImpl();

        /// <summary>
        /// método que devuelve listado de Tipos de Elementos Seleccionables Para edicion
        /// </summary>
        /// <returns>El método devuelve una lista</returns>
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

                BusinessEntity.FormModels.FormElementos collection = new BusinessEntity.FormModels.FormElementos
                {
                    elementList = new List<SelectListItem> { new SelectListItem { Text = "Seleccione ...", Value = "0" } }
                };

                // sp_app_extrae_elemento_detalle
                var dataSetSQL = elementosBusinessImpl.AllInfoElement(User.Identity.Name, 0, 0);

                if (dataSetSQL.intError != 0)
                    throw new Exception(dataSetSQL.strError);

                var allElementTypeList = dataSetSQL.dsSQL.Tables[0].AsEnumerable()
                    //.Where(x => (bool)x["ele_vig"] )
                    .Select(m => new SelectListItem
                    {
                        Value = ((short)m["ele_id"]).ToString(),
                        Text = $"{(string)m["ele_des"]} ({((short)m["ele_id"]).ToString()})",
                        Selected = false,
                        Disabled = !(bool)m["ele_edt_vig"]
                    }).OrderBy(x => x.Disabled).ThenBy(x => x.Text).ToList();

                collection.elementList.AddRange(allElementTypeList);

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
        [HttpGet]
        public ActionResult Listar(short id_tipo_elemento)
        {
            BusinessEntity.FormModels.FormElementos collectionSetup = new BusinessEntity.FormModels.FormElementos();

            try
            {
                /// sp_app_extrae_reportes
                var dataSetSQL = reportBusinessImpl.get_Report(User.Identity.Name, id_tipo_elemento, 0);

                if (dataSetSQL.intError != 0)
                    throw new Exception(dataSetSQL.strError);

                var elemento = dataSetSQL.dsSQL.Tables[0].AsEnumerable()
                    .Select(m => new BusinessEntity.FormModels.FormElementos()
                    {
                        id_tipo_elemento = (Int16)m["tel_ele_id"],
                        nombre_tipo_elemento = (string)m["tel_ele_nom"]
                    }).FirstOrDefault();

                DataColumn Col = dataSetSQL.dsSQL.Tables[1].Columns.Add("[+]", System.Type.GetType("System.String"));
                Col.SetOrdinal(0);// to put the column in position 0;

                var icant = dataSetSQL.dsSQL.Tables[1].Columns.Count;

                DataColumn Col2 = dataSetSQL.dsSQL.Tables[1].Columns.Add(" ", System.Type.GetType("System.String"));
                Col2.SetOrdinal(icant);// to put the column in position 0;

                collectionSetup.ElementosDataTable = dataSetSQL.dsSQL.Tables[1].Copy();
                collectionSetup.id_tipo_elemento = elemento.id_tipo_elemento;
                collectionSetup.nombre_tipo_elemento = elemento.nombre_tipo_elemento;

                if (TempData["mensaje"] != null)
                {
                    ViewData["mensaje"] = TempData["mensaje"];
                    ViewData["tipo"] = TempData["tipo"];
                    TempData["mensaje"] = null;
                }
            }
            catch (Exception ex)
            {
                ViewData["mensaje"] = ex.InnerException is null ? ex.Message : ex.InnerException.Message;
                ViewData["tipo"] = "error";

                collectionSetup.ElementosDataTable = null;
                collectionSetup.id_tipo_elemento = id_tipo_elemento;
                collectionSetup.nombre_elemento = string.Empty;
            }

            string output = JsonConvert.SerializeObject(collectionSetup);

            return View(collectionSetup);
        }

        [HttpPost]
        [Authorize]
        public virtual JsonResult ListarJSON_MarkerHistory(string idTipoMarker, string idMarker)
        {
            System.Threading.Thread.Sleep(1000);
            List<BusinessEntity.PlanDeMantencionBusinessEntity.PlanIngreso> allTableMaintenanceList = new List<BusinessEntity.PlanDeMantencionBusinessEntity.PlanIngreso>();

            try
            {
                // PASO 3) -  TABLA DE MANTENCION
                DateTime date = DateTime.Now.AddDays(365); // Adds 1 days to the date
                string formattedDate = date.ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                var startDate = DateTime.ParseExact($"01/01/2000 00:00:00", "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                var endDate = DateTime.ParseExact(formattedDate, "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                allTableMaintenanceList = PlanDeMantencion.Controllers.PlanMantencionController.fnc_get_all_maitenance(User.Identity.Name, 1000, Int32.Parse(idTipoMarker), Int32.Parse(idMarker), startDate, endDate);

                return Json(new
                {
                    status = allTableMaintenanceList.Any() ? true : false,
                    list = allTableMaintenanceList
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

        #region ADMINISTRACION  - EDITAR ELEMENTO
        [Authorize]
        [HttpGet]
        public ActionResult Editar(string id_elemento, string id_tipo_elemento)
        {
            var myNullDate = DateTime.ParseExact("1900-01-01 00:00:00", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            BusinessEntity.ParametrosBusinessEntity collection = new BusinessEntity.ParametrosBusinessEntity
            {
                allTramoList = new List<SelectListItem> { new SelectListItem { Text = "Seleccione Opción...", Value = "0", Selected = false } },
                AllLadoList = new List<SelectListItem> { new SelectListItem { Text = "Seleccione Opción...", Value = "0", Selected = false } },
                AllMacroubicacionList = new List<SelectListItem> { new SelectListItem { Text = "Seleccione Opción...", Value = "0", Selected = false } }
            };

            try
            {
                // PASO 1) - Valida carpeta del Tipo de elemento
                var strImageElementDirectory = CrosscuttingUtiles.Archivos.fnc_FormatImageElementDirectory(4, id_tipo_elemento);

                // PASO 2) - Valida directorio 
                var relativePath = $"~/Content/img/elementos/{strImageElementDirectory}/";
                var absolutePath = HttpContext.Server.MapPath(relativePath);

                if (String.IsNullOrEmpty(id_elemento) || String.IsNullOrEmpty(id_tipo_elemento))
                    throw new Exception("Error al enviar parámetros del elemento seleccionado.");

                var dataSetSQL = elementosBusinessImpl.AllInfoElement(User.Identity.Name, Int16.Parse(id_tipo_elemento), Int32.Parse(id_elemento));

                if (dataSetSQL.intError != 0)
                    throw new Exception(dataSetSQL.strError);

                // PASO 2) - ELEMENTO
                collection = dataSetSQL.dsSQL.Tables[1].AsEnumerable()
                    .Select(m => new ParametrosBusinessEntity
                    {
                        USUARIO = User.Identity.Name,
                        Param_sp_crud = "sp_editar_agregar_elemento",
                        // TUPLA FIJA 
                        Param_id = (int)m["Id"],
                        Param_id_int = null,//m["Id Interno"] == DBNull.Value ? null : (string)m["Id Interno"],
                        Param_id_mop = m["Id Mop"] == DBNull.Value ? null : (string)m["Id Mop"],
                        Param_tip_ele = (byte)m["Id Elemento"],
                        Param_ele_nom = m["Nombre Elemento"] == DBNull.Value ? null : (string)m["Nombre Elemento"],
                        Param_id_inv = 0,
                        Param_dm_ini = m["Dm Inicio"] == DBNull.Value ? (decimal)0 : (decimal)m["Dm Inicio"],
                        Param_dm_fin = m["Dm Fin"] == DBNull.Value ? (decimal?)null : (decimal)m["Dm Fin"],
                        Param_crd_lat_ini = m["Latitud Inicio"] == DBNull.Value ? null : ((string)m["Latitud Inicio"]).Trim(),
                        Param_crd_lon_ini = m["Longitud Inicio"] == DBNull.Value ? null : ((string)m["Longitud Inicio"]).Trim(),
                        Param_crd_lat_fin = m["Latitud Fin"] == DBNull.Value ? null : ((string)m["Latitud Fin"]).Trim(),
                        Param_crd_lon_fin = m["Longitud Fin"] == DBNull.Value ? null : ((string)m["Longitud Fin"]).Trim(),
                        Param_vig = (string)m["Vigente"] == "SI" ? true : false,
                        Param_obs = m["Dato Adicional"] == DBNull.Value ? "" : ((string)m["Dato Adicional"]).Trim(),

                        AllLadoList = fnc_get_selected_item("LADO", (m["Lado"] == DBNull.Value ? (Int16?)0 : (Int16)m["Lado"]), true, dataSetSQL.dsSQL.Tables[2]),
                        Param_lad = m["Lado"] == DBNull.Value ? (Int16)0 : (Int16)m["Lado"],

                        AllMacroubicacionList = fnc_get_selected_item("MACROUBICACION", m["Id Macroubicacion"] == DBNull.Value ? (Int16)0 : (Int16)m["Id Macroubicacion"], true, dataSetSQL.dsSQL.Tables[2]),
                        Param_Mac = m["Id Macroubicacion"] == DBNull.Value ? (Int16)0 : (Int16)m["Id Macroubicacion"],

                        // /.--- TUPLA AVRIABLE---
                        allTramoList = fnc_get_selected_item("TRAMO", (m["Id Tramo"] == DBNull.Value ? (Int16?)0 : (Int16)m["Id Tramo"]), true, dataSetSQL.dsSQL.Tables[2]),
                        Param_tra_id = m["Id Tramo"] == DBNull.Value ? (Int16)0 : (Int16)m["Id Tramo"],
                        Param_tip_ele_nom = m["Nombre Tipo"] == DBNull.Value ? "-" : (string)m["Nombre Tipo"],

                        allBitacoraList = new List<Bitacora>(),
                        allImagenesList = fnc_get_image_list(User.Identity.Name, dataSetSQL.dsSQL.Tables[4], absolutePath)
                    }).FirstOrDefault();

                if (TempData["mensaje"] != null)
                {
                    ViewData["mensaje"] = TempData["mensaje"];
                    ViewData["tipo"] = TempData["tipo"];
                    TempData["mensaje"] = null;
                }

            }
            catch (Exception ex)
            {
                ViewData["mensaje"] = ex.InnerException is null ? ex.Message : ex.InnerException.Message;
                ViewData["tipo"] = "error";
            }

            return View(collection);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Editar(BusinessEntity.ParametrosBusinessEntity collection)
        {
            try
            {

                var dataSetSQL = elementosBusinessImpl.CreateEditElements(User.Identity.Name, collection);

                if (dataSetSQL.intError != 0)
                    throw new Exception(dataSetSQL.strError);

                TempData["mensaje"] = $"El elemento ({collection.Param_ele_nom.ToUpper()}) se ha actualizado satisfactoriamente.";
                TempData["tipo"] = "ok";

                return RedirectToAction("Listar", new { id_tipo_elemento = collection.Param_tip_ele });

            }
            catch (Exception ex)
            {
                TempData["mensaje"] = ex.InnerException is null ? ex.Message : ex.InnerException.Message;
                TempData["tipo"] = "error";

                return RedirectToAction("Listar", new { id_tipo_elemento = collection.Param_tip_ele });
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult DeleteImage(string id_tipo_elemento, string id_elemento, string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id_tipo_elemento) || string.IsNullOrEmpty(id_elemento) || string.IsNullOrEmpty(id))
                    throw new Exception("Parámetros incorrectos.");

                // PASO 1) -  Elimina desde la BD
                var result = elementosBusinessImpl.DeleteElementImage(Int32.Parse(id), User.Identity.Name);

                if (result.intError != 0)
                    throw new Exception(result.strError);

                TempData["mensaje"] = $"La imagen se ha eliminado satisfactoriamente.";
                TempData["tipo"] = "ok";
                return RedirectToAction("Editar", new { id_elemento = id_elemento, id_tipo_elemento = id_tipo_elemento });
            }
            catch (Exception ex)
            {
                TempData["mensaje"] = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
                TempData["tipo"] = "error";
                return RedirectToAction("Editar", new { id_elemento = id_elemento, id_tipo_elemento = id_tipo_elemento });
            }
        }

        public ActionResult AgregarImagen(string id_tipo_elemento, string id_elemento, string nombre_elemento)
        {
            BusinessEntity.FormModels.FormElementos model = new BusinessEntity.FormModels.FormElementos();
            try
            {
                var dataSet = configuracionBusinessImpl.ListAll(User.Identity.Name, string.Empty);

                if (dataSet.intError != 0)
                    throw new Exception(dataSet.strError);

                model.id_tipo_elemento = (short)(int.TryParse(id_tipo_elemento, out int intOutParameter) ? intOutParameter : -1);
                model.id_elemento = int.TryParse(id_elemento, out int intOutParameter2) ? intOutParameter2 : -1;
                model.nombre_elemento = nombre_elemento;
                model.uploadImageConfig = CrosscuttingUtiles.Archivos.fnc_ImageUploadDocumentConfig("", dataSet.dsSQL.Tables[0]);
            }
            catch
            {

            }

            return View("SubirImagen", model);
        }

        public ActionResult CargarImagenJSON(string id_tipo_elemento, string id_elemento, string strObservacion)
        {
            //var blUpload = false;
            var str = new System.Text.StringBuilder();
            var uploadFile = new UploadFile();

            DataTable dt = new DataTable();
            dt.Clear();
            dt.Columns.Add("img_tip_ele", typeof(int));
            dt.Columns.Add("img_ele_id", typeof(int));
            dt.Columns.Add("img_nom", typeof(string));
            dt.Columns.Add("img_des", typeof(string));
            dt.Columns.Add("img_pat", typeof(string));
            dt.Columns.Add("img_siz", typeof(decimal));
            dt.Columns.Add("img_ext", typeof(string));

            try
            {
                if (Request.Files.Count == 0)
                    return Json(new { error = $"No hay archivos seleccionados." });

                if (string.IsNullOrEmpty(id_tipo_elemento))
                    return Json(new { error = $"Tipo de elemento incorrecto" });


                // PASO 1) - Valida carpeta del Tipo de elemento
                var strImageElementDirectory = CrosscuttingUtiles.Archivos.fnc_FormatImageElementDirectory(4, id_tipo_elemento);

                // PASO 2) - Valida directorio 
                var relativePath = $"~/Content/img/elementos/{strImageElementDirectory}";
                var absolutePath = HttpContext.Server.MapPath(relativePath);

                if (!System.IO.File.Exists(absolutePath))
                    Directory.CreateDirectory(absolutePath);


                //  Get all files from Request object  
                var files = Request.Files;

                for (var i = 0; i < files.Count; i++)
                {
                    var file = files[i];
                    var extension = System.IO.Path.GetExtension(file.FileName);
                    var size = file.ContentLength;

                    uploadFile.fileName = $"{strImageElementDirectory}_{CrosscuttingUtiles.Archivos.fnc_FormatImageElementDirectory(8, id_elemento)}_{DateTime.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture)}{extension}";

                    // Get the complete folder path and store the file inside it.  
                    var fullFileName = Path.Combine(absolutePath, uploadFile.fileName);
                    file.SaveAs(fullFileName);


                    object[] o = { id_tipo_elemento, id_elemento, uploadFile.fileName, strObservacion, strImageElementDirectory, size, extension, };
                    dt.Rows.Add(o);
                }

                // PASO 3) - Valida directorio 
                var dataSetSQL = elementosBusinessImpl.AddImageElements(User.Identity.Name, dt);

                if (dataSetSQL.intError != 0)
                    throw new Exception(dataSetSQL.strError);


                uploadFile.error = "";
                uploadFile.initialPreview = new List<string>();
                uploadFile.initialPreviewConfig = new List<UploadFilePreviewConfig>();
                uploadFile.initialPreviewAsData = true;
                uploadFile.message = $"La imagen (<b>{uploadFile.fileName.ToUpper()}</b>) se ha cargado satisfactoriamente.";   // Returns message that successfully uploaded  

                return Json(uploadFile);
            }
            catch (Exception ex)
            {
                uploadFile.message = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
                return Json(new { error = (ex.InnerException == null ? ex.Message : ex.InnerException.Message) });
            }
        }

        #endregion

        [HttpPost]
        [Authorize]
        public virtual JsonResult ListarJSON_ElementList(string idTipoMarker)
        {
            List<SelectListItem> allElementByTypeList = new List<SelectListItem>();
            try
            {

                return Json(new
                {
                    status = allElementByTypeList.Any() ? true : false,
                    list = allElementByTypeList
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new { status = false, exception = ex.InnerException == null ? ex.Message : ex.InnerException.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        #region FUNCIONES Y DELEGADOS

        public List<ElementoBusinessEntity> get_ElementosByType(string idTipoMarker)
        {
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

                return new List<ElementoBusinessEntity>();
            }
            catch
            {
                return new List<ElementoBusinessEntity>();
            }

        }



        /// <summary>
        /// Función que devuelve lista de imagenes de un elemento
        /// </summary>
        public static Func<string, DataTable, string, List<ElementoImagenBusinessEntity>> fnc_get_image_list = (item, dt, absolutePath) =>
        {
            List<ElementoImagenBusinessEntity> list = new List<ElementoImagenBusinessEntity>();

            try
            {
                var listFiltered = dt.AsEnumerable()
                     .Select(m => new ElementoImagenBusinessEntity
                     {
                         ele_img_id = (Int32)m["img_id"],
                         ele_img_tip_ele = (Int16)m["img_tip_ele"],
                         ele_img_ele_id = (Int32)m["img_ele_id"],
                         ele_img_nom = m["img_nom"] == DBNull.Value ? null : (string)m["img_nom"],
                         ele_img_des = m["img_des"] == DBNull.Value ? null : (string)m["img_des"],
                         ele_img_pat = m["img_pat"] == DBNull.Value ? null : (string)m["img_pat"],
                         ele_img_vig = m["img_vig"] == DBNull.Value ? false : (bool)m["img_vig"],
                         ele_img_cre_usr = m["img_cre_usr"] == DBNull.Value ? null : (string)m["img_cre_usr"],
                         ele_img_cre_usr_nom = m["img_cre_usr_nom"] == DBNull.Value ? null : (string)m["img_cre_usr_nom"],
                         ele_img_cre_fec = m["img_cre_fec"] == DBNull.Value ? (DateTime?)null : (DateTime)m["img_cre_fec"],
                         ele_img_mod_usr = m["img_mod_usr"] == DBNull.Value ? null : (string)m["img_mod_usr"],
                         ele_img_mod_usr_nom = m["img_mod_usr_nom"] == DBNull.Value ? null : (string)m["img_mod_usr_nom"],
                         ele_img_mod_fec = m["img_mod_fec"] == DBNull.Value ? (DateTime?)null : (DateTime)m["img_mod_fec"],
                         ele_img_siz = m["img_siz"] == DBNull.Value ? (decimal)0 : (decimal)m["img_siz"],
                         ele_img_siz_txt = CrosscuttingUtiles.Helpers.fnc_FormatFileSize((m["img_siz"] == DBNull.Value ? (decimal)0 : (decimal)m["img_siz"])),
                         ele_img_ext = m["img_ext"] == DBNull.Value ? null : (string)m["img_ext"],
                         ele_img_xts = m["img_pat"] == DBNull.Value ? false :
                                      m["img_nom"] == DBNull.Value ? false :
                                      CrosscuttingUtiles.Archivos.fnc_FileExists($"{absolutePath}", (string)m["img_nom"])

                     }).ToList();

                list.AddRange(listFiltered);

            }
            catch
            {
                list = new List<ElementoImagenBusinessEntity>();
            }

            return list;
        };

        ///fnc_get_selected_item
        public static Func<string, Int16?, bool, DataTable, List<SelectListItem>> fnc_get_selected_item = (item, selectItem, SelectOption, dtConfiguracion) =>
            {
                List<SelectListItem> list = new List<SelectListItem>();

                if (SelectOption)
                    list = new List<SelectListItem> { new SelectListItem { Text = "Seleccione Opción...", Value = "0", Selected = false } };

                try
                {
                    var listFiltered = dtConfiguracion.AsEnumerable()
                       .Where(x => x.Field<string>("par_tbl").Equals(item, StringComparison.OrdinalIgnoreCase))
                       .Select(m => new SelectListItem
                       {
                           Value = ((Int16)m["par_obj_id"]).ToString(),
                           Text = ((string)m["par_gls"]).ToUpper(),
                           Selected = selectItem != null && selectItem == (Int16)m["par_obj_id"]
                       }).ToList();

                    list.AddRange(listFiltered);
                }
                catch
                {
                    list = new List<SelectListItem> { new SelectListItem { Text = "Seleccione Opción...", Value = "0", Selected = false } };
                }

                return list.OrderBy(x => Convert.ToInt32(x.Value)).ToList();
            };

        #endregion

    }
}
