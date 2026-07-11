using Microsoft.EntityFrameworkCore;
using DEV1.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. تفعيل الذاكرة المؤقتة وإعدادات الـ Session للوحة التحكم
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2); // سيبقى تسجيل دخولك كأدمن فعالاً لمدة ساعتين
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 2. إضافة خدمات الـ Controllers والـ Views (MVC)
builder.Services.AddControllersWithViews();

// 3. ربط سياق البيانات (DbContext) بقاعدة البيانات بناءً على الإعدادات
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// إعدادات بيئة التشغيل (Development / Production)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 4. تفعيل الـ Session (يجب أن يكون قبل UseAuthorization)
app.UseSession();

app.UseAuthorization();

// 5. إعداد روابط التوجيه الافتراضية للمشروع (Routing)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Products}/{action=Index}/{id?}");

app.Run();