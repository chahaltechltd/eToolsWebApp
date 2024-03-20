<Query Kind="Program">
  <Connection>
    <ID>f43526e9-1488-421b-bd33-b585624089af</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Driver Assembly="(internal)" PublicKeyToken="no-strong-name">LINQPad.Drivers.EFCore.DynamicDriver</Driver>
    <Server>.</Server>
    <Database>eTools</Database>
    <DisplayName>eTools-Entity</DisplayName>
    <DriverData>
      <PreserveNumeric1>True</PreserveNumeric1>
      <EFProvider>Microsoft.EntityFrameworkCore.SqlServer</EFProvider>
    </DriverData>
  </Connection>
</Query>

void Main()
{
	try
	{
	//Test data to retrieve a suggested purchase Order when a current active order does not exist
	//PurchaseOrderDetailsServices_FetchPurchaseOrderDetails(4);
	
	//Test data to retrieve a purchase Order when a current (not yet placed) order exists.
	PurchaseOrderDetailsServices_FetchPurchaseOrderDetails(1);
	
	//Getting a list of all vendors
	GetAllVendors();
	}
	catch (AggregateException ex)
	{
		foreach (var error in ex.InnerExceptions)
		{
			error.Message.Dump();
		}
	}
	catch (ArgumentNullException ex)
	{
		GetInnerException(ex).Message.Dump();
	}
	catch (Exception ex)
	{
		GetInnerException(ex).Message.Dump();
	}
}

#region Methods
private Exception GetInnerException(Exception ex)
{
	while (ex.InnerException != null)
		ex = ex.InnerException;
	return ex;
}
#endregion

// You can define other methods, fields, classes and namespaces here

//Query Model
public class StockItem
{
  public int StockItemID {get; set;}
  public string Description {get; set;}
  public int QOH {get; set;}
  public int ROL {get; set;}
  public int QOO {get; set;}
  public int QTO {get; set;}
  public decimal Price {get; set;}
}


public class Vendor
{
  
  public string VendorPhone {get; set;}
  public string VendorCity {get; set;}
 
}

public class PurchaseOrder
{
  public int PurchaseOrderID {get; set;}
  public List<StockItem> OrderedItems {get; set;}
  public List<StockItem> VendorStockItems {get; set;}
   public decimal GST {get; set;}
  public decimal SubTotal {get; set;}
  public decimal Total {get; set;}
 
}
public class SelectionList
{
   public int ValueID {get; set;}
   public string DisplayText {get; set;}
 
}

public List<Vendor> DisplayVendorDetails(int VendorID)
{
   IEnumerable<Vendor> vendorDetail = Vendors
                                .Where(x => x.VendorID == VendorID )
								.Select(x => new Vendor{
								VendorPhone = x.Phone,
								VendorCity = x.City,
								})
								.Dump();
								
								return vendorDetail.ToList();
}


public List<PurchaseOrder> DisplayPO(int VendorID)
{
    IEnumerable<PurchaseOrder> purchaseOrder = PurchaseOrderDetails
	                                           .Where(x => x.PurchaseOrder.VendorID == VendorID
											   && x.PurchaseOrder.OrderDate == null)
	                                           .Select(x => new PurchaseOrder{
											   PurchaseOrderID = x.PurchaseOrderID,
											   OrderedItems = PurchaseOrderDetails
											                  .Where(s => s.PurchaseOrderID == x.PurchaseOrderID)
															  .Select(s => new StockItem{
															  StockItemID = s.StockItemID,
															  Description = s.StockItem.Description,
															  QOH = s.StockItem.QuantityOnHand,
															  ROL = s.StockItem.ReOrderLevel,
															  QOO = s.StockItem.QuantityOnOrder,
															  QTO = s.Quantity,
															  Price = s.PurchasePrice
															  }).ToList(),
											   VendorStockItems = StockItems
											                  .Where(v => v.VendorID == x.PurchaseOrder.VendorID
															  && v.StockItemID!= x.StockItemID)
															  .Select(v => new StockItem{
															  StockItemID = v.StockItemID,
															  Description = v.Description,
															  QOH = v.QuantityOnHand,
															  ROL = v.ReOrderLevel,
															  QOO = v.QuantityOnOrder,
															  QTO = x.Quantity,
															  Price = x.PurchasePrice
															  }).ToList(),
								GST = x.PurchaseOrder.TaxAmount,
								SubTotal = x.PurchaseOrder.SubTotal,
								Total = x.PurchaseOrder.TaxAmount + x.PurchaseOrder.SubTotal
															  
											   }).Dump();
											   return purchaseOrder.ToList();					                           
}




public List<PurchaseOrder> DisplaySuggestedPO(int VendorID)
{
 IEnumerable<PurchaseOrder> suggestedOrder = Vendors
                                               .Where(x => x.VendorID == VendorID)
	                                           .Select(x => new PurchaseOrder{
											   PurchaseOrderID = 0,
											  OrderedItems = StockItems
										                  .Where(s => s.VendorID == VendorID
														  && s.ReOrderLevel - ( s.QuantityOnOrder + s.QuantityOnHand) > 0)
														  .Select(s => new StockItem{
														  StockItemID = s.StockItemID,
														  Description = s.Description,
														  QOH = s.QuantityOnHand,
														  ROL = s.ReOrderLevel,
														  QOO = s.QuantityOnOrder,
														  QTO = 1,
														  Price = s.PurchasePrice
														  }).ToList(),
											   VendorStockItems = StockItems
										                  .Where(v => v.VendorID == VendorID
														  && v.ReOrderLevel - ( v.QuantityOnOrder + v.QuantityOnHand) < 0)
														  .Select(v => new StockItem{
														  StockItemID = v.StockItemID,
														  Description = v.Description,
														  QOH = v.QuantityOnHand,
														  ROL = v.ReOrderLevel,
														  QOO = v.QuantityOnOrder,
														  QTO = 0,
														  Price = v.PurchasePrice
														  }).ToList(),
														  	
											   }).Dump();
											   return suggestedOrder.ToList();
					                           											  
}



public void PurchaseOrderDetailsServices_FetchPurchaseOrderDetails(int vendorID)
{

    Vendors vendorExists = Vendors
	                       .Where(x => x.VendorID == vendorID)
						   .FirstOrDefault();
						   
	 List<Exception> errorList = new List<Exception>();

    if(vendorID == 0)	
	{
	   throw new ArgumentNullException("Please select a VendorID");
	}
	if(vendorExists == null)
	{
	   throw new Exception("Supplied Vendor does not exist.");
	}
    PurchaseOrderDetails activepurchaseOrder = PurchaseOrderDetails
	                                           .Where(x => x.PurchaseOrder.VendorID == vendorID
											   && x.PurchaseOrder.OrderDate == null)
											   .FirstOrDefault();
     DisplayVendorDetails(vendorID);
											   
	if(activepurchaseOrder != null)
	{
        
	    DisplayPO(vendorID);
	}
	else
	{
	 
	  DisplaySuggestedPO(vendorID);
	}
}

public List<SelectionList> GetAllVendors()
{
  IEnumerable<SelectionList> vendors = Vendors
                                        .Select(v => new SelectionList
										{
										   ValueID = v.VendorID,
										   DisplayText = v.VendorName
										}).Dump();
										return vendors.ToList();
}

