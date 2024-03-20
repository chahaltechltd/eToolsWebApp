<Query Kind="Program">
  <Connection>
    <ID>632fb5f7-1f3b-4718-8d13-1d2e40cbd63e</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Driver Assembly="(internal)" PublicKeyToken="no-strong-name">LINQPad.Drivers.EFCore.DynamicDriver</Driver>
    <Server>AMAN</Server>
    <Database>eTools2021</Database>
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
	
	List <categoriesInfo> cateInfo;	
	cateInfo = fetch_Categories ();
	
	int CID = 4;
	int SaleID =19;
	List <AddItemInfo> addInfo;
	addInfo = fetch_item_ByCategories(CID);
	
	List <AddItemInfo> addItemInfo;
	addItemInfo = AddItem();

cateInfo.Dump();
addInfo.Dump();
addItemInfo.Dump();
 Cart(addItemInfo);
 RemoveItem(34,addItemInfo);
refresh( 35, addItemInfo,  20)	;
Return_Refund(SaleID);	
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

public class AddItemInfo
{
public int StockItemID {get; set;}
public  string ItemName {get; set;}
public double SellingPrice {get; set;}
public int QuentityOnHand {get; set;}
public bool Discounted {get; set;}
public int CategoryID {get; set;}
public int Quantity {get; set;}
}

public class categoriesInfo
{
public int categoryID {get; set;}
public string Description {get; set; }
}

public class cartInfo
{
public int employeeID {get; set;}
public double total {get; set;}
//public List <AddItemInfo> AddItem {get; set;} 
}
//Command Models
public class SaleServicTRXInfo
{
public char PaymentType {get; set;}
public decimal TaxAmount {get; set;}
public decimal SubTotal {get; set;}
public int CouponID {get; set;}
public int employeeID {get; set;}
public DateTime SaleDate { get;set;}
public int SaleID {get; set;}
}

public class SaleDetailesInfo
{
public int SaleID {get; set;}
public int StockItemID {get; set;}
public double sellingPrice {get; set;}
public int Quantity {get; set;}
}
 public class Return
 {
 public string item{get; set;}
 public int orgQty{get; set;}
 public decimal price{get; set;}
 public int rntQty {get; set;}
 }
 
#region Fetch Methods
List <categoriesInfo> fetch_Categories ()
{
IEnumerable <categoriesInfo> item=  Categories. Select(x => new categoriesInfo {categoryID = x. CategoryID,Description =x.Description});
return item.ToList();
}
List <AddItemInfo> fetch_item_ByCategories(int CategoryID)
{
IEnumerable <AddItemInfo> item=  StockItems. Where (x => x .CategoryID == CategoryID) .Select(x => new AddItemInfo {SellingPrice =Decimal. ToDouble(x. SellingPrice) ,ItemName =x.Description,
QuentityOnHand = x.QuantityOnHand,Discounted = x.Discontinued, CategoryID = x.CategoryID,StockItemID =x.StockItemID});


return item.ToList();
}

#endregion

//Add Button for Shopping
#region AddItem

List <AddItemInfo> AddItem()
{
List <AddItemInfo> item =new List <AddItemInfo> ();

item.Add(new AddItemInfo()
{StockItemID = 23, ItemName = "Dewalt Multi Speed Drill",SellingPrice = 45.65,QuentityOnHand = 65,Discounted =true,CategoryID =4,Quantity = 10
});
item.Add(new AddItemInfo()
{StockItemID = 34, ItemName = "Dewalt Multi Speed Sander",SellingPrice = 49.65,QuentityOnHand = 15,Discounted =true,CategoryID =13,Quantity = 8
});
item.Add(new AddItemInfo()
{StockItemID = 35, ItemName = "Dewalt Combined Power Set",SellingPrice = 152.36,QuentityOnHand = 52,Discounted =true,CategoryID =7,Quantity = 12
});
item.Add(new AddItemInfo()
{StockItemID = 82, ItemName = "57mm #10 Wood Screws - 100",SellingPrice = 12.69,QuentityOnHand = 1230,Discounted =true,CategoryID =5,Quantity = 1
});
item.Add(new AddItemInfo()
{StockItemID = 83, ItemName = "72mm #10 Wood Screws - 100",SellingPrice = 12.99,QuentityOnHand = 1550,Discounted =true,CategoryID =5,Quantity = 3
});
return item;
}
#endregion

//Cart Button 
#region
public void Cart(List<AddItemInfo> CartList)
{
//response .redirect cart page
string name;
double Price;
double total; 
string DIS;
List<cartInfo> CartListInfo= new List<cartInfo>();
 for (int i = CartList.Count-1; i >=0; i--)
{
name= CartList[i].ItemName;
Price =CartList[i].SellingPrice;
total =CartList[i].SellingPrice*CartList[i].Quantity;
DIS = name +"   "+ Price + "    " + total ;
DIS.Dump();
CartListInfo .Add (new cartInfo(){employeeID =12,total =total});
}

CartListInfo.Dump();
CheckOut(CartList,CartListInfo);
}
#endregion

#region Cart Module

//Remove Button

public void  RemoveItem(int StockItemID,List <AddItemInfo> item)
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
	

	var count = item.Count();
	if (count == 0)
	{
		throw new ArgumentNullException("No list of item submitted");
	}

	checkItem = item
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
		
		 for (int i = item.Count-1; i >=0; i--)
{
    if (item[i].StockItemID == StockItemID)
    {
        item.RemoveAt(i);
    }
	}
		
	}
	item.Dump();
}

//refresh

public void refresh(int StockItemID,List <AddItemInfo> item, int Qty)
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
	
	var count = item.Count();
	if (count == 0)
	{
		throw new ArgumentNullException("No list of item submitted");
	}

	checkItem = item
				.Where(x => x.StockItemID.Equals(StockItemID))
						.Select(x => x)
						.FirstOrDefault();

	if (checkItem == null)
	{
		errorList.Add(new Exception($"List {StockItemID} does not exist in the cart."));
	}
	else
	{
	for (int i = item.Count-1; i >=0; i--)
	{

		checkItem.Quantity = Qty;
		item = item.Where(w => w.StockItemID == StockItemID).Select(s => { s.Quantity = Qty; return s; }).ToList();
	}
	}
	item.Dump();
}
//Checkout 

public void CheckOut(List<AddItemInfo> CartList,List<cartInfo> cartInfo)
{
// Redrict to check out page
cartInfo detail = cartInfo.FirstOrDefault();
double SubTotal=0;
double Tax =0;
double Discount =0;
int CouponID=4;
int employeeID =detail.employeeID;
string Dis;
//CartList.Dump();
double Total =0;
Total=cartInfo .Sum(x => x.total);
Tax = 10*Total/100;
Discount = 5*Total/100;
SubTotal = Total+Tax-Discount;
Dis="Tax="+Tax;
Dis.Dump();
Dis="Discount="+Discount;
Dis.Dump();
Dis="SubTotal="+SubTotal;
Dis.Dump();

//total.Dump();
PlaceOrder('C', Tax,SubTotal,CouponID,
employeeID,CartList);
}

//Shopping
public void Shopping()
{
// Redrict to shopping page
}
#endregion

//PlaceOrder Button
#region
public void PlaceOrder(char PayType,double Tax,double Total,int CID,
int empID,List<AddItemInfo>CartList)

{
SaleServicTRXInfo SaleDetail= null;
SaleDetailesInfo newSaleRec=new SaleDetailesInfo (); 
try
{
SaleDetail=new SaleServicTRXInfo( ){SaleID =1,PaymentType =PayType,TaxAmount=Convert.ToDecimal(Tax),SubTotal=Convert.ToDecimal(Total),CouponID=CID,employeeID=empID};
Sales newSales =new Sales(){PaymentType ="C",TaxAmount=Convert.ToDecimal(Tax),SubTotal=Convert.ToDecimal(Total),CouponID=CID,EmployeeID=empID,PaymentToken =Guid.NewGuid()};
Sales.Add(newSales);
SaleDetails RecSale = new SaleDetails(); 
SaveChanges();
 for (int i = CartList.Count-1; i >=0; i--)
{
RecSale = new SaleDetails(){ SaleID = newSales.SaleID,StockItemID= CartList[i].StockItemID,SellingPrice = Convert.ToDecimal(CartList[i].SellingPrice), Quantity = CartList[i].Quantity};
SaleDetails .Add(RecSale);
}
SaveChanges();
List<StockItems> QuentityUpdate = new List<StockItems>();
QuentityUpdate = StockItems. ToList();
int c=StockItems.Count();
StockItems QtyUpdate = new StockItems();
for (int i = CartList.Count-1; i >=0; i--)
{
	for (int j = c-1; j >=0; j--)
	{

	if(QuentityUpdate [j] .StockItemID ==CartList[i].StockItemID)
	{
//QuentityUpdate.Dump();
	//QuentityUpdate = QuentityUpdate.Where(w => w.StockItemID == CartList[i].StockItemID).Select(s => { s.QuantityOnHand =CartList[i]. Quantity; return s; }).ToList();
	}
	
	}
}
//QuentityUpdate.Dump();

}
catch(Exception ex)
{
throw ex ;
}
}
#endregion

//Refunds/Returns
#region
public void Return_Refund(int SaleID)
{
Sales Return=new Sales ();
int rqty = 1;

if ((SaleID < 1))
	{
		throw new ArgumentNullException("Sale ID is missing");
	}
	
StockItems sitem = new StockItems ();
List<SaleDetails>  RSDetails = new List<SaleDetails>();
RSDetails= SaleDetails.Where (x=> x. SaleID== SaleID).ToList();
List<Return> rdetail = new List<Return>();
List<StockItems> SItem = new List<StockItems>();
SItem= StockItems . ToList();
Return = Sales. Where (x=> x. SaleID== SaleID).FirstOrDefault();
if(Return ==null)
{
 
}
else
{
for(int i =RSDetails.Count()-1;i  >=0; i--)
{
for(int j =SItem.Count()-1;j  >=0; j--)
{
sitem= SItem . Where(x => x .StockItemID == RSDetails [i].StockItemID).FirstOrDefault();
}
rdetail . Add(new Return(){item =sitem.Description,orgQty =RSDetails[i].Quantity,price = RSDetails[i].SellingPrice,rntQty =rqty });
}
//CartList.Dump();
rdetail.Dump();
}
}
#endregion