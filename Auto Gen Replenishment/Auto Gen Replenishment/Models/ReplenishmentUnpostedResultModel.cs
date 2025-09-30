using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoGenReplenishment.Models
{
    internal class ReplenishmentUnpostedResultModel
    {
        public ReplenishmentUnpostedMeta Meta { get; set; } = new();
        public Location FromLocation { get; set; } = new();
        public Location ToLocation { get; set; } = new();
        //public int LineCount { get; set; }
        public List<ReplenishmentUnpostedLineResultModel> Lines { get; set; } = new();
    }
}
