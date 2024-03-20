using SalesSystem.DAL;
using SalesSystem.Entities;
using SalesSystem.viewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SalesSystem.DAL;
using Microsoft.EntityFrameworkCore;

namespace SalesSystem.BLL
{
    public class SaleServices
    {
        #region Constructor and Context Dependency

        private readonly eToolsContext _context;
        internal SaleServices(eToolsContext context)
        {
            _context = context;
        }
        #endregion
        public static int SaleID { get; set; }
        public int PlaceOrder(char PayType, double Tax, double Total, int CID, int empID, List<AddItemInfo> CartList)

        {
            SaleServicTRXInfo SaleDetail = null;
            SaleDetailesInfo newSaleRec = new SaleDetailesInfo();
            try
            {
                SaleDetail = new SaleServicTRXInfo() { SaleID = 1, PaymentType = PayType, TaxAmount = Convert.ToDecimal(Tax), SubTotal = Convert.ToDecimal(Total), CouponID = CID, employeeID = empID };
                Sale newSales = new Sale() { PaymentType = PayType.ToString(), TaxAmount = Convert.ToDecimal(Tax), SubTotal = Convert.ToDecimal(Total), CouponID = CID, EmployeeID = empID, PaymentToken = Guid.NewGuid() };
                _context.Sales.Add(newSales);
                SaleDetail RecSale = new SaleDetail();
                    _context. SaveChanges();
                SaleID = newSales.SaleID;
               
                for (int i = CartList.Count - 1; i >= 0; i--)
                {
                    RecSale = new SaleDetail() { SaleID = newSales.SaleID, StockItemID = CartList[i].StockItemID, SellingPrice = Convert.ToDecimal(CartList[i].SellingPrice), Quantity = CartList[i].Quantity };
                    _context.SaleDetails.Add(RecSale);
                }
                _context.SaveChanges();
                List<StockItem> QuentityUpdate = new List<StockItem>();
                QuentityUpdate = _context.StockItems.ToList();
                int c = _context.StockItems.Count();
                StockItem QtyUpdate = new ();
                for (int i = CartList.Count - 1; i >= 0; i--)
                {
                    for (int j = c - 1; j >= 0; j--)
                    {

                        if (QuentityUpdate[j].StockItemID == CartList[i].StockItemID)
                        {
                            //   QuentityUpdate.Dump();
                          /*  QtyUpdate.Add(new StockItem()
                            {
                                //StockItemID = CartList[i].StockItemID,
                                Description = CartList[i].ItemName,
                                SellingPrice = Convert.ToDecimal (CartList[i].SellingPrice),
                                QuantityOnHand = CartList[i].QuentityOnHand,
                                Discontinued = CartList[i].Discounted,
                                CategoryID = CartList[i].CategoryID,
                              
                            });
                            
                            _context.StockItems.Update(QtyUpdate);*/
                        }

                    }
                }
                //QuentityUpdate.Dump();
                NewAddItem = null; 
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return SaleID;
        }

        //Reutrn and refund
        public void Return_Refund(int SaleID)
        {
            Sale Return = new Sale();
            int rqty = 1;

            if ((SaleID < 1))
            {
                throw new ArgumentNullException("Sale ID is missing");
            }

            StockItem sitem = new StockItem();
            List<SaleDetail> RSDetails = new List<SaleDetail>();
            RSDetails = _context.SaleDetails.Where(x => x.SaleID == SaleID).ToList();
            List<Return> rdetail = new List<Return>();
            List<StockItem> SItem = new List<StockItem>();
            SItem = _context.StockItems.ToList();
            Return = _context.Sales.Where(x => x.SaleID == SaleID).FirstOrDefault();
            if (Return == null)
            {

            }
            else
            {
                for (int i = RSDetails.Count() - 1; i >= 0; i--)
                {
                    for (int j = SItem.Count() - 1; j >= 0; j--)
                    {
                        sitem = SItem.Where(x => x.StockItemID == RSDetails[i].StockItemID).FirstOrDefault();
                    }
                    rdetail.Add(new Return() { item = sitem.Description, orgQty = RSDetails[i].Quantity, price = RSDetails[i].SellingPrice, rntQty = rqty });
                }

            }
        }
        public List<Return> Return_RefundFetch(int SaleID)
        {
            StockItem sitem = new StockItem();
            List<SaleDetail> RSDetails = new List<SaleDetail>();
            List<Return> rdetail = new List<Return>();
            List<StockItem> SItem = new List<StockItem>();
            SItem = _context.StockItems.ToList();

            if ((SaleID < 1))
            {
                throw new ArgumentNullException("Sale ID is missing");
            }
            else
            {

                RSDetails = _context.SaleDetails.Where(x => x.SaleID == SaleID).ToList();
                for (int i = RSDetails.Count() - 1; i >= 0; i--)
                {
                    for (int j = SItem.Count() - 1; j >= 0; j--)
                    {
                        sitem = SItem.Where(x => x.StockItemID == RSDetails[i].StockItemID).FirstOrDefault();
                    }
                    rdetail.Add(new Return()
                    {
                        StockItemID = sitem.StockItemID,
                        item = sitem.Description,
                        orgQty = RSDetails[i].Quantity,
                        price =
                        RSDetails[i].SellingPrice
                    });

                }

            }
            return rdetail;
        }

        public static List<AddItemInfo> NewAddItem = new();
        public List<AddItemInfo> AddToCart(int id, int Qty, List<AddItemInfo> AddItem)
        {

            try
                
            {
                foreach (var item in NewAddItem)
                {
                    if (item.StockItemID== id)
                    {
                        throw new Exception("Item Already exist in cart.");
                    }
                }
                if (id == 0)
                {
                    throw new Exception("Select Valed stock Item.");
                }
                if (Qty ==0)
                {
                    throw new Exception("Quentity must be greate then 0.");
                }
                else
                {


                    foreach (var itemItem in AddItem)
                    {
                        if (itemItem.StockItemID == id)
                        {
                            NewAddItem.Add(new AddItemInfo()
                            {
                                StockItemID = itemItem.StockItemID,
                                ItemName = itemItem.ItemName,
                                SellingPrice = itemItem.SellingPrice,
                                QuentityOnHand = itemItem.QuentityOnHand,
                                Discounted = itemItem.Discounted,
                                CategoryID = itemItem.CategoryID,
                                Quantity = Qty,
                                Total = itemItem.SellingPrice * Qty
                            });

                        }

                    }
                }
            }
            catch
            {

            }

            return NewAddItem;

        }

        public List<AddItemInfo> newItem()
        {
            return NewAddItem;
        }


        //Refresh

        public List<AddItemInfo> refresh(int StockItemID, int Qty)
        {
            AddItemInfo refresh = null;
            AddItemInfo checkItem = null;
            //AddItemInfo changeItem = null;
            int tracknumber = 0;

            //  we need a container to hold x number of Exception messages
            List<Exception> errorList = new List<Exception>();

            //	parameter validation
            if ((StockItemID < 1))
            {
                throw new ArgumentNullException("Stock Item ID is missing");
            }

            var count = NewAddItem.Count();
            if (count == 0)
            {
                throw new ArgumentNullException("No list of item submitted");
            }

            checkItem = NewAddItem
                        .Where(x => x.StockItemID.Equals(StockItemID))
                                .Select(x => x)
                                .FirstOrDefault();

            if (checkItem == null)
            {
                errorList.Add(new Exception($"List {StockItemID} does not exist in the cart."));
            }
            else
            {
                for (int i = NewAddItem.Count - 1; i >= 0; i--)
                {

                    checkItem.Quantity = Qty;
                    NewAddItem = NewAddItem.Where(w => w.StockItemID == StockItemID).Select(s => { s.Quantity = Qty; return s; }).ToList();
                }
            }
            return NewAddItem;
        }

        //remove
        public List<AddItemInfo> RemoveItem(int StockItemID)
        {
            //  local variables
            AddItemInfo checkItem = null;
            //AddItemInfo changeItem = null;

            List<Exception> errorList = new List<Exception>();

            //	parameter validation
            if ((StockItemID < 1))
            {
                throw new ArgumentNullException("Stock Item ID is missing");
            }


            var count = NewAddItem.Count();
            if (count == 0)
            {
                throw new ArgumentNullException("No list of item submitted");
            }

            checkItem = NewAddItem
                        .Where(x => x.StockItemID.Equals(StockItemID))
                                .Select(x => x)
                                .FirstOrDefault();

            if (checkItem == null)
            {
                errorList.Add(new Exception($"List {StockItemID} does not exist in the cart."));
            }
            else
            {
                if (errorList.Count() > 0)
                {
                    throw new AggregateException("Unable to remove request tracks.  Check concerns", errorList);
                }

                for (int i = NewAddItem.Count - 1; i >= 0; i--)
                {
                    if (NewAddItem[i].StockItemID == StockItemID)
                    {
                        NewAddItem.RemoveAt(i);
                    }
                }

            }
            return NewAddItem;
        }

    }
}