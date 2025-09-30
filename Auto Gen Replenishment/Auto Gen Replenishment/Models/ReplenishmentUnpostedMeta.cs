using System.ComponentModel.DataAnnotations.Schema;

namespace AutoGenReplenishment.Models
{
    internal class ReplenishmentUnpostedMeta
    {
        public string XFER_NO { get; set; }
        public DateTime SHIP_DAT { get; set; }
        public string FROM_LOC_ID { get; set; }
        public string TO_LOC_ID { get; set; }
        public string SHIP_BY { get; set; }
        public string ReplenishByName { get; set; }
        [Column("USR_calcMode")]
        public int CalcMode { get; set; }
        [Column("comment")]
        public string? COMMNT_1 { get; set; }
        public string? COMMNT_2 { get; set; }
        public string? COMMNT_3 { get; set; }
        public DateTime LST_MAINT_DT { get; set; }
        public string BAT_ID { get; set; }
    }
}
