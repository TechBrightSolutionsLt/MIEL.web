using System.Collections.Generic;
using MIEL.web.Models.EntityModels;

namespace MIEL.web.Models.ViewModel
{
    public class ProductMasterVM
    {
        public int SelectedCategoryId { get; set; }

        public string ProductCode { get; set; }

        public string ProductName { get; set; }

        public string Brand { get; set; }

        public string ProductDescription { get; set; }

        public string Occasion { get; set; }

        public string ComboPackage { get; set; }

        public string HSNNo { get; set; }

        public List<Category> Categories { get; set; } = new List<Category>();
    }
}
