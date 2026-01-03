using System.Collections.Generic;
using System.Web.Mvc;
using System.Data;
using System;


namespace BusinessEntity.FormModels
{
    public class FormElementos
    {
        public string StoredProcedure { get; set; }
        public DataTable ElementosDataTable { get; set; }
        public List<SelectListItem> elementList { get; set; }
        public List<SelectListItem> allTypeElementList { get; set; }

        public int id_elemento{ get; set; }
        public int id_interno { get; set; }
        public string id_mop { get; set; }
        public Int16 id_tramo { get; set; }
        public short id_tipo_elemento { get; set; }


        public string nombre_tramo { get; set; }
        public string nombre_elemento { get; set; }
        public string nombre_tipo_elemento { get; set; }
        public string observacion_imagen_elemento { get; set; }
        public List<ElementoImagenBusinessEntity> imageList { get; set; }
        public BusinessEntity.UploadFileConfigBusinessEntity.ImagenUploadConfig uploadImageConfig { get; set; }
        public List<ElementoBusinessEntity> allElementList { get; set; }
    }

}
