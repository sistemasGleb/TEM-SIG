using BusinessEntity;
using BusinessEntity.FormModels;
using BusinessImpl;
using ClosedXML.Excel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using WebApplication.Controllers;

namespace WebApplication.Areas.Reportes.Controllers
{
    public class InformesController : BaseController
    {
        private readonly ReportBusinessImpl reportBusinessImpl = new ReportBusinessImpl();
        private readonly MenuBusinessImpl menuBusinessImpl = new MenuBusinessImpl();
        private readonly MapBusinessImpl mapBusinessImpl = new MapBusinessImpl();
        private readonly LogBusinessImpl logBusinessImpl = new LogBusinessImpl();

        [Authorize]
        public ActionResult Index()
        {
            #region LOG
            logBusinessImpl._AddNewLog(new LogBusinessEntity()
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

            FormReportes formReportes = new FormReportes
            {
                allReportList = new List<SelectListItem>(),
                AllTramoList = new List<SelectListItem>(),
                AllTipoElementoList = new List<SelectListItem>(),
                AllElementoList = new List<SelectListItem>()

            };

            var dataSetSQL = reportBusinessImpl.get_Report(User.Identity.Name, 0, 0);

            if (dataSetSQL.intError != 0)
                throw new Exception("No se pudieron listar reportes.");

            // Listado de tipos de reportes habilitados
            formReportes.allReportList = dataSetSQL.dsSQL.Tables[0].AsEnumerable()
                .Where(m => (bool)m["ele_dis"] == false)
                .Select(m => new SelectListItem
                {
                    Text = (string)m["tel_ele_nom"],
                    Value = ((short)m["tel_ele_id"]).ToString(),
                    Selected = false,
                    Disabled = (bool)m["ele_dis"],
                })
                .OrderBy(x => x.Text).ToList();

            // Listado de Tramos
            formReportes.AllTramoList = dataSetSQL.dsSQL.Tables[2].AsEnumerable()
                .Where(x => (string)x["par_tbl"] == "TRAMO" && (bool)x["par_vig"])
                .Select(m => new SelectListItem
                {
                    Text = (string)m["par_gls"],
                    Value = ((short)m["par_obj_id"]).ToString(),
                    Selected = false
                })
                .OrderBy(x => short.Parse(x.Value)).ToList();

            return View(formReportes);
        }

        #region INFORME 1 - INVENTARIO
        [HttpPost]
        [Authorize]
        public ActionResult ListarJSON_Reportes()
        {
            try
            {
                var dataSetSQL = reportBusinessImpl.get_Report(User.Identity.Name, 0, 0);

                if (dataSetSQL.intError != 0)
                    throw new Exception("No se pudieron listar reportes.");

                // Listado de tipos de reportes habilitados
                var allreportsList = dataSetSQL.dsSQL.Tables[0].AsEnumerable()
                    .Where(m => (bool)m["ele_dis"] == false)
                    .Select(m => new SelectListItem
                    {
                        Text = (string)m["tel_ele_nom"],
                        Value = ((short)m["tel_ele_id"]).ToString(),
                        Selected = false,
                        Disabled = (bool)m["ele_dis"],
                    })
                    .OrderBy(x => x.Text).ToList();

                // Listado de Tramos
                var allTramosList = dataSetSQL.dsSQL.Tables[2].AsEnumerable()
                    .Where(x => (string)x["par_tbl"] == "TRAMO" && (bool)x["par_vig"])
                    .Select(m => new SelectListItem
                    {
                        Text = (string)m["par_gls"],
                        Value = ((short)m["par_obj_id"]).ToString(),
                        Selected = false
                    })
                    .OrderBy(x => short.Parse(x.Value)).ToList();

                return Json(new { status = allreportsList.Any(), List = allreportsList, AllTramosList = allTramosList });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = false,
                    List = "[]",
                    message = ex.InnerException == null ? ex.Message : ex.InnerException.Message
                });
            }

        }


        [HttpPost]
        [Authorize]
        public ActionResult JSON_GenerarReporte1(FormReportes.Root collection)
        {
            try
            {
                if (collection.tipo_elementos is null)
                    throw new Exception("Debe seleccionar al menos un reporte.");

                Thread.Sleep(1000);

                var fileExport = new
                {
                    guardarArchivo = true,
                    fileName = $"Rpt_Inventario_{DateTime.UtcNow.ToString("yyyy-MM-dd_HHmmssfff")}.xlsx",
                    Directorios = ConfigurationManager.AppSettings["DirectorioInformes"].ToString()
                };

                var wbListado = new XLWorkbook();

                var allreportsList = collection.tipo_elementos.Select(Int16.Parse).ToList();

                if (allreportsList.Count > 0)
                {
                    foreach (var items in allreportsList)
                    {
                        var intFila = 2;

                        var dataSetSQL = reportBusinessImpl.get_Report(User.Identity.Name, items, 0);

                        if (dataSetSQL.intError != 0)
                            throw new Exception(dataSetSQL.strError);

                        var dt = dataSetSQL.dsSQL.Tables[1].Copy();

                        // DELETE COLS 
                        string[] ColumnsToBeDeleted = { "Historial", "Descripcion", "Id", "Bitácora De Mantención" };

                        foreach (string ColName in ColumnsToBeDeleted)
                        {
                            if (dt.Columns.Contains(ColName))
                                dt.Columns.Remove(ColName);
                        }

                        var worksheetsName = dataSetSQL.dsSQL.Tables[0].AsEnumerable()
                            .Select(m => (string)m["tel_ele_nom"]).DefaultIfEmpty($"Hoja{items}").FirstOrDefault();

                        var worksheet = wbListado.Worksheets.Add(worksheetsName.Length > 30 ? worksheetsName.Substring(0, 30) : worksheetsName);
                        worksheet.PageSetup.AdjustTo(25);
                        worksheet.Style.Font.FontName = "Calibri";
                        worksheet.Style.Font.FontSize = 10;

                        for (var y = 0; y < dt.Columns.Count; y++)
                        {
                            var colName = dt.Columns[y].ColumnName;

                            worksheet.Cell(1, y + 1).SetValue(colName.ToUpper()).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Font.SetBold(true);
                            worksheet.Cell(1, y + 1).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                        }

                        //PASO 2) - LOAD DATA
                        for (var i = 0; i < dt.Rows.Count; i++)
                        {
                            for (var X = 0; X < dt.Columns.Count; X++)
                            {
                                var nom = dt.Columns[X].ColumnName;

                                String originalString = dt.Rows[i][X].ToString();
                                string pattern = "^(\r\n)*";
                                System.Text.RegularExpressions.Regex rgx = new System.Text.RegularExpressions.Regex(pattern);


                                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("\r\n");
                                var newText = regex.Replace(originalString, string.Empty, 1);

                                worksheet.Cell(intFila, X + 1).SetValue(newText);
                            }

                            intFila++;
                        }

                        var startCell = worksheet.Cell(1, 1);
                        var endCell = worksheet.Cell(dt.Rows.Count + 1, dt.Columns.Count);                //For 2d data, with 'n' no. of rows and columns.

                        worksheet.Range(startCell, endCell).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                        worksheet.Range(startCell, endCell).Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
                        worksheet.Range(1, 1, 1, dt.Columns.Count).Style.Fill.SetBackgroundColor(XLColor.Yellow);
                        worksheet.Columns().AdjustToContents();
                    }
                }


                if (fileExport.guardarArchivo)
                    wbListado.SaveAs(fileExport.Directorios + fileExport.fileName);

                var stream = new MemoryStream();
                wbListado.SaveAs(stream);

                var formModalExportar = new
                {
                    fileContents =
                        Convert.ToBase64String(stream.ToArray()), // Encode the byte array using Base64 encoding
                    contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileDownloadName = fileExport.fileName
                };

                return Json(new { status = true, formModalExportar, responseText = string.Empty });

            }
            catch (Exception ex)
            {
                return Json(new { status = false, responseText = $"Exception: {(ex.InnerException == null ? ex.Message : ex.InnerException.Message)}" });
            }

        }
        #endregion

        #region  INFORME 2 - HISTORIAL DE LOS ELEMENTOS
        public ActionResult ListarJSON_Reportes_Filtros(FormReportes.Root collection)
        {
            try
            {

                switch (collection.switch_case)
                {
                    case "TRAMO":
                        // code block
                        break;
                    case "TIPO":
                        // code block
                        if (collection.tramos == null)
                            return Json(new { status = false, List = new SelectListItem() });

                        var dtTramo = ConvertToDataTable(collection.tramos.Where(x => x != null)
                            .Select(x => new
                            {
                                tra_cod = Int32.Parse(x.ToString())
                            }).ToList());

                        DataTable dtTipo = new DataTable();
                        dtTipo.Columns.Add("tip_ele_id", typeof(Int16));

                        DataTable dtElemento = new DataTable();
                        dtElemento.Columns.Add("ate_id", typeof(Int16));

                        var dataSetSQL = reportBusinessImpl.get_Report_Filter(User.Identity.Name, dtTramo, dtTipo, dtElemento);

                        if (dataSetSQL.intError != 0)
                            throw new Exception(dataSetSQL.strError);

                        // LLENA CON CATEGORIA
                        List<SelectListItem> listado_estado_gestion = new List<SelectListItem>();

                        SelectListGroup estadoGestion = new SelectListGroup();
                        String estado_gestion = "";

                        foreach (DataRow drFila in dataSetSQL.dsSQL.Tables[1].Rows)
                        {
                            if (dataSetSQL.dsSQL.Tables[1].Columns.Contains("men_tra_nom"))
                            {
                                if (estado_gestion != (string)drFila["men_tra_nom"])
                                {
                                    estadoGestion = new SelectListGroup() { Disabled = false, Name = (string)drFila["men_tra_nom"] };
                                    estado_gestion = (string)drFila["men_tra_nom"];
                                }
                            }

                            listado_estado_gestion.Add(new SelectListItem()
                            {
                                Text = (string)drFila["men_ele_tip_nom"],
                                Value = ((Int16)drFila["men_ele_tip_id"]).ToString(),
                                Group = (dataSetSQL.dsSQL.Tables[1].Columns.Contains("men_tra_nom") ? estadoGestion : null)
                            });
                        };
                        // .LLENA CON CATEGORIA
                        var OrderedList = listado_estado_gestion.OrderBy(x => x.Group.Name).ThenBy(x => x.Text);

                        string output = JsonConvert.SerializeObject(OrderedList);

                        return Json(new
                        {
                            status = listado_estado_gestion.Any(),
                            List = listado_estado_gestion.OrderBy(x => x.Group.Name).ThenBy(x => x.Text),
                            message = listado_estado_gestion.Any() ? "" : "No se encontraron Elementos para los filtros seleccionados"
                        });

                    case "ELEMENTO":
                        // code block
                        break;
                    default:
                        // code block
                        return Json(new { status = false, List = "[]", message = "Opción no válida." });
                }

                return Json(new { status = true });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = false,
                    List = "[]",
                    message = ex.InnerException == null ? ex.Message : ex.InnerException.Message
                });
            }

        }

        public ActionResult JSON_GenerarReporte2(FormReportes.Root collection)
        {
            var iIcon = string.Empty;
            List<BusinessEntity.PlanDeMantencionBusinessEntity.PlanIngreso> allObservationList = new List<BusinessEntity.PlanDeMantencionBusinessEntity.PlanIngreso>();
            var intFila = 3;

            try
            {
                Thread.Sleep(1000);

                // PASO 1) -  CONFIGURACION DEL REPORTE
                var fileExport = new
                {
                    guardarArchivo = true,
                    fileName = $"Rpt_Historial_Elementos_{DateTime.UtcNow.ToString("yyyy-MM-dd_HHmmssfff")}.xlsx",
                    Directorios = ConfigurationManager.AppSettings["DirectorioInformes"].ToString()
                };


                // PASO 2)-  LISTA ELEMENTOS ACTIVOS DESDE EL MENU DEL USUARIO
                var dataSetSQL0 = menuBusinessImpl.fncDevolverMenu(User.Identity.Name);


                var allreportsList = collection.tipo_elementos.Select(Int16.Parse).ToList();

                var listSubMenus = dataSetSQL0.dsSQL.Tables[1].AsEnumerable()
                    .Where(m => (bool)m["men_tra_vig"] && (bool)m["men_ele_vig"] && (bool)m["men_ele_opc_vis"] && allreportsList.Contains((short)m["men_id"]))
                    .Select(m => new
                    {
                        //menu_cod = ((short)m["men_id"]).ToString(),
                        //menu_tra = (short)m["men_tra_cod"],
                        //menu_tra_nom = (string)m["men_tra_nom"],
                        //menu_nom = (string)m["men_ele_nom"],
                        //menu_sel = (bool)m["men_ele_sel_chk"],
                        menu_row = (short)m["men_id"]
                    }).OrderBy(x => x.menu_row).ToList();

                if (listSubMenus.Count == 0)
                    throw new Exception("No hay elementos seleccionados en menu de elementos.");

                var _List = JsonConvert.SerializeObject(listSubMenus);


                // Lista de marcadores seleccionados
                var filteredMarkersChecked = listSubMenus.Select(x => (int)x.menu_row).ToList();

                // PASO 3) -
                var dataSetSQL1 = mapBusinessImpl.GetAllMarkers(User.Identity.Name, filteredMarkersChecked);

                if (dataSetSQL1.intError != 0)
                    throw new Exception(dataSetSQL1.strError);

                var allTramoList = dataSetSQL1.dsSQL.Tables[4].AsEnumerable()
                    .Select(m => new
                    {
                        tra_id = (Int16)m["tra_id"],
                        tra_cod = (Int32)m["tra_cod"],
                        tra_nom = (string)m["tra_nom"],
                        tra_des = (string)m["tra_des"]
                    }).ToList();

                var allMarkerList = dataSetSQL1.dsSQL.Tables[1].AsEnumerable()
                .Where(m => (bool)m["vigente"] && m["lat_ini"] != DBNull.Value && m["lon_ini"] != DBNull.Value)
                .Select(m => new
                {
                    com_id = (int)m["id"],
                    com_tipo = (Int32)(short)m["tipo"],
                    com_subtipo = (short)m["subtipo"],
                    com_categoria = (string)m["categoria"],
                    com_nombre = (string)m["nombre"],
                    com_lat_ini = m["lat_ini"] == DBNull.Value ? "" : (string)m["lat_ini"],
                    com_lon_ini = m["lon_ini"] == DBNull.Value ? "" : (string)m["lon_ini"],
                    com_lat_fin = m["lat_fin"] == DBNull.Value ? "" : (string)m["lat_fin"],
                    com_lon_fin = m["lon_fin"] == DBNull.Value ? "" : (string)m["lon_fin"],
                    com_vigente = (bool)m["vigente"],
                    com_creado = (DateTime)m["creado"],
                    com_map_icon = fnc_getmarkerIcon(iIcon, (string)m["icon_active"], (string)m["icon_active_ext"]),
                    com_map_ubicacion_inicio = m["lat_ini"] == DBNull.Value || m["lon_ini"] == DBNull.Value
                        ? ""
                        : $"{((string)m["lat_ini"]).Substring(0, 9)},{((string)m["lon_ini"]).Substring(0, 9)}",
                    com_map_ubicacion_fin = m["lat_fin"] == DBNull.Value || m["lon_fin"] == DBNull.Value
                        ? ""
                        : $"{((string)m["lat_fin"]).Substring(0, 9)},{((string)m["lon_fin"]).Substring(0, 9)}",
                    com_map_ubicacion_flag = m["lat_fin"] == DBNull.Value ? true :
                        (string)m["lat_fin"] == (string)m["lat_ini"] ? true : false,
                    com_map_img = m["img"] == DBNull.Value ? "" : (string)m["img"] == "" ? "" : fnc_getmarkerImagePreview((string)m["img"]),
                    com_img_vig = m["img"] == DBNull.Value ? false : (string)m["img"] == "" ? false : true,
                    com_map_tramo = (string)m["tramo"],
                    com_id_mop = m["id_mop"] == DBNull.Value ? "" : (string)m["id_mop"],
                    com_ruta = (string)m["ruta"],
                    com_tramo_desc = (string)m["tramo"],
                    com_id_inv = m["id_inv"] == DBNull.Value ? "" : (string)m["id_inv"],
                    com_title =
                        $"{(string)m["categoria"]} {(m["id_inv"] == DBNull.Value ? "" : $"/ {(string)m["id_inv"]}")}"
                }).ToList();

                // PASO 4) - LISTA DE ELEMENTOS JOIN OBSERVACIONES
                var allMarkerJoinedList = (from cnf in allMarkerList
                                           join tmp in allTramoList on new { tramo = cnf.com_map_tramo } equals new { tramo = tmp.tra_nom }
                                               into EmployeeAddressGroup
                                           from tmp in EmployeeAddressGroup.DefaultIfEmpty()
                                           select new
                                           {
                                               com_tramo_id = tmp == null ? 0 : (Int32)tmp.tra_cod,
                                               com_tramo = cnf.com_map_tramo,
                                               com_tipo = cnf.com_tipo,
                                               com_categoria = cnf.com_categoria,
                                               com_id = cnf.com_id,
                                               com_id_inv = cnf.com_id_inv,
                                               com_id_mop = cnf.com_id_mop,
                                               com_img = cnf.com_img_vig ? "SI" : "NO",
                                               com_map_ubicacion_inicio = cnf.com_map_ubicacion_inicio,
                                               com_map_ubicacion_fin = cnf.com_map_ubicacion_fin,
                                               com_vigente = cnf.com_vigente ? "SI" : "NO",
                                               com_ruta = cnf.com_ruta
                                           }).OrderBy(x => x.com_tramo_id).ToList();


                // PASO 5) - OBSERVACIONES DEL ELEMENTO
                DateTime date = DateTime.Now.AddDays(365); // Adds 1 days to the date
                string formattedDate = date.ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                var startDate = DateTime.ParseExact($"01/01/2000 00:00:00", "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                var endDate = DateTime.ParseExact(formattedDate, "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                allObservationList = PlanDeMantencion.Controllers.PlanMantencionController.fnc_get_all_maitenance(User.Identity.Name, 1000, 0, 0, startDate, endDate);

                // PASO 5) - Unimos con Spread
                var allMarkerCommentList = (from cnf in allMarkerJoinedList
                                            join tmp in allObservationList on new
                                            { TipoElemento = (int)cnf.com_tipo, IdElemento = cnf.com_id } equals new
                                            { TipoElemento = (int)tmp.tra_tip_ele_id, IdElemento = (int)tmp.tra_ele_id }
                                            into EmployeeAddressGroup
                                            from tmp in EmployeeAddressGroup.DefaultIfEmpty()
                                            select new
                                            {
                                                // 1) - ELEMENTO
                                                com_tramo_id = cnf.com_tramo_id,
                                                com_tramo = cnf.com_tramo,
                                                com_tipo = cnf.com_tipo,
                                                com_categoria = cnf.com_categoria,
                                                com_id = cnf.com_id,
                                                com_id_inv = cnf.com_id_inv,
                                                com_id_mop = cnf.com_id_mop,
                                                com_img = cnf.com_img,
                                                com_map_ubicacion_inicio = cnf.com_map_ubicacion_inicio,
                                                com_map_ubicacion_fin = cnf.com_map_ubicacion_fin,
                                                com_vigente = cnf.com_vigente,
                                                com_ruta = cnf.com_ruta,

                                                // 2) - PLAN DE MANTENCION
                                                com_tra_fec_ing = tmp == null ? "" : tmp.tra_fec_ing?.ToString("yyyy-MM-dd HH:mm:ss"),
                                                com_tra_tip_mant_nom = tmp == null ? "" : tmp.tra_tip_mant_nom,
                                                com_tra_tip_act_nom = tmp == null ? "" : tmp.tra_tip_act_nom,
                                                com_tra_tip_ele_nom = tmp == null ? "" : tmp.tra_tip_ele_nom,
                                                com_tra_ele_nom = tmp == null ? "" : tmp.tra_ele_nom,
                                                com_tra_res_nom = tmp == null ? "" : tmp.tra_res_nom,
                                                com_tra_can = tmp == null ? "" : tmp.tra_can.ToString(),
                                                com_tra_uni_nom = tmp == null ? "" : tmp.tra_uni_nom,
                                                com_tra_obs = tmp == null ? "" : tmp.tra_obs
                                            }).OrderBy(x => x.com_tramo_id).ToList();


                var wbListado = new XLWorkbook();

                if (allMarkerCommentList.Count > 0)
                {
                    var filterDistinctTramo = allMarkerCommentList.Select(i => i.com_tramo_id).Distinct().ToList();

                    foreach (var tramo in filterDistinctTramo)
                    {
                        intFila = 3;

                        var dt = ConvertToDataTable(allMarkerCommentList.Where(x => x.com_tramo_id == tramo)
                            .Select(x => new
                            {
                                Ruta = x.com_ruta,
                                Tramo = x.com_tramo,
                                Tipo_elemento = x.com_categoria,
                                Id_elemento = x.com_id,
                                Id_Mop = x.com_id_mop,
                                Imagenes = x.com_img,
                                Coordenadas_Inicio = x.com_map_ubicacion_inicio,
                                Coordenadas_Fin = x.com_map_ubicacion_fin,
                                Vigente = x.com_vigente,
                                // 2 - plan de mantencion
                                FECHA_MANTENCION = x.com_tra_fec_ing,
                                TIPO_MANTENCION = x.com_tra_tip_mant_nom,
                                ACTIVIDAD = x.com_tra_tip_act_nom,
                                TIPO_ELEMENTO = x.com_tra_tip_ele_nom,
                                ELEMENTO = x.com_tra_ele_nom,
                                RESPONSABLE = x.com_tra_res_nom,
                                CANTIDAD = x.com_tra_can,
                                UNIDAD = x.com_tra_uni_nom,
                                OBSERVACIÓN = x.com_tra_obs
                            }).OrderBy(x => x.Tipo_elemento).ThenBy(x => x.Id_elemento).ThenByDescending(x =>
                                DateTime.ParseExact(x.FECHA_MANTENCION == "" ? "2000-01-01 00:00:00" : x.FECHA_MANTENCION, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture))
                            .ToList());

                        var worksheetsName = dt.AsEnumerable().Select(m => (string)m["Tramo"]).DefaultIfEmpty($"Hoja_{tramo}").FirstOrDefault();

                        var worksheet = wbListado.Worksheets.Add(worksheetsName.Length > 30 ? worksheetsName.Substring(0, 30) : worksheetsName);
                        worksheet.PageSetup.AdjustTo(25);
                        worksheet.Style.Font.FontName = "Calibri";
                        worksheet.Style.Font.FontSize = 10;

                        Int32 InitRowCols = 2;

                        worksheet.Cell(1, 1).SetValue("ELEMENTO").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Font.SetBold(true);
                        worksheet.Cell(1, 1).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                        worksheet.Cell(1, 10).SetValue("BITÁCORA DE MANTENCIÓN").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Font.SetBold(true);
                        worksheet.Cell(1, 10).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                        for (var y = 0; y < dt.Columns.Count; y++)
                        {
                            var colNameFirstReplace = dt.Columns[y].ColumnName;

                            worksheet.Cell(InitRowCols, y + 1).SetValue(colNameFirstReplace.Replace("_", " ").ToUpper()).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Font.SetBold(true);
                            worksheet.Cell(InitRowCols, y + 1).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                        }

                        //PASO 2) - LOAD DATA
                        for (var i = 0; i < dt.Rows.Count; i++)
                        {
                            for (var X = 0; X < dt.Columns.Count; X++)
                            {
                                var ColumnName = dt.Columns[X].ColumnName;

                                var value = dt.Rows[i][X].ToString();

                                worksheet.Cell(intFila, X + 1).SetValue(value);
                            }

                            intFila++;
                        }

                        // FILA 1
                        var startCellElemento = worksheet.Cell(1, 1);
                        var endCellElemento = worksheet.Cell(1, 9);                //For 2d data, with 'n' no. of rows and columns.

                        worksheet.Range(startCellElemento, endCellElemento).Merge();
                        worksheet.Range(startCellElemento, endCellElemento).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                        worksheet.Range(startCellElemento, endCellElemento).Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);

                        // FILA 2
                        var startCellBitacora = worksheet.Cell(1, 10);
                        var endCellBitacora = worksheet.Cell(1, dt.Columns.Count);

                        worksheet.Range(startCellBitacora, endCellBitacora).Merge();
                        worksheet.Range(startCellBitacora, endCellBitacora).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                        worksheet.Range(startCellBitacora, endCellBitacora).Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
                        worksheet.Range(startCellBitacora, endCellBitacora).Style.Fill.SetBackgroundColor(XLColor.LightGray);


                        // FILA 3
                        var startCell = worksheet.Cell(InitRowCols, 1);
                        var endCell = worksheet.Cell(dt.Rows.Count + InitRowCols, dt.Columns.Count);                //For 2d data, with 'n' no. of rows and columns.

                        worksheet.Range(startCell, endCell).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                        worksheet.Range(startCell, endCell).Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);

                        worksheet.Range(InitRowCols, 1, InitRowCols, 9).Style.Fill.SetBackgroundColor(XLColor.Yellow);
                        worksheet.Range(2, 10, 2, dt.Columns.Count).Style.Fill.SetBackgroundColor(XLColor.GreenYellow);
                        worksheet.Columns().AdjustToContents();
                    }
                }

                if (fileExport.guardarArchivo)
                    wbListado.SaveAs(fileExport.Directorios + fileExport.fileName);

                var stream = new MemoryStream();
                wbListado.SaveAs(stream);

                var formModalExportar = new
                {
                    fileContents =
                        Convert.ToBase64String(stream.ToArray()), // Encode the byte array using Base64 encoding
                    contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileDownloadName = fileExport.fileName
                };

                return Json(new { status = true, formModalExportar, responseText = string.Empty });

            }
            catch (Exception ex)
            {
                return Json(new { status = false, responseText = $"Exception: {(ex.InnerException == null ? ex.Message : ex.InnerException.Message)}" });
            }

        }
        #endregion

        #region INFORME 3 - ELEMENTOS POR TRAMO
        public ActionResult JSON_GenerarReporte3(FormReportes.Root root)
        {
            var iIcon = string.Empty;

            try
            {
                Thread.Sleep(1000);

                // PASO 1) -  CONFIGURACION DEL REPORTE
                var fileExport = new
                {
                    guardarArchivo = true,
                    fileName = $"Rpt_Elementos_Por_Tramo_{DateTime.UtcNow.ToString("yyyy-MM-dd_HHmmssfff")}.xlsx",
                    Directorios = ConfigurationManager.AppSettings["DirectorioInformes"].ToString()
                };


                // PASO 2)-  LISTA ELEMENTOS ACTIVOS DESDE EL MENU DEL USUARIO
                var dataSetSQL0 = menuBusinessImpl.fncDevolverMenu(User.Identity.Name);


                var listSubMenus = dataSetSQL0.dsSQL.Tables[1].AsEnumerable()
                    .Where(m => (bool)m["men_tra_vig"] && (bool)m["men_ele_vig"] && (bool)m["men_ele_opc_vis"])
                    .Select(m => new
                    {
                        menu_cod = ((short)m["men_id"]).ToString(),
                        menu_tra = (short)m["men_tra_cod"],
                        menu_tra_nom = (string)m["men_tra_nom"],
                        menu_nom = (string)m["men_ele_nom"],
                        menu_sel = (bool)m["men_ele_sel_chk"],
                        menu_row = (short)m["men_id"]
                    }).OrderBy(x => x.menu_row).ToList();

                if (listSubMenus.Count == 0)
                    throw new Exception("No hay elementos disponibles para generar el reporte.");

                //var _List = JsonConvert.SerializeObject(listSubMenus);


                // Lista de marcadores seleccionados
                var filteredMarkersChecked = listSubMenus.Select(x => (int)x.menu_row).ToList();

                // PASO 3) -
                var dataSetSQL1 = mapBusinessImpl.GetAllMarkers(User.Identity.Name, filteredMarkersChecked);

                if (dataSetSQL1.intError != 0)
                    throw new Exception(dataSetSQL1.strError);

                var allTramoList = dataSetSQL1.dsSQL.Tables[4].AsEnumerable()
                    .Select(m => new
                    {
                        tra_id = (Int16)m["tra_id"],
                        tra_cod = (Int32)m["tra_cod"],
                        tra_nom = (string)m["tra_nom"],
                        tra_des = (string)m["tra_des"]
                    }).ToList();

                var allMarkerList = dataSetSQL1.dsSQL.Tables[1].AsEnumerable()
                .Where(m => (bool)m["vigente"] && m["lat_ini"] != DBNull.Value && m["lon_ini"] != DBNull.Value)
                .Select(m => new
                {
                    com_id = (int)m["id"],
                    com_tipo = (short)m["tipo"],
                    com_subtipo = (short)m["subtipo"],
                    com_categoria = (string)m["categoria"],
                    com_nombre = (string)m["nombre"],
                    com_lat_ini = m["lat_ini"] == DBNull.Value ? "" : (string)m["lat_ini"],
                    com_lon_ini = m["lon_ini"] == DBNull.Value ? "" : (string)m["lon_ini"],
                    com_lat_fin = m["lat_fin"] == DBNull.Value ? "" : (string)m["lat_fin"],
                    com_lon_fin = m["lon_fin"] == DBNull.Value ? "" : (string)m["lon_fin"],
                    com_vigente = (bool)m["vigente"],
                    com_creado = (DateTime)m["creado"],
                    com_map_icon = fnc_getmarkerIcon(iIcon, (string)m["icon_active"], (string)m["icon_active_ext"]),
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
                    com_title =
                        $"{(string)m["categoria"]} {(m["id_inv"] == DBNull.Value ? "" : $"/ {(string)m["id_inv"]}")}"
                }).ToList();

                // PASO 5) - Unimos con Spread
                var allreportsList = (from cnf in allMarkerList
                                      join tmp in allTramoList on new { tramo = cnf.com_map_tramo } equals new { tramo = tmp.tra_nom }
                                          into EmployeeAddressGroup
                                      from tmp in EmployeeAddressGroup.DefaultIfEmpty()
                                      select new
                                      {
                                          com_tramo_id = tmp == null ? 0 : (Int32)tmp.tra_cod,
                                          com_tramo = cnf.com_map_tramo,
                                          com_tipo = cnf.com_tipo,
                                          com_categoria = cnf.com_categoria,
                                          com_id = cnf.com_id,
                                          com_id_inv = cnf.com_id_inv,
                                          com_id_mop = cnf.com_id_mop,
                                          com_img = cnf.com_img_vig ? "SI" : "NO",
                                          com_map_ubicacion_inicio = cnf.com_map_ubicacion_inicio,
                                          com_map_ubicacion_fin = cnf.com_map_ubicacion_fin,
                                          com_vigente = cnf.com_vigente ? "SI" : "NO",
                                          com_ruta = cnf.com_ruta
                                      }).OrderBy(x => x.com_tramo_id).ToList();

                //var _List2 = JsonConvert.SerializeObject(allreportsList);


                var wbListado = new XLWorkbook();

                if (allreportsList.Count > 0)
                {
                    var filterDistinctTramo = allreportsList.Select(i => i.com_tramo_id).Distinct().ToList();

                    foreach (var items in filterDistinctTramo)
                    {
                        var intFila = 2;

                        var dt = ConvertToDataTable(allreportsList.Where(x => x.com_tramo_id == items)
                                                            .Select(x => new
                                                            {
                                                                Ruta = x.com_ruta,
                                                                Tramo = x.com_tramo,
                                                                Tipo_elemento = x.com_categoria,
                                                                Id_elemento = x.com_id,
                                                                //Id_Inventario=x.com_id_inv,
                                                                Id_Mop = x.com_id_mop,
                                                                Imagenes = x.com_img,
                                                                Coordenadas_Inicio = x.com_map_ubicacion_inicio,
                                                                Coordenadas_Fin = x.com_map_ubicacion_fin,
                                                                Vigente = x.com_vigente
                                                            }).OrderBy(x => x.Tipo_elemento).ThenBy(x => x.Id_elemento).ToList());

                        var worksheetsName = dt.AsEnumerable().Select(m => (string)m["Tramo"]).DefaultIfEmpty($"Hoja_{items}").FirstOrDefault();

                        var worksheet = wbListado.Worksheets.Add(worksheetsName.Length > 30 ? worksheetsName.Substring(0, 30) : worksheetsName);
                        worksheet.PageSetup.AdjustTo(25);
                        worksheet.Style.Font.FontName = "Calibri";
                        worksheet.Style.Font.FontSize = 10;

                        // PASO 1) -  GENERAMOS ENCABEZADO
                        for (var y = 0; y < dt.Columns.Count; y++)
                        {
                            var colName = dt.Columns[y].ColumnName;

                            worksheet.Cell(1, y + 1).SetValue(colName.Replace("_", " ").ToUpper()).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Font.SetBold(true);
                            worksheet.Cell(1, y + 1).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                        }

                        //PASO 2) - CAEGAMOS DATA 
                        for (var i = 0; i < dt.Rows.Count; i++)
                        {
                            for (var X = 0; X < dt.Columns.Count; X++)
                            {
                                var nom = dt.Columns[X].ColumnName;

                                var val = dt.Rows[i][X].ToString();

                                worksheet.Cell(intFila, X + 1).SetValue(val);
                            }

                            intFila++;
                        }

                        var startCell = worksheet.Cell(1, 1);
                        var endCell = worksheet.Cell(dt.Rows.Count + 1, dt.Columns.Count);                //For 2d data, with 'n' no. of rows and columns.

                        worksheet.Range(startCell, endCell).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                        worksheet.Range(startCell, endCell).Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
                        worksheet.Range(1, 1, 1, dt.Columns.Count).Style.Fill.SetBackgroundColor(XLColor.Yellow);
                        worksheet.Columns().AdjustToContents();
                    }
                }


                if (fileExport.guardarArchivo)
                    wbListado.SaveAs(fileExport.Directorios + fileExport.fileName);

                var stream = new MemoryStream();
                wbListado.SaveAs(stream);

                var formModalExportar = new
                {
                    fileContents =
                        Convert.ToBase64String(stream.ToArray()), // Encode the byte array using Base64 encoding
                    contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileDownloadName = fileExport.fileName
                };

                return Json(new { status = true, formModalExportar, responseText = string.Empty });

            }
            catch (Exception ex)
            {
                return Json(new { status = false, responseText = $"Exception: {(ex.InnerException == null ? ex.Message : ex.InnerException.Message)}" });
            }

        }
        #endregion



        #region FUNCIONES
        public DataTable ConvertToDataTable<T>(IList<T> data)
        {
            System.ComponentModel.PropertyDescriptorCollection properties = System.ComponentModel.TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (System.ComponentModel.PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (System.ComponentModel.PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;

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
        #endregion
    }
}
