namespace MIEL.web.Models
{
    public class PurchaseReportVM
    {
        public string InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string SupplierName { get; set; }

        public string VarientCode { get; set; }
        public string ProductName { get; set; }

        public int Quantity { get; set; }
        public decimal Rate { get; set; }
        public decimal DiscAmount { get; set; }
        public decimal NetAmount { get; set; }

        public decimal TotalTaxable { get; set; }
        public decimal TotalTax { get; set; }
        public int SupplierId { get; set; }

    }


}
