using System;


namespace BusinessEntity
{
    public class ConfiguracionEntity
    {
        public Int32 cnf_cod { get; set; } //[int] IDENTITY(1,1) NOT NULL,
        public Int16 cnf_tip_cod { get; set; } //[smallint] NOT NULL,
        public string cnf_key { get; set; } //[varchar] (5) NOT NULL,
        public string cnf_nom { get; set; } //[varchar] (50) NULL,
        public string cnf_des { get; set; } //[varchar] (200) NULL,
        public Int16? cnf_num_001 { get; set; } //[smallint] NULL,
        public Int32? cnf_num_002 { get; set; } //[int] NULL,
        public Int64? cnf_num_003 { get; set; } //[bigint] NULL,
        public decimal? cnf_dec_001 { get; set; } //[decimal](18, 12) NULL,
        public decimal? cnf_dec_002 { get; set; } //[decimal](18, 12) NULL,
        public decimal? cnf_dec_003 { get; set; } //[decimal](18, 12) NULL,
        public DateTime? cnf_fec_001 { get; set; } //[datetime] NULL,
        public DateTime? cnf_fec_002 { get; set; } //[datetime] NULL,
        public DateTime? cnf_fec_003 { get; set; } //[datetime] NULL,
        public string cnf_str_001 { get; set; } //[varchar] (100) NULL,
        public string cnf_str_002 { get; set; } //[varchar] (1000) NULL,
        public string cnf_str_003 { get; set; } //[varchar] (max) NULL,
        public bool cnf_bit_001 { get; set; } //[bit] NULL,
        public bool cnf_bit_002 { get; set; } //[bit] NULL,
        public bool cnf_bit_003 { get; set; } //[bit] NULL,
        public bool cnf_vig { get; set; } //[bit] NOT NULL,
        public string cnf_cre_usr { get; set; } //[varchar] (10) NOT NULL,
        public DateTime? cnf_cre_fec { get; set; } //[datetime] NOT NULL,
        public string cnf_mod_usr { get; set; } //[varchar] (10) NULL,
        public DateTime? cnf_mod_fec { get; set; } //[datetime] NULL,
    }
}
