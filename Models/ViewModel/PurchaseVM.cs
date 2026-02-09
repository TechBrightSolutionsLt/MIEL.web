using MIEL.web.Models.EntityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MIEL.web.Models.ViewModel
{
    public class PurchaseVM
    {
        // =============================
        // PURCHASE HEADER
        // =============================
        public int PurchaseId { get; set; }

        public string PurchaseCode { get; set; }

        public DateTime PurchaseDate { get; set; }

        public string InvoiceNo { get; set; }

        // =============================
        // SUPPLIER
        // =============================
        public int SupplierId { get; set; }

        public string SupplierName { get; set; }

        public string SupplierMobile { get; set; }

        // =============================
        // TOTALS
        // =============================
        public decimal TotalAmount { get; set; }

        public string Status { get; set; }   // Draft / Completed

        // =============================
        // LOOKUPS
        // =============================
        public List<ProductMaster> Products { get; set; }

        // =============================
        // PURCHASE ITEMS
        // =============================
        //  public List<PurchaseItemVM> Items { get; set; } = new();
    }

    // =================================
    // PURCHASE ITEM VM
    // =================================
  
}