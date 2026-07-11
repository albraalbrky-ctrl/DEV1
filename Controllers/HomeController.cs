using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DEV1.Data;

namespace DEV1.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context) => _context = context;

        // صفحة تواصل معنا للزبائن
        public IActionResult ContactUs() => View();

        // صفحة الاستعلام عن حالة الطلب (GET)
        public IActionResult TrackOrder() => View();

        // البحث الفوري عن الطلب وإرجاع التفاصيل بجافا سكربت (AJAX) لسرعة خيالية للزبون
        [HttpGet]
        public async Task<IActionResult> GetOrderStatus(string orderCode)
        {
            if (string.IsNullOrEmpty(orderCode)) return Json(new { success = false, msg = "يرجى إدخال كود صحيح" });

            var order = await _context.Orders
                .Include(o => o.Product)
                .FirstOrDefaultAsync(o => o.OrderCode.Trim() == orderCode.Trim());

            if (order == null)
            {
                return Json(new { success = false, msg = "عذراً، كود الطلب هذا غير مسجل لدينا، تأكد من صحة الحروف." });
            }

            return Json(new {
                success = true,
                code = order.OrderCode,
                customer = order.CustomerName,
                product = order.Product?.Name,
                qty = order.Quantity,
                total = order.TotalPrice.ToString("0.00"),
                status = order.Status,
                date = order.OrderDate.ToString("yyyy-MM-dd")
            });
        }
    }
}