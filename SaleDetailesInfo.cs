using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesSystem.viewModel
{
    public class SaleDetailesInfo
    {
        public int SaleID { get; set; }
        public int StockItemID { get; set; }
        public double sellingPrice { get; set; }
        public int Quantity { get; set; }
    }
}
