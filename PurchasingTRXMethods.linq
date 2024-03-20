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
	try{
	
	//Test Data
	//Update PO details or create a new PO record 
		List<PurchaseOrderDetail> newItems = new List<PurchaseOrderDetail>();
		newItems.Add(new PurchaseOrderDetail()
			{
				PurchaseOrderDetailID = 40,
				StockItemID = 5587,
				QTO = 8,
				Price = 154.5600m
			});
		PurchaseOrderDetailsServices_UpdatePurchaseOrderDetails(newItems, 789, 2);	
		
	//Test data for place and delete PO.	
		PurchaseOrder po = new PurchaseOrder();
		po.PurchaseOrderID = 365;
		po.OrderDate = null;
		po.VendorID = 4;
		po.EmployeeID = 2;
		po.TaxAmount =2225.0000m;
		po.SubTotal =44500.0000m;
		po.Closed = false;
		po.Notes = null;
		
		//PurchaseOrderDetailServices_DeletePurchaseOrder(po);
		//PurchaseOrderDetailsServices_PlacePurchaseOrder(po);
		
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
//Model classes
public class PurchaseOrderDetail
{
  public int PurchaseOrderDetailID {get; set;}
  public int StockItemID {get; set;}
  public int QTO {get; set;}
  public decimal Price {get; set;} 
}

public class PurchaseOrder
{
  public int PurchaseOrderID {get; set;}
  public DateTime? OrderDate {get; set;}
  public int VendorID {get; set;}
  public int EmployeeID {get; set;}
  public decimal TaxAmount {get; set;}
  public decimal SubTotal {get; set;}
  public bool Closed {get; set;}
  public string Notes {get; set;}
}

public void PurchaseOrderDetailsServices_UpdatePurchaseOrderDetails(List<PurchaseOrderDetail> lineItems, int PurchaseOrderID, int VendorID)
{
	//assuming that VendorID will always be valid since we are retrieving VendorID's from a query to populate the list to display vendor selection.

    PurchaseOrderDetails itemDetails = null;
	PurchaseOrders existingpurchaseOrder = null;
	StockItems stockItemExists = null;
	
    List<Exception> errorList = new List<Exception>();
    
	//local variables
	decimal subTotal = 0.0m;
	decimal gst = 0.0m;
	
	
	var itemCount = lineItems.Count();
	if(itemCount == 0)
	{
	   throw new ArgumentNullException("Please add items to the order. Cannot create/ update an empty order.");
	}
	
	existingpurchaseOrder = PurchaseOrders
	                .Where(x => x.PurchaseOrderID == PurchaseOrderID)
					.Select(x => x)
					.FirstOrDefault()
					;
					
	if(existingpurchaseOrder.OrderDate != null)
	{
	  errorList.Add(new Exception("You are not allowed to update a placed order."));
	}
					
	if(existingpurchaseOrder != null)
	{
	   foreach(PurchaseOrderDetail item in lineItems)
		{
		   stockItemExists = StockItems
		                     .Where(x => x.StockItemID == item.StockItemID).Select(x => x).SingleOrDefault();
		   
		   itemDetails = PurchaseOrderDetails
		                 .Where(x => x.StockItemID == item.StockItemID
						 && x.PurchaseOrderID == PurchaseOrderID && x.PurchaseOrderDetailID == item.PurchaseOrderDetailID)
						 .FirstOrDefault();
			if(stockItemExists == null)
			{
				errorList.Add(new Exception(
	       $"The stockItem {item.StockItemID} does not exist."));
			}
		   if(item.QTO < 0 || item.Price < 0.0m)
		   {
			   errorList.Add(new Exception(
	       $"Price and QTO should be positive and non-zero."));
		   }
					 
		   if(itemDetails == null)
		   {
			itemDetails = new PurchaseOrderDetails();
			itemDetails.StockItemID = item.StockItemID;	   
			itemDetails.Quantity = item.QTO;
		    itemDetails.PurchasePrice = item.Price;
			existingpurchaseOrder.PurchaseOrderDetails.Add(itemDetails);
		   }
		   
		   else
		   {
		  
		    itemDetails.Quantity = item.QTO;
		    itemDetails.PurchasePrice = item.Price;
		    PurchaseOrderDetails.Update(itemDetails);

			}
			
			subTotal += (decimal)item.QTO * item.Price;
			gst += (((decimal)item.QTO * item.Price)* 5)/100;
			
		
	    } 
		
		existingpurchaseOrder.SubTotal = subTotal;
			existingpurchaseOrder.TaxAmount = gst;
			PurchaseOrders.Update(existingpurchaseOrder);
	}
	
	else
	{
	   
		existingpurchaseOrder = new PurchaseOrders();
		existingpurchaseOrder.OrderDate = null;
		existingpurchaseOrder.EmployeeID = 3;
		existingpurchaseOrder.VendorID = VendorID;
		existingpurchaseOrder.TaxAmount = gst;
		existingpurchaseOrder.SubTotal = subTotal;
		existingpurchaseOrder.Closed = false;
		existingpurchaseOrder.Notes = null;
		PurchaseOrders.Add(existingpurchaseOrder);
		
	}
		
	
	if (errorList.Count() > 0)
	{
		throw new AggregateException("Unable to add or update the purchase order details.  Check concerns", errorList);
	}
	else
	{
		SaveChanges();
	}
	
}


public void PurchaseOrderDetailsServices_PlacePurchaseOrder(PurchaseOrder purchaseOrder)
{
	List<Exception> errorList = new List<Exception>();
	PurchaseOrders placepurchaseOrder = null;
	List<PurchaseOrderDetails> purchaseOrderDetails = null;
	StockItems stockItem = null;
	purchaseOrderDetails = PurchaseOrderDetails
	                       .Where(x => x.PurchaseOrderID == purchaseOrder.PurchaseOrderID)
						   .Select(x => x)
						   .ToList();
						   
	placepurchaseOrder = PurchaseOrders
	                    .Where(x => x.PurchaseOrderID == purchaseOrder.PurchaseOrderID && purchaseOrder.OrderDate == null)
						.Select(x => x)
						.FirstOrDefault();
						
	
						   
	if(purchaseOrderDetails == null)
	{
	  errorList.Add(new Exception("The details for this order have not been found. Please update first."));
	}
	
    if(placepurchaseOrder == null)
	{
	  errorList.Add(new Exception("This order has already been placed."));
	}
	else
	{
	
	  
	  placepurchaseOrder.OrderDate = DateTime.Now;
	  PurchaseOrders.Update(placepurchaseOrder);
	  
	  foreach(var item in purchaseOrderDetails)
	  {
	       stockItem = StockItems
	           .Where(x => x.StockItemID == item.StockItemID).Select(x => x).FirstOrDefault();
			   
	            stockItem.QuantityOnOrder = item.Quantity;
	            StockItems.Update(stockItem);
	  }
	 
	  
	}
		
	if (errorList.Count() > 0)
	{
		throw new AggregateException("Unable to place the purchase order details.  Check concerns", errorList);
	}
	else
	{
		SaveChanges();
	}
	
	
}

public void PurchaseOrderDetailServices_DeletePurchaseOrder(PurchaseOrder purchaseOrder)
{
	//will be using EntityState.Deleted
	//not accessible through linqpad.
	List<Exception> errorList = new List<Exception>();
	PurchaseOrders currentOrder = PurchaseOrders
	                              .Where(x => x.PurchaseOrderID == purchaseOrder.PurchaseOrderID)
								  .Select(x => x)
	                              .FirstOrDefault();
								  
	PurchaseOrderDetails poDetails = PurchaseOrderDetails
	                                .Where(x => x.PurchaseOrderID == purchaseOrder.PurchaseOrderID)
								  .Select(x => x)
	                              .FirstOrDefault();
	if(currentOrder == null)
	{
		errorList.Add(new Exception($"The supplied order does not exist."));
	}
	if(currentOrder.OrderDate != null)
	{
		errorList.Add(new Exception($"The supplied order is placed already. You are not allowed to delete it."));
	}
	else
	{
	   PurchaseOrderDetails.Remove(poDetails);
	   PurchaseOrders.Remove(currentOrder);
	   
	}
	
	if (errorList.Count() > 0)
	{
		throw new AggregateException("Unable to delete the purchase order details.  Check concerns", errorList);
	}
	else
	{
		SaveChanges();
	}
	
}
