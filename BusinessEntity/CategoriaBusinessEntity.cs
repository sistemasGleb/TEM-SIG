using System;

namespace BusinessEntity
{
    public class CategoriaBusinessEntity
    {
        public int doc_cat_cod { get; set; }
        public string doc_cat_nom { get; set; }
        public string doc_cat_des { get; set; }
        public bool doc_cat_est { get; set; }
        public string doc_cat_cre_usr { get; set; }
        public DateTime? doc_cat_cre_fec { get; set; }
        public string doc_cat_mod_usr { get; set; }
        public DateTime? doc_cat_mod_fec { get; set; }
    }
}
