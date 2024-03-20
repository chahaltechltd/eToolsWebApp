using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesSystem.viewModel
{
    public class SaleServicTRXInfo
    {
        public char PaymentType { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal SubTotal { get; set; }
        public int CouponID { get; set; }
        public int employeeID { get; set; }
        public DateTime SaleDate { get; set; }
        public int SaleID { get; set; }
    }
}
