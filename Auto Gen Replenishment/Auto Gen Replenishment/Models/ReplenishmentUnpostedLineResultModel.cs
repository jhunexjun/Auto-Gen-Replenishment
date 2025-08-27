using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace AutoGenReplenishment.Models
{
    internal class ReplenishmentUnpostedLineResultModel
    {
        [Key]
        public string ItemNoDims { get; set; }
        [NotMapped]
        public string ItemNo
        {
            get => ITEM_NO; set => ITEM_NO = value;
        }
        public string ITEM_NO { get; set; }
        public string? FromBin1 { get; set; }
        public string? FromBin2 { get; set; }
        public string? FromBin3 { get; set; }
        public string? FromBin4 { get; set; }
        public string? ToBin1 { get; set; }
        public string? ToBin2 { get; set; }
        public string? ToBin3 { get; set; }
        public string? ToBin4 { get; set; }
        public string TrkMeth { get; set; }
        public decimal QtyDecs { get; set; }
        public string Descr { get; set; }
        public string Dim1 { get; set; }
        public string Dim2 { get; set; }
        public string Dim3 { get; set; }
        [NotMapped]
        public string StkUnit
        {
            get => STK_UNIT; set => STK_UNIT = value;
        }
        public string STK_UNIT { get; set; }
        public decimal FromLastCost { get; set; }
        public decimal FromQtyOnHand { get; set; }
        public decimal FromQtyCommitted { get; set; }
        public decimal FromQtyAvailable { get; set; }
        public decimal FromMinQty { get; set; }
        public decimal FromMaxQty { get; set; }
        public decimal FromMTDAvgSales { get; set; }
        public decimal FromDASQ { get; set; }
        public decimal ToQtyOnHand { get; set; }
        public decimal ToQtyCommitted { get; set; }
        public decimal ToQtyAvailable { get; set; }
        public decimal ToMinQty { get; set; }
        public decimal ToMaxQty { get; set; }
        public decimal ReplenishQty { get; set; }
        public decimal ToMTDAvgSales { get; set; }
        public decimal ToDASQ { get; set; }
    }
}
