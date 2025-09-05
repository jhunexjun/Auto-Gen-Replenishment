using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoGenReplenishment.Models
{
    internal class ReplenishmentUnpostedMeta
    {
        public string DocNo { get; set; }
        public DateTime ReplenishDate { get; set; }
        public string ReplenishFrom { get; set; }
        public string ReplenishTo { get; set; }
        public string ReplenishBy { get; set; }
        public string ReplenishByName { get; set; }
        public int CalcMode { get; set; }
        [Column("comment")]
        public string? Comment1 { get; set; }
        public string? Comment2 { get; set; }
        public string? Comment3 { get; set; }
    }
}
