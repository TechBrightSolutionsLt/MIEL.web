using Microsoft.AspNetCore.Mvc.Rendering;
using MIEL.web.Models.EntityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MIEL.web.Models.ViewModel
{
    public class PurchaseVM
    {
        // Purchase master
        public int SupplierId { get; set; }
        public string PurchaseCode { get; set; }
        public DateTime PurchaseDate { get; set; }

        public string BatchNo { get; set; }

        // Dropdowns
        public List<SelectListItem> Suppliers { get; set; } = new();

        // Item entry
        public int ProductId { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public string VariantCode { get; set; }
        public int VariantId { get; set; }

        public decimal CostPrice { get; set; }
        public int Quantity { get; set; }

        // Grid
        public List<PurchaseItemVM> Items { get; set; } = new();
    }

    // =================================
    // PURCHASE ITEM VM
    // =================================

}