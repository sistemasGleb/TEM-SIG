using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace BusinessEntity
{
    public class UploadFileConfigBusinessEntity
    {
        public class DocumentoUploadConfig
        {  
        public string doc_nombre { get; set; }
        public List<SelectListItem> categorias { get; set; }
        public Int16 cod_categoria { get; set; }
        public string doc_titulo { get; set; }
        public string doc_descripcion { get; set; }
        public List<SelectListItem> AllowedFileExtensions { get; set; }
        public long maxFileSize { get; set; }
        public string maxFileSizeIso { get; set; }
        public int maxFileCount { get; set; }
        }

        public class ImagenUploadConfig
        {
            public string img_nombre { get; set; }
            public string img_descripcion { get; set; }
            public List<SelectListItem> img_AllowedFileExtensions { get; set; }

            public string img_JsonAllowedFileExtensions { get; set; }
            public long img_MaxFileSize { get; set; }
            public string img_MaxFileSizeIso { get; set; }
            public int img_MaxFileCount { get; set; }
        }
    }
}
