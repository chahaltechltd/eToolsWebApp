using SalesSystem.DAL;
using SalesSystem.viewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesSystem.BLL
{
    public class StockItemServices
    {
        #region Constructor and Context Dependency

        private readonly eToolsContext _context;
        internal StockItemServices(eToolsContext context)
        {
            _context = context;
        }
        #endregion

        public List<AddItemInfo> fetchStockItem(int cateID)
        {
            IEnumerable<AddItemInfo> display = _context.StockItems.Where (x => x.CategoryID== cateID).Select(x => new AddItemInfo 
            { StockItemID = x.StockItemID, ItemName = x.Description,
                SellingPrice = Decimal.ToDouble(x.SellingPrice),
                QuentityOnHand =x.QuantityOnHand, Discounted =x.Discontinued,
                CategoryID =x.CategoryID });
            return display.ToList();
        }

        

    }
}
