
using MIEL.web.Models.EntityModels;

namespace MIEL.web.Models.ViewModel
{
    public class SalesReportVM
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? CustomerId { get; set; }
        public int? SalesMode { get; set; }
        public string PaymentType { get; set; }

        //public List<SalesReportResultVM> Results { get; set; } = new();

        public List<SalesReportGroupVM> Results { get; set; } = new();


        public List<userModel> Customers { get; set; }
    }


public class SalesReportResultVM
    {
        public int SalesId { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime SalesDate { get; set; }
        public string CustomerName { get; set; }
        public string ProductName { get; set; }
        public string BatchNumber { get; set; }
        public int SalesMode { get; set; }
        public decimal NetAmount { get; set; }
        //  public List<Customer> Customers { get; set; } = new();
        public List<userModel> Customers { get; set; }
    }



    public class SalesReportGroupVM
    {
        public int SalesId { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime SalesDate { get; set; }
        public string CustomerName { get; set; }
        public decimal NetAmount { get; set; }

        public List<SalesReportItemVM> Items { get; set; } = new();
    }

    public class SalesReportItemVM
    {
        public string ProductName { get; set; }
        public string BatchNumber { get; set; }
    }
}
