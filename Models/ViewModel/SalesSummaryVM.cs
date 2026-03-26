namespace MIEL.web.Models.ViewModel
{
    public class SalesSummaryVM
    {



        public string InvoiceNo { get; set; }
        public DateTime SalesDate { get; set; }
        public decimal TaxableAmount { get; set; }
        public decimal GSTAmount { get; set; }
        public decimal NetAmount { get; set; }
        public string CustomerName { get; set; }

        public decimal TaxAmount { get; set; }





    }
}
