using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MIEL.web.Models.EntityModels
{
    public class ProdColImage
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("procolrsizevarnt")]
        public int VariantId { get; set; }

        public string col { get; set; } = string.Empty;
        public string ? ImagePath { get; set; }

        public procolrsizevarnt ? Variant { get; set; }
    }
}
