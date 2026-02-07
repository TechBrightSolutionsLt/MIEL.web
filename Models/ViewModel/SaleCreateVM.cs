using MIEL.web.Models.EntityModels;
using System;
using System.Collections.Generic;

namespace MIEL.web.Models.ViewModel
{
    public class SaleCreateVM
    {
        public string SaleCode { get; set; }
        public DateTime SaleDate { get; set; }

        public string CustomerName { get; set; }
        public string Mobile { get; set; }

        // Dropdown data
        public List<ProductMaster> Products { get; set; }
       public List<Customer> Customers { get; set; }
    }
}
