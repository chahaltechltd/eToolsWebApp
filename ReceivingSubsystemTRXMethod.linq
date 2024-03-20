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
	//calling unorderitems 
	List<UnorderedItems>UnOrderedItems=new List<UnorderedItems> ();
	UnOrderedItems.Add(new UnorderedItems()
	{
	itemID=2,
	itemDescription="efefe",
	VendorPartNumber=" frf3",
	Quantity=2
	});
    Add_UnOrderedItems(UnOrderedItems);
	
	UnOrderedItems.Dump();
	List<ReceiveOrder>Orders=new List<ReceiveOrder>();
	Orders.Add(new ReceiveOrder()
	{
	PurchaseOrderID=358,
     PurchaseOrderDetailID=10,
    QuantityReceived=12,
   QuantityReturned=2,
   ReturnReason="ddd"
	
	});
	int PurchaseOrderID=358;
	int EmployeeID=3;
	
	
	Order_ReceiveOrder(PurchaseOrderID,EmployeeID,Orders,UnOrderedItems);
	
     
	
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
public int PurchaseOrderDetailID{get;set;}
public int StockItemID{get;set;}
public string StockItemDescription{get;set;}
public int QuantityOnOrder{get;set;}
public int QuantityOutstanding{get;set;}


}
public class ReceiveOrder{
public int PurchaseOrderID{get;set;}
public int PurchaseOrderDetailID{get;set;}
public int QuantityReceived{get;set;}
public int QuantityReturned{get;set;}
public string ReturnReason{get;set;}



}
public class UnorderedItems{
public int itemID{get;set;}
public string itemDescription{get;set;}
public string VendorPartNumber{get;set;}
public int Quantity{get;set;}



}



#endregion

// You can define other methods, fields, classes and namespaces here


#region Command TRX

//Force close

public void Force_Close(string Reason,List<StockItemDetail>Stockitems,int PurchaseOrderID)
{

if(string.IsNullOrEmpty(Reason))
{
throw new Exception("$Reason required for force close");
}
else
{
List<Exception>errorlist=new List<Exception>();
var OutstandingOrder = (from x in PurchaseOrders 
                        where x.Closed== false && x.OrderDate != null
						select x).FirstOrDefault();
if(OutstandingOrder == null)
{
throw new Exception("$OrderOutstanding does not exist");

}
foreach(var item in Stockitems)
{
int StockItemId=(from x in PurchaseOrderDetails
                 where x.PurchaseOrderDetailID == item.PurchaseOrderDetailID
				 select x.StockItemID).FirstOrDefault();
		var itemExist = (from x in StockItems
		                 where x.StockItemID == StockItemId
						 select x).FirstOrDefault();
	if(itemExist == null)
	{
	errorlist.Add(new Exception("$Invalid item"));
	
	}

	
	if(errorlist.Count()>0){
	throw new Exception("Transaction contains errors");
	
	}
	else
	{
	PurchaseOrders PurchaseOrderExist=(from x in PurchaseOrders
	                    where x.PurchaseOrderID==PurchaseOrderID
						select x).FirstOrDefault();
	PurchaseOrderExist.Closed = true;
	PurchaseOrderExist.Notes=Reason;
	                             
    if(Stockitems == null)
	{
	throw new Exception("$ No items in the list");
	}
	if(itemExist != null){
    
	itemExist.QuantityOnOrder = itemExist.QuantityOnOrder - item.QuantityOutstanding;
	}
	StockItems.Update(itemExist);
}

SaveChanges();

}
}
}
public void Add_UnOrderedItems(List<UnorderedItems>UnorderedItems)
{
UnOrderedItems UnOrderedItemsExist = null;
//Adding items in the unordered item list
List<Exception> errorlist = new List<Exception>();
var ItemNotInPO = (from x in PurchaseOrderDetails
                    where x.StockItemID != x.StockItem.StockItemID
                       
					   select x).FirstOrDefault();
if(ItemNotInPO == null)
{
errorlist.Add(new Exception("$All the items sent by vendor are in Purchase order list"));
}
else{
foreach(var item in UnorderedItems)
{
if(item.Quantity < 1)
{
errorlist.Add(new Exception("$Item quantity must not be 0"));
}
else{
UnOrderedItemsExist = new UnOrderedItems();
UnOrderedItemsExist.ItemID = item.itemID;
UnOrderedItemsExist.ItemName= item.itemDescription;
UnOrderedItemsExist.VendorProductID=item.VendorPartNumber;
UnOrderedItemsExist.Quantity=item.Quantity;
UnOrderedItems.Add(UnOrderedItemsExist);
SaveChanges();

}

}
}

}


public void Order_ReceiveOrder(int PurchaseOrderID,int EmployeeID,List<ReceiveOrder>OrderReceived,List<UnorderedItems>UnorderedItems)
{
UnOrderedItems UnOrderedItemsExist= null;
//checking the entry of outstanding order
List<Exception> errorlist = new List<Exception>();
//
var OutstandingOrder =(from x in PurchaseOrders
                       where x.Closed==false && x.OrderDate != null
					   select x).FirstOrDefault();
if(OutstandingOrder == null)
{
throw new Exception("$There is no outstanding orders");

}
else
{
//receive order entry
ReceiveOrders newReceiveOrder = new ReceiveOrders();
newReceiveOrder.PurchaseOrderID= PurchaseOrderID;
newReceiveOrder.EmployeeID = EmployeeID;
newReceiveOrder.ReceiveDate = DateTime.Now;
foreach(var x in OrderReceived)
{
//Quantity received entry in the receiveorderdetail
if(x.QuantityReceived > 0)
{
ReceiveOrderDetails ReceiveOrderDetailExist = new ReceiveOrderDetails();
ReceiveOrderDetailExist.QuantityReceived = x.QuantityReceived;
ReceiveOrderDetails.Add(ReceiveOrderDetailExist);
//Decrement of Quantity on order After receiveng amount
int ItemID = (from y in PurchaseOrderDetails 
              where y.PurchaseOrderDetailID == x.PurchaseOrderDetailID 
			  select y.StockItemID ).FirstOrDefault();
			  StockItems StockItemExist = StockItems.Find(ItemID);
			  StockItemExist.QuantityOnOrder -=x.QuantityReceived;
			  if(StockItemExist.QuantityOnOrder <0)
			  {
			  StockItemExist.QuantityOnOrder=0;
			  }
			  //Increment of QuantityOnhand after receiving item
			  StockItemExist.QuantityOnHand += x.QuantityReceived;
			  StockItems.Update(StockItemExist);
}
else
{
errorlist.Add(new Exception("$Quantity received is not valid"));

}
if(x.QuantityReturned > 0)
{
//ReturnQuantity and Reason is updated
ReturnedOrderDetails ReturnOrderDetailExist = new ReturnedOrderDetails();
ReturnOrderDetailExist.Quantity = x.QuantityReceived;
ReturnOrderDetailExist.Reason =x.ReturnReason;
ReturnedOrderDetails.Add(ReturnOrderDetailExist);

}


}
//Adding unordered items to unpurchased items
foreach(var Unorderitem in UnorderedItems)
{
ReturnedOrderDetails newReturnItem = new ReturnedOrderDetails();
newReturnItem.ItemDescription = Unorderitem.itemDescription;
newReturnItem.Quantity=Unorderitem.Quantity;
newReturnItem.VendorStockNumber=Unorderitem.VendorPartNumber;
newReceiveOrder.ReturnedOrderDetails.Add(newReturnItem);
//Empty the cart
UnOrderedItems.Remove(UnOrderedItemsExist);

OutstandingOrder.ReceiveOrders.Add(newReceiveOrder);

}
}
//Order is checked to see if it can be closed 
int QuantityCount = 0;
var orderDetails = (from s in PurchaseOrderDetails
                     where s.PurchaseOrderID == OutstandingOrder.PurchaseOrderID
					 select s).ToList();
foreach(var item in orderDetails)
{
int existingQuantity =((from a in ReceiveOrderDetails 
                         where a.PurchaseOrderDetailID == item.PurchaseOrderDetailID
						 select a.QuantityReceived).Sum());
int newQuantity =((from b in OrderReceived
                   where b.PurchaseOrderDetailID == item.PurchaseOrderDetailID
				   select b.QuantityReceived).FirstOrDefault());
				   int receiveTotal = existingQuantity+newQuantity;
				   if(receiveTotal < item.Quantity)
				   {
				   QuantityCount++;
				   }
				  
}
if(QuantityCount == 0)
{
OutstandingOrder.Closed = true;
PurchaseOrders.Update(OutstandingOrder);


}
if(errorlist.Count() > 0)
{
throw new AggregateException("Unable to receive order.check concern", errorlist);

}
else{
SaveChanges();

}
					 


}

#endregion
