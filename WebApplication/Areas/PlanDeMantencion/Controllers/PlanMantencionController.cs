using BusinessEntity.FormModels;
using BusinessImpl;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace WebApplication.Areas.PlanDeMantencion.Controllers
{
    public class PlanMantencionController : WebApplication.Controllers.BaseController
    {
        #region campos Privados
        private readonly PlanDeMantencionBusinessImpl planDeMantencionBusinessImpl = new PlanDeMantencionBusinessImpl();
        private readonly ConfiguracionBusinessImpl configuracionBusinessImpl = new ConfiguracionBusinessImpl();
        private readonly IntegracionSIGBusinessImpl integracionSIGBusinessImpl = new IntegracionSIGBusinessImpl();
        private readonly WebApplication.Areas.IntegracionSIG.Controllers.IntegracionCapaSIGController _integracionCapaSIGController;
        #endregion

        #region Constructores
        public PlanMantencionController()
        {
            _integracionCapaSIGController = new IntegracionSIG.Controllers.IntegracionCapaSIGController();
        }
        #endregion

        #region PLAN DE MANTENCION - LISTAR        
        // GET: PlanDeMantencion/PlanMantencion

        [Authorize]
        [HttpGet]
        public ActionResult Index()
        {
            var collection = new FormPlanDeMantencion.PlanDeMantencion
            {
                allPlanDeMantencionList = new System.Data.DataTable(),
                AllPlanningList = new List<SelectListItem> { new SelectListItem { Text = "Seleccione ...", Value = "0" } },
                observations = string.Empty
            };

            try
            {
                var dataSetSQL = planDeMantencionBusinessImpl.ListAllPlanning(User.Identity.Name, string.Empty);

                if (dataSetSQL.intError != 0)
                    throw new Exception(dataSetSQL.strError);

                var allYearList = dataSetSQL.dsSQL.Tables[0].AsEnumerable()
                    .Select(m => new SelectListItem
                    {
                        Value = m.Field<String>("pla_guid"),
                        Text = $"{m.Field<Int32>("pla_year")}_v{m.Field<Int16>("pla_ver")}",
                    }).ToList();

                collection.AllPlanningList.AddRange(allYearList);

                ViewData["mensaje"] = string.Empty;
                ViewData["tipo"] = string.Empty;
            }
            catch (Exception ex)
            {
                ViewData["mensaje"] = (ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                ViewData["tipo"] = "error";

                collection = new FormPlanDeMantencion.PlanDeMantencion
                {
                    allPlanDeMantencionList = new System.Data.DataTable(),
                    AllPlanningList = new List<SelectListItem> { new SelectListItem { Text = "Seleccione ...", Value = "0" } }
                };
            }

            return View(collection);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Details(string inputString)
        {
            var collection = new FormPlanDeMantencion.PlanDeMantencion
            {
                allPlanDeMantencionList = new System.Data.DataTable(),
                observations = ""
            };

            try
            {
                var dataSetSQL = planDeMantencionBusinessImpl.ListFilterByGuid(User.Identity.Name, inputString);

                if (dataSetSQL.intError != 0)
                    throw new Exception(dataSetSQL.strError);

                //PASO 1) -  DETALLE DE PLAN DE CONSERVACION
                collection.allPlanDeMantencionList = (from t1 in dataSetSQL.dsSQL.Tables[0].AsEnumerable()
                                                      select t1).CopyToDataTable<DataRow>();

                // PASO 2) - OBSERVACIONES DEL PLAN SELECCIONADO
                collection.observations = dataSetSQL.dsSQL.Tables[1].AsEnumerable()
                                    .Select(m =>

                                        m.Field<String>("pla_obs")
                                     ).DefaultIfEmpty(string.Empty).FirstOrDefault();
            }
            catch
            {
                collection = new FormPlanDeMantencion.PlanDeMantencion
                {
                    allPlanDeMantencionList = new System.Data.DataTable(),
                    observations = string.Empty
                };
            }

            return PartialView("_PartialViewTablaPlan", collection);
        }
        #endregion

        #region PLAN DE MANTENCION - CARGAR
        /// <summary>
        /// VAlida si existe version del archivo que se esta subiendo
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public JsonResult JSON_ValidPlanningExists(string username)
        {
            try
            {
                var dataSetSQL = planDeMantencionBusinessImpl.ListAllPlanning(User.Identity.Name, string.Empty);

                if (dataSetSQL.intError != 0)
                    throw new Exception(dataSetSQL.strError);

                var allPlanningList = dataSetSQL.dsSQL.Tables[0].AsEnumerable()
                                    .Where(x => x.Field<bool>("pla_vig"))
                                    .Select(m => new SelectListItem
                                    {
                                        Value = m.Field<String>("pla_guid"),
                                        Text = $"{m.Field<Int32>("pla_year")}_v{m.Field<Int16>("pla_ver")}",
                                    }).ToList();

                var isCount = allPlanningList.Where(m => m.Text.Equals(username)).Count();

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

        [HttpPost]
        public ActionResult CargarArchivoJSON(string inputString1, string inputString2, string inputString3)
        {
            //var blUpload = false;
            var str = new System.Text.StringBuilder();
            var uploadFile = new BusinessEntity.UploadFile();
            var blUpload = true;

            try
            {
                /// parametros de entrada
                Int32 yearSelected = 0;
                Int16 versionSelected = 0;
                Int32.TryParse(inputString1, out yearSelected);
                Int16.TryParse(inputString2, out versionSelected);

                if (Request.Files.Count == 0)
                    return Json(new { error = $"No hay archivos seleccionados." });

                var strDirectorios = $"{System.Configuration.ConfigurationManager.AppSettings["DirectorioSubidas"].ToString()}planConservacion\\";

                if (string.IsNullOrEmpty(strDirectorios))
                    throw new Exception("No se encuentra el directorio de archivos!.");

                //  Get all files from Request object  
                var files = Request.Files;
                for (var i = 0; i < files.Count; i++)
                {
                    var file = files[i];
                    string fname;

                    // PASO 1) - Checking for Internet Explorer  
                    if (Request.Browser.Browser.ToUpper() == "IE" ||
                        Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                    {
                        var testfiles = file.FileName.Split(new char[] { '\\' });
                        fname = CrosscuttingUtiles.Helpers.fnc_NormaliceIsNullOrEmpty(testfiles[testfiles.Length - 1]);
                    }
                    else
                    {
                        fname = CrosscuttingUtiles.Helpers.fnc_NormaliceIsNullOrEmpty(file.FileName);
                    }

                    uploadFile.fileName = fname;
                    ///-------------------------------------------------------------------------------------
                    /// PASO 1 CARGA DE ARCHIVO FISICO
                    ///-------------------------------------------------------------------------------------

                    ////////// PASO 2) - VALIDAMOS SI EXISTE EN BD
                    ////////var dataSetSQL = documentosBusinessImpl.ListAll(User.Identity.Name);

                    ////////if (dataSetSQL.intError != 0)
                    ////////    throw new Exception(dataSetSQL.strError);

                    ////////// PASO 3) - OBTENEMOS TODAS LAS CATEGORIAS Y SUS DOCUMENTOS
                    ////////var alldocumentList = dataSetSQL.dsSQL.Tables[0].AsEnumerable()
                    ////////    .Where(X => (string)X["doc_nombre"] == fname)
                    ////////    .Select(m => new DocumentoBusinessEntity
                    ////////    {
                    ////////        doc_guid = (string)m["doc_guid"]
                    ////////    }).OrderByDescending(x => x.doc_usr_mod_fec).ToList();

                    ////////if (alldocumentList.Count > 0)
                    ////////    throw new Exception("El DOCUMENTO ya existe en los registros!.");

                    // PASO 4) - Valida directorio y archivo
                    var exists = Directory.Exists(strDirectorios);

                    if (!exists)
                        Directory.CreateDirectory(strDirectorios);

                    var isFileExists = System.IO.File.Exists(Path.Combine(strDirectorios, fname));
                    if (isFileExists)
                        throw new Exception($"ya existe un archivo con nombre '{fname}'.");

                    // PASO 5) -  Get the complete folder path and store the file inside it.  
                    var fullFileName = Path.Combine(strDirectorios, fname);
                    file.SaveAs(fullFileName);

                    /// -------------------------------------------------------------------------------------
                    /// PASO 2 CARGA DE ARCHIVO EN LA BD
                    /// -------------------------------------------------------------------------------------
                    System.Data.DataTable dtCategory = new System.Data.DataTable(); // 1
                                                                                    //Add column to table:
                    dtCategory.Columns.Add(new DataColumn { ColumnName = "id", DataType = typeof(string), AllowDBNull = true });
                    dtCategory.Columns.Add(new DataColumn { ColumnName = "Tipo", DataType = typeof(string), AllowDBNull = true });
                    dtCategory.Columns.Add(new DataColumn { ColumnName = "categoria", DataType = typeof(string), AllowDBNull = true });

                    System.Data.DataTable dt = new System.Data.DataTable(); // 1
                                                                            //Add column to table:
                    dt.Columns.Add(new DataColumn { ColumnName = "year", DataType = typeof(string), AllowDBNull = true });
                    dt.Columns.Add(new DataColumn { ColumnName = "categoria", DataType = typeof(string), AllowDBNull = true });
                    dt.Columns.Add(new DataColumn { ColumnName = "codigo", DataType = typeof(string), AllowDBNull = true });
                    dt.Columns.Add(new DataColumn { ColumnName = "operacion", DataType = typeof(string), AllowDBNull = true });
                    dt.Columns.Add(new DataColumn { ColumnName = "unidad", DataType = typeof(string), AllowDBNull = true });
                    dt.Columns.Add(new DataColumn { ColumnName = "enero", DataType = typeof(string), AllowDBNull = true });
                    dt.Columns.Add(new DataColumn { ColumnName = "febrero", DataType = typeof(string), AllowDBNull = true });
                    dt.Columns.Add(new DataColumn { ColumnName = "marzo", DataType = typeof(string), AllowDBNull = true });
                    dt.Columns.Add(new DataColumn { ColumnName = "abril", DataType = typeof(string), AllowDBNull = true });
                    dt.Columns.Add(new DataColumn { ColumnName = "mayo", DataType = typeof(string), AllowDBNull = true });
                    dt.Columns.Add(new DataColumn { ColumnName = "junio", DataType = typeof(string), AllowDBNull = true });
                    dt.Columns.Add(new DataColumn { ColumnName = "julio", DataType = typeof(string), AllowDBNull = true });
                    dt.Columns.Add(new DataColumn { ColumnName = "agosto", DataType = typeof(string), AllowDBNull = true });
                    dt.Columns.Add(new DataColumn { ColumnName = "septiembre", DataType = typeof(string), AllowDBNull = true });
                    dt.Columns.Add(new DataColumn { ColumnName = "octubre", DataType = typeof(string), AllowDBNull = true });
                    dt.Columns.Add(new DataColumn { ColumnName = "noviembre", DataType = typeof(string), AllowDBNull = true });
                    dt.Columns.Add(new DataColumn { ColumnName = "diciembre", DataType = typeof(string), AllowDBNull = true });
                    dt.Columns.Add(new DataColumn { ColumnName = "total", DataType = typeof(string), AllowDBNull = true });
                    dt.Columns.Add(new DataColumn { ColumnName = "frecuencia", DataType = typeof(string), AllowDBNull = true });
                    dt.Columns.Add(new DataColumn { ColumnName = "id", DataType = typeof(string), AllowDBNull = true });
                    dt.Columns.Add(new DataColumn { ColumnName = "Tipo", DataType = typeof(string), AllowDBNull = true });

                    int j = 0;
                    var isCabecera = false;
                    var categoria = string.Empty;

                    using (var excelWorkbook = new XLWorkbook(fullFileName))
                    {
                        var nonEmptyDataRows = excelWorkbook.Worksheet(1).RowsUsed();

                        foreach (var dataRow in nonEmptyDataRows)
                        {
                            //for row number check
                            if (dataRow.RowNumber() >= 1 && dataRow.RowNumber() <= 2000)
                            {
                                //to get column # 3's data
                                var cell1 = dataRow.Cell(1).Value;
                                var cell2 = dataRow.Cell(2).Value;
                                var cell3 = dataRow.Cell(3).Value;
                                var cell4 = dataRow.Cell(4).Value;
                                var cell5 = dataRow.Cell(5).Value;
                                var cell6 = dataRow.Cell(6).Value;
                                var cell7 = dataRow.Cell(7).Value;
                                var cell8 = dataRow.Cell(8).Value;
                                var cell9 = dataRow.Cell(9).Value;
                                var cell10 = dataRow.Cell(10).Value;
                                var cell11 = dataRow.Cell(11).Value;
                                var cell12 = dataRow.Cell(12).Value;
                                var cell13 = dataRow.Cell(13).Value;
                                var cell14 = dataRow.Cell(14).Value;
                                var cell15 = dataRow.Cell(15).Value;
                                var cell16 = dataRow.Cell(16).Value;
                                var cell17 = dataRow.Cell(17).Value;
                                var cell18 = dataRow.Cell(18).Value;
                                var cell19 = dataRow.Cell(19).Value;

                                // VALIDAMOS QUE LA CABECERA DE COLUMNAS EXISTAN
                                if (cell3.ToString() == "Código" || cell3.ToString() == "Ene")
                                {
                                    isCabecera = true;

                                    //if (isCabecera && !string.IsNullOrEmpty(cell3.ToString()) && cell3.ToString() == "Código")
                                    //{
                                    //    // cabecera de item
                                    //    var Codigo = cell3.ToString();
                                    //    var NombreOperación = cell4.ToString();
                                }
                                //else if (isCabecera && string.IsNullOrEmpty(cell3.ToString()) &&
                                //                        !string.IsNullOrEmpty(cell4.ToString()) &&
                                //                        string.IsNullOrEmpty(cell5.ToString()))
                                //{
                                else if (isCabecera && !string.IsNullOrEmpty(cell4.ToString()))
                                {
                                    if (categoria != cell4.ToString() && !String.IsNullOrEmpty(categoria))
                                    {
                                        dtCategory.Rows.Add(new object[] { "0", "1", categoria });

                                        // columna de categoria
                                        //var Codigo = cell3.ToString();
                                        //var NombreOperación = cell4.ToString();
                                        categoria = cell4.ToString();
                                        //var unidad = cell5.ToString();
                                    }
                                    ////    dtCategory.Rows.Add(new object[] { "0", "1", categoria });

                                    ////// columna de categoria
                                    ////var Codigo = cell3.ToString();
                                    ////var NombreOperación = cell4.ToString();
                                    ////categoria = cell4.ToString();
                                    ////var unidad = cell5.ToString();
                                    //}
                                    //else if (isCabecera && !string.IsNullOrEmpty(cell3.ToString()) &&
                                    //                        !string.IsNullOrEmpty(cell4.ToString()) &&
                                    //                        !string.IsNullOrEmpty(cell5.ToString()))
                                    //{

                                    // columna de dato
                                    var year = yearSelected;
                                    var categoriaItem = categoria;
                                    var Codigo = String.IsNullOrEmpty(cell3.ToString()) ? "-" : cell3.ToString();
                                    var NombreOperación = cell4.ToString();
                                    var unidad = cell5.ToString();
                                    var Ene = cell6.ToString();
                                    var Feb = cell7.ToString();
                                    var Mar = cell8.ToString();
                                    var Abr = cell9.ToString();
                                    var May = cell10.ToString();
                                    var Jun = cell11.ToString();
                                    var Jul = cell12.ToString();
                                    var Ago = cell13.ToString();
                                    var Sep = cell14.ToString();
                                    var Oct = cell15.ToString();
                                    var Nov = cell16.ToString();
                                    var Dic = cell17.ToString();
                                    var Total = cell18.ToString();
                                    var Frecuencia = cell19.ToString();

                                    dt.Rows.Add(new object[] { year, categoriaItem, Codigo, NombreOperación, unidad, Ene, Feb, Mar, Abr, May, Jun, Jul, Ago, Sep, Oct, Nov, Dic, Total, Frecuencia, j, "2" });
                                }
                                else
                                {
                                    //NADA
                                }

                            }
                            j++;
                        }
                    }


                    var model = new BusinessEntity.FormModels.FormPlanDeMantencion.Parametros
                    {
                        currentUserSelected = User.Identity.Name,
                        observations = inputString3,
                        year = yearSelected,
                        version = versionSelected,
                        fileName = fname,
                        CategoryTable = dtCategory,
                        PlanningTable = dt,
                        guidIdentifier = Guid.NewGuid()
                    };

                    var dataSetSQL = planDeMantencionBusinessImpl.SubirArchivoPlanMantencion(model);

                    if (dataSetSQL.intError != 0)
                        throw new Exception(dataSetSQL.strError);
                }

                //Returns message that successfully uploaded
                blUpload = true;

                uploadFile.error = "";
                uploadFile.initialPreview = new List<string>();
                uploadFile.initialPreviewConfig = new List<BusinessEntity.UploadFilePreviewConfig>();
                uploadFile.initialPreviewAsData = true;


                if (blUpload)
                    return Json(uploadFile);
                else
                    return Json(new { error = $"No se cargaron los archivos adjuntos." });
            }
            catch (Exception ex)
            {
                uploadFile.message = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
                return Json(new { error = $"Error : {(ex.InnerException == null ? ex.Message : ex.InnerException.Message)}" });
            }
        }

        /// <summary>
        /// Carga archivo en Base de Datos.
        /// </summary>
        /// <returns></returns>
        public ActionResult SubirArchivo()
        {
            FormPlanDeMantencion.PlanFiltros collection = new FormPlanDeMantencion.PlanFiltros();
            short maxLength = 0;

            try
            {
                // ---------------------------------------------------------------------------------------------
                // PASO 1) -  EXTRAE VALORES DE LA TABLA CONFIGURACION
                // ---------------------------------------------------------------------------------------------
                var dataSet = configuracionBusinessImpl.ListAll(User.Identity.Name, string.Empty);

                if (dataSet.intError != 0)
                    throw new Exception(dataSet.strError);

                // PASO 0) - EXTENSIONES HABILITADAS
                var allExtensionesList = dataSet.dsSQL.Tables[0].AsEnumerable()
                    .Where(m => (string)m["gen_key_row"] == "PLAN_EXT" && (bool)m["gen_row_vig"])
                    .Select(m => new
                    {
                        codigo = (Int32)m["gen_id"],
                        glosa = (string)m["gen_nom"],
                        Valor = (Int32)m["gen_val"],
                    }).ToList();

                // PASO 1) - TAMAÑO MAXIMO DE ARCHIVOS
                var maxFileSize = dataSet.dsSQL.Tables[0].AsEnumerable()
                    .Where(m => (string)m["gen_key_row"] == "MAX_FILE_SIZE" && (bool)m["gen_row_vig"])
                    .Select(m => new
                    {
                        maxFileSize = (Int32)m["gen_val"]
                    }).DefaultIfEmpty(new { maxFileSize = (Int32)0 }).FirstOrDefault();

                //collection.AllYearList = new List<SelectListItem> { new SelectListItem { Text = "SELECCIONE UNA OPCIÓN", Value = "0", Selected = true } };
                collection.AllowedFileExtensions = allExtensionesList.AsEnumerable()
                                                                                .Select(m => new SelectListItem
                                                                                {
                                                                                    Value = m.glosa,
                                                                                    Text = m.glosa
                                                                                }).ToList();
                collection.maxFileSize = maxFileSize.maxFileSize;
                collection.maxFileSizeIso = ByteSizeLib.ByteSize.FromKiloBytes(maxFileSize.maxFileSize).ToString();

                // ---------------------------------------------------------------------------------------------
                // PASO 1) - LISTADO DE ARCHIVOS
                // ---------------------------------------------------------------------------------------------
                var dataSetSQL = planDeMantencionBusinessImpl.ListAllPlanning(User.Identity.Name, string.Empty);

                if (dataSetSQL.intError != 0)
                    throw new Exception(dataSetSQL.strError);

                if (dataSetSQL.dsSQL.Tables[0].Rows.Count > 0)
                    maxLength = dataSetSQL.dsSQL.Tables[0].AsEnumerable().Max(word => word.Field<short>("pla_ver"));

                maxLength++;

                collection.version = maxLength.ToString();
            }
            catch (Exception ex)
            {
                //return new HttpStatusCodeResult(500);
                collection = new FormPlanDeMantencion.PlanFiltros
                {
                    AllYearList = new List<SelectListItem> { new SelectListItem { Text = "Seleccione ...", Value = "0" } },

                    AllowedFileExtensions = new List<SelectListItem>
                    {
                        new SelectListItem { Text = "xlsx", Value = "xlsx" },
                    },
                    maxFileSize = 1000000,
                    maxFileSizeIso = ByteSizeLib.ByteSize.FromKiloBytes(1000000).ToString(),
                    version = "1"
                };

                ViewData["mensaje"] = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
                ViewData["tipo"] = "error";
            }

            var AllowedFileExtensions = Newtonsoft.Json.JsonConvert.SerializeObject(collection.AllowedFileExtensions.Select(x => x.Value).ToList<string>());
            ViewData["formatos"] = AllowedFileExtensions;

            return View("Subir", collection);

        }


        #endregion

        #region PLAN DE MANTENCION - INGRESO NUEVA MANTENCION
        /// <summary>
        /// Formulario inicial de Ingreso de nueva mantención
        /// </summary>
        /// <returns></returns>

        [Authorize]
        [HttpGet]
        public ActionResult Agregar()
        {
            var model = new BusinessEntity.FormModels.FormPlanDeMantencion.PlanFiltros();
            model.allMaintenanceType = new List<SelectListItem> { new SelectListItem { Text = "Seleccione ...", Value = "0" } };
            model.allActivity = new List<SelectListItem> { new SelectListItem { Text = "Seleccione ...", Value = "0" } };
            model.allElementType = new List<SelectListItem> { new SelectListItem { Text = "Seleccione ...", Value = "0" } };
            model.allElementCode = new List<SelectListItem> { new SelectListItem { Text = "Seleccione ...", Value = "0" } };
            model.allResponsible = new List<SelectListItem> { new SelectListItem { Text = "Seleccione ...", Value = "0" } };

            try
            {
                var collection = new FormPlanDeMantencion.Parametros
                {
                    currentUserSelected = User.Identity.Name,
                    YearMaintenancePlanning = 0,
                    IdMaintenancePlanning = 0,
                    IdActivity = 0
                };

                //============================================================================================2
                // CARGAMOS TAREAS DL PLAN VIGENTE SELECCIONADO
                //============================================================================================

                var dataSetSQL = planDeMantencionBusinessImpl.ListFilterActivityByPlan(collection);

                if (dataSetSQL.intError != 0)
                    throw new Exception(dataSetSQL.strError);

                // PASO 1.1) - TIPOS DE ACTIVIDAD
                var filteredTableActivity = dataSetSQL.dsSQL.Tables[0].AsEnumerable()
                                            .Where(r => r.Field<bool>("vigente"))
                                            .Select(r => new SelectListItem
                                            {
                                                Value = (r.Field<Int32>("valor")).ToString(),
                                                Text = $"{r.Field<String>("nombre")} ({r.Field<String>("codigo")})"
                                            }).OrderBy(x => Int32.Parse(x.Value)).ToList();

                model.allActivity.AddRange(filteredTableActivity);

                // PASO 2) - (CONFIGURACION) - CARGAMOS TAREAS DL PLAN VIGENTE SELECCIONADO
                var dataSetSQL2 = configuracionBusinessImpl.ListAll(User.Identity.Name, string.Empty);

                if (dataSetSQL2.intError != 0)
                    throw new Exception(dataSetSQL2.strError);

                // TIPO MANTENCION
                var allTableMaintenanceType = dataSetSQL2.dsSQL.Tables[0].AsEnumerable()
                    .Where(r => r.Field<Int16>("gen_tip_idd") == 99 && r.Field<String>("gen_key_row").Equals("TIPO_MANT"))
                    .Select(r => new SelectListItem
                    {
                        Value = r.Field<Int32>("gen_val").ToString(),
                        Text = r.Field<String>("gen_nom")
                    }).OrderBy(x => x.Text).ToList();

                model.allMaintenanceType.AddRange(allTableMaintenanceType);

                // TIPO ELEMENTO
                var allTableElementType = dataSetSQL2.dsSQL.Tables[0].AsEnumerable()
                    .Where(r => r.Field<Int16>("gen_tip_idd") == 99 && r.Field<String>("gen_key_row").Equals("TIP_ELE") && r.Field<bool>("gen_row_vig"))
                    .Select(r => new SelectListItem
                    {
                        Value = r.Field<Int32>("gen_val").ToString(),
                        Text = r.Field<String>("gen_nom")
                    }).OrderBy(x => x.Text).ToList();

                model.allElementType.AddRange(allTableElementType);

                // TABLA RESPONSABLE
                var allTableResponsible = dataSetSQL2.dsSQL.Tables[0].AsEnumerable()
                    .Where(r => r.Field<Int16>("gen_tip_idd") == 99 && r.Field<String>("gen_key_row").Equals("PLAN_RESP"))
                    .Select(r => new SelectListItem
                    {
                        Value = r.Field<Int32>("gen_val").ToString(),
                        Text = r.Field<String>("gen_nom")
                    }).OrderBy(x => x.Text).ToList();

                model.allResponsible.AddRange(allTableResponsible);
                model.unit = "";
                model.MaintenanceDate = null;

                if (TempData["mensaje"] != null)
                {
                    ViewData["mensaje"] = TempData["mensaje"];
                    ViewData["tipo"] = TempData["tipo"];
                    TempData["mensaje"] = null;
                }
            }
            catch (Exception ex)
            {
                model.allMaintenanceType = new List<SelectListItem> { new SelectListItem { Text = "Seleccione ...", Value = "0" } };
                model.allActivity = new List<SelectListItem> { new SelectListItem { Text = "Seleccione ...", Value = "0" } };
                model.allElementType = new List<SelectListItem> { new SelectListItem { Text = "Seleccione ...", Value = "0" } };
                model.allElementCode = new List<SelectListItem> { new SelectListItem { Text = "Seleccione ...", Value = "0" } };
                model.allResponsible = new List<SelectListItem> { new SelectListItem { Text = "Seleccione ...", Value = "0" } };

                model.MaintenanceTypeSelected = "0";

                ViewData["mensaje"] = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
                ViewData["tipo"] = "error";
            }

            return View(model);
        }

        /// <summary>
        /// Formulario de Ingreso de nueva mantención con informacion
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public ActionResult Agregar(BusinessEntity.FormModels.FormPlanDeMantencion.PlanFiltros collection)
        {
            try
            {
                if (String.IsNullOrEmpty(collection.MaintenanceTypeSelected) ||
                    String.IsNullOrEmpty(collection.ActivitySelected) ||
                    String.IsNullOrEmpty(collection.ElementTypeSelected) ||
                    String.IsNullOrEmpty(collection.ElementCodeSelected) ||
                    String.IsNullOrEmpty(collection.ResponsibleSelected) ||
                    String.IsNullOrEmpty(collection.MaintenanceDate))
                    throw new Exception("Faltan parametros de ingreso.");

                var strMaintenanceDate = DateTime.ParseExact($"{collection.MaintenanceDate} 00:00:00", "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                var parameters = new FormPlanDeMantencion.Parametros
                {
                    guidIdentifier = Guid.NewGuid(),
                    currentUserSelected = User.Identity.Name,
                    IdMaintenancePlanning = Int32.Parse(collection.MaintenanceTypeSelected),
                    IdMaintenanceType = Int16.Parse(collection.MaintenanceTypeSelected),
                    IdActivity = Int32.Parse(collection.ActivitySelected),
                    ElementType = Int32.Parse(collection.ElementTypeSelected),
                    ElementCode = Int32.Parse(collection.ElementCodeSelected),
                    MaintenanceDate = strMaintenanceDate,
                    IdResponsible = Int32.Parse(collection.ResponsibleSelected),
                    quantity = collection.quantity,
                    observations = collection.observations,
                    unit = collection.unit is null ? fnc_get_unit_of_measure(User.Identity.Name, collection.ActivitySelected).Select(x => Int16.Parse(x.Value)).FirstOrDefault() : (short)0
                };

                // PASO 1) -  GUARDADO DE NUEVA MANTENCION
                var dataSetSQL = planDeMantencionBusinessImpl.guardarMantencion(parameters);

                if (dataSetSQL.intError < 0)
                    throw new Exception(dataSetSQL.strError);

                var idMantencion = dataSetSQL.dsSQL.Tables[0].AsEnumerable()
                                             .Where(r => r.Field<Int32>("ID_ERROR") > 0)
                                             .Select(r => r.Field<Int32>("ID_ERROR")).DefaultIfEmpty((int)0).FirstOrDefault();

                // PASO 2) -  GUARDADO DE INTEGRACION
                var dataSetSQL2 = integracionSIGBusinessImpl.guardarMantencionIntegracion(idMantencion, parameters);

                if (dataSetSQL2.intError == 0)
                {
                    // PASO 2.2) - ENVIO DE DATOS AL WS
                    _integracionCapaSIGController.EnviarIntegracionWS(idMantencion, parameters);
                }


                // PASO 3) - Fin del proceso
                TempData["mensaje"] = $"La  Nueva Mantención Se Ha Agregado Satisfactoriamente.";
                TempData["tipo"] = "ok";

                return RedirectToAction("Agregar");
            }
            catch (Exception ex)
            {
                TempData["mensaje"] = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
                TempData["tipo"] = "error";
                return RedirectToAction("Agregar");
            }
        }

        #endregion

        #region PLAN DE MANTENCION - REPORTE
        [Authorize]
        public ActionResult Reportes()
        {
            var model = new BusinessEntity.FormModels.FormPlanDeMantencion.PlanReporte();
            try
            {
                model.allMaintenanceType = new List<SelectListItem> { new SelectListItem { Text = "Seleccione ...", Value = "0" },
                new SelectListItem { Text = "TODOS", Value = "1000" } };

                //============================================================================================
                // PASO 2) - CARGAMOS TAREAS DL PLAN VIGENTE SELECCIONADO
                //============================================================================================
                var dataSetSQL = configuracionBusinessImpl.ListAll(User.Identity.Name, string.Empty);

                if (dataSetSQL.intError != 0)
                    throw new Exception(dataSetSQL.strError);

                var allTableMaintenanceType = dataSetSQL.dsSQL.Tables[0].AsEnumerable()
                    .Where(r => r.Field<Int16>("gen_tip_idd") == 99 && r.Field<String>("gen_key_row").Equals("TIPO_MANT"))
                    .Select(r => new SelectListItem
                    {
                        Value = r.Field<Int32>("gen_val").ToString(),
                        Text = r.Field<String>("gen_nom")
                    }).OrderBy(x => x.Text).ToList();

                model.allMaintenanceType.AddRange(allTableMaintenanceType);
            }
            catch (Exception ex)
            {
                var exe = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            }

            return View(model);
        }

        public ActionResult BuscarByFilter(string inputString, string inputString2, string inputString3)
        {
            var collection = new BusinessEntity.FormModels.FormPlanDeMantencion.PlanReporte();
            Int16 maintenanceTypeSelected;

            try
            {
                var startDate = DateTime.ParseExact($"{inputString2} 00:00:00", "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                var endDate = DateTime.ParseExact($"{inputString3} 23:59:59", "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                short.TryParse(inputString, out maintenanceTypeSelected);

                var allTableMaintenanceType = fnc_get_all_maitenance(User.Identity.Name, maintenanceTypeSelected, (Int32)0, (Int32)0, startDate, endDate);

                collection.AllPlanIngresoList = allTableMaintenanceType.Any() ? allTableMaintenanceType : new List<BusinessEntity.PlanDeMantencionBusinessEntity.PlanIngreso>();
                collection.status = true;
            }
            catch (Exception ex)
            {
                collection = new BusinessEntity.FormModels.FormPlanDeMantencion.PlanReporte();
                collection.status = false;
                collection.message = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            }

            return PartialView("_PartialViewReporteMantenciones", collection);
        }

        [HttpPost]
        [Authorize]
        public ActionResult JSON_GenerarReporte(string inputString, string inputString2, string inputString3)
        {
            var collection = new BusinessEntity.FormModels.FormPlanDeMantencion.PlanReporte();
            Int16 maintenanceTypeSelected, dtColumnsCount;


            try
            {
                if (string.IsNullOrEmpty(inputString) || string.IsNullOrEmpty(inputString2) || string.IsNullOrEmpty(inputString3))
                    throw new Exception("Debe seleccionar al menos un reporte.");

                short.TryParse(inputString, out maintenanceTypeSelected);
                var startDate = DateTime.ParseExact($"{inputString2} 00:00:00", "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                var endDate = DateTime.ParseExact($"{inputString3} 23:59:59", "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                System.Threading.Thread.Sleep(1000);


                var fileExport = new
                {
                    guardarArchivo = false,
                    fileName = $"Reporte_Mantenciones_{DateTime.UtcNow.ToString("yyyy-MM-dd_HHmmssfff")}.xlsx",
                    Directorios = System.Configuration.ConfigurationManager.AppSettings["DirectorioInformes"].ToString(),
                    worksheet = $"{"Data_"}_{DateTime.UtcNow.ToString("yyyyMMdd_HHmmss")}"
                };



                var dataSetSQL = planDeMantencionBusinessImpl.ListIngresoFiltered(User.Identity.Name, maintenanceTypeSelected, startDate, endDate);

                if (dataSetSQL.intError != 0)
                    throw new Exception(dataSetSQL.strError);

                var allSolicitudesList = dataSetSQL.dsSQL.Tables[0].AsEnumerable()
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
                                                tra_ele_nom = r.Field<Int32>("tra_ele_id") > 0 ? $"{r.Field<String>("tra_ele_nom")} ({r.Field<Int32>("tra_ele_id")})" : "",
                                            }).OrderBy(x => x.tra_fec_ing).ToList();

                if (!allSolicitudesList.Any())
                    throw new Exception("No se encontraron resultados para exportar.");

                collection.AllPlanIngresoList = allSolicitudesList.Any() ? allSolicitudesList : new List<BusinessEntity.PlanDeMantencionBusinessEntity.PlanIngreso>();

                var _List = Newtonsoft.Json.JsonConvert.SerializeObject(allSolicitudesList);


                var intFila = 2;
                var wbListado = new XLWorkbook();
                var worksheet = wbListado.Worksheets.Add(fileExport.worksheet);

                worksheet.PageSetup.AdjustTo(25);
                worksheet.Style.Font.FontName = "Calibri";
                worksheet.Style.Font.FontSize = 10;

                #region HEADER
                //-- 1) - ENCABEZADO 
                worksheet.Cell(1, 1).SetValue("Tipo Mantención").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Font.SetBold(true);
                worksheet.Cell(1, 1).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                worksheet.Cell(1, 2).SetValue("Actividad").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Font.SetBold(true);
                worksheet.Cell(1, 2).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                worksheet.Cell(1, 3).SetValue("Elemento").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Font.SetBold(true);
                worksheet.Cell(1, 3).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                worksheet.Cell(1, 4).SetValue("Fecha Mantención").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Font.SetBold(true);
                worksheet.Cell(1, 4).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                worksheet.Cell(1, 5).SetValue("Responsable").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Font.SetBold(true);
                worksheet.Cell(1, 5).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                worksheet.Cell(1, 6).SetValue("Cantidad").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Font.SetBold(true);
                worksheet.Cell(1, 6).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                worksheet.Cell(1, 7).SetValue("Unidad").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Font.SetBold(true);
                worksheet.Cell(1, 7).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                worksheet.Cell(1, 8).SetValue("Observación").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Font.SetBold(true);
                worksheet.Cell(1, 8).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                dtColumnsCount = 8;
                #endregion

                foreach (var item in allSolicitudesList)
                {
                    //-- 1) - ENCABEZADO 
                    worksheet.Cell(intFila, 1).SetValue(item.tra_tip_mant_nom).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left); // Tipo Mantención
                    worksheet.Cell(intFila, 2).SetValue(item.tra_tip_act_nom).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);  // Actividad
                    worksheet.Cell(intFila, 3).SetValue(item.tra_ele_nom).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);      // Elemento
                    worksheet.Cell(intFila, 4).SetValue(item.tra_fec_ing).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);      // Fecha Instalación
                    worksheet.Cell(intFila, 5).SetValue(item.tra_res_nom).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);      // Responsable
                    worksheet.Cell(intFila, 6).SetValue(item.tra_can).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);          // Cantidad
                    worksheet.Cell(intFila, 6).SetValue(item.tra_can).Style.NumberFormat.Format = "#,##0.##";

                    worksheet.Cell(intFila, 7).SetValue(item.tra_uni_nom).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);      // Unidad
                    worksheet.Cell(intFila, 8).SetValue(item.tra_obs).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);      // 

                    intFila++;
                }

                var startCell = worksheet.Cell(1, 1);
                var endCell = worksheet.Cell(allSolicitudesList.Count + 1, dtColumnsCount);                //For 2d data, with 'n' no. of rows and columns.

                worksheet.Range(startCell, endCell).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                worksheet.Range(startCell, endCell).Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
                worksheet.Range(1, 1, 1, dtColumnsCount).Style.Fill.SetBackgroundColor(XLColor.Yellow);
                worksheet.Columns().AdjustToContents();

                if (fileExport.guardarArchivo)
                    wbListado.SaveAs(fileExport.Directorios + fileExport.fileName);

                var stream = new System.IO.MemoryStream();
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

        #region PLAN DE MANTENCION - JSON
        public JsonResult BuscarElementoByType(string inputString)
        {
            Int32 elementTypeSelected;

            try
            {
                Int32.TryParse(inputString, out elementTypeSelected);

                var dataSetSQL = planDeMantencionBusinessImpl.ListElementsByType(User.Identity.Name, elementTypeSelected);

                if (dataSetSQL.intError != 0)
                    throw new Exception(dataSetSQL.strError);

                var allList = dataSetSQL.dsSQL.Tables[0].AsEnumerable()
                                            .Select(r => new SelectListItem
                                            {
                                                Value = (r.Field<Int32>("codigo")).ToString(),
                                                Text = $"{r.Field<String>("nombre")} ({r.Field<String>("codigo_mop")})"
                                            }).OrderBy(x => x.Text).ToList();

                return Json(new { status = allList.Any(), list = allList, message = string.Empty });
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
        public JsonResult BuscarActivityByCode(string inputString)
        {
            Int32 activitySelected;

            try
            {
                if (string.IsNullOrEmpty(inputString))
                    return Json(new { status = false, message = "Faltan parametros de ingreso." });

                Int32.TryParse(inputString, out activitySelected);


                var parametros = new FormPlanDeMantencion.Parametros
                {
                    currentUserSelected = User.Identity.Name,
                    IdMaintenancePlanning = 0,
                    YearMaintenancePlanning = 0,
                };

                var dataSetSQL = planDeMantencionBusinessImpl.ListFilterActivityByPlan(parametros);

                if (dataSetSQL.intError != 0)
                    throw new Exception(dataSetSQL.strError);

                // PASO 1) - TIPOS DE ACTIVIDAD
                var filterTableActivity = dataSetSQL.dsSQL.Tables[0].AsEnumerable()
                                            .Where(r => r.Field<Int32>("valor") == activitySelected)
                                            .Select(r => new
                                            {
                                                id = r.Field<Int32>("id"),
                                                id_guid = r.Field<String>("id_guid"),
                                                llave = r.Field<String>("llave"),
                                                tipo = r.Field<Int16>("tipo"),
                                                subtipo = r.Field<Int16>("subtipo"),
                                                vigente = r.Field<bool>("vigente"),
                                                codigo = r.Field<String>("codigo"),
                                                nombre = r.Field<String>("nombre"),
                                                unidad_id = r.Field<Int16>("unidad_id"),
                                                unidad_nom = r.Field<String>("unidad_nom"),
                                                frecuencia_id = r.Field<Int16>("frecuencia_id"),
                                                frecuencia_nom = r.Field<String>("frecuencia_nom"),
                                                valor = r.Field<Int32>("valor"),
                                                grupo_id = r.Field<Int16>("grupo_id"),
                                                grupo_nombre = r.Field<String>("grupo_nombre"),
                                            }).FirstOrDefault();

                return Json(new { status = (bool)(filterTableActivity is null ? false : true), filteredActivity = filterTableActivity, message = (filterTableActivity is null ? "No se encontraron resultados" : string.Empty) });
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
        #endregion

        #region FUNCIONES DE VALOR
        public static Func<string, string, List<SelectListItem>> fnc_get_unit_of_measure = (strCurrentUser, inputString) =>
        {
            ConfiguracionBusinessImpl configuracionBusinessImpl = new ConfiguracionBusinessImpl();
            PlanDeMantencionBusinessImpl planDeMantencionBusinessImpl = new PlanDeMantencionBusinessImpl();
            var collection = new List<SelectListItem>();
            int activitySelected;

            try
            {
                /// paso 1) -  extraemos seun codigo de actividad (Obligatorio)
                if (string.IsNullOrEmpty(inputString))
                    throw new Exception("Faltan parametros de ingreso.");

                Int32.TryParse(inputString, out activitySelected);

                var parametros = new FormPlanDeMantencion.Parametros
                {
                    currentUserSelected = strCurrentUser,
                    IdMaintenancePlanning = 0,
                    YearMaintenancePlanning = 0,
                };

                var dataSetSQL = planDeMantencionBusinessImpl.ListFilterActivityByPlan(parametros);

                if (dataSetSQL.intError != 0)
                    throw new Exception(dataSetSQL.strError);

                // PASO 1) - TIPOS DE ACTIVIDAD
                collection = dataSetSQL.dsSQL.Tables[0].AsEnumerable()
                                            .Where(r => r.Field<String>("llave").Equals("ACTIVITY") && r.Field<Int32>("valor") == activitySelected)
                                            .Select(r => new SelectListItem
                                            {
                                                Value = (r.Field<Int16>("unidad_id")).ToString(),
                                                Text = r.Field<string>("unidad_nom")
                                            }).OrderBy(x => x.Text).ToList();

            }
            catch
            {
                return new List<SelectListItem>();
            }
            return collection;
        };

        public static Func<string, Int16, Int32, Int32, DateTime, DateTime, List<BusinessEntity.PlanDeMantencionBusinessEntity.PlanIngreso>> fnc_get_all_maitenance = (strCurrentUser, typeMaytenance, markerType, marker, startDate, endDate) =>
            {
                List<BusinessEntity.PlanDeMantencionBusinessEntity.PlanIngreso> allTableMaintenanceList = new List<BusinessEntity.PlanDeMantencionBusinessEntity.PlanIngreso>();
                PlanDeMantencionBusinessImpl planDeMantencionBusinessImpl = new PlanDeMantencionBusinessImpl();
                try
                {
                    // PASO 3) -  TABLA DE MANTENCION
                    var dataSetSQL = planDeMantencionBusinessImpl.ListIngresoFiltered(strCurrentUser, typeMaytenance, startDate, endDate);

                    if (dataSetSQL.intError != 0)
                        throw new Exception(dataSetSQL.strError);

                    allTableMaintenanceList = dataSetSQL.dsSQL.Tables[0].AsEnumerable()
                                                .Where(r => (typeMaytenance == 1000 || r.Field<Int32>("tra_tip_ele_id") == typeMaytenance) &&
                                                            (marker == 0 || r.Field<Int32>("tra_ele_id") == marker))
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

                }
                catch
                {

                    allTableMaintenanceList = new List<BusinessEntity.PlanDeMantencionBusinessEntity.PlanIngreso>();
                }

                return allTableMaintenanceList;
            };

        #endregion
    }
}