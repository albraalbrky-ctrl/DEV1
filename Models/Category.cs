using System.ComponentModel.DataAnnotations;

namespace DEV1.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        // علاقة: الفئة الواحدة لها قائمة منتجات
        public ICollection<Product> Products { get; set; }
    }
}