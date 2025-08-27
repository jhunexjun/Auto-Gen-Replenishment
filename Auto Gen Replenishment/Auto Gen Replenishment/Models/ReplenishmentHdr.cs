using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoGenReplenishment.Models
{
    internal class ReplenishmentHdr
    {
        [Key]
        public int DocNo { get; set; }
        [Required]
        public DateTime ReplenishmentDate { get; set; }
        [Required]
        public string ReplenishmentFrom { get; set; }
        [Required]
        public string ReplenishmentTo { get; set; }
        [Required]
        public string ReplenishmentBy { get; set; }
        [Required]
        public int CalcMode { get; set; }
        public string? Comment1 { get; set; }
        public string? Comment2 { get; set; }
        public string? Comment3 { get; set; }
        public DateTime SavedUtcDate { get; set; }
        public string XFerNo { get; set; }
        public string TransferOutEventNo { get; set; }
        public DateTime TransferOutDtPosted { get; set; }
        public string TransferInEventNo { get;set; }
        public DateTime TransferInDtPosted { get; set; }
        public string ErrorMsg { get; set; }
    }
}
