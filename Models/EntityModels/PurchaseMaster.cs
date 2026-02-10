using System.ComponentModel.DataAnnotations;

namespace MIEL.web.Models.EntityModels
{
    public class PurchaseMaster
    {
        [Key]
        public int PurchaseId { get; set; }
        public int SupplierId { get; set; }

        public string InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal TotalDisc { get; set; }
        public decimal TotalTaxable { get; set; }
        public decimal TotalTax { get; set; }

        public decimal GrandTotal { get; set; }
    }
}
