

using System;
using System.Web.Mvc;
using System.Collections.Generic;

namespace BusinessEntity.FormModels
{
    public class FormDocumentos
    {
            public List<RootList> allDocumentList { get; set; }

    }

    public class RootList
    {
        public short doc_cat_cod { get; set; }
        public string doc_cat_nom { get; set; }
        public List<DocumentoBusinessEntity> groupedList { get; set; }
    }

    public class Documento
    {
        public DocumentoBusinessEntity singleDocumento { get; set; }
        public List<SelectListItem> categorias { get; set; }
        public Int16 cod_categoria  { get; set; }
        public string doc_titulo{ get; set; }
        public string doc_descripcion { get; set; }
        public List<SelectListItem> AllowedFileExtensions { get; set; }
        public long maxFileSize { get; set; }
        public string maxFileSizeIso { get; set; }
    }
}
