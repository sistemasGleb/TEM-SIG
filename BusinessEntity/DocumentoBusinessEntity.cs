using System;


namespace BusinessEntity
{
    public class DocumentoBusinessEntity
    {
        public string doc_nombre { get; set; }
        public string doc_guid { get; set; }
        public string doc_titulo { get; set; }
        public string doc_descripcion { get; set; }
        public string doc_directorio { get; set; }
        public string doc_extension { get; set; }
        public int doc_size { get; set; }
        public bool doc_estado { get; set; }
        public string doc_usr_cre_usr { get; set; }
        public string doc_usr_cre_nom { get; set; }
        public DateTime doc_usr_cre_fec { get; set; }
        public string doc_usr_mod_usr { get; set; }
        public string doc_usr_mod_nom { get; set; }
        public DateTime doc_usr_mod_fec { get; set; }
        public string doc_usr_cre_glosa { get; set; }
        public int doc_cat_cod { get; set; }
        public string doc_cat_nom { get; set; }
        public bool doc_file_exists { get; set; }
        public bool doc_file_delete { get; set; }
    }
}
