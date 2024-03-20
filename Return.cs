using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesSystem.viewModel
{
    public class Return
    {
    
        public string item { get; set; }
        public int StockItemID { get; set; }
        public int orgQty { get; set; }
        public decimal price { get; set; }
        public int rntQty { get; set; }
    }
}
