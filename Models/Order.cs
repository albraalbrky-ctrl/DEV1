using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DEV1.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        // كود فريد للطلب يظهر للزبون (مثال: ORD-A89F)
        public string OrderCode { get; set; } = "ORD-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

        // حالة الطلب: تحت التجهيز، أو تم التسليم بنجاح
        public string Status { get; set; } = "تحت التجهيز";

        [Required(ErrorMessage = "يرجى إدخال اسمك الكامل")]
        [StringLength(100)]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "يرجى إدخال رقم الهاتف لنتواصل معك")]
        [Phone(ErrorMessage = "رقم الهاتف غير صحيح")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "يرجى إدخال عنوان التوصيل بالتفصيل")]
        public string Address { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Required]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product? Product { get; set; }

        [Required(ErrorMessage = "يرجى تحديد الكمية")]
        [Range(1, 100, ErrorMessage = "الكمية يجب أن تكون بين 1 و 100")]
        public int Quantity { get; set; } = 1;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }
    }
}