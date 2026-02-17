using MIEL.web.Models.EntityModels;
using System;
using System.Collections.Generic;

namespace MIEL.web.Models.ViewModel
{
    public class SaleCreateVM
    {
        public string SaleCode { get; set; }
        public DateTime SaleDate { get; set; }

        public string FirstName { get; set; }
        //CustomerName
        public string Mobile { get; set; }

        // Dropdown data
        public List<ProductMaster> Products { get; set; }
        public List<userModel> Customers { get; set; }

    }
}
