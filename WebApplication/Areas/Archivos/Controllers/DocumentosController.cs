using BusinessEntity;
using BusinessEntity.FormModels;
using BusinessImpl;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace WebApplication.Areas.Archivos.Controllers
{
    public class DocumentosController : WebApplication.Controllers.BaseController
    {
        private readonly DocumentosBusinessImpl documentosBusinessImpl = new DocumentosBusinessImpl();
        private readonly CategoriaBusinessImpl categoriaBusinessImpl = new CategoriaBusinessImpl();
        private readonly LogBusinessImpl _logBusinessImpl = new LogBusinessImpl();
        private readonly ConfiguracionBusinessImpl _configuracionBusinessImpl = new ConfiguracionBusinessImpl();

        [Authorize]
        public ActionResult Listar()
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

            return View();
        }

        [Authorize]
        public ActionResult Subir()
        {
            Documento form = new Documento();

            try
            {
                // ---------------------------------------------------------------------------------------------
                // PASO 1) -  EXTRAE VALORES DE LA TABLA CONFIGURACION
                // ---------------------------------------------------------------------------------------------
                var dataSet = _configuracionBusinessImpl.ListAll(User.Identity.Name,string.Empty);

                if (dataSet.intError != 0)
                    throw new Exception(dataSet.strError);

                // PASO 0) - TABLA CONFIGURACION
                var allExtensionesList = dataSet.dsSQL.Tables[0].AsEnumerable()
                    .Where(m => (string)m["gen_key_row"] == "FILE_EXT" && (bool)m["gen_row_vig"])
                    .Select(m => new
                    {
                        codigo = (Int32)m["gen_id"],
                        glosa = (string)m["gen_nom"],
                        Valor = (Int32)m["gen_val"],
                    }).ToList();

                var maxFileSize = dataSet.dsSQL.Tables[0].AsEnumerable()
                    .Where(m => (string)m["gen_key_row"] == "MAX_FILE_SIZE" && (bool)m["gen_row_vig"])
                    .Select(m => new
                    {
                        maxFileSize = (Int32)m["gen_val"]
                    }).DefaultIfEmpty(new { maxFileSize = (Int32)0 }).FirstOrDefault();

                form.categorias = new List<SelectListItem> { new SelectListItem { Text = "SELECCIONE UNA OPCIÓN", Value = "0", Selected = true } };
                form.AllowedFileExtensions = allExtensionesList.AsEnumerable()
                                                                                .Select(m => new SelectListItem
                                                                                {
                                                                                    Value = m.glosa,
                                                                                    Text = m.glosa
                                                                                }).ToList();
                form.maxFileSize = maxFileSize.maxFileSize;
                form.maxFileSizeIso = ByteSizeLib.ByteSize.FromKiloBytes(maxFileSize.maxFileSize).ToString();

                // ---------------------------------------------------------------------------------------------
                // PASO 1) - TABLA CATEGORIAS
                // ---------------------------------------------------------------------------------------------
                var dataSetSQL = categoriaBusinessImpl.ListAll(User.Identity.Name);

                if (dataSetSQL.intError != 0)
                    throw new Exception(dataSetSQL.strError);

                var allCategoriaList = dataSetSQL.dsSQL.Tables[0].AsEnumerable()
                    .Where(m => (bool)m["doc_cat_est"])
                    .Select(m => new SelectListItem
                    {
                        Value = ((int)m["doc_cat_cod"]).ToString(),
                        Text = (string)m["doc_cat_nom"]
                    }).ToList();

                form.categorias.AddRange(allCategoriaList);

                if (TempData["mensaje"] != null)
                {
                    ViewData["mensaje"] = TempData["mensaje"];
                    ViewData["tipo"] = TempData["tipo"];
                    TempData["mensaje"] = null;
                }
            }
            catch (Exception ex)
            {
                form.categorias = new List<SelectListItem> { new SelectListItem { Text = "SELECCIONE UNA OPCIÓN", Value = "0", Selected = true } };
                form.AllowedFileExtensions = new List<SelectListItem> { new SelectListItem { Text = "pdf", Value = "0", Selected = true } };
                form.maxFileSize = (Int32)1000;
                form.maxFileSizeIso = ByteSizeLib.ByteSize.FromKiloBytes((Int32)1000).ToString();

                ViewData["mensaje"] = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
                ViewData["tipo"] = "error";
            }

            var AllowedFileExtensions = JsonConvert.SerializeObject(form.AllowedFileExtensions.Select(x => x.Value).ToList<string>());
            ViewData["formatos"] = AllowedFileExtensions;

            return View(form);
        }

        // GET: DocumentosController/ListarJSONDocumentos
        [HttpGet]
        public JsonResult ListarJSONDocumentos()
        {
            try
            {
                // PASO 1) - DIRECTORIO
                var strDirectorios = ConfigurationManager.AppSettings["DirectorioInformes"].ToString();
                if (string.IsNullOrEmpty(strDirectorios))
                    throw new Exception();

                // PASO 2) - GET DATA
                var dataSetSQL = documentosBusinessImpl.ListAll(User.Identity.Name);

                if (dataSetSQL.intError != 0)
                    throw new Exception(dataSetSQL.strError);

                // PASO 3) -  OBTENEMOS TODAS LAS CATEGORIAS Y SUS DOCUMENTOS
                var alldocumentList = dataSetSQL.dsSQL.Tables[0].AsEnumerable()
                    .Select(m => new DocumentoBusinessEntity
                    {
                        doc_nombre = (string)m["doc_nombre"],
                        doc_guid = (string)m["doc_guid"],
                        doc_titulo = (string)m["doc_titulo"],
                        doc_descripcion = (string)m["doc_descripcion"],
                        doc_directorio = (string)m["doc_directorio"],
                        doc_extension = (string)m["doc_extension"],
                        doc_size = (int)m["doc_size"],
                        doc_estado = (bool)m["doc_estado"],
                        doc_usr_cre_usr = (string)m["doc_usr_cre_usr"],
                        doc_usr_cre_nom = (string)m["doc_usr_cre_nom"],
                        doc_usr_cre_fec = (DateTime)m["doc_usr_cre_fec"],
                        doc_usr_mod_usr = (string)m["doc_usr_mod_usr"],
                        doc_usr_mod_nom = (string)m["doc_usr_mod_nom"],
                        doc_usr_mod_fec = (DateTime)m["doc_usr_mod_fec"],
                        doc_usr_cre_glosa = fnc_Datediff((DateTime)m["doc_usr_cre_fec"]),
                        doc_cat_cod = (byte)m["doc_cat_cod"],
                        doc_cat_nom = (string)m["doc_cat_nom"],
                        doc_file_exists = CrosscuttingUtiles.Archivos.fnc_FileExists(strDirectorios,(string)m["doc_nombre"]),
                        doc_file_delete = true
                    }).OrderByDescending(x => x.doc_usr_mod_fec).ToList();

                return Json(new { status = "success", list = alldocumentList, exception = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", exception = ex.InnerException == null ? ex.Message : ex.InnerException.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: DocumentosController/DeleteFilesSync
        [HttpGet]
        public JsonResult DeleteFilesSync(string pGuid)
        {
            System.Threading.Thread.Sleep(2000);

            try
            {
                var strPath = ConfigurationManager.AppSettings["DirectorioInformes"].ToString();

                //if (!DatosUsuario.fncValidarOpcionUsuario(User, 2))
                //    throw new Exception("Usuario sin acceso a eliminar documentos.");

                // PASO 0 - Validamos si viene GUID
                if (string.IsNullOrEmpty(pGuid))
                    throw new Exception("eL GUID del documento no es válido");

                // PASO 0 - Consulta
                var dataSetSQL = documentosBusinessImpl.ListAll(User.Identity.Name);

                if (dataSetSQL.intError != 0)
                    throw new Exception(dataSetSQL.strError);

                // PASO 1) -  OBTENEMOS TODAS LAS CATEGORIAS Y SUS DOCUMENTOS
                var alldocumentList = dataSetSQL.dsSQL.Tables[0].AsEnumerable()
                    .Where(x => pGuid.Contains((string)x["doc_guid"]))
                    .Select(m => new DocumentoBusinessEntity
                    {
                        doc_nombre = (string)m["doc_nombre"],
                        doc_guid = (string)m["doc_guid"],
                        doc_titulo = (string)m["doc_titulo"],
                        doc_descripcion = (string)m["doc_descripcion"],
                        doc_directorio = (string)m["doc_directorio"],
                        doc_extension = (string)m["doc_extension"],
                        doc_size = (int)m["doc_size"],
                        doc_estado = (bool)m["doc_estado"],
                        doc_usr_cre_usr = (string)m["doc_usr_cre_usr"],
                        doc_usr_cre_nom = (string)m["doc_usr_cre_nom"],
                        doc_usr_cre_fec = (DateTime)m["doc_usr_cre_fec"],
                        doc_usr_mod_usr = (string)m["doc_usr_mod_usr"],
                        doc_usr_mod_nom = (string)m["doc_usr_mod_nom"],
                        doc_usr_mod_fec = (DateTime)m["doc_usr_mod_fec"],
                        doc_usr_cre_glosa = fnc_Datediff((DateTime)m["doc_usr_cre_fec"]),
                        doc_cat_cod = (byte)m["doc_cat_cod"],
                        doc_cat_nom = (string)m["doc_cat_nom"],
                        doc_file_exists = CrosscuttingUtiles.Archivos.fnc_FileExists(strPath, (string)m["doc_nombre"]),
                        doc_file_delete = true
                    }).DefaultIfEmpty(new DocumentoBusinessEntity()).FirstOrDefault();

                // PASO 2) - ELIMINAMOS EL ARCHIVO FISICO
                var isFileExists = System.IO.File.Exists(Path.Combine(strPath, alldocumentList.doc_nombre));

                if (isFileExists)
                {
                    System.IO.File.Delete(Path.Combine(strPath, alldocumentList.doc_nombre));
                }

                // PASO 3) - ELIMINAMOS FILES DEL SERVIDOR
                var dataSetSQL2 = documentosBusinessImpl.DeleteByGuid(pGuid, User.Identity.Name);

                if (dataSetSQL2.intError != 0)
                    throw new Exception(dataSetSQL2.strError);

                return Json(new { status = "success", exception = $"El DOCUMENTO ha sido eliminado satisfactoriamente." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", exception = ex.InnerException == null ? ex.Message : ex.InnerException.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: DocumentosController/DownloadAttachment
        [Authorize]
        [HttpGet]
        public ActionResult DownloadAttachment(string fileGuid)
        {
            try
            {
                if (string.IsNullOrEmpty(fileGuid))
                    throw new Exception();

                var strDirectorios = ConfigurationManager.AppSettings["DirectorioInformes"].ToString();

                if (string.IsNullOrEmpty(strDirectorios))
                    throw new Exception();

                // PASO 2 - Consulta
                var dataSetSQL = documentosBusinessImpl.ListAll(User.Identity.Name);

                if (dataSetSQL.intError != 0)
                    throw new Exception(dataSetSQL.strError);

                // PASO 1) -  OBTENEMOS TODAS LAS CATEGORIAS Y SUS DOCUMENTOS
                var singleDdocumentList = dataSetSQL.dsSQL.Tables[0].AsEnumerable()
                    .Where(m => (string)m["doc_guid"] == fileGuid)
                    .Select(m => new DocumentoBusinessEntity
                    {
                        doc_nombre = (string)m["doc_nombre"],
                        doc_guid = (string)m["doc_guid"],
                        doc_directorio = (string)m["doc_directorio"],
                    }).Single();

                byte[] fileBytes = System.IO.File.ReadAllBytes(@"" + strDirectorios + singleDdocumentList.doc_nombre);

                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, singleDdocumentList.doc_nombre);
            }
            catch
            {
                return new HttpNotFoundResult();
            }
        }

        #region ADJUNTOS
        [HttpPost]
        public ActionResult CargarArchivoJSON(string strCategoria, string strTitulo, string strDescripcion)
        {
            //var blUpload = false;
            var str = new System.Text.StringBuilder();
            var uploadFile = new UploadFile();

            try
            {
                if (Request.Files.Count == 0)
                    return Json(new { error = $"No hay archivos seleccionados." });

                var strDirectorios = ConfigurationManager.AppSettings["DirectorioInformes"].ToString();

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

                    // PASO 2) - VALIDAMOS SI EXISTE EN BD
                    var dataSetSQL = documentosBusinessImpl.ListAll(User.Identity.Name);

                    if (dataSetSQL.intError != 0)
                        throw new Exception(dataSetSQL.strError);

                    // PASO 3) - OBTENEMOS TODAS LAS CATEGORIAS Y SUS DOCUMENTOS
                    var alldocumentList = dataSetSQL.dsSQL.Tables[0].AsEnumerable()
                        .Where(X => (string)X["doc_nombre"] == fname)
                        .Select(m => new DocumentoBusinessEntity
                        {
                            doc_guid = (string)m["doc_guid"]
                        }).OrderByDescending(x => x.doc_usr_mod_fec).ToList();

                    if (alldocumentList.Count > 0)
                        throw new Exception("El DOCUMENTO ya existe en los registros!.");

                    // PASO 4) - Valida directorio y archivo
                    var exists = Directory.Exists(strDirectorios);

                    if (!exists)
                        Directory.CreateDirectory(strDirectorios);

                    var isFileExists = System.IO.File.Exists(Path.Combine(strDirectorios, fname));
                    if (isFileExists)
                        throw new Exception("El DOCUMENTO ya existe en el repositorio!.");


                    // PASO 5) -  Get the complete folder path and store the file inside it.  
                    var fullFileName = Path.Combine(strDirectorios, fname);
                    file.SaveAs(fullFileName);

                    var collection = new DocumentoBusinessEntity()
                    {
                        doc_guid = Guid.NewGuid().ToString(),
                        doc_cat_cod = int.Parse(strCategoria),
                        doc_nombre = fname,
                        doc_titulo = strTitulo,
                        doc_descripcion = strDescripcion == null ? "" : strDescripcion,
                        doc_directorio = strDirectorios,
                        doc_extension = fname.Substring(fname.LastIndexOf('.'), fname.Length - fname.LastIndexOf('.')),// fname.Split('.')[1], JOEL.ESTAY.PDF
                        doc_size = (int)file.ContentLength,
                        doc_usr_cre_usr = User.Identity.Name,
                        doc_usr_mod_usr = User.Identity.Name
                    };

                    var dataSetSQL2 = documentosBusinessImpl.SaveDocument(collection);

                    if (dataSetSQL2.intError != 0)
                        throw new Exception(dataSetSQL.strError);
                }

                uploadFile.error = "";
                uploadFile.initialPreview = new List<string>();
                uploadFile.initialPreviewConfig = new List<UploadFilePreviewConfig>();
                uploadFile.initialPreviewAsData = true;

                // Returns message that successfully uploaded  
                //return Json("File Uploaded Successfully!");
                //blUpload = true;


                str.Append("<div class='alert alert-success alert-styled-left alert-arrow-left alert-dismissible'>");
                str.Append("<button type='button' class='close' data-dismiss='alert'><span>×</span></button>");
                str.Append(
                    $"<span class='font-weight-semibold text-capitalize'>El DOCUMENTO ({uploadFile.fileName.ToUpper()}) se ha cargado satisfactoriamente.  <a href='#' class='alert-link font-weight-bold' onclick='fnc_volver();' style='color: #0074cc;'>Aceptar</a></span>");
                str.Append("</div>");

                uploadFile.message = str.ToString();
                return Json(uploadFile);
            }
            catch (Exception ex)
            {
                str.Append("<div class='alert alert-danger alert-styled-left alert-arrow-left alert-dismissible'>");
                str.Append("<button type='button' class='close' data-dismiss='alert'><span>×</span></button>");
                str.Append(
                    $"<span class='font-weight-semibold text-capitalize'>{(ex.InnerException == null ? ex.Message : ex.InnerException.Message)}</span>");
                str.Append("</div>");

                uploadFile.message = str.ToString();
                return Json(new { error = $"Error : {(ex.InnerException == null ? ex.Message : ex.InnerException.Message)}" });
            }
        }
        #endregion

        #region Funciones y Delegados
        //public static Func<string, bool> fnc_FileExists = (FileDownloadName) =>
        //{
        //    try
        //    {
        //        var strDirectorios = ConfigurationManager.AppSettings["DirectorioInformes"].ToString();
        //        if (string.IsNullOrEmpty(strDirectorios))
        //            throw new Exception();

        //        if (System.IO.File.Exists(@"" + strDirectorios + FileDownloadName))
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    catch
        //    {
        //        return false;
        //    }

        //};
        public static Func<DateTime, string> fnc_Datediff = (dateTimeNow) =>
        {
            var _StringtDate = String.Empty;

            try
            {
                //Determine the number of days between the two dates.
                var minutes = (long)(DateTime.Now - dateTimeNow).TotalMinutes;
                var hours = (long)(DateTime.Now - dateTimeNow).TotalHours;
                var days = (long)(DateTime.Now - dateTimeNow).TotalDays;

                if (minutes < 1)
                {
                    _StringtDate = "Hace unos segundos";
                }

                else
                {
                    if (minutes == 1)
                    {
                        _StringtDate = "Hace 1 minuto";
                    }
                    else if (minutes > 1 && minutes < 60)
                    {
                        _StringtDate = "Hace " + minutes.ToString() + " minutos";
                    }
                    else
                    {
                        if (hours == 1)
                        {
                            _StringtDate = "Hace " + hours.ToString() + " hora";
                        }
                        else if (hours > 1 && hours < 24)
                        {
                            _StringtDate = "Hace " + hours.ToString() + " horas";
                        }
                        else
                        {
                            if (days == 1)
                                _StringtDate = "Hace " + days.ToString() + " dia";
                            else if (days > 1 && days <= 30)
                                _StringtDate = "Hace " + days.ToString() + " dias";
                            else
                                _StringtDate = dateTimeNow.ToString("dd/MM/yyyy"); // case sensitive
                        }
                    }
                }
            }
            catch
            {
                _StringtDate = "Sin Información";
            }

            return _StringtDate;
        };

        #endregion

    }
}