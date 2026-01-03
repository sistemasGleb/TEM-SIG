using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace BusinessEntity
{
    public class ParametrosBusinessEntity
    {
        public string Param_sp_crud { get; set; }

        public string USUARIO { get; set; }
        public Int32 Param_id { get; set; }
        public string Param_id_int { get; set; }
        public string Param_id_mop { get; set; }
        public short Param_tra_id { get; set; }
        public byte Param_tip_ele { get; set; }
        public string Param_tip_ele_nom { get; set; }

        public string Param_ele_nom { get; set; }
       
        public Int32 Param_id_inv { get; set; }
        public decimal Param_dm_ini { get; set; }
        public Nullable<decimal> Param_dm_fin { get; set; }
        public decimal Param_crd_utm_est_ini { get; set; }
        public decimal Param_crd_utm_nor_ini { get; set; }
        public decimal Param_crd_utm_est_fin { get; set; }
        public decimal Param_crd_utm_nor_fin { get; set; }
        public string Param_crd_lat_ini { get; set; }
        public string Param_crd_lon_ini { get; set; }
        public string Param_crd_lat_fin { get; set; }
        public string Param_crd_lon_fin { get; set; }
        public bool Param_vig { get; set; }
        public string Param_obs { get; set; }

        //--- OBSERVACIONES
        public List<Bitacora> allBitacoraList { get; set; }

        //--- IMAGENES
        public List<ElementoImagenBusinessEntity> allImagenesList { get; set; }

        //--- TUPLA AVRIABLE---
        public List<SelectListItem> AllAccesoList { get; set; }
        public List<SelectListItem> AllPosicionList { get; set; }
        public List<SelectListItem> AllLadoList { get; set; }
        public List<SelectListItem> AllMaterialList { get; set; }
        public List<SelectListItem> AllColorList { get; set; }
        public List<SelectListItem> AllMacroubicacionList { get; set; }


        public int Param_Mac { get; set; }
        public byte Param_acc { get; set; } // ACCESO
        public byte Param_col { get; set; } // COLOR
        public byte Param_pos { get; set; }
        public Int16 Param_lad { get; set; }
        public byte Param_mat { get; set; }

        public string Param_mar { get; set; }           // MARCA
        public string Param_mod { get; set; }           // MODELO
        public byte? Param_gar { get; set; }             // GARANTIA
        public DateTime? Param_fec_ins { get; set; }    // FECHA INSTALACION
        public bool Param_fec_ins_val { get; set; }
        public string fecha_termino { get; set; }

        public byte? Param_pot { get; set; }            // POTENCIA
        public byte? Param_vda_utl { get; set; }         // VIDA UTIL
        public string Param_ind_pro { get; set; }       // INDICADOR DE PROTECCION
        public string Param_grd_imp { get; set; }       // GRADO IMPACTO

        //--- DROPDOWNLIST -----
        public List<SelectListItem> allTramoList { get; set; }
        public short usr_per_cod { get; set; }

        // Actualiacion

        public UploadFileConfigBusinessEntity.ImagenUploadConfig ImageUploadFileConfig { get; set; }
        public List<AttachedFile> AttachedFileList { get; set; }
    }

    public class AttachedFile
    {
        public string attached_guid { get; set; }
        public string attached_nombre { get; set; }
    }

    public class Bitacora
    {
        public short btc_tra_id { get; set; }
        public byte btc_tip_ele { get; set; }
        public short btc_ele_id { get; set; }
        public string btc_usr_cre { get; set; }
        public string btc_usr_fec { get; set; }
        public string btc_usr_obs { get; set; }
    }
}
