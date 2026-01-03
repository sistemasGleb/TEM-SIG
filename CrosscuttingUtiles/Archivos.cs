
//using BusinessEntity;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace CrosscuttingUtiles
{
    /// <summary>
    /// Método que valida la existencia de una rchivo Físico
    /// </summary>
    public class Archivos
    {
        public static Func<int,string, string> fnc_FormatImageElementDirectory = (iRepeat,strCadena) =>
        {
            try
            {
                int iLength = strCadena.Trim().Length;
                var strRepeat = $"{string.Concat(Enumerable.Repeat("0", (iRepeat - iLength)))}{strCadena}";

                return strRepeat;
            }
            catch
            {
                return string.Concat(Enumerable.Repeat("0", iRepeat));
            }
        };


        public static Func<string, string, bool> fnc_FileExists = (strDirectorios, FileDownloadName) =>
        {
            try
            {
                if (string.IsNullOrEmpty(strDirectorios))
                    throw new Exception("Directorio es requerido");

                var im = @"" + strDirectorios + "/" + FileDownloadName;

                if (System.IO.File.Exists(@"" + im))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }

        };

        public static Func<string, DataTable, BusinessEntity.UploadFileConfigBusinessEntity.ImagenUploadConfig> fnc_ImageUploadDocumentConfig = (tipo_solicitud, dtTable) =>
        {
            BusinessEntity.UploadFileConfigBusinessEntity.ImagenUploadConfig config = new BusinessEntity.UploadFileConfigBusinessEntity.ImagenUploadConfig();
            try
            {
                // EXTENSIONES 
                var allExtensionesList = dtTable.AsEnumerable()
                    .Where(m => (string)m["gen_key_row"] == "IMG_EXT" && (bool)m["gen_row_vig"])
                    .Select(m => new SelectListItem
                    {
                        Value = (string)m["gen_nom"],
                        Text = (string)m["gen_nom"]
                    }).ToList();

                // TAMAÑO DEL ARCHIVO
                var maxFileSize = dtTable.AsEnumerable()
                    .Where(m => (string)m["gen_key_row"] == "IMG_SIZE" && (bool)m["gen_row_vig"])
                    .Select(m => new
                    {
                        maxFileSize = (Int32)m["gen_val"]
                    }).DefaultIfEmpty(new { maxFileSize = (Int32)10000 }).FirstOrDefault();

                // CANTIDAD DE ARCHIVO
                var maxFileCountSingle = dtTable.AsEnumerable()
                    .Where(m => (string)m["gen_key_row"] == "IMG_CANT" && (bool)m["gen_row_vig"])
                    .Select(m => (Int32)m["gen_val"]).DefaultIfEmpty((Int32)1).FirstOrDefault();

                config.img_AllowedFileExtensions = allExtensionesList;
                config.img_JsonAllowedFileExtensions = JsonConvert.SerializeObject(allExtensionesList.AsEnumerable().Select(x => x.Value).ToList<string>());
                config.img_MaxFileSize = Convert.ToInt64(maxFileSize.maxFileSize);
                config.img_MaxFileSizeIso = ByteSizeLib.ByteSize.FromKiloBytes(maxFileSize.maxFileSize).ToString();
                config.img_MaxFileCount = maxFileCountSingle;
            }
            catch
            {
                config = new BusinessEntity.UploadFileConfigBusinessEntity.ImagenUploadConfig();
            }

            return config;
        };
    }
}
