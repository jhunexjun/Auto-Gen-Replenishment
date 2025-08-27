using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoGenReplenishment.Models
{
    internal class ReplenishmentItemsByMinimum
    {
        [Key]
        public string ItemNoDims { get; set; }
        [Column("ITEM_NO")]
        public string ItemNo {  get; set; }
        [Column("TRK_METH", TypeName = "varchar(1)")]
        public string TrkMeth { get; set; }
        [Column("QTY_DECS")]
        public int QtyDecs { get; set; }
        public string Descr { get; set; }
        public string Dim1 { get; set; }
        public string Dim2 { get; set; }
        public string Dim3 { get; set; }
        [Column("STK_UNIT")]
        public string StkUnit { get; set; }
        public decimal FromLastCost { get; set; }
        public decimal FromQtyOnHand { get; set; }
        public decimal FromQtyCommitted { get; set; }
        public decimal FromQtyAvailable { get; set; }
        public decimal FromMinQty { get; set; }
        public decimal ToQtyOnHand { get; set; }
        public decimal ToQtyCommitted { get; set; }
        public decimal ToQtyAvailable { get; set; }
        public decimal ToMinQty { get; set; }
        public decimal ToMaxQty { get; set; }
        public decimal ReplenishQty { get; set; }        
    }
}
