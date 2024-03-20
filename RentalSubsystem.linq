<Query Kind="Program">
  <Connection>
    <ID>48907362-94f5-49bd-b790-585cbbf5ffde</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Driver Assembly="(internal)" PublicKeyToken="no-strong-name">LINQPad.Drivers.EFCore.DynamicDriver</Driver>
    <Server>DESKTOP-H8GU78A\SQLEXPRESS</Server>
    <Database>eTools2021</Database>
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
	
	#region Query method
    // For renting
	
	int customerid = 1;
	int employeeid = 86;
	int couponid = 4;
	List<RentalEquipmentInfo> rentingequipments = new List<RentalEquipmentInfo>(){new RentalEquipmentInfo(){RentalEquipmentID = 1, Description = "Dewalt", SerialNumber = "546TR4", Rate = 13.00m, OutCondition = "good"}};
	
	
	//For return
	int rentalId = 2;
	int employeeId = 22;
	string paymentType = "C";
	List<UpdateRentalEquipmentInfo> updateequipments = new List<UpdateRentalEquipmentInfo>(){new UpdateRentalEquipmentInfo(){RentalEquipmentID = 1, InCondition = "good", Available = true, Comments = "nice"}};
	
	#endregion
	
	
	#region Command method 
	// For renting
	Renting_Equipment(customerid, employeeid, couponid, rentingequipments);
	
	
	//For return
	Return_Equipment (employeeId, rentalId, paymentType, updateequipments);
	#endregion
	
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

	private Exception GetInnerException(Exception ex)
	{
	    while (ex.InnerException != null)
	      ex = ex.InnerException;
	    return ex;
	}
	// Models
//For renting
public class Customer
{
  public int CustomerID {get; set;}
  public string FullName {get; set;}
  public string Address {get; set;}
}

public class CustomerInfo
{
  public int CustomerID {get; set;}
  public string FullName {get; set;}
  public string Address {get; set;}
  public string City {get; set;}
}


public class CouponsList 
{
  public int CouponID {get; set;}
  public string CouponIDValue {get; set;}
  public int CouponDiscount {get; set;}
  public DateTime StartDate {get; set;}
  public DateTime EndDate {get; set;}
}

public class RentalEquipmentInfo
{
   public int RentalEquipmentID {get; set;}
   public string Description {get; set;}
   public string SerialNumber {get; set;}
   public decimal Rate {get; set;}
   public string OutCondition {get; set;}
   
}

public class RentalList 
{
  public string FullName {get; set;}
  public string ContactPhone {get; set;}
  public DateTime RentalDateOut {get; set;}
  public int RentalID {get; set;}
}

public class EquipmentAvailable 
{
   public int RentalEquipmentID {get; set;}
   public string Description {get; set;}
   public string SerialNumber {get; set;}
   public decimal Rate {get; set;}  
}

public class RentalRecord
{
  public int EmployeeID {get; set;}
  public int CustomerID {get; set;}
  public int? CouponID {get; set;}
  public decimal SubTotal {get; set;}
  public decimal TaxAmount {get; set;}
  public DateTime RentalDateOut {get; set;}
  public DateTime RentalDateIn {get; set;}
  public Char PaymentType {get; set;}
}


public class UpdateRentalEquipmentInfo
{
  public int RentalEquipmentID {get; set;}
  public string InCondition {get; set;}
  public bool Available {get; set;}
  public string Comments {get; set;}
}

// You can define other methods, fields, classes and namespaces here

//query models
public List<Customer> Customer_ByPhone(string contactphone)
{

var customer = Customers
.Where(x=>x.ContactPhone == contactphone)
.Select(x => new Customer
{
   CustomerID = x.CustomerID,
   FullName = x.FirstName + " " + x.LastName,
   Address = x.Address
});
return customer.ToList();
}

public List<CustomerInfo> Customer_ByID(int customerid)
{

var customer = Customers
.Where(x=>x.CustomerID == customerid)
.Select(x => new CustomerInfo
{
  CustomerID = x.CustomerID,
  FullName = x.FirstName + " " + x.LastName,
  Address = x.Address,
  City = x.City
});
return customer.ToList();
}


public List<CouponsList> Coupons_ByValue(string value)
{
var coupon = Coupons
.Where(x=> x.CouponIDValue == value)
.Select(x=> new CouponsList
{
   CouponID = x.CouponID,
   CouponIDValue = x.CouponIDValue,
   CouponDiscount = x.CouponDiscount,
   StartDate = x.StartDate,
   EndDate = x.EndDate
});
return coupon.ToList();
}

public List<RentalList> Rentals_ByPhone(string phone)
{
var rentals = Rentals
.Where(x=> x.Customer.ContactPhone == phone)
.Select(x=> new RentalList
{
   RentalID = x.RentalID,
   FullName = x.Customer.FirstName + " " + x.Customer.LastName,
   ContactPhone = x.Customer.ContactPhone,
   RentalDateOut = x.RentalDateOut
  
});
return rentals.ToList();
}

public List<RentalList> Rentals_ByRentalID(int rentalId)
{
var rentals = Rentals
.Where(x=> x.RentalID == rentalId)
.Select(x=> new RentalList
{
   RentalID = x.RentalID,
   FullName = x.Customer.FirstName + " " + x.Customer.LastName,
   ContactPhone = x.Customer.ContactPhone,
   RentalDateOut = x.RentalDateOut
  
});
return rentals.ToList();
}

public List<RentalEquipmentInfo> Fetch_Equipments(int rentalid)
{
var rentals = RentalDetails
.Where(x => x.RentalID == rentalid)
.Select(x => new RentalEquipmentInfo
{
  RentalEquipmentID= x.RentalEquipment.RentalEquipmentID,
  Description = x.RentalEquipment.Description,
  SerialNumber = x.RentalEquipment.SerialNumber,
  Rate = x.RentalRate,
  OutCondition = x.OutCondition
});
return rentals.ToList();
}

public List<EquipmentAvailable> Get_Equipments()
{
var equipments = RentalEquipments
.Select(x => new EquipmentAvailable
{ 
  RentalEquipmentID = x.RentalEquipmentID,
  Description = x.Description,
  SerialNumber = x.SerialNumber,
  Rate = x.DailyRate
});
return equipments.ToList();
}


public List<UpdateRentalEquipmentInfo> update_return(int rentalid)
{
var returns = RentalDetails
.Where(x => x.RentalID == rentalid)
.Select(x => new UpdateRentalEquipmentInfo
{
  RentalEquipmentID= x.RentalEquipment.RentalEquipmentID,
  InCondition = x.InCondition,
  Available = x.RentalEquipment.Available,
  Comments = x.Comments
});
return returns.ToList();

}


//command models
public void Renting_Equipment ( int customerid, int employeeid, int? couponid, List<RentalEquipmentInfo> rentingequipments)
{
   Customers customerExists = null;
   Employees employeeExists = null;
   Coupons couponExists = null;
   Coupons validation = null;
   Rentals record = null;
   RentalDetails detail = null;
   RentalEquipment equipment = null;
   RentalEquipment available = null;
   int rentalDays = 1;
   DateTime rentalOut = DateTime.Now;
   DateTime rentalIn = rentalOut.AddDays(1);
   //Business logic and parameter exceptions
	    List <Exception> errorList = new List<Exception>();
	
	
	if (customerid == 0)
	{
	   throw new ArgumentNullException("CustomerID is missing");
	}
	if (employeeid == 0)
	{
	   throw new ArgumentNullException("EmployeeID is missing");
	}
	
	customerExists = Customers
	             .Where(x => x.CustomerID == customerid
				 )
				 .Select(x => x)
				 .FirstOrDefault();
	if(customerExists == null)
	{
	errorList.Add(new ArgumentNullException("CustomerID not exists"));
	}
	
	employeeExists = Employees
	             .Where(x => x.EmployeeID == employeeid
				 )
				 .Select(x => x)
				 .FirstOrDefault();
	if(employeeExists == null)
	{
	errorList.Add(new ArgumentNullException("EmployeeID not exists"));
	}			 
	
	if (couponid != null)
	{
		couponExists = Coupons
		              .Where(x => x.CouponID == couponid
					  )
					  .Select(x => x)
					  .FirstOrDefault();
		if(couponExists == null)
	    {
	         errorList.Add(new ArgumentNullException("Coupon not exists"));
	    }
		
		validation = Coupons 
		             .Where(x=>x.CouponID == couponid &&
					 (DateTime.Today <x.StartDate || DateTime.Today > x.EndDate))
					 .Select(x => x)
					 .FirstOrDefault();
		
		if(validation == null)
	    {
	         errorList.Add(new ArgumentNullException("Coupon must be validate"));
	    }
	}
	
	record = new Rentals()
    {
        CustomerID = customerid,
        EmployeeID = employeeid,
        CouponID = couponid,
        SubTotal = (decimal)0.00,
        TaxAmount = (decimal)0.00,
        RentalDateOut = rentalOut,
        RentalDateIn = rentalIn,
        PaymentType = "C"
    };

    Rentals.Add(record);
		
	foreach(RentalEquipmentInfo value in rentingequipments)
    {
	   available = RentalEquipments
	                            .Where(x=>x.RentalEquipmentID == value.RentalEquipmentID && x.Available == true)
								.Select(x=>x)
								.FirstOrDefault();
								
	   if (available == null)
	   {
	     errorList.Add(new ArgumentNullException("Equipment not available"));
	   }
	  equipment = RentalEquipments
									.Where(x => x.RentalEquipmentID == value.RentalEquipmentID)
									.Select(x => x)
									.FirstOrDefault();
									
		equipment.Available = false;
									
		detail = new RentalDetails()
		{
			RentalID = record.RentalID,
			RentalEquipmentID = value.RentalEquipmentID,
			RentalDays = rentalDays,
			RentalRate = value.Rate,
			OutCondition = value.OutCondition,
			InCondition = value.OutCondition,
			DamageRepairCost = (decimal)0.00,
			Comments = null
		};
		
		record.RentalDetails.Add(detail);
	}
	
	if (errorList.Count > 0)
	{
		throw new AggregateException("Unable to add rentals. check concerns", errorList);
	}
	else 
    {
		SaveChanges();
	}
}

//For return
void Return_Equipment(int employeeId, int rentalId, string paymentType, List<UpdateRentalEquipmentInfo> updateequipments)
	{
	  
      Rentals record = null;
      RentalDetails detail = null;
      RentalEquipment equipment = null;
	  
	  //Business logic and parameter exceptions
	    List <Exception> errorList = new List<Exception>();
	
	
	if (rentalId == 0)
	{
	   throw new ArgumentNullException("RnetalID is missing");
	}
	
	if (employeeId == 0)
	{
	   throw new ArgumentNullException("EmployeeID is missing");
	}
	
	if (paymentType == null)
	{
	   throw new ArgumentNullException("Payment type not selected");
	}
	
	if (paymentType != "M" && paymentType != "D" && paymentType != "C")
	{
	   throw new ArgumentNullException("Payment type can be money, credit and debit only");
	}
	
	if (updateequipments.Count() == 0)
	{
	   throw new ArgumentNullException("No equipment to return");
	}
	
	record = Rentals 
	              .Where(x=>x.RentalID == rentalId)
				  .Select(x=>x)
				  .FirstOrDefault();
		
	if (record == null)
	{
	throw new ArgumentNullException("Payment type not selected");
	}
	
	int? couponDiscountId = Rentals
	                      .Where(x=>x.RentalID == rentalId)
						  .Select(x=>x.CouponID)
						  .FirstOrDefault();
						  
	double discount;
	
    if (couponDiscountId == null)
		{
		  discount = 0;
		}
	else
		{
		  discount = Coupons
		             .Where(x=>x.CouponID== couponDiscountId)
					 .Select(x=>x.CouponDiscount)
					 .FirstOrDefault();
					 
				discount = discount/100;
		}
	decimal subtotal = detail.RentalRate;
	
	record.EmployeeID = employeeId;
	record.SubTotal = subtotal;
	record.TaxAmount = (decimal)0.05 * subtotal;
	record.RentalDateIn = DateTime.Now;	
	record.PaymentType =paymentType;
	
	double RentDays = (record.RentalDateIn - record.RentalDateOut).TotalDays;
	
	Rentals.Update(record);
	
	foreach(UpdateRentalEquipmentInfo value in updateequipments)
	{
	    detail = RentalDetails
	            .Where(x=>x.RentalID== rentalId)
				.Select(x=>x)
				.FirstOrDefault();
	
	    detail.RentalDays = (decimal)RentDays;
		detail.InCondition = value.InCondition;
		detail.Comments = value.Comments;
		
		equipment = RentalEquipments
		            .Where(x=> x.RentalEquipmentID == value.RentalEquipmentID)
					.Select(x=>x)
					.FirstOrDefault();
					
		RentalDetails.Update(detail);
		RentalEquipments.Update(equipment);
	}
	if (errorList.Count > 0)
	{
		throw new AggregateException("Unable to return data. check concerns", errorList);
	}
	else 
    {
		SaveChanges();
	}
}