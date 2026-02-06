using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MIEL.web.Models.EntityModels
{
    [Table("ProductMasters")]   // ✅ Explicit table name
    public class ProductMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; }   // ✅ PK

        public int CategoryId { get; set; }

        [Required]
        [MaxLength(50)]
        public string ProductCode { get; set; }

        [Required]
        [MaxLength(200)]
        public string ProductName { get; set; }

        [MaxLength(100)]
        public string? Brand { get; set; }

        public string? ProductDescription { get; set; }

        [MaxLength(50)]
        public string? Occasion { get; set; }

        [MaxLength(50)]
        public string? ComboPackage { get; set; }

        [MaxLength(50)]
        public string? HSNNo { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public int SupplierId { get; set; }
        public string? BarcodeNo { get; set; }

        public string? SizeChartImg { get; set; }
        public string sizechartPath { get; set; }

    }
}
