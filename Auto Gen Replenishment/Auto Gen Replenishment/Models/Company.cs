using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AutoGenReplenishment.Models
{
    [Table("SY_COMP")]
    internal class Company
    {
        // NAM, ADRS_1, ADRS_2, ADRS_3, CITY, STATE, ZIP_COD, CNTRY, PHONE_1, CONTCT_1, EMAIL_ADRS_1
        [Key]
        [Column("KEY_ID")]
        public int KeyId { get; set; }
        public string Nam { get; set; }
        [Column("ADRS_1")]
        public string? Adrs1 { get; set; }
        [Column("ADRS_2")]
        public string? Adrs2 { get; set; }
        [Column("ADRS_3")]
        public string? Adrs3 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        [Column("ZIP_COD")]
        public string? ZipCod { get; set; }
        public string? Cntry { get; set; }
        [Column("PHONE_1")]
        public string? Phone1 { get; set; }
        [Column("CONTCT_1")]
        public string? Contact1 { get; set; }
        [Column("EMAIL_ADRS_1")]
        public string? EmailAdrs1 { get; set; }

    }
}
