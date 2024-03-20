using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesSystem.viewModel
{
    public class AddItemInfo
    {
        public int StockItemID { get; set; }
        public string ItemName { get; set; }
        public double SellingPrice { get; set; }
        public int QuentityOnHand { get; set; }
        public bool Discounted { get; set; }
        public int CategoryID { get; set; }
        public int Quantity { get; set; }
        public double Total { get; set; }
    }
}
