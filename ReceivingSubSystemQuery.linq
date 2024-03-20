<Query Kind="Program">
  <Connection>
    <ID>a320cef0-b0eb-47e5-b4a0-cacf8ad54916</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Driver Assembly="(internal)" PublicKeyToken="no-strong-name">LINQPad.Drivers.EFCore.DynamicDriver</Driver>
    <Server>.</Server>
    <Database>eTools2021</Database>
    <DisplayName>eTools2021Entity</DisplayName>
    <DriverData>
      <PreserveNumeric1>True</PreserveNumeric1>
      <EFProvider>Microsoft.EntityFrameworkCore.SqlServer</EFProvider>
    </DriverData>
  </Connection>
</Query>

void Main()
{
	try{
    //Calling OutstandingOrderlist
	List<OutstandingOrderlist> OutstandingOrderlist = Order_OutstandingOrderlist();
	OutstandingOrderlist.Dump();
	//calling orderdetails
	List<OrderDetails> OrderDetails = Order_OrderDetails();
	OrderDetails.Dump();
	//calling unorderitems 
	
	
     
	
	}
	catch(ArgumentNullException ex)
	{
	GetInnerException(ex).Message.Dump();
	
	}
	catch(ArgumentException ex)
	{
	GetInnerException(ex).Message.Dump();
	}
	catch(AggregateException ex)
	{
	foreach(var error in ex.InnerExceptions)
	{
	error.Message.Dump();
	}
	}
	catch(Exception ex)
	{
	GetInnerException(ex).Message.Dump();
	}
}
#region Driver Methods
private Exception GetInnerException(Exception ex)
{
while (ex.InnerException != null)
ex = ex.InnerException;
return ex;

}
#endregion
#region CQRS Queries/command models
public class OutstandingOrderlist
{
public int OrderPOId{get;set;}
public DateTime OrderDate{get;set;}
public string VendorName{get;set;}
public string VendorContactPhone{get;set;}

}
public class OrderDetails
{

public string VendorName{get;set;}
public string ContactPhone{get;set;}
public List<StockItemDetail> OrderItems {get;set;}



}
public class StockItemDetail
{
public int StockItemID{get;set;}
public string StockItemDescription{get;set;}
public int QuantityOnOrder{get;set;}
public int QuantityOutstanding{get;set;}


}

public class UnorderedItems{
public int itemID{get;set;}
public string itemDescription{get;set;}
public string VendorPartNumber{get;set;}
public int Quantity{get;set;}


}



#endregion

#region Query OutstandingOrder

public List<OutstandingOrderlist> Order_OutstandingOrderlist()
{
IEnumerable<OutstandingOrderlist> results = PurchaseOrders
                                            .Where(x=>x.OrderDate != null && x.Closed == false)
											.Select(x=> new OutstandingOrderlist
											{
											OrderPOId = x.PurchaseOrderID,
											OrderDate =(DateTime) x.OrderDate,
											VendorName = x.Vendor.VendorName,
											VendorContactPhone =x.Vendor.Phone,
											
											
					});
					return results.ToList();
}


#endregion

#region Query OrderDetails
public List<OrderDetails> Order_OrderDetails()
{
IEnumerable<OrderDetails> results = PurchaseOrders
                                         .Where(x=>x.OrderDate != null && x.Closed == false)
											.Select(x=> new OrderDetails
											{
											
					                        VendorName = x.Vendor.VendorName,
											ContactPhone =x.Vendor.Phone,
											OrderItems = x.PurchaseOrderDetails
											             .Select(o=> new StockItemDetail
														 {
														 StockItemID=o.StockItemID,
														 StockItemDescription=o.StockItem.Description,
														 QuantityOnOrder=o.StockItem.QuantityOnOrder,
														 QuantityOutstanding=o.StockItem.QuantityOnHand
														 
														 }).ToList()
														 });
														 return results.ToList();
														 
											
											
					
}


#endregion

