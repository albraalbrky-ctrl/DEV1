using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DEV1.Data;
using DEV1.Models;

namespace DEV1.Controllers
{
    public class OrdersController : Controller
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        // 1. صفحة طلب المنتج (GET)
        public async Task<IActionResult> Checkout(int productId)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);
            if (product == null || product.Stock == 0)
            {
                return RedirectToAction("Index", "Products"); // إعادة للمتجر إذا حاول التلاعب بالرابط لمنتج منتهي
            }

            var order = new Order
            {
                ProductId = product.Id,
                Product = product,
                TotalPrice = product.Price 
            };

            return View(order);
        }

        // 2. استقبال الطلب وحفظه (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(Order order)
        {
            ModelState.Remove("Product");

            var product = await _context.Products.FindAsync(order.ProductId);
            if (product == null) return NotFound();

            // تحقق إضافي من الكمية المتاحة في السيرفر لمنع الثغرات
            if (product.Stock == 0)
            {
                ModelState.AddModelError("", "عذراً، هذا المنتج غير متوفر حالياً.");
            }
            else if (order.Quantity > product.Stock)
            {
                ModelState.AddModelError("Quantity", $"عذراً، الكمية المطلوبة أكبر من المتاحة في المخزن (المتوفر حالياً: {product.Stock} قطع فقط).");
            }

            if (ModelState.IsValid)
            {
                order.TotalPrice = product.Price * order.Quantity;
                order.OrderDate = DateTime.Now;

                // تحديث وخصم الكمية من المخزن للمنتج
                product.Stock -= order.Quantity;
                _context.Update(product);

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // 🌟 هنا التعديل الجديد والمهم: نمرر كود الطلب الفريد لكي تقرأه واجهة النجاح
                TempData["LatestOrderCode"] = order.OrderCode;

                return RedirectToAction(nameof(OrderSuccess));
            }

            order.Product = product;
            return View(order);
        }

        public IActionResult OrderSuccess()
        {
            return View();
        }
    }
}